using ODataQuery.Tests.Data;
using System.Linq;
using Xunit;

namespace ODataQuery.Tests
{
  public class Select
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
  }
}