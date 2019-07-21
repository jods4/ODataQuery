using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Pidgin;
using ODataQuery.Tests.Data;

namespace ODataQuery.Tests
{
  public class FilterTests
  {
    private static List<string> testInString = new List<string> { "One", "Two", "Three" };
    private static List<int> testInInt = new List<int> { 1, 2, 3 };

    private readonly IQueryable<TestData> data = new[]
    {
      new TestData(4, "Four", "2019-07-20T08:34:12"),
      new TestData(5, "Five", "2018-06-20T08:34:12"),
      new TestData(1, "One",  "2019-05-06T14:20:10"),
      new TestData(2, "Two",  "2017-07-06T14:20:10"),
    }.AsQueryable();

    private static object Q(Expression<Func<TestData, bool>> predicate) => predicate;

    public static object[][] queries = new[] {
      new[] { "id lt 5 and not id eq 1", Q(x => x.Id < 5 && !(x.Id == 1)), 2 },
      new[] { "dec lt 1.5", Q(x => x.Dec < 1.5m), 1 },

      new[] { "contains(text, 'o')", Q(x => x.Text.Contains("o")), 2 },
      new[] { "startswith(text, 'F') eq false", Q(x => x.Text.StartsWith("F") == false), 2 },
      new[] { "not endswith(text, 'e')", Q(x => !x.Text.EndsWith("e")), 2 },

      new[] { "indexof(text, 'iv') eq 1", Q(x => x.Text.IndexOf("iv") == 1), 1 },
      new[] { "length(text) eq 3", Q(x => x.Text.Length == 3), 2 },
      new[] { "substring(text, 1) eq 'ne'", Q(x => x.Text.Substring(1) == "ne"), 1 },
      new[] { "substring(text, 2, 1) eq 'o'", Q(x => x.Text.Substring(2, 1) == "o"), 1 },
      new[] { "concat('+', text) eq '+One'", Q(x => string.Concat("+", x.Text) == "+One"), 1 },

      new[] { "year(date) eq 2019", Q(x => x.Date.Year == 2019), 2 },
      new[] { "month(date) eq 7", Q(x => x.Date.Month == 7), 2 },
      new[] { "day(date) eq 20", Q(x => x.Date.Day == 20), 2 },
      new[] { "hour(date) eq 14", Q(x => x.Date.Hour == 14), 2 },
      new[] { "minute(date) eq 34", Q(x => x.Date.Minute == 34), 2 },
      new[] { "second(date) eq 10", Q(x => x.Date.Second == 10), 2 },
      new[] { "date(date) eq 2019-05-06", Q(x => x.Date.Date == new DateTime(2019, 5, 6)), 1 },

      new[] { "year(datetz) eq 2019", Q(x => x.DateTz.Year == 2019), 2 },
      new[] { "month(datetz) eq 7", Q(x => x.DateTz.Month == 7), 2 },
      new[] { "day(datetz) eq 20", Q(x => x.DateTz.Day == 20), 2 },
      new[] { "hour(datetz) eq 14", Q(x => x.DateTz.Hour == 14), 2 },
      new[] { "minute(datetz) eq 34", Q(x => x.DateTz.Minute == 34), 2 },
      new[] { "second(datetz) eq 10", Q(x => x.DateTz.Second == 10), 2 },
      new[] { "date(datetz) eq 2019-05-06", Q(x => x.DateTz.Date == new DateTime(2019, 5, 6)), 1 },

      new[] { "ceiling(dec) le 2", Q(x => Math.Ceiling(x.Dec) <= 2), 1 },
      new[] { "floor(dec) le 2", Q(x => Math.Floor(x.Dec) <= 2), 2 },
      new[] { "round(dec) eq 4", Q(x => Math.Round(x.Dec) == 4), 1 },

      new[] { "text in ('One', 'Two', 'Three')", Q(x => testInString.Contains(x.Text)), 2 },
      new[] { "id in (1, 2, 3)", Q(x => testInInt.Contains(x.Id)), 2 },
      new[] { "id in ()", Q(x => false), 0 }, // OData grammar does not support

      new[] { "enum eq 'B'", Q(x => x.Enum == TestEnum.B), 2 },
      new[] { "enum eq 2", Q(x => x.Enum == TestEnum.C), 2 },
      new[] { "enum in ('A', 0, 'B')", Q(x => x.Enum == TestEnum.B), 2},
    };

    [Theory]
    [MemberData(nameof(queries))]
    public void Query(string query, Expression<Func<TestData, bool>> linq, int matches)
    {
      var result = data.ODataFilter(query);
      Assert.Equal(data.Where(linq), result);
      Assert.Equal(matches, result.Count());
    }
  }
}