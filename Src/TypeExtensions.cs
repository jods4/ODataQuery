using System;

namespace ODataQuery
{
  static class TypeExtensions
  {
    public static bool IsNullable(this Type type, out Type innerType)
    {
      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
      {
        innerType = type.GetGenericArguments()[0];
        return true;
      }
      else
      {
        innerType = null;
        return false;
      }
    }
  }
}
