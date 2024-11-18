using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ODataQuery.Nodes
{
  sealed class MethodNode : Node
  {
    private readonly MethodInfo method;
    private readonly Node @this;
    private readonly Node[] args;

    public MethodNode(MethodInfo method, Node @this, params Node[] args)
    {
      this.method = method;
      this.@this = @this;
      this.args = args;
    }

    public override Expression ToExpression(Expression instance)
    {
      var parameters = method.GetParameters();
      return Expression.Call(@this?.ToExpression(instance),
                             method,
                             args.Select((x, i) => x.ToExpression(instance, parameters[i].ParameterType)));
    }

    public override string ToString()
    {
      var list = new [] { method.Name }
                  .Concat(@this == null ? new string[0] : new [] { @this.ToString() })
                  .Concat(args.Select(a => a.ToString()));
      return $"Func[{string.Join(",", list)}]";
    }
  }
}