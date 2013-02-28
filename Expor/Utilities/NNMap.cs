using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities
{
    public class NNMap<T> :IEnumerable<KeyValuePair<KeyValuePair<int,int>,T>>
        where T :IComparable, IFormattable, IConvertible 
    {
        SortedDictionary<KeyValuePair<int, int>, T> map;
        public NNMap()
        {
            if (typeof(T) == typeof(int))
            {
                map = new SortedDictionary<KeyValuePair<int, int>, T>(new IntMapComparer());
            }
            else if (typeof(T) == typeof(double))
            {
                map = new SortedDictionary<KeyValuePair<int, int>, T>(new DoubleMapComparer());
            }
            else
            {
                throw new Exception("Type Param Can Only Either be \"int\" or \"double\"");
            }
            
        }
        public void Add(KeyValuePair<int, int> key,T value)
        {
            map.Add(key, value);
        }
        public bool ContainsKey(KeyValuePair<int, int> key)
        {
            return map.ContainsKey(key);
        }
        public T this[int i, int j]
        {
            get 
            {
                return map[new KeyValuePair<int, int>(i, j)];
            }
            set 
            {
                map[new KeyValuePair<int, int>(i, j)] = value;
            }
        }
        public T this[KeyValuePair<int, int> key]
        {
            get 
            {
                return map[key];
            }
            set 
            {
                map[key] = value;
            }
        }
        public void clear()
        {
            map.Clear();
        }


        public IEnumerator<KeyValuePair<KeyValuePair<int, int>, T>> GetEnumerator()
        {
            return map.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return map.GetEnumerator();
        }
    }
    public class IntMapComparer : IComparer<KeyValuePair<int, int>>
    {

        public int Compare(KeyValuePair<int, int> x, KeyValuePair<int, int> y)
        {

            if (x.Key < y.Key )
            {
                return -1;
            }
            else if (x.Key > y.Key)
            {
                return 1;
            }
            else
            {
                if (x.Value < y.Value)
                    return -1;
                else if (x.Value > y.Value)
                    return 1;
                else
                    return 0;
            }
        }
    }
    public class DoubleMapComparer : IComparer<KeyValuePair<int, int>>
    {

        public int Compare(KeyValuePair<int, int> x, KeyValuePair<int, int> y)
        {
            int amin = Math.Min(x.Key, x.Value);
            int amax = Math.Max(x.Key, x.Value);
            int bmin = Math.Min(y.Key, y.Value);
            int bmax = Math.Max(y.Key, y.Value);

            if (amin < bmin)
            {
                return -1;
            }
            else if (amin == bmin)
            {
                if (amax < bmax)
                {
                    return -1;
                }
                else if (amax > bmax)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 1;
            }
        }
    }
    public class NNMapDoubleComparer:IComparer<KeyValuePair<KeyValuePair<int,int>,double>>
    {

        public int Compare(KeyValuePair<KeyValuePair<int, int>, double> x, KeyValuePair<KeyValuePair<int, int>, double> y)
        {
            if (x.Value < y.Value)
            {
                return -1;
            }
            else if (x.Value == y.Value)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
