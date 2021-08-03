using Pidgin;
using System.Collections.Generic;

using static Pidgin.Parser;
using static ODataQuery.Parsers.Literals;

namespace ODataQuery.Parsers
{
  static class Select
  {
    public static readonly Parser<char, IEnumerable<Node>> Parser =
      Identifier
        .Separated(Char(',').Between(BWS))
        .Before(Parser<char>.End);
  }
}
