using System;

namespace Gongchengshi
{
   public static class UriExtensions
   {
      public static string TopDirectories(this Uri @this, int count)
      {
         var url = (@this.IsAbsoluteUri ? @this.AbsolutePath : @this.OriginalString);
         return url.Fields(1, count, '/');
      }

      public  static Uri ReplaceHost(this Uri @this, string newHost)
      {
         return new Uri(@this.AbsoluteUri.Replace(@this.Host, newHost));
      }
   }
}
