using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{

    /**
     * Class using a primitive int[] array as storage.
     * 
     * @author Erich Schubert
     */
    public class ArrayModifiableInt32DbIds : IArrayModifiableDbIds, IArrayDbIds
    {
        /**
         * The actual Trove array list.
         */
        private int[] store;

        /**
         * Occupied size.
         */
        private int size;

        /**
         * Initial size.
         */
        public static int INITIAL_SIZE = 21;

        /**
         * Constructor.
         * 
         * @param isize Initial size
         */
        internal ArrayModifiableInt32DbIds(int isize)
        {

            this.store = new int[isize < 3 ? 3 : isize];
            // default this.size = 0;
        }

        /**
         * Constructor.
         */
        internal ArrayModifiableInt32DbIds()
        {

            this.store = new int[INITIAL_SIZE];
            // default: this.size = 0;
        }

        /**
         * Constructor.
         * 
         * @param existing Existing ids
         */
        internal ArrayModifiableInt32DbIds(IDbIds existing) :
            this(existing.Count)
        {
            if (existing is Int32DbIdRange)
            {
                Int32DbIdRange range = (Int32DbIdRange)existing;
                for (int i = 0; i < range.Count; i++)
                {
                    store[i] = range.Start + i;
                }
                size = range.Count;
            }
            else
            {
                this.AddDbIds(existing);
            }
        }


        public int Count
        {
            get { return size; }
        }


        public bool IsEmpty()
        {
            return size == 0;
        }


        public IDbId this[int i]
        {
            get { return new Int32DbId(store[i]); }
            set { store[i] = value.Int32Id; }
        }


        public void AssignVar(int index, IDbIdVar var)
        {
            if (var is Int32DbIdVar)
            {
                ((Int32DbIdVar)var).InternalSetIndex(store[index]);
            }
            else
            {
                // less efficient, involves object creation.
                var.Set(this[(index)]);
            }
        }

        /**
         * Resize as desired.
         * 
         * @param minsize Desired size
         */
        private void EnsureSize(int minsize)
        {
            if (minsize <= store.Length)
            {
                return;
            }
            int asize = store.Length;
            while (asize < minsize)
            {
                asize = (asize >> 1) + asize;
            }
            int[] prev = store;
            store = new int[asize];
            Array.Copy(prev, 0, store, 0, size);
        }

        /**
         * Grow array by 50%.
         */
        private void Grow()
        {
            int newsize = store.Length + (store.Length >> 1);
            int[] prev = store;
            store = new int[newsize];
            Array.Copy(prev, 0, store, 0, size);
        }


        public bool AddDbIds(IDbIds ids)
        {
            EnsureSize(size + ids.Count);
            // for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
            foreach (var iter in ids)
            {
                store[size] = iter.InternalGetIndex();
                ++size;
            }
            return true;
        }


        public bool RemoveDbIds(IDbIds ids)
        {
            bool success = false;
            //  for (DbIdIter id = ids.iter(); id.valid(); id.advance()) {
            foreach (var id in ids)
            {
                int rm = id.InternalGetIndex();
                // TODO: when sorted, use binary search!
                for (int i = 0; i < size; i++)
                {
                    if (store[i] == rm)
                    {
                        --size;
                        store[i] = store[size];
                        success = true;
                        break;
                    }
                }
            }
            return success;
        }


        public bool Add(IDbIdRef e)
        {
            if (size == store.Length)
            {
                Grow();
            }
            store[size] = e.InternalGetIndex();
            ++size;
            return true;
        }


        public bool Remove(IDbIdRef o)
        {
            int rm = o.InternalGetIndex();
            // TODO: when sorted, use binary search!
            for (int i = 0; i < size; i++)
            {
                if (store[i] == rm)
                {
                    --size;
                    store[i] = store[size];
                    return true;
                }
            }
            return false;
        }


        public IDbId Set(int index, IDbIdRef element)
        {
            int prev = store[index];
            store[index] = element.InternalGetIndex();
            return new Int32DbId(prev);
        }


        public IDbId RemoveAt(int index)
        {
            IDbId ret = new Int32DbId(store[index]);
            --size;
            if (size > 0)
            {
                store[index] = store[size];
            }
            return ret;
        }


        public void Clear()
        {
            size = 0;
        }


        public int BinarySearch(IDbIdRef key)
        {
            return Array.IndexOf(store, key.InternalGetIndex());
        }


        public bool Contains(IDbIdRef o)
        {
            // TODO: recognize sorted arrays, then use binary search?
            int oid = o.InternalGetIndex();
            for (int i = 0; i < size; i++)
            {
                if (store[i] == oid)
                {
                    return true;
                }
            }
            return false;
        }


        public void Sort()
        {
            Array.Sort(store, 0, size);
        }


        public void Sort(Comparison<IDbIdRef> comparator)
        {
            Int32DbIdArrayQuickSort.Sort(store, 0, size, comparator);
        }


        public void Sort(int start, int end, Comparison<IDbIdRef> comparator)
        {
            Int32DbIdArrayQuickSort.Sort(store, start, end, comparator);
        }


        public void Swap(int a, int b)
        {
            int tmp = store[b];
            store[b] = store[a];
            store[a] = tmp;
        }


        public IArrayDbIds Slice(int begin, int end)
        {
            return new SliceItem(this, begin, end);
        }



        /**
         * Slice of an array.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public class SliceItem : IArrayDbIds
        {
            /**
             * Slice positions.
             */
            int begin, end;
            ArrayModifiableInt32DbIds ids;
            /**
             * Constructor.
             * 
             * @param begin Begin, inclusive
             * @param end End, exclusive
             */
            public SliceItem(ArrayModifiableInt32DbIds ids, int begin, int end)
            {
                this.ids = ids;
                this.begin = begin;
                this.end = end;
            }


            public int Count
            {
                get { return end - begin; }
            }


            public bool Contains(IDbIdRef o)
            {
                // TODO: recognize sorted arrays, then use binary search?
                int oid = o.InternalGetIndex();
                for (int i = begin; i < end; i++)
                {
                    if (ids.store[i] == oid)
                    {
                        return true;
                    }
                }
                return false;
            }


            public bool IsEmpty()
            {
                return begin == end;
            }


            public IDbId this[int i]
            {
                get { return ids[(begin + i)]; }
                set { throw new InvalidOperationException(); }
            }


            public void AssignVar(int index, IDbIdVar var)
            {
                ids.AssignVar(begin + index, var);
            }


            public int BinarySearch(IDbIdRef key)
            {
                return Array.IndexOf(ids.store, begin, key.InternalGetIndex()) - begin;
            }





            public IArrayDbIds Slice(int begin, int end)
            {
                return new SliceItem(ids, begin + begin, begin + end);
            }


            public IEnumerator<IDbId> GetEnumerator()
            {
                for (int i = begin; i < end; i++)
                {
                    yield return ids[i];
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public IEnumerator<IDbId> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public virtual void Sort(IComparer<IDbId> comparer)
        {
            IDbId[] data = new IDbId[store.Length];
            for (int i = 0; i < store.Length; i++)
            {
                data[i] = new Int32DbId(store[i]);
            }
            // Sort
            Array.Sort(data, comparer);
            // Copy back
            for (int i = 0; i < store.Length; i++)
            {
                store[i] = data[i].Int32Id;
            }
        }

    }

}
