using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hapil
{
    internal class AtomicDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly Hashtable[] m_Hashtables;
        private readonly object[] m_WriterSyncRoots;
        private readonly int m_MillisecondsValueFactoryTimeout;
        private readonly int m_PartitionCount;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public AtomicDictionary()
            : this(valueFactoryTimeout: TimeSpan.FromSeconds(30), partitionCount: 8)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public AtomicDictionary(TimeSpan valueFactoryTimeout, int partitionCount)
        {
            m_PartitionCount = partitionCount;
            m_Hashtables = new Hashtable[m_PartitionCount];
            m_WriterSyncRoots = new object[m_PartitionCount];

            for ( int i = 0 ; i < m_PartitionCount ; i++ )
            {
                m_Hashtables[i] = new Hashtable();
                m_WriterSyncRoots[i] = new object();
            }

            m_MillisecondsValueFactoryTimeout = (int)valueFactoryTimeout.TotalMilliseconds;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        #region Implementation of IEnumerable

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var enumerable = m_Hashtables.SelectMany(hashtable => hashtable
                .Cast<object>()
                .Select(key => new KeyValuePair<TKey, TValue>(
                    (TKey)key,
                    (TValue)hashtable[key])
                ));
            
            return enumerable.GetEnumerator();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        #region Implementation of ICollection<KeyValuePair<TKey,TValue>>

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if ( !TryAdd(item.Key, item.Value) )
            {
                throw new InvalidOperationException("Specified key was already added: " + item.Key.ToString());
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public void Clear()
        {
            for ( int i = 0 ; i < m_Hashtables.Length ; i++ )
            {
                AcquireWriterLockOrThrow(i);

                try
                {
                    m_Hashtables[i].Clear();
                }
                finally
                {
                    ReleaseWriterLock(i);
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var entry = (ValueEntry)m_Hashtables[GetPartitionIndex(item.Key)][item.Key];

            if ( entry != null )
            {
                return ValuesAreEqual(entry.GetValue(item.Key, m_MillisecondsValueFactoryTimeout), item.Value);
            }

            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            TKey[] keys;
            TValue[] values;
            CopyKeyValueArrays(out keys, out values);

            for ( int i = 0 ; i < keys.Length ; i++ )
            {
                array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            var partitionIndex = GetPartitionIndex(key);
            var partitionHashtable = m_Hashtables[partitionIndex];
            var entry = (ValueEntry)partitionHashtable[key];

            if ( entry != null && ValuesAreEqual(entry.GetValue(key, m_MillisecondsValueFactoryTimeout), item.Value) )
            {
                AcquireWriterLockOrThrow(partitionIndex);

                try
                {
                    if (partitionHashtable[key] != null)
                    {
                        partitionHashtable.Remove(key);
                        return true;
                    }
                }
                finally
                {
                    ReleaseWriterLock(partitionIndex);
                }
            }

            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public int Count
        {
            get
            {
                var totalCount = 0;

                for ( int i = 0 ; i < m_Hashtables.Length ; i++ )
                {
                    totalCount += m_Hashtables[i].Count;
                }

                return totalCount;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        #region Implementation of IDictionary<TKey,TValue>

        public bool ContainsKey(TKey key)
        {
            var partitionIndex = GetPartitionIndex(key);
            var partitionHashtable = m_Hashtables[partitionIndex];
            return partitionHashtable.ContainsKey(key);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public void Add(TKey key, TValue value)
        {
            if ( !TryAdd(key, value) )
            {
                throw new InvalidOperationException("Specified key was already added: " + key.ToString());
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public bool Remove(TKey key)
        {
            var partitionIndex = GetPartitionIndex(key);
            var partitionHashtable = m_Hashtables[partitionIndex];
            var entry = (ValueEntry)partitionHashtable[key];

            if ( entry != null )
            {
                AcquireWriterLockOrThrow(partitionIndex);

                try
                {
                    if (partitionHashtable[key] != null)
                    {
                        partitionHashtable.Remove(key);
                        return true;
                    }
                }
                finally
                {
                    ReleaseWriterLock(partitionIndex);
                }
            }

            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public bool TryGetValue(TKey key, out TValue value)
        {
            var partitionIndex = GetPartitionIndex(key);
            var partitionHashtable = m_Hashtables[partitionIndex];
            var entry = (ValueEntry)partitionHashtable[key];

            if ( entry != null )
            {
                value = entry.GetValue(key, m_MillisecondsValueFactoryTimeout);
                return true;
            }

            value = default(TValue);
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public TValue this[TKey key]
        {
            get
            {
                var partitionIndex = GetPartitionIndex(key);
                var partitionHashtable = m_Hashtables[partitionIndex];
                var entry = (ValueEntry)partitionHashtable[key];

                if ( entry != null )
                {
                    return entry.GetValue(key, m_MillisecondsValueFactoryTimeout);
                }

                throw new KeyNotFoundException("Specified key was not found in the dictionary: " + key.ToString());
            }
            set
            {
                var partitionIndex = GetPartitionIndex(key);
                var partitionHashtable = m_Hashtables[partitionIndex];
                
                AcquireWriterLockOrThrow(partitionIndex);

                try
                {
                    partitionHashtable[key] = new ValueEntry(value);
                }
                finally
                {
                    ReleaseWriterLock(partitionIndex);
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public ICollection<TKey> Keys
        {
            get
            {
                var keys = m_Hashtables.SelectMany(h => h.Keys.Cast<TKey>()).ToArray();
                return keys;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public ICollection<TValue> Values 
        {
            get
            {
                TKey[] keys;
                TValue[] values;
                CopyKeyValueArrays(out keys, out values);
                
                return values;
            }
        }

        #endregion

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            bool wasAdded;

            return AtomicAddOrGetUpdated(
                key,
                addValueFactory: addValueFactory,
                optionalUpdateValueFactory: updateValueFactory,
                wasAdded: out wasAdded);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory, out bool wasAdded)
        {
            return AtomicAddOrGetUpdated(
                key,
                addValueFactory: addValueFactory,
                optionalUpdateValueFactory: updateValueFactory,
                wasAdded: out wasAdded);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            bool wasAdded;
            
            return AtomicAddOrGetUpdated(
                key, 
                addValueFactory: valueFactory, 
                optionalUpdateValueFactory: null, 
                wasAdded: out wasAdded);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory, out bool wasAdded)
        {
            return AtomicAddOrGetUpdated(
                key,
                addValueFactory: valueFactory,
                optionalUpdateValueFactory: null,
                wasAdded: out wasAdded);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public bool TryAdd(TKey key, TValue value)
        {
            var partitionIndex = GetPartitionIndex(key);
            var partitionHashtable = m_Hashtables[partitionIndex];
            var existingEntry = (ValueEntry)partitionHashtable[key];

            if ( existingEntry == null )
            {
                AcquireWriterLockOrThrow(partitionIndex);

                try
                {
                    if ( !partitionHashtable.ContainsKey(key) )
                    {
                        partitionHashtable[key] = new ValueEntry(value);
                        return true;
                    }
                }
                finally
                {
                    ReleaseWriterLock(partitionIndex);
                }
            }

            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            var partitionIndex = GetPartitionIndex(key);
            var partitionHashtable = m_Hashtables[partitionIndex];
            var existingEntry = (ValueEntry)partitionHashtable[key];

            if ( existingEntry != null && ValuesAreEqual(existingEntry.GetValue(key, m_MillisecondsValueFactoryTimeout), comparisonValue) )
            {
                AcquireWriterLockOrThrow(partitionIndex);

                try
                {
                    existingEntry = (ValueEntry)partitionHashtable[key];

                    if ( existingEntry != null && ValuesAreEqual(existingEntry.GetValue(key, m_MillisecondsValueFactoryTimeout), comparisonValue) )
                    {
                        existingEntry.UpdateValue(key, newValue, m_MillisecondsValueFactoryTimeout);
                        return true;
                    }
                }
                finally
                {
                    ReleaseWriterLock(partitionIndex);
                }
            }

            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private int GetPartitionIndex(TKey key)
        {
            var hash = key.GetHashCode();

            if ( hash < 0 )
            {
                hash *= -1;
            }

            return (hash % m_PartitionCount);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private TValue AtomicAddOrGetUpdated(
            TKey key,
            Func<TKey, TValue> addValueFactory,
            Func<TKey, TValue, TValue> optionalUpdateValueFactory,
            out bool wasAdded)
        {
            var partitionIndex = GetPartitionIndex(key);
            var partitionHashtable = m_Hashtables[partitionIndex];

            ValueEntry newEntry = null;
            var existingEntry = (ValueEntry)partitionHashtable[key];

            if ( existingEntry == null )
            {
                AcquireWriterLockOrThrow(partitionIndex);

                try
                {
                    existingEntry = (ValueEntry)partitionHashtable[key];

                    if ( existingEntry == null )
                    {
                        newEntry = new ValueEntry();
                        partitionHashtable[key] = newEntry;
                    }
                }
                finally
                {
                    ReleaseWriterLock(partitionIndex);
                }

                if ( newEntry != null )
                {
                    TValue constructedValue;

                    try
                    {
                        constructedValue = addValueFactory(key);
                        newEntry.SetConstructedValue(key, constructedValue);
                    }
                    catch ( Exception e )
                    {
                        newEntry.SetValueConstructionFailure(key, e);

                        AcquireWriterLockOrThrow(partitionIndex);
                        
                        try
                        {
                            partitionHashtable.Remove(key);
                        }
                        finally
                        {
                            ReleaseWriterLock(partitionIndex);
                        }
                        
                        throw;
                    }

                    wasAdded = true;
                    return constructedValue;
                }
            }

            wasAdded = false;

            if ( optionalUpdateValueFactory != null )
            {
                var updatedValue = optionalUpdateValueFactory(key, existingEntry.GetValue(key, m_MillisecondsValueFactoryTimeout));
                existingEntry.UpdateValue(key, updatedValue, m_MillisecondsValueFactoryTimeout);
                return updatedValue;
            }

            return existingEntry.GetValue(key, m_MillisecondsValueFactoryTimeout);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void CopyKeyValueArrays(out TKey[] keys, out TValue[] values)
        {
            keys = m_Hashtables.SelectMany(h => h.Keys.Cast<TKey>()).ToArray();
            values = new TValue[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                var partitionIndex = GetPartitionIndex(key);
                var entry = (ValueEntry)m_Hashtables[partitionIndex][key];

                if (entry != null)
                {
                    values[i] = entry.GetValue(key, m_MillisecondsValueFactoryTimeout);
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void AcquireWriterLockOrThrow(int partitionIndex)
        {
            if (!Monitor.TryEnter(m_WriterSyncRoots[partitionIndex], 30000))
            {
                throw new TimeoutException("Timed out waiting for exclusive writer lock on the atomic dictionary.");
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void ReleaseWriterLock(int partitionIndex)
        {
            Monitor.Exit(m_WriterSyncRoots[partitionIndex]);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private bool ValuesAreEqual(TValue value1, TValue value2)
        {
            if ( value1 == null )
            {
                return (value2 == null);
            }

            return value1.Equals(value2);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        #if false

        private class KeyValuePairEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly TKey[] m_Keys;
            private readonly TValue[] m_Values;
            private int m_Index;

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public KeyValuePairEnumerator(TKey[] keys, TValue[] values)
            {
                m_Keys = keys;
                m_Values = values;
                m_Index = -1;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            #region Implementation of IDisposable

            public void Dispose()
            {
            }

            #endregion

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            #region Implementation of IEnumerator

            public bool MoveNext()
            {
                m_Index++;
                return (m_Index < m_Keys.Length);
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public void Reset()
            {
                m_Index = -1;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    if ( m_Index < 0 || m_Index >= m_Keys.Length )
                    {
                        throw new InvalidOperationException();
                    }

                    return new KeyValuePair<TKey, TValue>(m_Keys[m_Index], m_Values[m_Index]);
                }
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            #endregion
        }

        #endif

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class ValueEntry
        {
            private TValue m_Value;
            private bool m_ValueUnderConstruction;
            private Exception m_ValueFactoryException;

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public ValueEntry()
            {
                m_ValueUnderConstruction = true;
                m_Value = default(TValue);

                Monitor.Enter(this);
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public ValueEntry(TValue value)
            {
                m_Value = value;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public void SetConstructedValue(TKey key, TValue value)
            {
                if (!m_ValueUnderConstruction)
                {
                    throw new InvalidOperationException("Constructed value was already set. Key: " + key.ToString());
                }

                m_Value = value;
                Monitor.Exit(this);
                Volatile.Write(ref m_ValueUnderConstruction, false);
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public void SetValueConstructionFailure(TKey key, Exception valueFactoryException)
            {
                if (!m_ValueUnderConstruction)
                {
                    throw new InvalidOperationException("Constructed value was already set. Key: " + key.ToString());
                }

                m_ValueFactoryException = valueFactoryException;
                Monitor.Exit(this);
                Volatile.Write(ref m_ValueUnderConstruction, false);
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public void UpdateValue(TKey key, TValue value, int millisecondsTimeout)
            {
                EnsureValueConstructed(key, millisecondsTimeout);
                
                m_Value = value;
                m_ValueFactoryException = null;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TValue GetValue(TKey key, int millisecondsTimeout)
            {
                EnsureValueConstructed(key, millisecondsTimeout);
                return m_Value;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            private void EnsureValueConstructed(TKey key, int millisecondsTimeout)
            {
                var constructionSyncRootSnapshot = Volatile.Read(ref m_ValueUnderConstruction);

                if (constructionSyncRootSnapshot)
                {
                    if ( !Monitor.TryEnter(this, millisecondsTimeout) )
                    {
                        throw new TimeoutException("Timed out waiting for constructed value while in concurrent cache miss. Key: " + key.ToString());
                    }

                    Monitor.Exit(this);
                }

                if ( m_ValueFactoryException != null )
                {
                    throw new KeyNotFoundException(
                        string.Format("Value construction failed for key: " + key.ToString()),
                        innerException: m_ValueFactoryException);
                }
            }
        }

        public void PrintPartitionCounts()
        {
            Console.WriteLine("-------------------");

            for (int i = 0; i < m_Hashtables.Length; i++)
            {
                Console.WriteLine("{0:000} : {1}", i, m_Hashtables[i].Count);
            }
        }
    }
}
