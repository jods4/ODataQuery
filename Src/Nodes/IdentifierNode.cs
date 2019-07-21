using System.Linq.Expressions;
using System.Reflection;

namespace ODataQuery.Nodes
{
  sealed class IdentifierNode : Node
  {
    public string Name { get; }

    public IdentifierNode(string name)
    {
      Name = name;
    }

    public override Expression ToExpression(Expression instance)
    {
      var propInfo = instance.Type.GetProperty(Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
      return Expression.Property(instance, propInfo);
    }

    public override string ToString() => $"Ident[{Name}]";
  }
}