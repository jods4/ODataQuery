using System;
using System.Linq.Expressions;

namespace ODataQuery.Nodes
{
  enum Comparator { None, Eq, Ne, Lt, Le, Gt, Ge }

  sealed class ComparisonNode : Node
  {
    public Node Left { get; }
    public Node Right { get; }
    public Comparator Comparator { get; }

    public ComparisonNode(Comparator comparator, Node left, Node right)
    {
      Comparator = comparator;
      Left = left;
      Right = right;
    }

    public override Expression ToExpression(Expression instance)
    {
      var left = Left.ToExpression(instance);
      var right = Right.ToExpression(instance);

      // If one side is a constant, try to convert it to the appropriate Type (we parse all numbers as decimal constants)
      if (Left is ConstantNode c1) left = c1.ToExpression(instance, right.Type);
      else if (Right is ConstantNode c2) right = c2.ToExpression(instance, left.Type);

      // If one side is nullable, but the other is not, introduce a conversion
      // An expression tree like `int? == int` does not compile
      var nullableLeft = left.Type.IsNullable(out _);
      var nullableRight = right.Type.IsNullable(out _);
      if (nullableLeft && !nullableRight)
        right = Expression.Convert(right, left.Type);
      else if (nullableRight && !nullableLeft)
        left = Expression.Convert(left, right.Type);

      switch (Comparator)
      {
        case Comparator.Eq:
          return Expression.Equal(left, right);
        case Comparator.Ne:
          return Expression.NotEqual(left, right);
        case Comparator.Lt:
          return Expression.LessThan(left, right);
        case Comparator.Le:
          return Expression.LessThanOrEqual(left, right);
        case Comparator.Gt:
          return Expression.GreaterThan(left, right);
        case Comparator.Ge:
          return Expression.GreaterThanOrEqual(left, right);
        default:
          throw new NotSupportedException("Unknown comparator: " + Comparator);
      }
    }

    public override string ToString() => $"{Comparator}[{Left},{Right}]";
  }
}