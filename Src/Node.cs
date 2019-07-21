using System;
using System.Linq.Expressions;

namespace ODataQuery
{
  abstract class Node
  {
    public abstract Expression ToExpression(Expression instance);

    public virtual Expression ToExpression(Expression instance, Type type)
    {
      var result = ToExpression(instance);
      return result.Type == type ?
        result :
        Expression.Convert(result, type);
    }
  }
}