using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Results.Optics
{

    public class DoubleDistanceClusterOrderEntry : IComparable<IClusterOrderEntry>, IClusterOrderEntry
    {
        /**
         * The id of the entry.
         */
        private IDbId objectID;

        /**
         * The id of the entry's predecessor.
         */
        private IDbId predecessorID;

        /**
         * The reachability of the entry.
         */
        private double reachability;

        /**
         * Creates a new entry in a cluster order with the specified parameters.
         * 
         * @param objectID the id of the entry
         * @param predecessorID the id of the entry's predecessor
         * @param reachability the reachability of the entry
         */
        public DoubleDistanceClusterOrderEntry(IDbId objectID, IDbId predecessorID, double reachability)
        {
            this.objectID = objectID;
            this.predecessorID = predecessorID;
            this.reachability = reachability;
        }

        /**
         * Indicates whether some other object is "equal to" this one.
         * 
         * NOTE: for the use in an UpdatableHeap, only the ID is compared!
         * 
         * @param o the reference object with which to compare.
         * @return <code>true</code> if this object has the same attribute values as
         *         the o argument; <code>false</code> otherwise.
         */

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || !(o is IClusterOrderEntry))
            {
                return false;
            }

            IClusterOrderEntry that = (IClusterOrderEntry)o;
            // Compare by ID only, for UpdatableHeap!
            return objectID.IsSameDbId(that.GetID());
        }

        /**
         * Returns a hash code value for the object.
         * 
         * @return the object id if this entry
         */

        public override int GetHashCode()
        {
            return objectID.GetHashCode();
        }

        /**
         * Returns a string representation of the object.
         * 
         * @return a string representation of the object.
         */

        public override String ToString()
        {
            return objectID + "(" + predecessorID + "," + reachability + ")";
        }

        /**
         * Returns the object id of this entry.
         * 
         * @return the object id of this entry
         */

        public IDbId GetID()
        {
            return objectID;
        }

        /**
         * Returns the id of the predecessor of this entry if this entry has a
         * predecessor, null otherwise.
         * 
         * @return the id of the predecessor of this entry
         */

        public IDbId GetPredecessorID()
        {
            return predecessorID;
        }

        /**
         * Returns the reachability distance of this entry
         * 
         * @return the reachability distance of this entry
         */

        public IDistanceValue GetReachability()
        {
            return new DoubleDistanceValue(reachability);
        }


        public int CompareTo(IClusterOrderEntry o)
        {
            if (o is DoubleDistanceClusterOrderEntry)
            {
                int delta = this.reachability.CompareTo(((DoubleDistanceClusterOrderEntry)o).reachability);
                if (delta != 0)
                {
                    return delta;
                }
            }
            else
            {
                int delta = this.GetReachability().CompareTo(o.GetReachability());
                if (delta != 0)
                {
                    return delta;
                }
            }
            return GetID().CompareTo(o.GetID());
        }
    }
}
