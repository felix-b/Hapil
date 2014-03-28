using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Members;

namespace Happil.Writers
{
	public class ImplementationClassWriter<TBase> : ClassWriterBase
	{
		private readonly Type m_BaseType;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter(ClassType ownerClass)
			: this(ownerClass, typeof(TBase))
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter(ClassType ownerClass, Type baseType)
			: base(ownerClass)
		{
			m_BaseType = TypeTemplate.Resolve(baseType);
			
			if ( m_BaseType.IsInterface )
			{
				ownerClass.AddInterface(m_BaseType);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> DefaultConstructor()
		{
			var constructorMember = new MethodMember(
				OwnerClass, 
				methodFactory: ConstructorMethodFactory.DefaultConstructor(OwnerClass));

			OwnerClass.AddMember(constructorMember);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ITemplateMethodSelector AllMethods(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector(this, TypeMemberCache.Of<TBase>().ImplementableMethods.SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected override void Flush()
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> ImplementEmpty();
			ImplementationClassWriter<TBase> Throw<TException>(string message);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface ITemplateMethodSelector : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<TemplateMethodWriter> write);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class MethodSelector : 
			IMethodSelectorBase,
			ITemplateMethodSelector
		{
			private readonly ClassType m_OwnerClass;
			private readonly ImplementationClassWriter<TBase> m_ClassWriter;
			private readonly MethodInfo[] m_SelectedMethods;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public MethodSelector(ImplementationClassWriter<TBase> classWriter, IEnumerable<MethodInfo> selectedMethods)
			{
				m_OwnerClass = classWriter.OwnerClass;
				m_ClassWriter = classWriter;
				m_SelectedMethods = selectedMethods.ToArray();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IMethodSelectorBase Members

			ImplementationClassWriter<TBase> IMethodSelectorBase.ImplementEmpty()
			{
				DefineMethodImplementations(method => new EmptyMethodWriter(method));
				return m_ClassWriter;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			ImplementationClassWriter<TBase> IMethodSelectorBase.Throw<TException>(string message)
			{
				throw new NotImplementedException();
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region ITemplateMethodSelector Members

			ImplementationClassWriter<TBase> ITemplateMethodSelector.Implement(Action<TemplateMethodWriter> write)
			{
				throw new NotImplementedException();
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void DefineMethodImplementations(Action<MethodMember> implementation)
			{
				var methodsToImplement = m_OwnerClass.TakeNotImplementedMembers(m_SelectedMethods);

				foreach ( var method in methodsToImplement )
				{
					var methodFactory = new VirtualMethodFactory(m_OwnerClass, method);
					var methodMember = new MethodMember(m_OwnerClass, methodFactory);
					m_OwnerClass.AddMember(methodMember);
					implementation(methodMember);
				}
			}
		}
	}
}
