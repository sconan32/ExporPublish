using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Distance
{
    public abstract class AbstractDistanceDbIdList<TPair> : IDistanceDbIdList
        where TPair : IDistanceDbIdPair
    {
        IDistanceDbIdPair IDistanceDbIdList.this[int off] { get { return this[off]; } }

        public abstract TPair this[int index]
        {
            get;
            set;
        }

        public virtual IDbIds ToDbIds()
        {
            var ids = DbIdUtil.NewArray();
            var it = GetEnumerator();
            while (it.MoveNext())
            {
                ids.Add(it.Current.DbId);
            }
            return ids;
        }


        public abstract int Count { get; }

        void ICollection<IDistanceDbIdPair>.Add(IDistanceDbIdPair pair)
        {
            this.Add((TPair)pair);
        }
        public abstract void Add(TPair pair);

        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        public virtual IEnumerator<IDistanceDbIdPair> GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        public abstract void Clear();


        bool ICollection<IDistanceDbIdPair>.Contains(IDistanceDbIdPair item)
        {
            return Contains((TPair)item);
        }
        public abstract bool Contains(TPair item);


        public void CopyTo(IDistanceDbIdPair[] array, int arrayIndex)
        {

            var arr = this.ToArray();
            Array.Copy(arr, array, arr.Length);
        }

        bool ICollection<IDistanceDbIdPair>.Remove(IDistanceDbIdPair item)
        {
            return this.Remove((TPair)item);
        }
        public abstract bool Remove(TPair pair);

        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }
    }
}
