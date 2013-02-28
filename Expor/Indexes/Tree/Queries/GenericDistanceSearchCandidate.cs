using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Indexes.Tree.Queries
{

    public class GenericDistanceSearchCandidate : IComparable<GenericDistanceSearchCandidate>
    {
        /**
         * Distance value
         */
        public IDistanceValue mindist;

        /**
         * Page id
         */
        public Int32 nodeID;

        /**
         * Constructor.
         * 
         * @param mindist The minimum distance to this candidate
         * @param pagenr The page number of this candidate
         */
        public GenericDistanceSearchCandidate(IDistanceValue mindist, Int32 pagenr)
        {

            this.mindist = mindist;
            this.nodeID = pagenr;
        }


        public override bool Equals(Object obj)
        {
            GenericDistanceSearchCandidate other = (GenericDistanceSearchCandidate)obj;
            return this.nodeID == other.nodeID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode()^ nodeID.GetHashCode();
        }

        public int CompareTo(GenericDistanceSearchCandidate o)
        {
            return this.mindist.CompareTo(o.mindist);
        }
    }
}
