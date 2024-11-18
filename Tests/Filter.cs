using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using ODataQuery.Tests.Data;

namespace ODataQuery.Tests;

public class FilterTests
{
  private static List<string> testInString = ["One", "Two", "Three"];
  private static List<int> testInInt = [1, 2, 3];

  private readonly IQueryable<TestData> data = new[]
  {
    new TestData(4, "Four", "2019-07-20T08:34:12"),
    new TestData(5, "Five", "2018-06-20T08:34:12"),
    new TestData(1, "One",  "2019-05-06T14:20:10"),
    new TestData(2, "Two",  "2017-07-06T14:20:10"),
  }.AsQueryable();

  public FilterTests()
  {
    ODataQueryableOptions.TryRegisterFunction(
      "number.isodd",
      args =>
      {
        return Expression.Equal(Expression.Modulo(args[0], Expression.Constant(2)), Expression.Constant(1));
      },
      [typeof(int)]);
  }

  private static object Q(Expression<Func<TestData, bool>> predicate) => predicate;

  public static object[][] queries = [
    ["id lt 5 and not id eq 1", Q(x => x.Id < 5 && !(x.Id == 1)), 2],
    ["dec lt 1.5", Q(x => x.Dec < 1.5m), 1],

    ["contains(name, 'o')", Q(x => x.Name.Contains("o")), 2],
    ["startswith(name, 'F') eq false", Q(x => x.Name.StartsWith("F") == false), 2],
    ["not endswith(name, 'e')", Q(x => !x.Name.EndsWith("e")), 2],

    ["indexof(name, 'iv') eq 1", Q(x => x.Name.IndexOf("iv") == 1), 1],
    ["length(name) eq 3", Q(x => x.Name.Length == 3), 2],
    ["substring(name, 1) eq 'ne'", Q(x => x.Name.Substring(1) == "ne"), 1],
    ["substring(name, 2, 1) eq 'o'", Q(x => x.Name.Substring(2, 1) == "o"), 1],
    ["concat('+', name) eq '+One'", Q(x => string.Concat("+", x.Name) == "+One"), 1],

    ["year(date) eq 2019", Q(x => x.Date.Year == 2019), 2],
    ["month(date) eq 7", Q(x => x.Date.Month == 7), 2],
    ["day(date) eq 20", Q(x => x.Date.Day == 20), 2],
    ["hour(date) eq 14", Q(x => x.Date.Hour == 14), 2],
    ["minute(date) eq 34", Q(x => x.Date.Minute == 34), 2],
    ["second(date) eq 10", Q(x => x.Date.Second == 10), 2],
    ["date(date) eq 2019-05-06", Q(x => x.Date.Date == new DateTime(2019, 5, 6)), 1],

    ["year(dateonly) eq 2019", Q(x => x.Date.Year == 2019), 2],
    ["month(dateonly) eq 7", Q(x => x.Date.Month == 7), 2],
    ["day(dateonly) eq 20", Q(x => x.Date.Day == 20), 2],

    ["year(datetz) eq 2019", Q(x => x.DateTz.Year == 2019), 2],
    ["month(datetz) eq 7", Q(x => x.DateTz.Month == 7), 2],
    ["day(datetz) eq 20", Q(x => x.DateTz.Day == 20), 2],
    ["hour(datetz) eq 14", Q(x => x.DateTz.Hour == 14), 2],
    ["minute(datetz) eq 34", Q(x => x.DateTz.Minute == 34), 2],
    ["second(datetz) eq 10", Q(x => x.DateTz.Second == 10), 2],
    ["date(datetz) eq 2019-05-06", Q(x => x.DateTz.Date == new DateTime(2019, 5, 6)), 1],

    ["ceiling(dec) le 2", Q(x => Math.Ceiling(x.Dec) <= 2), 1],
    ["floor(dec) le 2", Q(x => Math.Floor(x.Dec) <= 2), 2],
    ["round(dec) eq 4", Q(x => Math.Round(x.Dec) == 4), 1],

    ["name in ('One', 'Two', 'Three')", Q(x => testInString.Contains(x.Name)), 2],
    ["id in (1, 2, 3)", Q(x => testInInt.Contains(x.Id)), 2],
    ["id in ()", Q(x => false), 0], // OData grammar does not support

    ["enum eq 'B'", Q(x => x.Enum == TestEnum.B), 2],
    ["enum eq 2", Q(x => x.Enum == TestEnum.C), 2],
    ["enum in ('A', 0, 'B')", Q(x => x.Enum == TestEnum.B), 2],

    ["date gt 2019-01-01T00:00:00Z", Q(x => x.Date > DateTimeOffset.Parse("2019-01-01T00:00:00Z")), 2],   // Implicit conversion of DateTimeOffset to DateTime
    ["datetz gt 2019-01-01T00:00:00", Q(x => x.DateTz > DateTime.Parse("2019-01-01T00:00:00")), 2],       // Implicit conversion of DateTime to DateTimeOffset
    ["dateOnly gt 2019-01-01", Q(x => x.DateOnly > DateOnly.Parse("2019-01-01")), 2],                     // Implicit conversion of DateTime to DateTimeOffset}

    ["number.isodd(id)", Q(x => x.Id % 2 == 1), 2],
  ];

  [Theory]
  [MemberData(nameof(queries))]
  public void Query(string query, Expression<Func<TestData, bool>> linq, int matches)
  {
    var result = data.ODataFilter(query);
    Assert.Equal(data.Where(linq), result);
    Assert.Equal(matches, result.Count());
  }
}