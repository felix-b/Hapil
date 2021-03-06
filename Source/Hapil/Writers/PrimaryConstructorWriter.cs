﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Members;

namespace Hapil.Writers
{
	public class PrimaryConstructorWriter : ConstructorWriter 
	{
		private readonly FieldMember[] m_FieldsToInitialize;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PrimaryConstructorWriter(ConstructorMember ownerConstructor, FieldMember[] fieldsToInitialize)
			: base(ownerConstructor, script: null)
		{
			m_FieldsToInitialize = fieldsToInitialize;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{ 
			this.Base();

			ForEachArgument((argument, index) => {
				if ( index < m_FieldsToInitialize.Length )
				{
					m_FieldsToInitialize[index].AsOperand<TypeTemplate.TArgument>().Assign(argument);
				}
			});
			
			base.Flush();
		}
	}
}
