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
  public sealed class ODataQueryableAttribute : ResultFilterAttribute
  {
    public override void OnResultExecuting(ResultExecutingContext context)
    {
      // First check if we have a result and if all is good
      var result = context.Result as ObjectResult;
      if (result?.Value == null ||                              // Do we even have a value?
          !(result.Value is IQueryable) ||                      // Quick bail-out as IQueryable<T> implements IQueryable
          result.StatusCode < 200 || result.StatusCode >= 300)  // Only HTTP 2xx range means success
        return;

      var type = FindIQueryableOf(result.Value);
      if (type == null) return; // Might be IQueryable but not IQueryable<T>

      var method = applyODataMethod.MakeGenericMethod(type);
      result.Value = method.Invoke(null, new object[] { result.Value, context.HttpContext.Request.Query });
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

    private static readonly MethodInfo applyODataMethod = typeof(ODataQueryableAttribute).GetMethod(nameof(ApplyOData), BindingFlags.NonPublic | BindingFlags.Static);

    private static object ApplyOData<T>(IQueryable<T> source, IQueryCollection query)
    {
      var value = source.OData(query, out var count);
      return count < 0 ?
        new SortedList<string, object> { ["value"] = value } :
        new SortedList<string, object> { ["@odata.count"] = count, ["value"] = value };
    }
  }
}