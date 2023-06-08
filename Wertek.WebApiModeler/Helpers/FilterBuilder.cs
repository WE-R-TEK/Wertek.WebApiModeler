using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Wertek.WebApiModeler.Models;

namespace Wertek.WebApiModeler.Helpers;

public static class FilterBuilder
{
    public static Expression<Func<T, bool>>? ConfigureFilterQuery<T>(Clause clause)
    {
        //         Expression<Func<TEntity, bool>> leftexp = a => EF.Property<object>(a!, "aaa") == Convert.ChangeType("bbb", typeof(decimal));
        //         Expression<Func<TEntity, bool>> leftexp2 = a => EF.Property<object>(a!, "aaa") == Convert.ChangeType("bbb", typeof(decimal));
        //         Expression binaryexp = Expression.And(leftexp.Body, leftexp2.Body);
        //         ParameterExpression[] parameters = new ParameterExpression[1] {
        //     Expression.Parameter(typeof(TEntity), leftexp.Parameters.First().Name)
        // };
        //Expression<Func<TEntity, bool>> lambdaExp = Expression.Lambda<Func<TEntity, bool>>(binaryexp, parameters);
        //        query.Where(lambdaExp);

        Type propertyType = typeof(T).GetProperty(clause.Field)?.PropertyType ?? new object().GetType();

        switch (clause.Operator)
        {
            case Operators.Equal:
                return GetEqualQuery<T>(clause.Field, clause.Value, propertyType);
            case Operators.NotEqual:
                return GetNotEqualQuery<T>(clause.Field, clause.Value, propertyType);
            case Operators.GreaterThan:
                return GetGreaterThanQuery<T>(clause.Field, clause.Value, propertyType);
            case Operators.GreaterThanOrEqual:
                return GetGreaterThanOrEqualQuery<T>(clause.Field, clause.Value, propertyType);
            case Operators.LessThan:
                return GetLessThanQuery<T>(clause.Field, clause.Value, propertyType);
            case Operators.LessThanOrEqual:
                return GetLessThanOrEqualQuery<T>(clause.Field, clause.Value, propertyType);
            case Operators.Contains:
                return GetContainsQuery<T>(clause.Field, clause.Value, propertyType);
            case Operators.StartsWith:
                return GetStartsWithQuery<T>(clause.Field, clause.Value, propertyType);
            case Operators.EndsWith:
                return GetEndsWithQuery<T>(clause.Field, clause.Value, propertyType);
            case Operators.In:
                return GetInQuery<T>(clause.Field, clause.Value, propertyType);
            case Operators.NotIn:
                return GetNotInQuery<T>(clause.Field, clause.Value, propertyType);
            case Operators.Between:
                return GetBetweenQuery<T>(clause.Field, clause.Value, propertyType);
            case Operators.NotBetween:
                return GetNotBetweenQuery<T>(clause.Field, clause.Value, propertyType);
            case Operators.IsNull:
                return a => EF.Property<object>(a!, clause.Field) == null;
            case Operators.IsNotNull:
                return a => EF.Property<object>(a!, clause.Field) != null;
            default:
                return null;
        }
    }

    private static Expression<Func<T, bool>>? GetEqualQuery<T>(string propertyName, object value, Type propertyType)
    {
        switch (Type.GetTypeCode(propertyType)) {
            case TypeCode.String:
                return a => EF.Property<string>(a!, propertyName) == Convert.ToString(value);
            case TypeCode.Int32:
                return a => EF.Property<int>(a!, propertyName) == Convert.ToInt32(value);
            case TypeCode.Int64:
                return a => EF.Property<long>(a!, propertyName) == Convert.ToInt64(value);
            case TypeCode.Decimal:
                return a => EF.Property<decimal>(a!, propertyName) == Convert.ToDecimal(value);
            case TypeCode.Double:
                return a => EF.Property<double>(a!, propertyName) == Convert.ToDouble(value);
            case TypeCode.Single:
                return a => EF.Property<float>(a!, propertyName) == Convert.ToSingle(value);
            case TypeCode.DateTime:
                return a => EF.Property<DateTime>(a!, propertyName) == Convert.ToDateTime(value);
            case TypeCode.Object when propertyType == typeof(DateTimeOffset):
                return a => EF.Property<DateTimeOffset>(a!, propertyName) == Convert.ToDateTime(value);
            default:
                return null;

        }
    }

    private static Expression<Func<T, bool>>? GetNotEqualQuery<T>(string propertyName, object value, Type propertyType)
    {
        switch (Type.GetTypeCode(propertyType)) {
            case TypeCode.String:
                return a => EF.Property<string>(a!, propertyName) != Convert.ToString(value);
            case TypeCode.Int32:
                return a => EF.Property<int>(a!, propertyName) != Convert.ToInt32(value);
            case TypeCode.Int64:
                return a => EF.Property<long>(a!, propertyName) != Convert.ToInt64(value);
            case TypeCode.Decimal:
                return a => EF.Property<decimal>(a!, propertyName) != Convert.ToDecimal(value);
            case TypeCode.Double:
                return a => EF.Property<double>(a!, propertyName) != Convert.ToDouble(value);
            case TypeCode.Single:
                return a => EF.Property<float>(a!, propertyName) != Convert.ToSingle(value);
            case TypeCode.DateTime:
                return a => EF.Property<DateTime>(a!, propertyName) != Convert.ToDateTime(value);
            case TypeCode.Object when propertyType == typeof(DateTimeOffset):
                return a => EF.Property<DateTimeOffset>(a!, propertyName) != Convert.ToDateTime(value);
            default:
                return null;

        }
    }

    private static Expression<Func<T, bool>>? GetGreaterThanQuery<T>(string propertyName, object value, Type propertyType)
    {
        switch (Type.GetTypeCode(propertyType)) {
            case TypeCode.Int32:
                return a => EF.Property<int>(a!, propertyName) > Convert.ToInt32(value);
            case TypeCode.Int64:
                return a => EF.Property<long>(a!, propertyName) > Convert.ToInt64(value);
            case TypeCode.Decimal:
                return a => EF.Property<decimal>(a!, propertyName) > Convert.ToDecimal(value);
            case TypeCode.Double:
                return a => EF.Property<double>(a!, propertyName) > Convert.ToDouble(value);
            case TypeCode.Single:
                return a => EF.Property<float>(a!, propertyName) > Convert.ToSingle(value);
            case TypeCode.DateTime:
                return a => EF.Property<DateTime>(a!, propertyName) > Convert.ToDateTime(value);
            case TypeCode.Object when propertyType == typeof(DateTimeOffset):
                return a => EF.Property<DateTimeOffset>(a!, propertyName) > Convert.ToDateTime(value);
            default:
                return null;

        }
    }

    private static Expression<Func<T, bool>>? GetGreaterThanOrEqualQuery<T>(string propertyName, object value, Type propertyType)
    {
        switch (Type.GetTypeCode(propertyType)) {
            case TypeCode.Int32:
                return a => EF.Property<int>(a!, propertyName) >= Convert.ToInt32(value);
            case TypeCode.Int64:
                return a => EF.Property<long>(a!, propertyName) >= Convert.ToInt64(value);
            case TypeCode.Decimal:
                return a => EF.Property<decimal>(a!, propertyName) >= Convert.ToDecimal(value);
            case TypeCode.Double:
                return a => EF.Property<double>(a!, propertyName) >= Convert.ToDouble(value);
            case TypeCode.Single:
                return a => EF.Property<float>(a!, propertyName) >= Convert.ToSingle(value);
            case TypeCode.DateTime:
                return a => EF.Property<DateTime>(a!, propertyName) >= Convert.ToDateTime(value);
            case TypeCode.Object when propertyType == typeof(DateTimeOffset):
                return a => EF.Property<DateTimeOffset>(a!, propertyName) >= Convert.ToDateTime(value);
            default:
                return null;

        }
    }

    private static Expression<Func<T, bool>>? GetLessThanQuery<T>(string propertyName, object value, Type propertyType)
    {
        switch (Type.GetTypeCode(propertyType)) {
            case TypeCode.Int32:
                return a => EF.Property<int>(a!, propertyName) < Convert.ToInt32(value);
            case TypeCode.Int64:
                return a => EF.Property<long>(a!, propertyName) < Convert.ToInt64(value);
            case TypeCode.Decimal:
                return a => EF.Property<decimal>(a!, propertyName) < Convert.ToDecimal(value);
            case TypeCode.Double:
                return a => EF.Property<double>(a!, propertyName) < Convert.ToDouble(value);
            case TypeCode.Single:
                return a => EF.Property<float>(a!, propertyName) < Convert.ToSingle(value);
            case TypeCode.DateTime:
                return a => EF.Property<DateTime>(a!, propertyName) < Convert.ToDateTime(value);
            case TypeCode.Object when propertyType == typeof(DateTimeOffset):
                return a => EF.Property<DateTimeOffset>(a!, propertyName) < Convert.ToDateTime(value);
            default:
                return null;

        }
    }

    private static Expression<Func<T, bool>>? GetLessThanOrEqualQuery<T>(string propertyName, object value, Type propertyType)
    {
        switch (Type.GetTypeCode(propertyType)) {
            case TypeCode.Int32:
                return a => EF.Property<int>(a!, propertyName) <= Convert.ToInt32(value);
            case TypeCode.Int64:
                return a => EF.Property<long>(a!, propertyName) <= Convert.ToInt64(value);
            case TypeCode.Decimal:
                return a => EF.Property<decimal>(a!, propertyName) <= Convert.ToDecimal(value);
            case TypeCode.Double:
                return a => EF.Property<double>(a!, propertyName) <= Convert.ToDouble(value);
            case TypeCode.Single:
                return a => EF.Property<float>(a!, propertyName) <= Convert.ToSingle(value);
            case TypeCode.DateTime:
                return a => EF.Property<DateTime>(a!, propertyName) <= Convert.ToDateTime(value);
            case TypeCode.Object when propertyType == typeof(DateTimeOffset):
                return a => EF.Property<DateTimeOffset>(a!, propertyName) <= Convert.ToDateTime(value);
            default:
                return null;

        }
    }

    private static Expression<Func<T, bool>>? GetContainsQuery<T>(string propertyName, object value, Type propertyType)
    {
        switch (Type.GetTypeCode(propertyType)) {
            case TypeCode.String:
                return a => EF.Property<string>(a!, propertyName).Contains(Convert.ToString(value) ?? "");
            default:
                return null;

        }
    }

    private static Expression<Func<T, bool>>? GetStartsWithQuery<T>(string propertyName, object value, Type propertyType)
    {
        switch (Type.GetTypeCode(propertyType)) {
            case TypeCode.String:
                return a => EF.Property<string>(a!, propertyName).StartsWith(Convert.ToString(value) ?? "");
            default:
                return null;

        }
    }

    private static Expression<Func<T, bool>>? GetEndsWithQuery<T>(string propertyName, object value, Type propertyType)
    {
        switch (Type.GetTypeCode(propertyType)) {
            case TypeCode.String:
                return a => EF.Property<string>(a!, propertyName).EndsWith(Convert.ToString(value) ?? "");
            default:
                return null;

        }
    }

    private static Expression<Func<T, bool>>? GetInQuery<T>(string propertyName, object value, Type propertyType)
    {
        var values = (value.ToString() ?? "").Split(',');
        switch (Type.GetTypeCode(propertyType)) {
            case TypeCode.Int32:
                return a => values.Contains(EF.Property<int>(a!, propertyName).ToString());
            case TypeCode.Int64:
                return a => values.Contains(EF.Property<long>(a!, propertyName).ToString());
            case TypeCode.Decimal:
                return a => values.Contains(EF.Property<decimal>(a!, propertyName).ToString());
            case TypeCode.Double:
                return a => values.Contains(EF.Property<double>(a!, propertyName).ToString());
            case TypeCode.Single:
                return a => values.Contains(EF.Property<float>(a!, propertyName).ToString());
            case TypeCode.DateTime:
                return a => values.Contains(EF.Property<DateTime>(a!, propertyName).ToString());
            case TypeCode.Object when propertyType == typeof(DateTimeOffset):
                return a => values.Contains(EF.Property<DateTimeOffset>(a!, propertyName).ToString());
            default:
                return null;

        }
    }
    private static Expression<Func<T, bool>>? GetNotInQuery<T>(string propertyName, object value, Type propertyType)
    {
        var values = (value.ToString() ?? "").Split(',');
        switch (Type.GetTypeCode(propertyType)) {
            case TypeCode.Int32:
                return a => !values.Contains(EF.Property<int>(a!, propertyName).ToString());
            case TypeCode.Int64:
                return a => !values.Contains(EF.Property<long>(a!, propertyName).ToString());
            case TypeCode.Decimal:
                return a => !values.Contains(EF.Property<decimal>(a!, propertyName).ToString());
            case TypeCode.Double:
                return a => !values.Contains(EF.Property<double>(a!, propertyName).ToString());
            case TypeCode.Single:
                return a => !values.Contains(EF.Property<float>(a!, propertyName).ToString());
            case TypeCode.DateTime:
                return a => !values.Contains(EF.Property<DateTime>(a!, propertyName).ToString());
            case TypeCode.Object when propertyType == typeof(DateTimeOffset):
                return a => !values.Contains(EF.Property<DateTimeOffset>(a!, propertyName).ToString());
            default:
                return null;

        }
    }

    private static Expression<Func<T, bool>>? GetBetweenQuery<T>(string propertyName, object value, Type propertyType)
    {
        var values = (value.ToString() ?? "").Split(',');
        switch (Type.GetTypeCode(propertyType)) {
            case TypeCode.Int32:
                return a => EF.Property<int>(a!, propertyName) >= Convert.ToInt32(values[0]) && EF.Property<int>(a!, propertyName) <= Convert.ToInt32(values[1]);
            case TypeCode.Int64:
                return a => EF.Property<long>(a!, propertyName) >= Convert.ToInt64(values[0]) && EF.Property<long>(a!, propertyName) <= Convert.ToInt64(values[1]);
            case TypeCode.Decimal:
                return a => EF.Property<decimal>(a!, propertyName) >= Convert.ToDecimal(values[0]) && EF.Property<decimal>(a!, propertyName) <= Convert.ToDecimal(values[1]);
            case TypeCode.Double:
                return a => EF.Property<double>(a!, propertyName) >= Convert.ToDouble(values[0]) && EF.Property<double>(a!, propertyName) <= Convert.ToDouble(values[1]);
            case TypeCode.Single:
                return a => EF.Property<float>(a!, propertyName) >= Convert.ToSingle(values[0]) && EF.Property<float>(a!, propertyName) <= Convert.ToSingle(values[1]);
            case TypeCode.DateTime:
                return a => EF.Property<DateTime>(a!, propertyName) >= Convert.ToDateTime(values[0]) && EF.Property<DateTime>(a!, propertyName) <= Convert.ToDateTime(values[1]);
            case TypeCode.Object when propertyType == typeof(DateTimeOffset):
                return a => EF.Property<DateTimeOffset>(a!, propertyName) >= Convert.ToDateTime(values[0]) && EF.Property<DateTimeOffset>(a!, propertyName) <= Convert.ToDateTime(values[1]);
            default:
                return null;

        }
    }

    private static Expression<Func<T, bool>>? GetNotBetweenQuery<T>(string propertyName, object value, Type propertyType)
    {
        var values = (value.ToString() ?? "").Split(',');
        switch (Type.GetTypeCode(propertyType)) {
            case TypeCode.Int32:
                return a => EF.Property<int>(a!, propertyName) < Convert.ToInt32(values[0]) || EF.Property<int>(a!, propertyName) > Convert.ToInt32(values[1]);
            case TypeCode.Int64:
                return a => EF.Property<long>(a!, propertyName) < Convert.ToInt64(values[0]) || EF.Property<long>(a!, propertyName) > Convert.ToInt64(values[1]);
            case TypeCode.Decimal:
                return a => EF.Property<decimal>(a!, propertyName) < Convert.ToDecimal(values[0]) || EF.Property<decimal>(a!, propertyName) > Convert.ToDecimal(values[1]);
            case TypeCode.Double:
                return a => EF.Property<double>(a!, propertyName) < Convert.ToDouble(values[0]) || EF.Property<double>(a!, propertyName) > Convert.ToDouble(values[1]);
            case TypeCode.Single:
                return a => EF.Property<float>(a!, propertyName) < Convert.ToSingle(values[0]) || EF.Property<float>(a!, propertyName) > Convert.ToSingle(values[1]);
            case TypeCode.DateTime:
                return a => EF.Property<DateTime>(a!, propertyName) < Convert.ToDateTime(values[0]) || EF.Property<DateTime>(a!, propertyName) > Convert.ToDateTime(values[1]);
            case TypeCode.Object when propertyType == typeof(DateTimeOffset):
                return a => EF.Property<DateTimeOffset>(a!, propertyName) < Convert.ToDateTime(values[0]) || EF.Property<DateTimeOffset>(a!, propertyName) > Convert.ToDateTime(values[1]);
            default:
                return null;

        }
    }
}