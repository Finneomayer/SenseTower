using MongoDB.Driver;
using SC.SenseTower.Common.Data;
using System.Linq.Expressions;

namespace SC.SenseTower.Common.Extensions
{
    public static class RequestConditionsExtensions
    {
        public static SortDefinition<T> ToDefinitions<T>(this QuerySorting[] sortings)
        {
            var definitions = new List<SortDefinition<T>>();
            foreach (var sorting in sortings.OrderBy(x => x.SortOrder))
            {
                var member = sorting.PropertyName.ToMemberOf<T>();
                definitions.Add(sorting.Ascending ? Builders<T>.Sort.Ascending(member) : Builders<T>.Sort.Descending(member));
            }
            return Builders<T>.Sort.Combine(definitions);
        }

        private static Expression<Func<T, object>> ToMemberOf<T>(this string name)
        {
            var parameter = Expression.Parameter(typeof(T), "_s");
            var propertyOrField = name.Split('.').Aggregate((Expression)parameter, Expression.PropertyOrField);
            var unaryExpression = Expression.MakeUnary(ExpressionType.Convert, propertyOrField, typeof(object));
            return Expression.Lambda<Func<T, object>>(unaryExpression, parameter);
        }
    }
}
