using System;
using System.Reflection;

namespace ODataQuery.Nodes
{
  static class StringFunc {
    private static readonly MethodInfo contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
    public static MethodNode Contains(Node @string, Node value) => new MethodNode(contains, @string, value);

    private static readonly MethodInfo startsWith = typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) });
    public static MethodNode StartsWith(Node @string, Node value) => new MethodNode(startsWith, @string, value);

    private static readonly MethodInfo endsWith = typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string) });
    public static MethodNode EndsWith(Node @string, Node value) => new MethodNode(endsWith, @string, value);

    private static readonly MethodInfo indexOf = typeof(string).GetMethod(nameof(string.IndexOf), new[] { typeof(string) });
    public static MethodNode IndexOf(Node @string, Node value) => new MethodNode(indexOf, @string, value);

    private static readonly MethodInfo length = typeof(string).GetProperty(nameof(string.Length)).GetGetMethod();
    public static PropertyNode Length(Node @string) => new PropertyNode(length, @string);

    private static readonly MethodInfo substring1 = typeof(string).GetMethod(nameof(string.Substring), new[] { typeof(int) });
    public static MethodNode Substring(Node @string, Node from) => new MethodNode(substring1, @string, from);

    private static readonly MethodInfo substring2 = typeof(string).GetMethod(nameof(string.Substring), new[] { typeof(int), typeof(int) });
    public static MethodNode Substring(Node @string, Node from, Node length) => new MethodNode(substring2, @string, from, length);

    private static readonly MethodInfo toUpper = typeof(string).GetMethod(nameof(string.ToUpper), Type.EmptyTypes);
    public static MethodNode ToUpper(Node @string) => new MethodNode(toUpper, @string);

    private static readonly MethodInfo toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
    public static MethodNode ToLower(Node @string) => new MethodNode(toLower, @string);

    private static readonly MethodInfo trim = typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes);
    public static MethodNode Trim(Node @string) => new MethodNode(trim, @string);

    private static readonly MethodInfo concat = typeof(string).GetMethod(nameof(string.Concat), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string), typeof(string) }, null);
    public static MethodNode Concat(Node string1, Node string2) => new MethodNode(concat, null, string1, string2);
  }

  static class DateFunc
  {
    private static readonly MethodInfo year1 = typeof(DateTime).GetProperty(nameof(DateTime.Year)).GetGetMethod();
    private static readonly MethodInfo year2 = typeof(DateTimeOffset).GetProperty(nameof(DateTimeOffset.Year)).GetGetMethod();
    private static readonly MethodInfo year3 = typeof(DateOnly).GetProperty(nameof(DateOnly.Year)).GetGetMethod();
    public static DatePropertyNode Year(Node date) => new DatePropertyNode(date, year1, year2, year3);

    private static readonly MethodInfo month1 = typeof(DateTime).GetProperty(nameof(DateTime.Month)).GetGetMethod();
    private static readonly MethodInfo month2 = typeof(DateTimeOffset).GetProperty(nameof(DateTimeOffset.Month)).GetGetMethod();
    private static readonly MethodInfo month3 = typeof(DateOnly).GetProperty(nameof(DateOnly.Month)).GetGetMethod();
    public static DatePropertyNode Month(Node date) => new DatePropertyNode(date, month1, month2, month3);

    private static readonly MethodInfo day1 = typeof(DateTime).GetProperty(nameof(DateTime.Day)).GetGetMethod();
    private static readonly MethodInfo day2 = typeof(DateTimeOffset).GetProperty(nameof(DateTimeOffset.Day)).GetGetMethod();
    private static readonly MethodInfo day3 = typeof(DateOnly).GetProperty(nameof(DateOnly.Day)).GetGetMethod();
    public static DatePropertyNode Day(Node date) => new DatePropertyNode(date, day1, day2, day3);

    private static readonly MethodInfo hour1 = typeof(DateTime).GetProperty(nameof(DateTime.Hour)).GetGetMethod();
    private static readonly MethodInfo hour2 = typeof(DateTimeOffset).GetProperty(nameof(DateTimeOffset.Hour)).GetGetMethod();
    public static DatePropertyNode Hour(Node date) => new DatePropertyNode(date, hour1, hour2);

    private static readonly MethodInfo minute1 = typeof(DateTime).GetProperty(nameof(DateTime.Minute)).GetGetMethod();
    private static readonly MethodInfo minute2 = typeof(DateTimeOffset).GetProperty(nameof(DateTimeOffset.Minute)).GetGetMethod();
    public static DatePropertyNode Minute(Node date) => new DatePropertyNode(date, minute1, minute2);

    private static readonly MethodInfo second1 = typeof(DateTime).GetProperty(nameof(DateTime.Second)).GetGetMethod();
    private static readonly MethodInfo second2 = typeof(DateTimeOffset).GetProperty(nameof(DateTimeOffset.Second)).GetGetMethod();
    public static DatePropertyNode Second(Node date) => new DatePropertyNode(date, second1, second2);

    private static readonly MethodInfo date1 = typeof(DateTime).GetProperty(nameof(DateTime.Date)).GetGetMethod();
    private static readonly MethodInfo date2 = typeof(DateTimeOffset).GetProperty(nameof(DateTimeOffset.Date)).GetGetMethod();
    public static DatePropertyNode Date(Node date) => new DatePropertyNode(date, date1, date2);
  }

  static class NumberFunc
  {
    public static Node Round(Node number) => new MathNode(nameof(Math.Round), number);
    public static Node Floor(Node number) => new MathNode(nameof(Math.Floor), number);
    public static Node Ceiling(Node number) => new MathNode(nameof(Math.Ceiling), number);
  }
}