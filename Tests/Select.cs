using ODataQuery.Parsers;
using ODataQuery.Tests.Data;
using Pidgin;
using System.Linq;
using Xunit;

namespace ODataQuery.Tests
{
  public class SelectTests
  {
    private IQueryable<TestData> data = new[] {
      new TestData(1, "One",    "2019-02-01"),
      new TestData(1, "OneBis", "2019-01-03"),
      new TestData(2, "Two",    "2019-01-03"),
      new TestData(2, "TwoBis", "2019-02-01"),
    }.AsQueryable();

    [Theory]
    [InlineData("id", new object[] { 1, 1, 2, 2 })]
    [InlineData("Name", new object[] { "One", "OneBis", "Two", "TwoBis" })]
    public void FlatSelect(string field, object[] expected)
    {
      var actual = data.FlatSelect(field).Cast<object>().ToArray();
      Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("abc", "Ident[abc]")]
    [InlineData("a1,b2,c3", "Ident[a1],Ident[b2],Ident[c3]")]
    [InlineData("w , x, y, z", "Ident[w],Ident[x],Ident[y],Ident[z]")] // OData grammar doesn't include spaces around commas in select
    public void Parse(string input, string expected)
    {
      var result = string.Join(",", Select.Parser.ParseOrThrow(input));
      Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("x nope")]
    [InlineData("a,,b")]    
    public void BadParse(string input) => Assert.Throws<ParseException>(() => Select.Parser.ParseOrThrow(input));

    [Fact]
    public void Project()
    {
      var mapped = data.ODataSelect("id,name");
      var serialize = string.Join("|", mapped.Select(x => string.Join(";", x.Select(y => $"{y.Key}:{y.Value}"))));
      Assert.Equal("id:1;name:One|id:1;name:OneBis|id:2;name:Two|id:2;name:TwoBis", serialize);
    }
  }
}