using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{

    /**
     * Class storing a double distance a DBID.
     * 
     * @author Erich Schubert
     * 
     * @param <D> Distance type
     */
    public class DistanceInt32DbIdPair : Pair<IDistanceValue,Int32>,IDistanceDbIdPair
    {


        /**
         * Constructor.
         * 
         * @param distance Distance
         * @param id Object ID
         */
        internal DistanceInt32DbIdPair(IDistanceValue distance, int id)
            :base(distance,id)
        {

            
        }


        public IDistanceValue Distance
        {
            get { return first; }
        }

        public Int32 Int32Id
        {
            get {return  second; }
        }
        public int InternalGetIndex()
        {
            return second;
        }

        public int CompareByDistance(IDistanceDbIdPair o)
        {
            return Distance.CompareTo(o.Distance);
        }


        public override String ToString()
        {
            return Distance.ToString() + ":" + Int32Id;
        }


        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o is DistanceInt32DbIdPair)
            {
                DistanceInt32DbIdPair p = (DistanceInt32DbIdPair)o;
                return (this.Int32Id == p.Int32Id) && Distance.Equals(p.Distance);
            }
            if (o is DoubleDistanceInt32DbIdPair && Distance is DoubleDistanceValue)
            {
                DoubleDistanceInt32DbIdPair p = (DoubleDistanceInt32DbIdPair)o;
                return (this.Int32Id == p.Int32Id) &&
                    (((DoubleDistanceValue)this.Distance).DoubleValue().CompareTo(p.Distance) == 0);
            }
            return false;
        }


        public override int GetHashCode()
        {
            return Distance.GetHashCode() ^ Int32Id;
        }

        IDbId IDbIdRef.DbId
        {
            get { return new Int32DbId(Int32Id); }
        }

        int IDbIdRef.Int32Id
        {
            get { return Int32Id; }
        }


        bool IDbIdRef.IsSameDbId(IDbIdRef other)
        {
            return this.Int32Id == other.Int32Id;
        }

        int IDbIdRef.CompareDbId(IDbIdRef other)
        {
            return this.Int32Id.CompareTo(other.Int32Id);
        }


    }

}
