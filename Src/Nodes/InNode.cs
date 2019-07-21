using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ODataQuery.Nodes
{
  sealed class InNode : Node
  {
    private readonly Node left;
    private readonly List<Node> right;

    public InNode(Node left, IEnumerable<Node> right)
    {
      this.left = left;
      this.right = right.AsList();
    }

    private static readonly MethodInfo createListMethod = typeof(InNode).GetMethod(nameof(CreateList));

    public override Expression ToExpression(Expression instance)
    {
      // Empty list is not supported in OData grammar
      if (right.Count == 0) return Expression.Constant(false);

      var valueExpr = left.ToExpression(instance);
      var type = valueExpr.Type;
      var list = createListMethod.MakeGenericMethod(type).Invoke(this, null);
      var contains = list.GetType().GetMethod(nameof(List<object>.Contains), new[] { type });
      return Expression.Call(Expression.Constant(list),
                             contains,
                             valueExpr);
    }

    public List<T> CreateList<T>() =>
      right.Cast<ConstantNode>()
           .Select(c => (T)c.As(typeof(T)))
           .ToList();
  }
}