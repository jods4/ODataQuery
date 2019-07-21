using System.Collections.Generic;
using Pidgin;

using static Pidgin.Parser;
using static ODataQuery.Parsers.Expressions;
using static ODataQuery.Parsers.Literals;

namespace ODataQuery.Parsers
{
  static class OrderBy
  {
    public static Parser<char, bool> Direction =>
      RWS
      .Then(String("asc").WithResult(true)
        .Or(String("desc").WithResult(false)))
      .Optional()
      .Select(x => x.HasValue ? x.Value : true);

    public static Parser<char, IEnumerable<(Node node, bool asc)>> Parser =>
      Map((node, dir) => (node, dir),
          Expression,
          Try(Direction)) // Try -> because Direction consumes whitespace, which makes it fail if it's followed by a comma
      .Separated(Char(',').Between(BWS))
      .Before(Parser<char>.End);
  }
}