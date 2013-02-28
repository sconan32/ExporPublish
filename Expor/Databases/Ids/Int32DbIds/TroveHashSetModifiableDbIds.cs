using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{
    public class TroveHashSetModifiableDbIds : IHashSetModifiableDbIds
    {
        /**
         * The actual store.
         */
        HashSet<int> store;

        /**
         * Constructor.
         * 
         * @param size Initial size
         */
        public TroveHashSetModifiableDbIds(int size) :
            base()
        {
            this.store = new HashSet<Int32>();
        }

        /**
         * Constructor.
         */
        public TroveHashSetModifiableDbIds() :
            base()
        {
            this.store = new HashSet<int>();
        }

        /**
         * Constructor.
         * 
         * @param existing Existing IDs
         */
        public TroveHashSetModifiableDbIds(IDbIds existing) :
            this(existing.Count)
        {
            this.AddDbIds(existing);
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


        public bool Add(IDbIdRef e)
        {
            return store.Add(e.Int32Id);
        }

        public bool Remove(IDbIdRef o)
        {
            return store.Remove(o.Int32Id);
        }


        public bool RetainAll(IDbIds set)
        {
            bool modified = false;
            foreach (var id in set)
            {
                if (store.Contains(id.Int32Id))
                {
                    store.Remove(id.Int32Id);
                    modified = true;
                }
            }
            return modified;

        }





        public bool IsEmpty()
        {
            return store.Count <= 0;
        }


        public void Clear()
        {
            store.Clear();
        }


        public bool Contains(IDbIdRef o)
        {
            return store.Contains(o.Int32Id);
        }


        public int Count
        {
            get { return store.Count; }
        }

        public IEnumerator<IDbId> GetEnumerator()
        {
            foreach (int i in store)
            {

                yield return new Int32DbId(i);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(this.GetType().Name);
            buf.Append(", ");
            buf.Append("Count=");
            buf.Append(Count);
               
            //buf.Append("[ ");
            //foreach (var item in store)
            //{
            //    buf.Append(item.ToString()).Append("  ");
            //}
            //buf.Append("]");
            return buf.ToString();
        }
    }
}
