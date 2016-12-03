using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Gongchengshi
{
   public static class UrlUtils
   {
      public static string SortQueryString(string queryString)
      {
         var origQuery = HttpUtility.ParseQueryString(queryString);

         var builder = new StringBuilder(queryString.Length);
         builder.Append("?");
         foreach (var key in origQuery.AllKeys.OrderBy(x => x, StringComparer.Ordinal))
         {
            builder.AppendFormat("{0}={1}&", key, origQuery[key]);
         }

         builder.Remove(builder.Length-1, 1);

         return builder.ToString();
      }

      public static string RemoveSchema(string url)
      {
         var schemaDelim = url.IndexOf("://", StringComparison.Ordinal);
         return (schemaDelim > 0 && schemaDelim < 16) ? url.Substring(schemaDelim + 3) : url;
      }

      public static string JoinFromRoot(params string[] segments)
      {
         var builder = new StringBuilder();

         foreach (var segment in segments)
         {
            builder.Append("/");
            builder.Append(segment.Trim('/'));
         }

         return builder.ToString();
      }

      public static string UrlEncodeQueryString(SchemalessUrl url)
      {
         var urlString = url.ToString();
         var filenameQueryFragmentPortion = Path.GetFileName(urlString);
         if (filenameQueryFragmentPortion != null)
         {
            return urlString.PythonSubstring(0, -filenameQueryFragmentPortion.Length) +
               HttpUtility.UrlEncode(filenameQueryFragmentPortion);
         }

         return urlString;
      }
   }
}
