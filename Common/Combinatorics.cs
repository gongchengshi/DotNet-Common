using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gongchengshi
{
   public static class Combinatorics
   {
      /// <summary>
      /// This is an implementation of the Revolving-door algorithm for creating all n choose r combinations of integers 0 ... n.
      /// It is great for selecting which indexes of an array to choose when generating combinations of the array items.
      /// 
      /// The algorithm was found in Donald Knuth's Art of Programming Fascicle 3A (Future volume 4 of the series).
      /// 
      /// The generated combinations can be quite numerous so they are lazily being created.  They are created in lexographic order.
      /// </summary>
      public static IEnumerable<IEnumerable<int>> Combinations(int n, int r)
      {
         int t = r;

         // W.H. Payne's revolving-door algorithm only works for t > 1

         if (t == 0)
         {
            yield return new int[0];
            yield break;
         }

         if (t == 1)
         {
            for (int i = 0; i < n; ++i)
            {
               yield return new[] { i };
            }

            yield break;
         }

         // Todo: I don't like that Knuth and W.H. Payne don't use a zero offset array 
         // but this algorithm is complicated enough for me not to try to fix it right now.
         var c = new int[t + 1 + 1];

         // R1
         for (int i = 1; i <= t; ++i)
         {
            c[i] = i - 1;
         }
         c[t + 1] = n;

         int j = 2;

         if (t.IsEven())
         {
            while (j <= t)
            {
               yield return c.Skip(1).Take(t); // R2

               if (c[1] > 0)
               {
                  c[1] -= 1;
                  continue;
               }

               for (j = 2; j <= t; ++j)
               {
                  // R5
                  Debug.Assert(c[j - 1] == j - 2);
                  if (c[j] + 1 < c[j + 1])
                  {
                     c[j - 1] = c[j];
                     c[j] = c[j] + 1;
                     break;
                  }

                  j += 1;

                  // R4
                  Debug.Assert(c[j] == c[j - 1] + 1);
                  if (c[j] >= j)
                  {
                     c[j] = c[j - 1];
                     c[j - 1] = j - 2;
                     break;
                  }
               }
            }
         }
         else // t is odd
         {
            while (j <= t)
            {
               yield return c.Skip(1).Take(t); // R2

               if (c[1] + 1 < c[2])
               {
                  c[1] += 1;
                  continue;
               }

               for (j = 2; j <= t; ++j)
               {
                  // R4
                  Debug.Assert(c[j] == c[j - 1] + 1);
                  if (c[j] >= j)
                  {
                     c[j] = c[j - 1];
                     c[j - 1] = j - 2;
                     break;
                  }

                  j += 1;

                  // R5
                  Debug.Assert(c[j - 1] == j - 2);
                  if (c[j] + 1 < c[j + 1])
                  {
                     c[j - 1] = c[j];
                     c[j] = c[j] + 1;
                     break;
                  }
               }
            }
         }
      }
   }
}
