using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Databases.Queries
{

    public class GenericDistanceResultPair : Pair<IDistanceValue, IDbId>, IDistanceResultPair
    {
        /**
         * Canonical constructor
         * 
         * @param first Distance
         * @param second Object ID
         */
        public GenericDistanceResultPair(IDistanceValue first, IDbId second)
            : base(first, second)
        {
        }

        /**
         * Getter for first
         * 
         * @return first element in pair
         */

        public IDistanceValue GetDistance()
        {
            return first;
        }

        /**
         * Setter for first
         * 
         * @param first new value for first element
         */

        public void SetDistance(IDistanceValue first)
        {
            this.first = first;
        }

        /**
         * Getter for second element in pair
         * 
         * @return second element in pair
         */

        public IDbId DbId
        {
            get
            {
                return second;
            }
            set { this.second = value; }
        }


        public int Int32Id
        {
            get
            {
                return second.Int32Id;
            }
        }


        public bool IsSameDbId(IDbIdRef other)
        {
            return second.IsSameDbId(other);
        }


        public int CompareDbId(IDbIdRef other)
        {
            return second.CompareDbId(other);
        }

        /**
         * Setter for second
         * 
         * @param second new value for second element
         */

        public void SetID(IDbId second)
        {
            this.second = second;
        }


        public int CompareByDistance(IDistanceResultPair o)
        {
            return first.CompareTo(o.GetDistance());
        }


        public int CompareTo(IDistanceResultPair o)
        {
            int ret = CompareByDistance(o);
            if (ret != 0)
            {
                return ret;
            }
            return second.CompareTo(o.DbId);
        }


        public override bool Equals(Object obj)
        {
            if (!(obj is IDistanceResultPair))
            {
                return false;
            }
            IDistanceResultPair other = (IDistanceResultPair)obj;
            return first.Equals(other.GetDistance()) && second.IsSameDbId(other.DbId);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override String ToString()
        {
            return "DistanceResultPair(" + GetFirst() + ", " + GetSecond() + ")";
        }



        public int InternalGetIndex()
        {
            return second.Int32Id;
        }
    }
}
