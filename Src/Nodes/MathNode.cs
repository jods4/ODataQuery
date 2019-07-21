using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ODataQuery.Nodes
{
  sealed class MathNode : Node
  {
    private readonly string methodName;
    private readonly Node arg;

    public MathNode(string methodName, Node arg)
    {
      this.methodName = methodName;
      this.arg = arg;
    }

    public override Expression ToExpression(Expression instance)
    {
      var arg = this.arg.ToExpression(instance);
      var method = typeof(Math).GetMethod(methodName, BindingFlags.Public | BindingFlags.Static, null, new[] { arg.Type }, null);
      return Expression.Call(null, method, arg);
    }
  }
}