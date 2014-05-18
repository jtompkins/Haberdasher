using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Haberdasher.Support.ExpressionBuilders
{
	public class GetterBuilder
	{
		public static Func<object, object> Build(PropertyInfo property) {
			var classType = property.DeclaringType;

			if (classType == null)
				return null;

			var model = Expression.Parameter(typeof(object), "model");

			var body = Expression.Property(Expression.Convert(model, classType), property.Name);

			var convertedBody = Expression.Convert(body, typeof (object));

			return Expression.Lambda<Func<object, object>>(convertedBody, model).Compile();
		}
	}
}
