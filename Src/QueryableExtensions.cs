using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Pidgin;
using ODataQuery.Parsers;
using System.Collections.Generic;
using ODataQuery.Nodes;

namespace ODataQuery
{
  public static class QueryableExtensions
  {
    public static IQueryable<T> OData<T>(this IQueryable<T> source, IQueryCollection query) => OData(source, query, out _, false);

    public static IQueryable<T> OData<T>(this IQueryable<T> source, IQueryCollection query, out int count) => OData(source, query, out count, true);

    internal static IQueryable ODataSelect<T>(this IQueryable<T> source, IQueryCollection query, out int count)
    {
      var result = source.OData(query, out count, true);

      var select = query.GetODataOption("$select");
      return select != null ? result.ODataSelect(select) : result;
    }

    private static IQueryable<T> OData<T>(this IQueryable<T> source, IQueryCollection query, out int count, bool performCount)
    {
      count = -1;
      var result = source;

      var filter = query.GetODataOption("$filter");
      result = result.ODataFilter(filter);

      if (performCount && query.GetODataOption<bool>("$count"))
        count = result.Count();

      var orderby = query.GetODataOption("$orderby");
      result = result.ODataOrderBy(orderby);

      var skip = query.GetODataOption<int>("$skip");
      if (skip > 0)
        result = result.Skip(skip);

      var top = query.GetODataOption<int>("$top");
      // $take is a synonym of $top for backward compatibility,
      // as this package incorrectly used $take until v1.1.
      if (top == 0) top = query.GetODataOption<int>("$take");
      if (top > 0)
        result = result.Take(top);

      return result;
    }

    public static string GetODataOption(this IQueryCollection query, string option)
    {
      var result = query[option];
      if (result.Count > 1)
        throw new InvalidOperationException($"Query contains {option} more than once.");
      return result.Count == 1 ? result[0] : null;
    }

    public static T GetODataOption<T>(this IQueryCollection query, string option)
    {
      var raw = GetODataOption(query, option);
      return raw == null ? default : (T)Convert.ChangeType(raw, typeof(T));
    }

    public static IQueryable<T> ODataFilter<T>(this IQueryable<T> source, string filter)
    {
      if (filter == null) return source;

      var node = ParseFilter(filter);
      var parameter = Expression.Parameter(typeof(T));
      var body = node.ToExpression(parameter);
      var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
      return source.Where(lambda);
    }

    private static Node ParseFilter(string input) =>
      Logical.LogicalExpr
             .Before(Parser<char>.End)
             .ParseOrThrow(input);

    public static IQueryable<T> ODataOrderBy<T>(this IQueryable<T> source, string orderby)
    {
      if (orderby == null) return source;

      var terms = OrderBy.Parser.ParseOrThrow(orderby);
      var parameter = Expression.Parameter(typeof(T));

      var invokeParams = new object[] { source, null, /*first:*/true, null };
      IOrderedQueryable<T> orderedSource = null;
      foreach (var term in terms)
      {
        var body = term.node.ToExpression(parameter);
        invokeParams[1] = Expression.Lambda(body, parameter);
        invokeParams[3] = term.asc;
        orderedSource = (IOrderedQueryable<T>)applyMethod.MakeGenericMethod(typeof(T), body.Type).Invoke(null, invokeParams);
        invokeParams[0] = orderedSource;
        invokeParams[2] = /*first:*/false;
      }

      return orderedSource ?? source;
    }

    private static MethodInfo applyMethod = typeof(QueryableExtensions).GetMethod(nameof(ApplyOrderBy), BindingFlags.NonPublic | BindingFlags.Static);

    private static IOrderedQueryable<T> ApplyOrderBy<T, TResult>(IQueryable<T> source, Expression<Func<T, TResult>> lambda, bool first, bool asc)
    {
      if (first)
        return asc ? source.OrderBy(lambda) : source.OrderByDescending(lambda);
      else
      {
        var orderedSource = (IOrderedQueryable<T>)source;
        return asc ? orderedSource.ThenBy(lambda) : orderedSource.ThenByDescending(lambda);
      }
    }

    public static IQueryable FlatSelect<T>(this IQueryable<T> source, string select)
    {
      // Returns an array of a single property (flatten select)
      var property = Literals.Identifier.ParseOrThrow(select);
      var parameter = Expression.Parameter(typeof(T));
      var body = property.ToExpression(parameter);
      var lambda = Expression.Lambda(body, parameter);
      return (IQueryable)selectMethod.MakeGenericMethod(typeof(T), lambda.ReturnType).Invoke(null, new object[] { source, lambda });
    }

    private static MethodInfo selectMethod = typeof(QueryableExtensions).GetMethod(nameof(ApplySelect), BindingFlags.NonPublic | BindingFlags.Static);

    private static IQueryable ApplySelect<T, R>(IQueryable<T> source, Expression<Func<T, R>> selector) => source.Select(selector);

    private static readonly MethodInfo selectAddMethod = typeof(SelectDictionary).GetMethod(nameof(SelectDictionary.Add), new[] { typeof(string), typeof(object) });

    internal static IQueryable<IDictionary<string, object>> ODataSelect<T>(this IQueryable<T> source, string select)
    {
      var props = Select.Parser.ParseOrThrow(select).Cast<IdentifierNode>();
      var parameter = Expression.Parameter(typeof(T));

      var init = props.Select(p =>
        Expression.ElementInit(
          selectAddMethod,
          Expression.Constant(p.Name),
          Expression.Convert(p.ToExpression(parameter), typeof(object))));

      var expr = Expression.Lambda<Func<T, IDictionary<string, object>>>(
        Expression.ListInit(Expression.New(typeof(SelectDictionary)), init),
        parameter);

      return source.Select(expr);
    }
  }
}