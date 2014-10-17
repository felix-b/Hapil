using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Hapil.Applied.ApiContracts
{
	[Serializable]
	public class ApiContractException : Exception, ISerializable
	{
		private readonly string m_ParamName;
		private readonly ApiContractCheckType m_FailedCheck;
		private readonly ApiContractCheckDirection m_FailedCheckDirecton;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ApiContractException(string paramName, ApiContractCheckType failedCheck, bool isOutput)
			: base(FormatMessage(paramName, failedCheck))
		{
			m_ParamName = paramName;
			m_FailedCheck = failedCheck;
			m_FailedCheckDirecton = (isOutput ? ApiContractCheckDirection.Output : ApiContractCheckDirection.Input);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected ApiContractException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			m_ParamName = info.GetString("ParamName");
			m_FailedCheck = (ApiContractCheckType)info.GetInt32("FailedCheck");
			m_FailedCheckDirecton = (ApiContractCheckDirection)info.GetInt32("FailedCheckDirection");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if ( info == null )
			{
				throw new ArgumentNullException("info");
			}

			base.GetObjectData(info, context);
			
			info.AddValue("ParamName", m_ParamName, typeof(string));
			info.AddValue("FailedCheck", (int)m_FailedCheck, typeof(int));
			info.AddValue("FailedCheckDirection", (int)m_FailedCheckDirecton, typeof(int));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string ParamName
		{
			get
			{
				return m_ParamName;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ApiContractCheckType FailedCheck
		{
			get
			{
				return m_FailedCheck;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ApiContractCheckDirection FailedCheckDirection
		{
			get
			{
				return m_FailedCheckDirecton;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static string FormatMessage(string paramName, ApiContractCheckType failedCheck)
		{
			return string.Format("API contract violation. Parameter [{0}] has failed [{1}] check.", paramName, failedCheck);
		}
	}
}