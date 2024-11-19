using System;
using System.Linq;
using System.Linq.Expressions;

namespace ODataQuery.Nodes;

sealed class FunctionMapper(string name, Func<Expression[], Expression> mapper, Type[] argTypes = null)
{
  public string Name => name;

  public Expression Call(Expression instance, Node[] args)
  {
    var exprs = new Expression[args.Length];

    for (int i = 0; i < args.Length; ++i)
    {
      exprs[i] = argTypes?[i] is { } t
        ? args[i].ToExpression(instance, t)
        : args[i].ToExpression(instance);
    }

    return mapper(exprs);
  }
}

sealed class FuncNode(FunctionMapper mapper, params Node[] args) : Node
{
  public override Expression ToExpression(Expression instance)
    => mapper.Call(instance, args);

  public override string ToString()
    => $"Func.{mapper.Name}[{string.Join(",", args.Select(a => a.ToString()))}]";
}