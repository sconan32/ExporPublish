using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Indexes.Tree.Queries
{

    public class DoubleDistanceSearchCandidate : IComparable<DoubleDistanceSearchCandidate>
    {
        /**
         * Distance value
         */
        public double mindist;

        /**
         * Page id
         */
        public int nodeID;

        /**
         * Constructor.
         * 
         * @param mindist The minimum distance to this candidate
         * @param pagenr The page number of this candidate
         */
        public DoubleDistanceSearchCandidate(double mindist, int pagenr)
        {

            this.mindist = mindist;
            this.nodeID = pagenr;
        }


        public override bool Equals(Object obj)
        {
            DoubleDistanceSearchCandidate other = (DoubleDistanceSearchCandidate)obj;
            return this.nodeID == other.nodeID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode()^this.nodeID.GetHashCode();
        }

        public int CompareTo(DoubleDistanceSearchCandidate o)
        {
            return this.mindist.CompareTo(o.mindist);
        }
    }
}
