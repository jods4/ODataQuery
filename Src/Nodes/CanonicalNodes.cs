using System;
using System.Reflection;

namespace ODataQuery.Nodes
{
  static class StringFunc {
    private static readonly MethodInfo contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
    public static FunctionNode Contains(Node @string, Node value) => new FunctionNode(contains, @string, value);

    private static readonly MethodInfo startsWith = typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) });
    public static FunctionNode StartsWith(Node @string, Node value) => new FunctionNode(startsWith, @string, value);

    private static readonly MethodInfo endsWith = typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string) });
    public static FunctionNode EndsWith(Node @string, Node value) => new FunctionNode(endsWith, @string, value);

    private static readonly MethodInfo indexOf = typeof(string).GetMethod(nameof(string.IndexOf), new[] { typeof(string) });
    public static FunctionNode IndexOf(Node @string, Node value) => new FunctionNode(indexOf, @string, value);

    private static readonly MethodInfo length = typeof(string).GetProperty(nameof(string.Length)).GetGetMethod();
    public static PropertyNode Length(Node @string) => new PropertyNode(length, @string);

    private static readonly MethodInfo substring1 = typeof(string).GetMethod(nameof(string.Substring), new[] { typeof(int) });
    public static FunctionNode Substring(Node @string, Node from) => new FunctionNode(substring1, @string, from);

    private static readonly MethodInfo substring2 = typeof(string).GetMethod(nameof(string.Substring), new[] { typeof(int), typeof(int) });
    public static FunctionNode Substring(Node @string, Node from, Node length) => new FunctionNode(substring2, @string, from, length);

    private static readonly MethodInfo toUpper = typeof(string).GetMethod(nameof(string.ToUpper), Type.EmptyTypes);
    public static FunctionNode ToUpper(Node @string) => new FunctionNode(toUpper, @string);

    private static readonly MethodInfo toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
    public static FunctionNode ToLower(Node @string) => new FunctionNode(toLower, @string);

    private static readonly MethodInfo trim = typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes);
    public static FunctionNode Trim(Node @string) => new FunctionNode(trim, @string);

    private static readonly MethodInfo concat = typeof(string).GetMethod(nameof(string.Concat), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string), typeof(string) }, null);
    public static FunctionNode Concat(Node string1, Node string2) => new FunctionNode(concat, null, string1, string2);
  }

  static class DateFunc
  {
    private static readonly MethodInfo year1 = typeof(DateTime).GetProperty(nameof(DateTime.Year)).GetGetMethod();
    private static readonly MethodInfo year2 = typeof(DateTimeOffset).GetProperty(nameof(DateTimeOffset.Year)).GetGetMethod();
    public static DatePropertyNode Year(Node date) => new DatePropertyNode(year1, year2, date);

    private static readonly MethodInfo month1 = typeof(DateTime).GetProperty(nameof(DateTime.Month)).GetGetMethod();
    private static readonly MethodInfo month2 = typeof(DateTimeOffset).GetProperty(nameof(DateTimeOffset.Month)).GetGetMethod();
    public static DatePropertyNode Month(Node date) => new DatePropertyNode(month1, month2, date);

    private static readonly MethodInfo day1 = typeof(DateTime).GetProperty(nameof(DateTime.Day)).GetGetMethod();
    private static readonly MethodInfo day2 = typeof(DateTimeOffset).GetProperty(nameof(DateTimeOffset.Day)).GetGetMethod();
    public static DatePropertyNode Day(Node date) => new DatePropertyNode(day1, day2, date);

    private static readonly MethodInfo hour1 = typeof(DateTime).GetProperty(nameof(DateTime.Hour)).GetGetMethod();
    private static readonly MethodInfo hour2 = typeof(DateTimeOffset).GetProperty(nameof(DateTimeOffset.Hour)).GetGetMethod();
    public static DatePropertyNode Hour(Node date) => new DatePropertyNode(hour1, hour2, date);

    private static readonly MethodInfo minute1 = typeof(DateTime).GetProperty(nameof(DateTime.Minute)).GetGetMethod();
    private static readonly MethodInfo minute2 = typeof(DateTimeOffset).GetProperty(nameof(DateTimeOffset.Minute)).GetGetMethod();
    public static DatePropertyNode Minute(Node date) => new DatePropertyNode(minute1, minute2, date);

    private static readonly MethodInfo second1 = typeof(DateTime).GetProperty(nameof(DateTime.Second)).GetGetMethod();
    private static readonly MethodInfo second2 = typeof(DateTimeOffset).GetProperty(nameof(DateTimeOffset.Second)).GetGetMethod();
    public static DatePropertyNode Second(Node date) => new DatePropertyNode(second1, second2, date);

    private static readonly MethodInfo date1 = typeof(DateTime).GetProperty(nameof(DateTime.Date)).GetGetMethod();
    private static readonly MethodInfo date2 = typeof(DateTimeOffset).GetProperty(nameof(DateTimeOffset.Date)).GetGetMethod();
    public static DatePropertyNode Date(Node date) => new DatePropertyNode(date1, date2, date);
  }

  static class NumberFunc
  {
    public static Node Round(Node number) => new MathNode(nameof(Math.Round), number);
    public static Node Floor(Node number) => new MathNode(nameof(Math.Floor), number);
    public static Node Ceiling(Node number) => new MathNode(nameof(Math.Ceiling), number);
  }
}