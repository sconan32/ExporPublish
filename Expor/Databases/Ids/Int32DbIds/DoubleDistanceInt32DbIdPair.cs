using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{

    /**
     * Class storing a double distance a DBID.
     * 
     * @author Erich Schubert
     */
    public class DoubleDistanceInt32DbIdPair : IDoubleDistanceDbIdPair
    {
        /**
         * The distance value.
         */
        internal double distance;

        /**
         * The integer DBID.
         */
        internal int id;

        /**
         * Constructor.
         * 
         * @param distance Distance value
         * @param id integer object ID
         */
        internal DoubleDistanceInt32DbIdPair(double distance, int id)
        {

            this.distance = distance;
            this.id = id;
        }

        public double DoubleDistance()
        {
            return distance;
        }


        public IDistanceValue Distance
        {
            get { return new DoubleDistanceValue(distance); }
        }


        public int InternalGetIndex()
        {
            return id;
        }


        public int CompareByDistance(IDistanceDbIdPair o)
        {
            if (o is IDoubleDistanceDbIdPair)
            {
                return distance.CompareTo(((IDoubleDistanceDbIdPair)o).DoubleDistance());
            }
            return distance.CompareTo(((DoubleDistanceValue)o.Distance).DoubleValue());
        }


        public override String ToString()
        {
            return distance + ":" + id;
        }


        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o is DoubleDistanceInt32DbIdPair)
            {
                DoubleDistanceInt32DbIdPair p = (DoubleDistanceInt32DbIdPair)o;
                return (this.id == p.id && this.distance.CompareTo(p.distance) == 0);
            }
            if (o is DistanceInt32DbIdPair)
            {
                DistanceInt32DbIdPair p = (DistanceInt32DbIdPair)o;
                if (p.Distance is DoubleDistanceValue)
                {
                    return (this.id == p.Int32Id) && (this.distance.CompareTo(((DoubleDistanceValue)p.Distance).DoubleValue()) == 0);
                }
            }
            return false;
        }


        public override int GetHashCode()
        {

            return distance.GetHashCode() ^ id;
        }



        public IDbId DbId
        {
            get { return new Int32DbId(id); }
        }

        public int Int32Id
        {
            get { return id; }
        }

        public bool IsSameDbId(IDbIdRef other)
        {
            return other.Int32Id == id;
        }

        public int CompareDbId(IDbIdRef other)
        {
            return id.CompareTo(other.Int32Id);
        }
    }

}
