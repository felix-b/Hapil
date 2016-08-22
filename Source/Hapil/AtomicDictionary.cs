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
    public class AtomicDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly Hashtable m_Hashtable;
        private readonly object m_WriterSyncRoot;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public AtomicDictionary()
        {
            m_Hashtable = new Hashtable();
            m_WriterSyncRoot = new object();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        #region Implementation of IEnumerable

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var enumerable = m_Hashtable.Keys
                .Cast<object>()
                .Select(key => new KeyValuePair<TKey, TValue>(
                    (TKey)key, 
                    (TValue)m_Hashtable[key])
                );

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
            AcquireWriterLockOrThrow();

            try
            {
                m_Hashtable.Clear();
            }
            finally
            {
                ReleaseWriterLock();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var entry = (ValueEntry)m_Hashtable[item.Key];

            if ( entry != null )
            {
                return ValuesAreEqual(entry.GetValue(item.Key), item.Value);
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
            var entry = (ValueEntry)m_Hashtable[item.Key];

            if ( entry != null && ValuesAreEqual(entry.GetValue(item.Key), item.Value) )
            {
                AcquireWriterLockOrThrow();

                try
                {
                    if ( m_Hashtable[item.Key] != null )
                    {
                        m_Hashtable.Remove(item.Key);
                        return true;
                    }
                }
                finally
                {
                    ReleaseWriterLock();
                }
            }

            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public int Count
        {
            get { return m_Hashtable.Count; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public bool IsReadOnly
        {
            get { return m_Hashtable.IsReadOnly; }
        }

        #endregion

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        #region Implementation of IDictionary<TKey,TValue>

        public bool ContainsKey(TKey key)
        {
            return m_Hashtable.ContainsKey(key);
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
            var entry = (ValueEntry)m_Hashtable[key];

            if ( entry != null )
            {
                AcquireWriterLockOrThrow();

                try
                {
                    if ( m_Hashtable[key] != null )
                    {
                        m_Hashtable.Remove(key);
                        return true;
                    }
                }
                finally
                {
                    ReleaseWriterLock();
                }
            }

            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public bool TryGetValue(TKey key, out TValue value)
        {
            var entry = (ValueEntry)m_Hashtable[key];

            if ( entry != null )
            {
                value = entry.GetValue(key);
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
                var entry = (ValueEntry)m_Hashtable[key];

                if ( entry != null )
                {
                    return entry.GetValue(key);
                }

                throw new KeyNotFoundException("Specified key was not found in the dictionary: " + key.ToString());
            }
            set
            {
                AcquireWriterLockOrThrow();

                try
                {
                    m_Hashtable[key] = new ValueEntry(value);
                }
                finally
                {
                    ReleaseWriterLock();
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public ICollection<TKey> Keys
        {
            get
            {
                var keys = m_Hashtable.Keys.Cast<TKey>().ToArray();
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
            var existingEntry = (ValueEntry)m_Hashtable[key];

            if ( existingEntry == null )
            {
                AcquireWriterLockOrThrow();

                try
                {
                    if ( !m_Hashtable.ContainsKey(key) )
                    {
                        m_Hashtable[key] = new ValueEntry(value);
                        return true;
                    }
                }
                finally
                {
                    ReleaseWriterLock();
                }
            }

            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            var existingEntry = (ValueEntry)m_Hashtable[key];

            if ( existingEntry != null && ValuesAreEqual(existingEntry.GetValue(key), comparisonValue) )
            {
                AcquireWriterLockOrThrow();

                try
                {
                    existingEntry = (ValueEntry)m_Hashtable[key];
                    
                    if ( existingEntry != null && ValuesAreEqual(existingEntry.GetValue(key), comparisonValue) )
                    {
                        existingEntry.UpdateValue(key, newValue);
                        return true;
                    }
                }
                finally
                {
                    ReleaseWriterLock();
                }
            }

            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private TValue AtomicAddOrGetUpdated(
            TKey key,
            Func<TKey, TValue> addValueFactory,
            Func<TKey, TValue, TValue> optionalUpdateValueFactory,
            out bool wasAdded)
        {
            ValueEntry newEntry = null;
            var existingEntry = (ValueEntry)m_Hashtable[key];

            if ( existingEntry == null )
            {
                AcquireWriterLockOrThrow();

                try
                {
                    existingEntry = (ValueEntry)m_Hashtable[key];

                    if ( existingEntry == null )
                    {
                        newEntry = new ValueEntry();
                        m_Hashtable[key] = newEntry;
                    }
                }
                finally
                {
                    ReleaseWriterLock();
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
                        
                        AcquireWriterLockOrThrow();
                        
                        try
                        {
                            m_Hashtable.Remove(key);
                        }
                        finally
                        {
                            ReleaseWriterLock();
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
                var updatedValue = optionalUpdateValueFactory(key, existingEntry.GetValue(key));
                existingEntry.UpdateValue(key, updatedValue);
                return updatedValue;
            }

            return existingEntry.GetValue(key);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void CopyKeyValueArrays(out TKey[] keys, out TValue[] values)
        {
            keys = m_Hashtable.Keys.Cast<TKey>().ToArray();
            values = new TValue[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                var entry = (ValueEntry)m_Hashtable[key];

                if (entry != null)
                {
                    values[i] = entry.GetValue(key);
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void AcquireWriterLockOrThrow()
        {
            if ( !Monitor.TryEnter(m_WriterSyncRoot, 30000) )
            {
                throw new TimeoutException("Timed out waiting for exclusive writer lock on the atomic dictionary.");
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void ReleaseWriterLock()
        {
            Monitor.Exit(m_WriterSyncRoot);
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
            private object m_ConstructionSyncRoot;
            private Exception m_ValueFactoryException;

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public ValueEntry()
            {
                m_ConstructionSyncRoot = new object();
                m_Value = default(TValue);

                Monitor.Enter(m_ConstructionSyncRoot);
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public ValueEntry(TValue value)
            {
                m_Value = value;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public void SetConstructedValue(TKey key, TValue value)
            {
                if ( m_ConstructionSyncRoot == null )
                {
                    throw new InvalidOperationException("Constructed value was already set. Key: " + key.ToString());
                }

                m_Value = value;
                Monitor.Exit(m_ConstructionSyncRoot);
                Volatile.Write(ref m_ConstructionSyncRoot, null);
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public void SetValueConstructionFailure(TKey key, Exception valueFactoryException)
            {
                if ( m_ConstructionSyncRoot == null )
                {
                    throw new InvalidOperationException("Constructed value was already set. Key: " + key.ToString());
                }

                m_ValueFactoryException = valueFactoryException;
                Monitor.Exit(m_ConstructionSyncRoot);
                Volatile.Write(ref m_ConstructionSyncRoot, null);
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public void UpdateValue(TKey key, TValue value)
            {
                EnsureValueConstructed(key);
                
                m_Value = value;
                m_ValueFactoryException = null;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TValue GetValue(TKey key)
            {
                EnsureValueConstructed(key);
                return m_Value;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            private void EnsureValueConstructed(TKey key)
            {
                var constructionSyncRootSnapshot = Volatile.Read(ref m_ConstructionSyncRoot);

                if ( constructionSyncRootSnapshot != null )
                {
                    if ( !Monitor.TryEnter(constructionSyncRootSnapshot, 30000) )
                    {
                        throw new TimeoutException("Timed out waiting for constructed value while in concurrent cache miss. Key: " + key.ToString());
                    }

                    Monitor.Exit(constructionSyncRootSnapshot);
                }

                if ( m_ValueFactoryException != null )
                {
                    throw new KeyNotFoundException(
                        string.Format("Value construction failed for key: " + key.ToString()),
                        innerException: m_ValueFactoryException);
                }
            }
        }
    }
}
