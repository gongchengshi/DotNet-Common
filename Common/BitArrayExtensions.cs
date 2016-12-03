using System.Collections;
using System.Text;

namespace Gongchengshi
{
   public static class BitArrayExtensions
   {
      static public string ToBinaryString(this BitArray a)
      {
         var binaryString = new StringBuilder(a.Length);
         foreach (var bit in a)
         {
            binaryString.Append((bool)bit ? "1" : "0");
         }
         return binaryString.ToString();
      }
   }
}
