using System;
using System.Linq;

namespace Gongchengshi
{
   public static class TypeExtensions
   {
      public static bool IsOn<T>(this Type type) where T : Attribute
      {
         return type.IsDefined(typeof(T), true);
      }

      public static T Get<T>(this Type type) where T : Attribute
      {
         if (!type.IsOn<T>())
         {
            throw new InvalidOperationException(string.Format("{0} attribute does not exist on type {1}", typeof(T), type));
         }

         // This won't throw because the previous condition already checked for the existance of the attribute.
         return type.GetCustomAttributes(typeof(T), true).OfType<T>().First();
      }

      public static int HammingDistance(this ulong @this, ulong value)
      {
         return (@this ^ value).BitCount();
      }

      public static byte BitCount(this ulong @this)
      {
         byte count = 0;

         while (@this != 0)
         {
            if ((@this & 0x1) == 0x1)
            {
               count++;
            }
            @this >>= 1;
         }
         return count;
      }
   }
}
