using System.Linq;
using Pidgin;
using Xunit;
using ODataQuery.Parsers;
using ODataQuery.Tests.Data;

namespace ODataQuery.Tests
{
  public class OrderByTests
  {
    [Theory]
    [InlineData("", "")]
    [InlineData("asc asc", "+Ident[asc]")]
    [InlineData("asc", "+Ident[asc]")]
    [InlineData("desc desc", "-Ident[desc]")]
    [InlineData("prop,toupper(x)   desc,3  asc", "+Ident[prop];-Func[ToUpper,Ident[x]];+Const[3]")]
    public void Parse(string input, string expected)
    {
      var result = string.Join(";",
                               OrderBy.Parser.ParseOrThrow(input)
                                      .Select(x => (x.asc ? "+" : "-") + x.node.ToString()));
      Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("x asc desc")]
    [InlineData("x nope")]
    [InlineData("a,,b")]
    [InlineData("x ,y, z")] // OData grammar doesn't include spaces around commas in orderby
    [InlineData("   ")] // Same as previous
    public void BadParse(string input) => Assert.Throws<ParseException>(() => OrderBy.Parser.ParseOrThrow(input));

    private IQueryable<TestData> data = new[] {
      new TestData(1, "One",    "2019-02-01"),
      new TestData(1, "OneBis", "2019-01-03"),
      new TestData(2, "Two",    "2019-01-03"),
      new TestData(2, "TwoBis", "2019-02-01"),
    }.AsQueryable();


    public delegate IOrderedQueryable<TestData> ApplyOrder(IQueryable<TestData> source);

    private static object Q(ApplyOrder orderby) => (ApplyOrder)((IQueryable<TestData> source) => orderby(source));

    public static object[][] queries = new[] {
      new[] { "text", Q(x => x.OrderBy(y => y.Text)) },
      new[] { "id,date", Q(x => x.OrderBy(y => y.Id).ThenBy(y => y.Date)) },
      new[] { "id,date desc", Q(x => x.OrderBy(y => y.Id).ThenByDescending(y => y.Date)) },
      new[] { "date desc,id", Q(x => x.OrderByDescending(y => y.Date).ThenBy(y => y.Id)) },
      new[] { "date asc,id desc", Q(x => x.OrderBy(y => y.Date).ThenByDescending(y => y.Id)) },
      new[] { "day(date) desc", Q(x => x.OrderByDescending(y => y.Date.Day)) },
    };

    [Theory]
    [MemberData(nameof(queries))]
    public void Query(string orderby, ApplyOrder linq)
    {
      var result = data.ODataOrderBy(orderby);
      var expected = linq(data);
      Assert.Equal(expected, result);
    }
  }
}