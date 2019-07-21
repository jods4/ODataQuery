using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ODataQuery.Nodes
{
  sealed class PropertyNode : Node
  {
    private readonly MethodInfo getter;
    private readonly Node @this;

    public PropertyNode(MethodInfo property, Node @this)
    {
      this.getter = property;
      this.@this = @this;
    }

    public override Expression ToExpression(Expression instance)
      => Expression.Property(@this?.ToExpression(instance), getter);
  }

  sealed class DatePropertyNode : Node
  {
    private readonly MethodInfo dtProperty;
    private readonly MethodInfo dtoProperty;
    private readonly Node @this;

    public DatePropertyNode(MethodInfo dtProperty, MethodInfo dtoProperty, Node @this)
    {
      this.dtProperty = dtProperty;
      this.dtoProperty = dtoProperty;
      this.@this = @this;
    }

    public override Expression ToExpression(Expression instance)
    {
      var target = @this.ToExpression(instance);
      return Expression.Property(target, target.Type == typeof(DateTime) ? dtProperty : dtoProperty);
    }
  }
}