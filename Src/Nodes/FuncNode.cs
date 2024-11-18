using System;
using System.Linq;
using System.Linq.Expressions;

namespace ODataQuery.Nodes;

sealed class FunctionMapper(string name, Func<Expression[], Expression> mapper, Type[] argTypes = null)
{
  public string Name => name;

  private void ConvertTypes(Expression[] args, Type[] types)
  {
    for (int i = 0; i < args.Length; ++i)
    {
      var type = types[i];
      if (type != null && args[i].Type != type)
        args[i] = Expression.Convert(args[i], type);
    }
  }

  internal Expression Call(Expression[] args)
  {
    if (argTypes != null)
      ConvertTypes(args, argTypes);
    return mapper(args);
  }
}

sealed class FuncNode(FunctionMapper mapper, params Node[] args) : Node
{
  public override Expression ToExpression(Expression instance)
  {
    var exprs = new Expression[args.Length];
    for (int i = 0; i < args.Length; ++i)
      exprs[i] = args[i].ToExpression(instance);
    return mapper.Call(exprs);
  }

  public override string ToString()
  {
    return $"Func.{mapper.Name}[{string.Join(",", args.Select(a => a.ToString()))}]";
  }
}