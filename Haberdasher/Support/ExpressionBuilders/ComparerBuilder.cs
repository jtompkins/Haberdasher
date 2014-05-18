using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Haberdasher.Support.ExpressionBuilders
{
	public class ComparerBuilder
	{
		public static Func<object, object, bool> Build(Type classType, PropertyInfo property) {
			var left = Expression.Parameter(typeof(object), "left");
			var right = Expression.Parameter(typeof(object), "right");

			var castLeft = Expression.Convert(left, classType);
			var castRight = Expression.Convert(right, classType);

			var body = Expression.Equal(Expression.Property(castLeft, property.Name),
										Expression.Property(castRight, property.Name));

			return Expression.Lambda<Func<object, object, bool>>(body, left, right).Compile();
		}
	}
}
