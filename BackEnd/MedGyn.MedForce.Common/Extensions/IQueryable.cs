
using System;
using System.Linq;
using System.Linq.Expressions;

public static class IQueryableExtensions
{

	// https://stackoverflow.com/questions/16208214/construct-lambdaexpression-for-nested-property-from-string/16208620#16208620
	public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> query, string key, bool ascending = true)
	{
		if (string.IsNullOrWhiteSpace(key))
		{
			return query;
		}

		var lambda = (dynamic)CreateExpression(typeof(TSource), key);

		return ascending
			? Queryable.OrderBy(query, lambda)
			: Queryable.OrderByDescending(query, lambda);
	}

	private static LambdaExpression CreateExpression(Type type, string propertyName)
	{
		var param = Expression.Parameter(type, "x");
		var props = type.GetProperties().Select(x => x.Name.ToLower()).ToHashSet();

		Expression body = param;
		foreach (var member in propertyName.Split('.'))
		{
			if(props.Contains(member.ToLower()))
				body = Expression.PropertyOrField(body, member);
		}

		return Expression.Lambda(body, param);
	}
}

