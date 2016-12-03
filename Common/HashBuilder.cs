using System.Collections;
using System.Collections.Generic;

namespace Gongchengshi
{
    public class HashBuilder : IEnumerable
    {
        private const int Prime1 = 17;
        private const int Prime2 = 23;
        
        public HashBuilder()
        {
            Result = Prime1;
        }

        public HashBuilder(int startHash)
        {
            Result = startHash;
        }       

        public int Result { get; private set; }

        public void Add<T>(T item)
        {
            unchecked
            {
                var itemHashCode = item.GetHashCode();
                if (itemHashCode != 0)
                {
                    Result = Result * Prime2 * itemHashCode;
                }
            }
        }

        public void AddItems<T>(params T[] items)
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }

        public void AddItems<T>(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }

        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
