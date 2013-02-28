using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{
    public class TroveArrayModifiableDbIds : TroveArrayDbIds, IArrayModifiableDbIds
    {
        /**
         * The actual trove array list
         */
        private List<int> store;

        /**
         * Constructor.
         * 
         * @param size Initial size
         */
        public TroveArrayModifiableDbIds(int size)
            : base()
        {
            this.store = new List<int>(size);
        }

        /**
         * Constructor.
         */
        public TroveArrayModifiableDbIds()
            : base()
        {
            this.store = new List<int>();
        }

        /**
         * Constructor.
         * 
         * @param existing Existing ids
         */
        public TroveArrayModifiableDbIds(IDbIds existing) :
            this(existing.Count)
        {
            this.AddDbIds(existing);
        }


        protected override IList<int> GetStore()
        {
            return store;
        }




        public IDbId Set(int i, IDbId newval)
        {
            store[i] = newval.Int32Id;
            int prev = store[i];
            return new Int32DbId(prev);
        }


        public void Sort()
        {
            (store as List<int>).Sort();
        }

        public void Sort(IComparer<IDbId> comparer)
        {
            IDbId[] data = new IDbId[store.Count];
            for (int i = 0; i < store.Count; i++)
            {
                data[i] = new Int32DbId(store[i]);
            }
            // Sort
            Array.Sort(data, comparer);
            // Copy back
            for (int i = 0; i < store.Count; i++)
            {
                store[i] = data[i].Int32Id;
            }
        }

        public IDbId RemoveAt(int i)
        {
            IDbId id = new Int32DbId(store[i]);
            store.RemoveAt(i);
            return id;
        }


        public void Swap(int a, int b)
        {
            int t = store[a];
            store[a] = store[b];
            store[b] = t;


        }

        public bool AddDbIds(IDbIds ids)
        {
            bool success = true;
            foreach (var id in ids)
            {
                store.Add(id.Int32Id);
            }
            return success;
        }

        public bool RemoveDbIds(IDbIds ids)
        {
            bool success = false;
            foreach (IDbId id in ids)
            {
                success |= store.Remove(id.Int32Id);
            }
            return success;
        }

        public bool Add(IDbIdRef id)
        {
            store.Add(id.Int32Id);
            return true;
        }

        public bool Remove(IDbIdRef id)
        {
            return store.Remove(id.Int32Id);
        }

        public void Clear()
        {
            store.Clear();
        }

        public new System.Collections.IEnumerator GetEnumerator()
        {
            return GetStore().GetEnumerator();
        }


        //public virtual void Sort(IComparer<IDbId> comparer)
        //{
        //    store.Sort((t1, t2) => comparer.Compare(new Int32DbId(t1), new Int32DbId(t2)));
        //}










        IArrayDbIds IArrayDbIds.Slice(int begin, int end)
        {
            throw new NotImplementedException();
        }


        public virtual void Sort(Comparison<IDbIdRef> comparer)
        {

        }
    }
}





