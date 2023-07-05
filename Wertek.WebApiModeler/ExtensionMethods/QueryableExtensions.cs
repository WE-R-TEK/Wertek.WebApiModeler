using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Wertek.WebApiModeler.Helpers;
using Wertek.WebApiModeler.Models;

namespace Wertek.WebApiModeler.ExtensionMethods;

public static class QueryableExtensions
{
    public static IQueryable<T> ToFilterView<T>(
        this IQueryable<T> query, FilterDTO? filter)
    {
        //filter
        if (filter == null)
            return query;

        query = Filter(query, filter.Filters);
        //sort
        if (filter.Sort.Any())
        {
            query = Sort(query, filter.Sort);
            // EF does not apply skip and take without order

        }
        //paging
        if (filter.Page > 0 && filter.PageSize > 0)
        {
            int skip = (filter.Page - 1) * filter.PageSize;
            query = Limit(query, filter.PageSize, skip);
        }

        return query;
    }

    private static IQueryable<T> Filter<T>(
        IQueryable<T> queryable, IEnumerable<Filter> filters)
    {
        if ((filters != null))
        {
            foreach (var filter in filters)
            {
                Expression<Func<T, bool>>? binaryexp = null;

                binaryexp = GetAllFilters<T>(filter.Clauses, filter.Logic);

                if (binaryexp != null)
                {
                    queryable = queryable.Where(binaryexp);
                }
            }
        }
        return queryable;
    }

    private static Expression<Func<T, bool>>? GetAllFilters<T>(
        IEnumerable<Clause> clauses, Logic logic)
    {
        Expression<Func<T, bool>>? binaryexp = null;

        foreach (var clause in clauses)
        {
            foreach (var filter in clause.Filters)
            {
                binaryexp = GetAllFilters<T>(filter.Clauses, filter.Logic);
            }

            var newexp = FilterBuilder.ConfigureFilterQuery<T>(clause);

            if(newexp != null)
                if (binaryexp == null)
                {
                    binaryexp = newexp;
                }
                else
                {
                    if (logic == Logic.And)
                        binaryexp = AndAlso<T>(binaryexp, newexp);
                    else
                        binaryexp = OrElse(binaryexp, newexp);
                }
        }
        return binaryexp;
    }

    public static Expression<Func<T, bool>> AndAlso<T>(
    this Expression<Func<T, bool>> expr1,
    Expression<Func<T, bool>> expr2)
    {
        // need to detect whether they use the same
        // parameter instance; if not, they need fixing
        ParameterExpression param = expr1.Parameters[0];
        if (ReferenceEquals(param, expr2.Parameters[0]))
        {
            // simple version
            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(expr1.Body, expr2.Body), param);
        }
        // otherwise, keep expr1 "as is" and invoke expr2
        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(
                expr1.Body,
                Expression.Invoke(expr2, param)), param);
    }

    public static Expression<Func<T, bool>> OrElse<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        // need to detect whether they use the same
        // parameter instance; if not, they need fixing
        ParameterExpression param = expr1.Parameters[0];
        if (ReferenceEquals(param, expr2.Parameters[0]))
        {
            // simple version
            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(expr1.Body, expr2.Body), param);
        }
        // otherwise, keep expr1 "as is" and invoke expr2
        return Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(
                expr1.Body,
                Expression.Invoke(expr2, param)), param);
    }

    private static IQueryable<T> Sort<T>(IQueryable<T> queryable, IEnumerable<Sort> sort)
    {
        IOrderedQueryable<T>? orderedQuery = null;
        if (sort != null)
        {
            foreach (var orderByProperty in sort)
            {
                if(orderByProperty.Capitalize == true)
                    orderByProperty.Field = orderByProperty.Field.Capitalize();
                if (orderByProperty.Dir == SortDirection.Desc)
                {
                    if (orderedQuery != null)
                    {
                        orderedQuery = orderedQuery.ThenByDescending(a => EF.Property<object>(a!, orderByProperty.Field));
                    }
                    else
                    {
                        orderedQuery = queryable.OrderByDescending(a => EF.Property<object>(a!, orderByProperty.Field));
                    }
                }
                else
                {
                    if (orderedQuery != null)
                    {
                        orderedQuery = orderedQuery.ThenBy(a => EF.Property<object>(a!, orderByProperty.Field));
                    }
                    else
                    {
                        orderedQuery = queryable.OrderBy(a => EF.Property<object>(a!, orderByProperty.Field));
                    }
                }

            }
        }
        return orderedQuery ?? queryable;
    }

    private static IQueryable<T> Limit<T>(
      IQueryable<T> queryable, int limit, int offset)
    {
        return queryable.Skip(offset).Take(limit);
    }
}
