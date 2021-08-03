using System;
using Pidgin;
using ODataQuery.Nodes;

using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace ODataQuery.Parsers
{
  static class Literals
  {
    // Required Whitespace
    public static readonly Parser<char, Unit> RWS = Char(' ').SkipAtLeastOnce();
    // Bad Whitespace
    public static readonly Parser<char, Unit> BWS = Char(' ').SkipMany();

    public static Parser<char, T> BetweenParen<T>(this Parser<char, T> x) =>
      x.Between(
        Char('(').Before(BWS),
        BWS.Before(Char(')'))
      );

    public static readonly Parser<char, Node> Identifier =
      Token(c => ((uint)c - 'a') < 26
              || ((uint)c - 'A') < 26
              || c == '_')
      .Then(Token(c => ((uint)c - 'a') < 26
                    || ((uint)c - 'A') < 26
                    || ((uint)c - '0') < 10
                    || c == '_').ManyString(),
            (first, rest) => (Node)new IdentifierNode(first + rest));

    public static readonly Parser<char, Node> StringLiteral =
      AnyCharExcept('\'')
        .Or(Try(String("''").WithResult('\'')))
        .ManyString()
        .Between(Char('\''))
        .Select<Node>(s => new ConstantNode(s));

    public static readonly Parser<char, Node> NumberLiteral =
      Map((s, m, f) => (Node)new ConstantNode(decimal.Parse((s.HasValue ? "-" : "") + m + (f.HasValue ? "." + f.Value : ""))),
        Char('-').Optional(),
        Digit.AtLeastOnceString(),
        Char('.').Then(Digit.AtLeastOnceString()).Optional()
      );

    public static readonly Parser<char, Node> DateLiteral =
      Map((y, m, d) => new DateTime(int.Parse(y), int.Parse(m), int.Parse(d)),
        Digit.RepeatString(4).Before(Char('-')),
        Digit.RepeatString(2).Before(Char('-')),
        Digit.RepeatString(2))
      .Then(
        Map((_, h, m, s, f) => new TimeSpan(0, int.Parse(h), int.Parse(m), int.Parse(s), !f.HasValue ? 0 : int.Parse(f.Value.PadRight(3, '0').Substring(0, 3))),
          Char('T'),
          Digit.RepeatString(2).Before(Char(':')),
          Digit.RepeatString(2).Before(Char(':')),
          Digit.RepeatString(2),
          Char('.').Then(Digit.AtLeastOnceString()).Optional())
        .Optional(),
        (dt, ts) => ts.HasValue ? dt.Add(ts.Value) : dt
      )
      .Then(
        OneOf(
          Char('Z').WithResult(TimeSpan.Zero),
          Char('+').Then(Digit.RepeatString(4)).Select(tz => new TimeSpan(int.Parse(tz.Substring(0, 2)), int.Parse(tz.Substring(2, 2)), 0)),
          Char('-').Then(Digit.RepeatString(4)).Select(tz => - new TimeSpan(int.Parse(tz.Substring(0, 2)), int.Parse(tz.Substring(2, 2)), 0))
        ).Optional(),
        (dt, tz) => tz.HasValue ? new DateTimeOffset(dt, tz.Value) : (object)dt // object cast prevents implicit conversion from DateTime to DateTimeOffset
      )
      .Select<Node>(d => new ConstantNode(d));

    public static readonly Parser<char, Node> KeywordLiteral =
      OneOf(
        String("false").WithResult(ConstantNode.False),
        String("null").WithResult(ConstantNode.Null),
        String("true").WithResult(ConstantNode.True)
      );

    public static readonly Parser<char, Node> Constant =
      OneOf(StringLiteral,
            Try(DateLiteral), // Try -> ambiguous with ints as both start with a digit
            NumberLiteral,
            KeywordLiteral);
  }
}