using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Hapil.Fluent;
using TT = Hapil.TypeTemplate;

// ReSharper disable ConvertToLambdaExpression

namespace Hapil
{
	public class XTupleFactory : HappilFactoryBase
	{
		public XTupleFactory(HappilModule module)
			: base(module)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public T New<T>() where T : class
		{
			var key = new HappilTypeKey(primaryInterface: typeof(T));
			var type = base.GetOrBuildType(key);
			return type.CreateInstance<T>();
		}
	
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override IHappilClassDefinition DefineNewClass(HappilModule module, HappilTypeKey key)
		{
			var classDefinition = Module.DeriveClassFrom<object>(MakeClassNameFrom(key.PrimaryInterface, prefix: "XTupleOf"));
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
			private readonly HappilTypeKey m_Key;
			private readonly IHappilClassBody<object> m_ClassBody;
			private readonly Dictionary<string, PropertyInfo> m_TupleProperties;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TupleClassBuilder(HappilTypeKey key, IHappilClassBody<object> classBody)
			{
				m_Key = key;
				m_ClassBody = classBody;
				m_TupleProperties = TypeMembers.Of(key.PrimaryInterface).ImplementableProperties.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
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
					.ImplementInterface<TT.TPrimary>()
					.AllProperties()
						.ImplementAutomatic()
					.AllMethods(m => m.Name == "Init")
						.Implement(m => {
							m.ForEachArgument(arg => {
								m.This<TT.TPrimary>().BackingFieldOf<TT.TArgument>(m_TupleProperties[arg.Name]).Assign(arg);
							});
							m.Return(m.This<TT.TReturn>());
						});
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void ImplementObjectEquals()
			{
				m_ClassBody
					.Method<object, bool>(cls => cls.Equals)
						.Implement((m, other) => {
							m.Return(
								m.This<IEquatable<TT.TPrimary>>().Func<TT.TPrimary, bool>(intf => intf.Equals, other.CastTo<TT.TPrimary>())
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

							m.This<TT.TPrimary>().Members.SelectAllProperties().ForEach(prop => {
								var field = m.This<TT.TPrimary>().BackingFieldOf<TT.TProperty>(prop);
								var fieldHashCodeExpression = m.Iif(
									condition: field != m.Const<TT.TProperty>(null),
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

					m.This<TT.TPrimary>().Members.SelectAllProperties().ForEach((index, count, prop) => {
						var field = m.This<TT.TPrimary>().BackingFieldOf<TT.TProperty>(prop);
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
					.ImplementInterface<IEquatable<TT.TPrimary>>()
					.Method<TT.TPrimary, bool>(intf => intf.Equals)
						.Implement((m, other) => {
							m.This<TT.TPrimary>().Members.SelectAllProperties().ForEach(prop => {
								m.If(m.This<TT.TPrimary>().BackingFieldOf<TT.TProperty>(prop) != other.Prop<TT.TProperty>(prop)).Then(() =>
									m.ReturnConst(false)
								);
							});
							m.ReturnConst(true);
						});
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void ImplementIComparable()
			{
				m_ClassBody
					.ImplementInterface<IComparable>()
					.Method<object, int>(intf => intf.CompareTo)
						.Implement((m, other) => {
							var otherTuple = m.Local(initialValue: other.As<TT.TPrimary>());
							var compareResult = m.Local<int>(initialValueConst: 0);

							m.If(Static.Func(object.ReferenceEquals, otherTuple, m.Const<object>(null))).Then(() => {
								m.ReturnConst(1);
							});

							m.This<TT.TPrimary>().Members.SelectAllProperties().ForEach(prop => {
								EmitPropertyComparison(m, prop, otherTuple, compareResult);
							});

							m.ReturnConst(0);
						});
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			private static void EmitPropertyComparison(
				IHappilMethodBody<int> m, 
				PropertyInfo prop, 
				HappilLocal<TypeTemplate.TPrimary> otherTuple, 
				HappilLocal<int> compareResult)
			{
				var backingField = m.This<TT.TPrimary>().BackingFieldOf<TT.TProperty>(prop);
				var otherValue = m.Local(initialValue: otherTuple.Prop<TT.TProperty>(prop));

				if ( backingField.OperandType.IsValueType )
				{
					m.If(backingField > otherValue).Then(() => {
						m.ReturnConst(1);
					})
					.ElseIf(backingField < otherValue).Then(() => {
						m.ReturnConst(-1);
					})
					.ElseIf(backingField != otherValue).Then(() => {
						m.ReturnConst(1);
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

