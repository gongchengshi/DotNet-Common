using System;
using System.Web;

namespace Gongchengshi
{
   public class SchemalessUrl
   {
      public string Url { get; protected set; }

      private Uri Uri { get { return new Uri("http://"+Url); }}

      public SchemalessUrl(Uri uri)
      {
         Url = UrlUtils.RemoveSchema(uri.AbsoluteUri);
      }

      public SchemalessUrl(string url)
      {
         Url = UrlUtils.RemoveSchema(url);
      }

      public SchemalessUrl(SchemalessUrl url)
      {
         Url = url.ToString();
      }

      protected SchemalessUrl()
      {}

      public static bool operator ==(SchemalessUrl lhs, SchemalessUrl rhs)
      {
         if (lhs as object == null || rhs as object == null)
         {
            return lhs as object == null && rhs as object == null;
         }

         return lhs.ToString() == rhs.ToString();
      }

      public static bool operator !=(SchemalessUrl lhs, SchemalessUrl rhs)
      {
         return !(lhs == rhs);
      }

      public SchemalessUrl SortQueryString()
      {
         var sortedQuery = UrlUtils.SortQueryString(Uri.Query);
         return new SchemalessUrl(string.IsNullOrWhiteSpace(sortedQuery)
                                     ? Url
                                     : Uri.AbsoluteUri.Replace(Uri.Query, sortedQuery));
      }

      public SchemalessUrl UrlEncodeQueryString()
      {
         var encodedQuery = HttpUtility.UrlEncode(Uri.Query);
         return new SchemalessUrl(string.IsNullOrWhiteSpace(encodedQuery)
                                     ? Url
                                     : Uri.AbsoluteUri.Replace(Uri.Query, encodedQuery));
      }

      public string[] Segments
      {
         get { return Uri.Segments; }
      }

      public string TopDirectories(int count)
      {
         return Uri.TopDirectories(count);
      }

      public SchemalessUrl ReplaceHost(string newHost)
      {
         return new SchemalessUrl(Uri.ReplaceHost(newHost));
      }

      public override string ToString()
      {
         return Url;
      }
   }
}
