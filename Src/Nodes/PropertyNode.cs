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
    private readonly Node @this;
    private readonly MethodInfo dtProperty;
    private readonly MethodInfo dtoProperty;
    private readonly MethodInfo doProperty;

    public DatePropertyNode(Node @this, MethodInfo dtProperty, MethodInfo dtoProperty, MethodInfo doProperty = null)
    {
      this.@this = @this;
      this.dtProperty = dtProperty;
      this.dtoProperty = dtoProperty;
      this.doProperty = doProperty;
    }

    public override Expression ToExpression(Expression instance)
    {
      var target = @this.ToExpression(instance);
      return Expression.Property(
        target,
        target.Type == typeof(DateTime) ? dtProperty
          : target.Type == typeof(DateTimeOffset) ? dtoProperty
          : target.Type == typeof(DateOnly) && doProperty != null ? doProperty
          : throw new InvalidOperationException("Date/time related function cannot be applied to parameter of type " + target.Type));
    }
  }
}