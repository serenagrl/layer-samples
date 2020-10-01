using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LeaveSample.Framework
{
    /// <summary>
    /// Dynamic expressions to extend the LINQ lambda methods.
    /// To learn more, refer to 
    /// http://msdn.microsoft.com/en-us/library/vstudio/bb882637.aspx
    /// </summary>
    public static class DynamicExpressions
    {
        /// <summary>
        /// Extension method to support dynamic sorting.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="source">The entity list.</param>
        /// <param name="statement">The order by statement.</param>
        /// <returns></returns>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string statement)
        {
            // NOTE: Currently only supports sorting of 1 column.
            string method = string.Empty;
            string[] parts = statement.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Determine sort order.
            method = (parts.Length > 1 && parts[1].ToUpper() == "DESC") ?
                        "OrderByDescending" :"OrderBy";

            // Create dynamic expression.
            var type = typeof(T);
            var property = type.GetProperty(parts[0]);
            var parameter = Expression.Parameter(type, "param");
            var member = Expression.MakeMemberAccess(parameter, property);
            var lambda = Expression.Lambda(member, parameter);
            
            var finalExpression = Expression.Call(typeof(Queryable), method, 
                                    new Type[] { type, property.PropertyType },
                                    source.Expression, Expression.Quote(lambda));

            return source.Provider.CreateQuery<T>(finalExpression);
        }
    }
}
