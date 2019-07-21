using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Pidgin;
using ODataQuery.Parsers;

namespace ODataQuery.Tests
{
  public class LogicalTests
  {
    [Theory]
    [InlineData("rate lt 100", "Lt[Ident[rate],Const[100]]")]
    [InlineData("rate lt 100 and vip eq 1 or booked ne 1", "Or[And[Lt[Ident[rate],Const[100]],Eq[Ident[vip],Const[1]]],Ne[Ident[booked],Const[1]]]")]
    [InlineData("((rate lt 100) and (vip eq 1 or booked ne 1))", "And[Lt[Ident[rate],Const[100]],Or[Eq[Ident[vip],Const[1]],Ne[Ident[booked],Const[1]]]]")]
    [InlineData("today gt 2000-01-01", "Gt[Ident[today],Const[2000-01-01T00:00:00]]")]
    [InlineData("not contains(text, 'abc') or text eq 'abc'", "Or[Not[Func[Contains,Ident[text],Const[abc]]],Eq[Ident[text],Const[abc]]]")]
    public void LogicalExpression(string input, string expected)
    {
      var result = Logical.LogicalExpr
                          .Before(Parser<char>.End)
                          .ParseOrThrow(input)
                          .ToString();
      Assert.Equal(expected, result);
    }
  }
}