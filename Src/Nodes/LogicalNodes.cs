using System.Linq.Expressions;

namespace ODataQuery.Nodes
{
  enum LogicOperator { None, And, Or }

  sealed class LogicalNode : Node
  {
    public Node Left { get; }
    public Node Right { get; }
    public LogicOperator Operator { get; }

    public LogicalNode(LogicOperator op, Node left, Node right)
    {
      Operator = op;
      Left = left;
      Right = right;
    }

    public override Expression ToExpression(Expression instance)
    {
      var left = Left.ToExpression(instance);
      var right = Right.ToExpression(instance);
      return Operator == LogicOperator.And ?
        Expression.AndAlso(left, right) :
        Expression.OrElse(left, right);
    }

    public override string ToString() => $"{Operator}[{Left},{Right}]";
  }

  sealed class NotNode : Node
  {
    public Node Child { get; }

    public NotNode(Node child)
    {
      Child = child;
    }

    public override Expression ToExpression(Expression instance) => Expression.Not(Child.ToExpression(instance));

    public override string ToString() => $"Not[{Child}]";
  }
}