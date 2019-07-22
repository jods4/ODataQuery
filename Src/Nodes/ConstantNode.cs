using System;
using System.Linq.Expressions;

namespace ODataQuery.Nodes
{
  sealed class ConstantNode : Node
  {
    public static readonly Node True = new ConstantNode(true);
    public static readonly Node False = new ConstantNode(false);
    public static readonly Node Null = new ConstantNode(null);

    public object Value { get; }

    public ConstantNode(object value)
    {
      Value = value;
    }

    public override Expression ToExpression(Expression instance) => Expression.Constant(Value);

    public override Expression ToExpression(Expression instance, Type type) => Expression.Constant(As(type));

    public object As(Type type)
    {
      if (type.IsEnum)
      {
        if (Value is string s)
          return Enum.Parse(type, s);
        else
          return Enum.ToObject(type, Convert.ChangeType(Value, type.GetEnumUnderlyingType()));
      }

      return Convert.ChangeType(Value, type);
    }

    public override string ToString() =>
      Value is DateTime || Value is DateTimeOffset ? $"Const[{Value:s}]" :
      $"Const[{Value}]";
  }
}