using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ODataQuery
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public abstract class QueryableResultFilterAttribute : ResultFilterAttribute, IActionFilter
  {
    public void OnActionExecuting(ActionExecutingContext context)
    {
      if (context.HttpContext.Request.Query.TryGetValue("$search", out var values))
        context.ActionArguments["search"] = values.First();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    { }

    public override void OnResultExecuting(ResultExecutingContext context)
    {
      // First check if we have a result and if all is good
      if (context.Result is not ObjectResult result ||
          result.Value is not IQueryable ||             // Quick bail-out as IQueryable<T> implements IQueryable
          result.StatusCode is (< 200 or >= 300))       // Only HTTP 2xx range means success, Caution: null is possible (means 200 by default)
        return;

      var type = FindIQueryableOf(result.Value);
      if (type == null) return; // Might be IQueryable but not IQueryable<T>

      var method = applyODataMethod.MakeGenericMethod(type);
      result.Value = method.Invoke(this, new[] { result.Value, context.HttpContext.Request.Query });
    }

    private Type FindIQueryableOf(object target)
    {
      foreach (var interfaceType in target.GetType().GetInterfaces())
      {
        if (interfaceType.IsGenericType &&
            interfaceType.GetGenericTypeDefinition() == typeof(IQueryable<>))
          return interfaceType.GetGenericArguments()[0];
      }
      return null;
    }

    private static readonly MethodInfo applyODataMethod = typeof(QueryableResultFilterAttribute).GetMethod(nameof(TransformResult), BindingFlags.Instance | BindingFlags.NonPublic);

    protected abstract object TransformResult<T>(IQueryable<T> source, IQueryCollection query);
  }

  public sealed class ODataQueryableAttribute : QueryableResultFilterAttribute
  {
    protected override object TransformResult<T>(IQueryable<T> source, IQueryCollection query)
    {
      var value = source.ODataSelect(query, out var count);
      return count < 0 ?
        new SortedList<string, object> { ["value"] = value } :
        new SortedList<string, object> { ["@odata.count"] = count, ["value"] = value };
    }
  }
}