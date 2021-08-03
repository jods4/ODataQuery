using System;
using Pidgin;
using Pidgin.Expression;
using ODataQuery.Nodes;

using static Pidgin.Parser;
using static ODataQuery.Parsers.Literals;
using static ODataQuery.Parsers.Expressions;

namespace ODataQuery.Parsers
{
  static class Logical
  {
    public static readonly Parser<char, Comparator> ComparisonOperator =
      OneOf(String("eq").WithResult(Comparator.Eq),
            String("ne").WithResult(Comparator.Ne),
            Char('g').Then(Char('t').WithResult(Comparator.Gt)
                       .Or(Char('e').WithResult(Comparator.Ge))),
            Char('l').Then(Char('t').WithResult(Comparator.Lt)
                       .Or(Char('e').WithResult(Comparator.Le)))
            );

    public static readonly Parser<char, Node> Comparison =
      Map(
        (x, op, y) => (Node)new ComparisonNode(op, x, y),
        Expression,
        ComparisonOperator.Between(RWS),
        Expression
      );

    public static readonly Parser<char, Node> In =
      Map(
        (e, list) => (Node)new InNode(e, list),
        Expression
          .Before(String("in").Between(RWS)),
        // We support empty list, although OData grammar does not
        Constant.Separated(Char(',').Between(BWS))
          .BetweenParen()
      );

    public static readonly Parser<char, Func<Node, Node>> Not =
      Try(    // Try -> amiguous with expressions that start with 'n'
        String("not")    
          .Before(RWS)      
          .WithResult<Func<Node, Node>>(x => new NotNode(x))
      );

    public static readonly Parser<char, Func<Node, Node, Node>> Or =
      String("or")
        .Between(RWS)
        .WithResult<Func<Node, Node, Node>>((x, y) => new LogicalNode(LogicOperator.Or, x, y));

    public static readonly Parser<char, Func<Node, Node, Node>> And =
      String("and")
        .Between(RWS)
        .WithResult<Func<Node, Node, Node>>((x, y) => new LogicalNode(LogicOperator.And, x, y));

    public static readonly Parser<char, Node> LogicalExpr =
      ExpressionParser.Build(
        x => OneOf(
          x.BetweenParen(),
          Try(In),          // Try -> ambiguous with later parsers because left-hand side may start with any expression
          Try(Comparison),  // Try -> ambiguous with BoolFunctionCall because "contains(text, 'a') eq true" is valid as well as just "contains(text, 'a')"
          BoolFunctionCall
        ),
        new [] {
          Operator.PrefixChainable(Not),
          // Try -> and/or are ambiguous because of leading space
          Operator.InfixL(Try(And)),
          Operator.InfixL(Try(Or)),
        });
  }
}
