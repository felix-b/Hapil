using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Members;
using Happil.Operands;
using Happil.Writers;

// ReSharper disable ConvertToLambdaExpression

namespace Happil.Applied.XTuple
{
	public class XTupleFactory : ObjectFactoryBase
	{
		public XTupleFactory(DynamicModule module)
			: base(module)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public T New<T>() where T : class
		{
			var key = new TypeKey(primaryInterface: typeof(T));
			var type = base.GetOrBuildType(key);
			return type.CreateInstance<T>();
		}
	
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override ClassType DefineNewClass(TypeKey key)
		{
			var classDefinition = DeriveClassFrom<object>(key);
			var builder = new TupleClassBuilder(key, classDefinition);

			builder.BuildClass();

			return classDefinition;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private string MakeClassNameFrom(Type type, string prefix = null, string suffix = null)
		{
			return type.Namespace + "." + (prefix ?? "") + type.Name + (suffix ?? "");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TupleClassBuilder
		{
			private readonly TypeKey m_Key;
			private readonly ImplementationClassWriter<object> m_ClassBody;
			private readonly Dictionary<string, PropertyInfo> m_TupleProperties;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TupleClassBuilder(TypeKey key, ImplementationClassWriter<object> classBody)
			{
				m_Key = key;
				m_ClassBody = classBody;
				m_TupleProperties = TypeMemberCache.Of(key.PrimaryInterface).ImplementableProperties.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void BuildClass()
			{
				m_ClassBody.DefaultConstructor();

				ImplementTupleInterface();
				ImplementObjectEquals();
				ImplementGetHashCode();
				ImplementToString();
				ImplementIEquatable();
				ImplementIComparable();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void ImplementTupleInterface()
			{
				m_ClassBody
					.ImplementInterface<TypeTemplate.TPrimary>()
					.AllProperties()
						.ImplementAutomatic()
					.AllMethods(m => m.Name == "Init")
						.Implement(m => {
							m.ForEachArgument(arg => {
								m.This<TypeTemplate.TPrimary>().BackingFieldOf<TypeTemplate.TArgument>(m_TupleProperties[arg.Name]).Assign(arg);
							});
							m.Return(m.This<TypeTemplate.TReturn>());
						});
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void ImplementObjectEquals()
			{
				m_ClassBody
					.Method<object, bool>(cls => cls.Equals)
						.Implement((m, other) => {
							m.Return(
								m.This<IEquatable<TypeTemplate.TPrimary>>().Func<TypeTemplate.TPrimary, bool>(intf => intf.Equals, other.CastTo<TypeTemplate.TPrimary>())
							);
						});
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void ImplementGetHashCode()
			{
				m_ClassBody
					.Method<int>(cls => cls.GetHashCode)
						.Implement(m => {
							var hashCode = m.Local<int>(0);

							m.This<TypeTemplate.TPrimary>().Members.SelectAllProperties().ForEach(prop => {
								var field = m.This<TypeTemplate.TPrimary>().BackingFieldOf<TypeTemplate.TProperty>(prop);
								var fieldHashCodeExpression = m.Iif(
									condition: field != m.Const<TypeTemplate.TProperty>(null),
									isTautology: field.OperandType.IsValueType,
									onTrue: field.Func<int>(x => x.GetHashCode),
									onFalse: m.Const<int>(0));

								hashCode.Assign(((hashCode << 5) + hashCode) ^ fieldHashCodeExpression);
							});

							m.Return(hashCode);
						});
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void ImplementToString()
			{
				m_ClassBody.Method<string>(cls => cls.ToString).Implement(m => {
					var stringBuilder = m.Local(m.New<StringBuilder>(m.Const("(")));

					m.This<TypeTemplate.TPrimary>().Members.SelectAllProperties().ForEach((index, count, prop) => {
						var field = m.This<TypeTemplate.TPrimary>().BackingFieldOf<TypeTemplate.TProperty>(prop);
						stringBuilder.Func<object, StringBuilder>(x => x.Append, field.CastTo<object>());
						stringBuilder.Func<string, StringBuilder>(x => x.Append, m.Const(index < count - 1 ? ", " : ")"));
					});

					m.Return(stringBuilder.Func<string>(x => x.ToString));
				});
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void ImplementIEquatable()
			{
				m_ClassBody
					.ImplementInterface<IEquatable<TypeTemplate.TPrimary>>()
					.Method<TypeTemplate.TPrimary, bool>(intf => intf.Equals)
						.Implement((m, other) => {
							m.This<TypeTemplate.TPrimary>().Members.SelectAllProperties().ForEach(prop => {
								m.If(m.This<TypeTemplate.TPrimary>().BackingFieldOf<TypeTemplate.TProperty>(prop) != other.Prop<TypeTemplate.TProperty>(prop)).Then(() =>
									m.Return(false)
								);
							});
							m.Return(true);
						});
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void ImplementIComparable()
			{
				m_ClassBody
					.ImplementInterface<IComparable>()
					.Method<object, int>(intf => intf.CompareTo)
						.Implement((m, other) => {
							var otherTuple = m.Local(initialValue: other.As<TypeTemplate.TPrimary>());
							var compareResult = m.Local<int>(initialValueConst: 0);

							m.If(Static.Func(object.ReferenceEquals, otherTuple, m.Const<object>(null))).Then(() => {
								m.Return(1);
							});

							m.This<TypeTemplate.TPrimary>().Members.SelectAllProperties().ForEach(prop => {
								EmitPropertyComparison(m, prop, otherTuple, compareResult);
							});

							m.Return(0);
						});
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			private static void EmitPropertyComparison(
				FunctionMethodWriter<int> m, 
				PropertyInfo prop, 
				LocalOperand<TypeTemplate.TPrimary> otherTuple,
				LocalOperand<int> compareResult)
			{
				var backingField = m.This<TypeTemplate.TPrimary>().BackingFieldOf<TypeTemplate.TProperty>(prop);
				var otherValue = m.Local(initialValue: otherTuple.Prop<TypeTemplate.TProperty>(prop));

				if ( backingField.OperandType.IsValueType )
				{
					m.If(backingField > otherValue).Then(() => {
						m.Return(1);
					})
					.ElseIf(backingField < otherValue).Then(() => {
						m.Return(-1);
					})
					.ElseIf(backingField != otherValue).Then(() => {
						m.Return(1);
					});
				}
				else if ( typeof(IComparable).IsAssignableFrom(backingField.OperandType) )
				{
					compareResult.Assign(backingField.CastTo<IComparable>().Func<object, int>(x => x.CompareTo, otherValue));
					m.If(compareResult != 0).Then(() => m.Return(compareResult));
				}
				else
				{
					throw new NotSupportedException("Cannot compare values of type: " + backingField.OperandType.FullName);
				}
			}
		}
	}
}

// ReSharper restore ConvertToLambdaExpression

