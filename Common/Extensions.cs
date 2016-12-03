using System;
using System.IO;
using System.Runtime.Serialization;

namespace Gongchengshi
{
    /// <summary>
    /// These were made to clean up the bit shifting code slightly.
    /// </summary>
    public static class ValueTypeExtensions
    {
       public static bool IsEven(this int @this)
       {
          return (@this & 1) == 0;
       }

       public static bool IsBitSetAt(this Int32 num, int bitpos)
        {
            return ((num & (1 << bitpos)) != 0);
        }

        public static bool IsBitSetAt(this Int16 num, int bitpos)
        {
            return ((num & (1 << bitpos)) != 0);
        }

        public static bool IsBitSetAt(this UInt32 num, int bitpos)
        {
            return ((num & (1 << bitpos)) != 0);
        }

        public static bool IsBitSetAt(this UInt16 num, int bitpos)
        {
            return ((num & (1 << bitpos)) != 0);
        }

        public static int Microseconds(this DateTime dateTime)
        {
            return (int)(((UInt64)dateTime.Ticks / Constants.TicksPerUSec) % Constants.USecPerSec);
        }

        public static DateTime AddMicroseconds(this DateTime dateTime, Int64 microseconds)
        {
            // One tick is 1/10 of a microsecond.
            return dateTime.AddTicks(microseconds * 10);
        }

        public static Int64 TotalMicroseconds(this TimeSpan timeSpan)
        {
            return timeSpan.Ticks / (Int64)Constants.TicksPerUSec;
        }

        public static bool Matches(this byte[] left, byte[] right)
        {
            if (left == right) return true;
            if (right == null) return false;
            if (left.Length != right.Length) return false;
            for (int i = 0; i < left.Length; i++)
                if (left[i] != right[i]) return false;
            return true;
        }

        /// <summary>
        /// Reverse count bytes in this array, starting at offset.
        /// </summary>
        public static void ReverseBytes(this byte[] bytes, int offset, int count)
        {
            for (int i = 0; i < count / 2; i++)
            {
                var leftIndex = offset + i;
                var rightIndex = offset + count - i - 1;
                var left = bytes[leftIndex];
                bytes[leftIndex] = bytes[rightIndex];
                bytes[rightIndex] = left;
            }
        }
    }

    public static class MathExtensions
    {
        public static T Max<T>(T value1, T value2) where T : IComparable
        {
            return (value1.CompareTo(value2) > 0) ? value1 : value2;
        }

        public static T Min<T>(T value1, T value2) where T : IComparable
        {
            return (value1.CompareTo(value2) < 0) ? value1 : value2;
        }
    }

    public static class Helpers
    {
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static void IfIsA<TBase, TSub>(this TBase @base, Action<TSub> then)
            where TBase : class
            where TSub : class, TBase
        {
            var sub = @base as TSub;
            if (sub != null)
            {
                then(sub);
            }
        }

        public static TReturn IfIsA<TBase, TSub, TReturn>(this TBase @base,
            Func<TSub, TReturn> then)
            where TBase : class
            where TSub : class, TBase
        {
            var sub = @base as TSub;
            if (sub != null)
            {
                return then(sub);
            }
            return default(TReturn);
        }

        public static TReturn IfIsA<TBase, TSub1, TSub2, TReturn>(this TBase @base,
                Func<TSub1, TReturn> then1, Func<TSub2, TReturn> then2)
            where TBase : class
            where TSub1 : class, TBase
            where TSub2 : class, TBase
        {
            var sub1 = @base as TSub1;
            if (sub1 != null)
            {
                return then1(sub1);
            }
            var sub2 = @base as TSub2;
            if (sub2 != null)
            {
                return then2(sub2);
            }
            return default(TReturn);
        }


        /// <summary>
        /// Invokes action up to limit times while it throws T.  If T is thrown, ifCatch is invoked.
        /// Once limit is reached, the exception is rethrown. If any other exceptions are thrown, the original 
        /// exception is rethrown.
        /// </summary>
        public static void WhileCatch<T>(this Action action, Action ifCatch, int limit) where T : Exception
        {
            for (int i = 1; i <= limit; ++i)
            {
                try
                {
                    action();
                    return;
                }
                catch (T)
                {
                    if (i == limit)
                    {
                        throw;
                    }

                    ifCatch();
                }
            }
        }

        public static T DeepCopy<T>(this T objectToCopy)
        {
            T copy;

            var serializer = new DataContractSerializer(typeof(T));

            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, objectToCopy);
                ms.Position = 0;
                copy = (T)serializer.ReadObject(ms);
            }

            return copy;
        }
        /// <summary>
        /// Returns true if the two objects are the same, false if not
        /// </summary>
        public static bool DeepCompare<T>( this T thisObject, T objectToCompare)
        {

            var serializer = new DataContractSerializer(typeof(T));
            using (var ms1 = new MemoryStream())
            {
                using (var ms2 = new MemoryStream())
                {
                    serializer.WriteObject(ms1, thisObject);
                    serializer.WriteObject(ms2, objectToCompare); 

                    if (ms1.Length != ms2.Length)
                        return false;

                    ms1.Position = 0; ms2.Position = 0;

                    while (ms1.Position < ms1.Length)
                    {
                        if (ms1.ReadByte() != ms2.ReadByte())
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }
    }
}
