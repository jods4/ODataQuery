using System;
using System.Collections.Generic;
using Pidgin;
using ODataQuery.Nodes;

using static Pidgin.Parser;
using static ODataQuery.Parsers.Literals;

namespace ODataQuery.Parsers
{
  static class Expressions
  {
    public static Parser<char, Node> Function(string name, int argsCount, Func<List<Node>, Node> selector) =>
      String(name)
        .Then(Rec(() => Expression.Separated(Char(',').Between(BWS))
                                  .BetweenParen()))
        .Select(nodes => nodes.AsList())
        .Assert(nodes => nodes.Count == argsCount)
        .Select(selector);

    public static readonly Parser<char, Node> BoolFunctionCall =
      OneOf(
        Function("contains", 2, nodes => StringFunc.Contains(nodes[0], nodes[1])),
        Function("endswith", 2, nodes => StringFunc.EndsWith(nodes[0], nodes[1])),
        Function("startswith", 2, nodes => StringFunc.StartsWith(nodes[0], nodes[1]))
      );

    public static readonly Parser<char, Node> FunctionCall =
      OneOf(
        Try(BoolFunctionCall),
        Try(Function("ceiling", 1, nodes => NumberFunc.Ceiling(nodes[0]))),
        Function("concat", 2, nodes => StringFunc.Concat(nodes[0], nodes[1])),
        Try(Function("date", 1, nodes => DateFunc.Date(nodes[0]))),
        Function("day", 1, nodes => DateFunc.Day(nodes[0])),
        Function("floor", 1, nodes => NumberFunc.Floor(nodes[0])),
        Function("hour", 1, nodes => DateFunc.Hour(nodes[0])),
        Function("indexof", 2, nodes => StringFunc.IndexOf(nodes[0], nodes[1])),
        Function("length", 1, nodes => StringFunc.Length(nodes[0])),
        Try(Function("minute", 1, nodes => DateFunc.Minute(nodes[0]))),
        Function("month", 1, nodes => DateFunc.Month(nodes[0])),
        Function("round", 1, nodes => NumberFunc.Round(nodes[0])),
        Try(Function("second", 1, nodes => DateFunc.Second(nodes[0]))),
        Try(Function("substring", 2, nodes => StringFunc.Substring(nodes[0], nodes[1]))),
        Function("substring", 3, nodes => StringFunc.Substring(nodes[0], nodes[1], nodes[2])),
        Try(Function("tolower", 1, nodes => StringFunc.ToLower(nodes[0]))),
        Try(Function("toupper", 1, nodes => StringFunc.ToUpper(nodes[0]))),
        Function("trim", 1, nodes => StringFunc.Trim(nodes[0])),
        Function("year", 1, nodes => DateFunc.Year(nodes[0]))
      );

    public static readonly Parser<char, Node> Expression =
      OneOf(Try(FunctionCall),  // Try -> ambiguous with identifiers and true/false/null constants
            Try(Constant),      // Try -> ambiguous with true/false/null constants
            Identifier);
  }
}