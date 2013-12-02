using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Fluent;
using System.Reflection.Emit;

namespace Happil
{
	public class HappilFactory
	{
	    private AssemblyBuilder m_AssemblyBuilder;

	    public HappilFactory(string fullName, AssemblyBuilderAccess access = AssemblyBuilderAccess.RunAndSave)
	    {
	        m_AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(fullName), access);
	    }

	    //-----------------------------------------------------------------------------------------------------------------------------------------------------
	    
        public IClass DefineClass(string fullName)
		{
            
			throw new NotImplementedException();
		}
	}
}
