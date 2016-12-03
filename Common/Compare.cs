using System;
using System.Collections.Generic;

namespace Gongchengshi
{
    public static class Compare
    {
        public static bool AreEqual(double left, double right, double delta)
        {
            return (Math.Abs(left - right) < delta);
        }
    }

    public class Comparer<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T x, T y)
        {
            return x.CompareTo(y);
        }

        public static IComparer<T> Default { get { return System.Collections.Generic.Comparer<T>.Default; } }
    }
}
