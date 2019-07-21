using System.Collections.Generic;
using System.Linq;

namespace ODataQuery
{
  static class EnumerableExtensions
  {
    public static List<T> AsList<T>(this IEnumerable<T> source) =>
      source is List<T> list ?
        list :
        source.ToList();
  }
}