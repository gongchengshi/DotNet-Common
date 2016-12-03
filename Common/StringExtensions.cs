using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gongchengshi
{
   public static class StringExtensions
   {
      public static IEnumerable<T> Split<T>(this string @this, Converter<string, T> converter)
      {
         return @this.Split().Select(x => converter(x));
      }

      /// <summary>
      /// This isn't a perfect implementation.  If a regex later in the list matches one of the earlier replacements 
      /// after it has been replaced it will replace it again with the later replacement.
      /// </summary>
      public static string ReplaceAll(this string @this, IEnumerable<KeyValuePair<Regex, string>> replacements)
      {
         return replacements.Aggregate(@this, (current, replacement) => replacement.Key.Replace(current, replacement.Value));
      }

      public static string PythonSubstring(this string @this, int start, int end)
      {
         if (end < 0)
         {
            end = @this.Length + end;
         }

         if (start < 0)
         {
            start = @this.Length + start;
         }

         return @this.Substring(start, end - start);
      }

      public static bool IsEmpty(this string @this)
      {
         return @this == string.Empty;
      }

      public static bool StartsWith(this string @this, char character)
      {
         if (string.IsNullOrEmpty(@this))
         {
            return false;
         }

         return @this[0] == character;
      }

      public static bool EndsWith(this string @this, char character)
      {
         if (string.IsNullOrEmpty(@this))
         {
            return false;
         }

         return @this[@this.Length - 1] == character;
      }

      public static string[] Split(this string @this, char delimiter, int max)
      {
         return @this.Split(new[] { delimiter }, 3);
      }

      public static string Field(this string @this, int index, char delimiter)
      {
         return @this.Split(delimiter)[index];
      }

      public static string Fields(this string @this, int startIndex, int count, char delimiter)
      {
         return string.Join(delimiter.ToString(), @this.Split(delimiter).Skip(startIndex).Take(count));
      }

      public static string FirstFields(this string @this, int count, char delimiter)
      {
         // This is a hack. There is a faster way to do this but this is the easiest.
         return string.Join(delimiter.ToString(), @this.Split(delimiter).Take(count));
      }

      public static string LastFields(this string @this, int startIndex, char delimiter)
      {
         // This is a hack. There is a faster way to do this but this is the easiest.
         return string.Join(delimiter.ToString(), @this.Split(delimiter).Skip(startIndex));
      }

      public static string Skip(this string @this, int count)
      {
         return @this.Substring(count);
         // Another way to do this:
         // return (@this as IEnumerable<char>).Skip(count).ToString();
      }

      public static string Take(this string @this, int count)
      {
         return @this.Substring(0, count);
         // Another way to do this:
         // return (@this as IEnumerable<char>).Take(count).ToString();
      }

      public static int NumNonEmptyFields(this string @this, char delimiter)
      {
         return @this.Split(delimiter).Count(field => !field.IsEmpty());
      }

      public static int NumFields(this string @this, char delimiter)
      {
         var count = @this.Count(delimiter) + 1;

         if (@this[0] == delimiter && @this[@this.Length - 1] == delimiter)
         {
            count += 1;
         }

         return count;
      }

      public static int Count(this string @this, char character)
      {
         return @this.Count(x => x == character);
      }

      public static string RemoveFromEnd(this string @this, string remove)
      {
         return @this.EndsWith(remove) ? @this.Substring(0, @this.Length - remove.Length) : @this;
      }
   }
}
