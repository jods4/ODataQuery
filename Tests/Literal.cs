using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Pidgin;
using ODataQuery.Parsers;
using ODataQuery.Nodes;

namespace ODataQuery.Tests
{
  public class LiteralTests
  {
    private Node Parse(Parser<char, Node> parser, string input) =>
      parser.Before(Parser<char>.End)
            .ParseOrThrow(input);

    [Theory]
    [InlineData("some_thing_42", "Ident[some_thing_42]")]
    [InlineData("_ABC", "Ident[_ABC]")]
    public void Identifier(string input, string expected)
    {
      var result = Parse(Literals.Identifier, input).ToString();
      Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("1non_char_first")]
    [InlineData("special-char")]
    [InlineData("special$")]
    public void BadIdentifier(string input) => Assert.Throws<ParseException>(() => Parse(Literals.Identifier, input));

    [Theory]
    [InlineData("'hello world'", "Const[hello world]")]
    [InlineData("'I love ''airquotes'''", "Const[I love 'airquotes']")]
    [InlineData("-345.23", "Const[-345.23]")]
    [InlineData("12345", "Const[12345]")]
    [InlineData("null", "Const[]")]
    [InlineData("true", "Const[True]")]
    [InlineData("false", "Const[False]")]
    public void Const(string input, string expected)
    {
      var result = Parse(Literals.Constant, input).ToString();
      Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("'hello world")]
    [InlineData("'I love 'airquotes''")]
    [InlineData("-345.")]
    [InlineData("+12345")]
    [InlineData("10e1")]
    [InlineData("random")]
    public void BadConst(string input) => Assert.Throws<ParseException>(() => Parse(Literals.Constant, input));

    [Theory]
    [InlineData("2017-04-27")]
    [InlineData("2017-04-27T13:52:18")]
    public void DateTimeLiteral(string input)
    {
      var result = (ConstantNode)Parse(Literals.DateLiteral, input);
      var expected = DateTime.Parse(input);
      Assert.Equal(expected, result.Value);
    }

    [Theory]
    [InlineData("2017-04-27T13:52:18Z")]
    [InlineData("2017-04-27T13:52:18+0500")]
    [InlineData("2017-04-27T13:52:18-0230")]
    [InlineData("2017-04-27T13:52:19.12Z")]
    public void DateTimeOffsetLiteral(string input)
    {
      var result = (ConstantNode)Parse(Literals.DateLiteral, input);
      var expected = DateTimeOffset.Parse(input);
      Assert.Equal(expected, result.Value);
    }
  }
}