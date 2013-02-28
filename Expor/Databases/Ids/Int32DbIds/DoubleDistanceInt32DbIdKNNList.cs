using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{

    /**
     * kNN list, but without automatic sorting. Use with care, as others may expect
     * the results to be sorted!
     * 
     * @author Erich Schubert
     */
    public class DoubleDistanceInt32DbIdKNNList : DoubleDistanceInt32DbIdList, IDoubleDistanceKNNList
    {
        /**
         * The k value this list was generated for.
         */
        protected int k;

        /**
         * Constructor.
         */
        public DoubleDistanceInt32DbIdKNNList() :
            base()
        {
            this.k = -1;
        }

        /**
         * Constructor.
         * 
         * @param k K parameter
         * @param size Actual size
         */
        public DoubleDistanceInt32DbIdKNNList(int k, int size) :
            base(size)
        {
            this.k = k;
        }


        public virtual int K
        {
            get { return k; }
        }




        public virtual double DoubleKNNDistance
        {
            get { return (Count >= k) ? this[k - 1].DoubleDistance() : Double.PositiveInfinity; }
        }


        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("kNNList[");
            // for (DoubleDistanceDbIdListIter iter = this.iter(); iter.valid();) {
            foreach (var iter in this)
            {
                var it = iter as IDoubleDistanceDbIdPair;
                buf.Append(it.DoubleDistance()).Append(':').Append(iter.InternalGetIndex());

                buf.Append(',');

            }
            buf.Append(']');
            return buf.ToString();
        }

        public override IEnumerator<IDistanceDbIdPair> GetEnumerator()
        {
            int minimun = Math.Min(K, this.Count);
            for (int i = 0; i < minimun; i++)
            {
                yield return this[i];
            }

        }
        public override IDbIds ToDbIds()
        {
            IModifiableDbIds ids = DbIdUtil.NewArray();
            foreach (var id in this)
            {
                ids.Add(id.DbId);
            }
            return ids;
        }



        public IDistanceValue KNNDistance
        {
            get { return new DoubleDistanceValue(DoubleKNNDistance); }
        }

        IDistanceDbIdPair IDistanceDbIdList.this[int off]
        {
            get { return this[off]; }
        }

    }

}
