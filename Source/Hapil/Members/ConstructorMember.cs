using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Hapil.Members
{
	public class ConstructorMember : MethodMember
	{
		private readonly Dictionary<FieldMember, int> m_DependencyFieldArgumentIndex;
	    private bool m_MustPreserveSignature = false;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal ConstructorMember(ClassType ownerClass, ConstructorMethodFactory methodFactory)
			: base(ownerClass, methodFactory)
		{
			m_DependencyFieldArgumentIndex = new Dictionary<FieldMember, int>();
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

	    public void PreserveSignature()
	    {
	        m_MustPreserveSignature = true;
	    }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void FreezeSignature(FieldMember[] dependencyFields)
		{
			if ( !this.MethodFactory.IsFinalSignature )
			{
				InjectDependencies(dependencyFields);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IEnumerable<KeyValuePair<FieldMember, int>> GetDependencyFieldArgumentIndex()
		{
			return m_DependencyFieldArgumentIndex;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool HasDependencyInjection
		{
			get
			{
				return (m_DependencyFieldArgumentIndex.Count > 0);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void InjectDependencies(FieldMember[] dependencyFields)
		{
			if ( IsStatic )
			{
				throw new InvalidOperationException("Static constructor cannot handle dependency injection.");
			}

			HashSet<FieldMember> unmappedFields = null;
			MethodSignature finalSignature;

            if ( dependencyFields.Length > 0 && !m_MustPreserveSignature )
			{
				unmappedFields = new HashSet<FieldMember>(dependencyFields);
				MapDependencyFieldsToArguments(unmappedFields);
				finalSignature = GetExtendedSignature(unmappedFields.ToArray());
			}
			else
			{
				finalSignature = this.Signature;
			}

			this.MethodFactory = this.MethodFactory.FreezeSignature(finalSignature);

			if ( unmappedFields != null && unmappedFields.Count > 0 )
			{
				MapDependencyFieldsToArguments(unmappedFields);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void MapDependencyFieldsToArguments(HashSet<FieldMember> unmappedFields)
		{
			var unmappedFieldsSnapshot = unmappedFields.ToArray();
			
			foreach ( var field in unmappedFieldsSnapshot )
			{
				for ( int i = 0 ; i < Signature.ArgumentType.Length ; i++ )
				{
					if ( field.FieldType.IsAssignableFrom(Signature.ArgumentType[i]) )
					{
						m_DependencyFieldArgumentIndex[field] = i;
						unmappedFields.Remove(field);
						break;
					}
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private MethodSignature GetExtendedSignature(FieldMember[] unmappedFields)
		{
			if ( unmappedFields.Length == 0 )
			{
				return this.Signature;
			}
			
			var unmappedTypes = new Type[unmappedFields.Length];
			var unmappedNames = new string[unmappedFields.Length];

			for ( int i = 0 ; i < unmappedFields.Length ; i++ )
			{
				unmappedTypes[i] = unmappedFields[i].FieldType;
				unmappedNames[i] = unmappedFields[i].Name.TrimPrefix("m_").ToCamelCase();
			}

			return Signature.Extend(unmappedTypes, unmappedNames);
		}
	}
}
