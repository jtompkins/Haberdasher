using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Haberdasher.Support.ExpressionBuilders
{
	public class SetterBuilder
	{
		public static Action<object, object> Build(Type classType, PropertyInfo property) {
			var instance = Expression.Parameter(typeof(object), "instance");
			var value = Expression.Parameter(typeof(object), "value");

			// value as T is slightly faster than (T)value, so if it's not a value type, use that
			var instanceCast = (!property.DeclaringType.IsValueType) ? Expression.TypeAs(instance, property.DeclaringType) : Expression.Convert(instance, property.DeclaringType);
			var valueCast = (!property.PropertyType.IsValueType) ? Expression.TypeAs(value, property.PropertyType) : Expression.Convert(value, property.PropertyType);

			var setMethod = property.GetSetMethod();

			if (setMethod == null)
				return null;

			var call = Expression.Call(instanceCast, setMethod, valueCast);

			return Expression.Lambda<Action<object, object>>(call, new[] { instance, value }).Compile();
		}
	}
}
