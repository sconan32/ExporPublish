using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;

namespace Socona.Expor.Results.Outliers
{
    public class OrderingFromRelation : IOrderingResult
    {
        /**
         * Outlier scores.
         */
        protected IRelation scores;

        /**
         * Factor for ascending (+1) and descending (-1) ordering.
         */
        protected int ascending = +1;

        /**
         * Constructor for outlier orderings
         * 
         * @param scores outlier score result
         * @param ascending Ascending when {@code true}, descending otherwise
         */
        public OrderingFromRelation(IRelation scores, bool ascending)
        {

            this.scores = scores;
            this.ascending = ascending ? +1 : -1;
        }

        /**
         * Ascending constructor.
         * 
         * @param scores
         */
        public OrderingFromRelation(IRelation scores) :
            this(scores, true)
        {
        }


        public IDbIds GetDbIds()
        {
            return scores.GetDbIds();
        }


        public IArrayModifiableDbIds Iter(IDbIds ids)
        {
            IArrayModifiableDbIds sorted = DbIdUtil.NewArray(ids);
            sorted.Sort(new ImpliedComparator(ascending, scores));
            return sorted;
        }


        public String LongName
        {
            get { return scores.LongName + " Order"; }
        }


        public String ShortName
        {
            get { return scores.ShortName + "_order"; }
        }

        /**
         * Internal comparator, accessing the map to sort objects
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        protected class ImpliedComparator : IComparer<IDbId>
        {

            int ascending;
            IRelation scores;
            public ImpliedComparator(int ascending, IRelation scores)
            {
                this.ascending = ascending;
                this.scores = scores;
            }
            public int Compare(IDbId id1, IDbId id2)
            {
                Double k1 =(double) scores[(id1)];
                Double k2 =(double) scores[(id2)];
                //Debug.Assert(k1 != null);
                //Debug.Assert(k2 != null);
                return ascending * k2.CompareTo(k1);
            }
        }
    }

}
