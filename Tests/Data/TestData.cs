using System;
using System.Linq;

namespace ODataQuery.Tests.Data
{
  public enum TestEnum { A, B, C };

  public class TestData
  {
    public int Id { get; }
    public decimal Dec {get; }
    public string Text { get; }
    public DateTime Date { get; }
    public DateTimeOffset DateTz { get; }
    public TestEnum Enum { get => (Id & 1) == 1 ? TestEnum.B : TestEnum.C; }

    public TestData(int id, string text, string date)
    {
      Id = id;
      Dec = id + 0.25m;
      Text = text;
      Date = DateTime.Parse(date);
      DateTz = Date;
    }
  }
}