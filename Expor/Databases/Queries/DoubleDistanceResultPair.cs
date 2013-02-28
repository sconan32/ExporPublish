using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries
{

    public class DoubleDistanceResultPair : IDistanceResultPair
    {
        /**
         * Distance value
         */
        double distance;

        /**
         * Object ID
         */
        IDbId id;

        /**
         * Constructor.
         * 
         * @param distance Distance value
         * @param id Object ID
         */
        public DoubleDistanceResultPair(double distance, IDbId id)
        {

            this.distance = distance;
            this.id = id;
        }


        public IDistanceValue GetDistance()
        {
            return new DoubleDistanceValue(distance);
        }


        public void SetDistance(IDistanceValue distance)
        {
            this.distance = (distance as DoubleDistanceValue).DoubleValue();
        }


        public IDbId DbId
        {
            get
            {
                return id;
            }
            set { id = value; }
        }


        public int Int32Id
        {
            get
            {
                return id.Int32Id;
            }
        }


        public bool IsSameDbId(IDbIdRef other)
        {
            return id.IsSameDbId(other);
        }


        public int CompareDbId(IDbIdRef other)
        {
            return id.CompareDbId(other);
        }



        /**
         * @deprecated Use {@link #getDoubleDistance} or {@link #getDistance} for clearness.
         */
        [Obsolete]
        public IDistanceValue GetFirst()
        {
            return (DoubleDistanceValue)GetDistance();
        }

        /**
         * @deprecated Use {@link #getDbId} for clearness.
         */
        [Obsolete]
        public IDbId GetSecond()
        {
            return id;
        }


        public int CompareByDistance(IDistanceResultPair o)
        {
            if (o is DoubleDistanceResultPair)
            {
                DoubleDistanceResultPair od = (DoubleDistanceResultPair)o;
                int delta = (distance > od.distance) ? 1 : (distance < od.distance ? -1 : 0);
                if (delta != 0)
                {
                    return delta;
                }
            }
            else
            {
                int delta = (distance >( o.GetDistance() as DoubleDistanceValue).DoubleValue()) ? 1 :
                    (distance < (o.GetDistance() as DoubleDistanceValue).DoubleValue() ? -1 : 0);
                if (delta != 0)
                {
                    return delta;
                }
            }
            return 0;
        }


        public int CompareTo(IDistanceResultPair o)
        {
            int delta = this.CompareByDistance(o);
            if (delta != 0)
            {
                return delta;
            }
            return id.CompareTo(o.DbId);
        }


        public override bool Equals(Object obj)
        {
            if (!(obj is IDistanceResultPair))
            {
                return false;
            }
            if (obj is DoubleDistanceResultPair)
            {
                DoubleDistanceResultPair ddrp = (DoubleDistanceResultPair)obj;
                return distance == ddrp.distance && id.IsSameDbId(ddrp.id);
            }
            IDistanceResultPair other = (IDistanceResultPair)obj;
            return other.GetDistance().Equals(distance) && id.IsSameDbId(other.DbId);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /**
         * Get the distance as double value.
         * 
         * @return distance value
         */
        public double GetDoubleDistance()
        {
            return distance;
        }


        public override String ToString()
        {
            return "DistanceResultPair(" + distance + ", " + id + ")";
        }

        public IDistanceValue First
        {
            get
            {
                return (DoubleDistanceValue)GetDistance();
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public IDbId Second
        {
            get
            {
                return id;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        IDistanceValue IDistanceResultPair.GetDistance()
        {
            throw new NotImplementedException();
        }

        void IDistanceResultPair.SetDistance(IDistanceValue first)
        {
            throw new NotImplementedException();
        }

        IDbId IDistanceResultPair.DbId
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        int IDistanceResultPair.CompareByDistance(IDistanceResultPair o)
        {
            throw new NotImplementedException();
        }

        IDistanceValue Utilities.Pairs.IPair<IDistanceValue, IDbId>.First
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        IDbId Utilities.Pairs.IPair<IDistanceValue, IDbId>.Second
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        int IComparable<IDistanceResultPair>.CompareTo(IDistanceResultPair other)
        {
            throw new NotImplementedException();
        }

        IDbId IDbIdRef.DbId
        {
            get { throw new NotImplementedException(); }
        }

        int IDbIdRef.Int32Id
        {
            get { throw new NotImplementedException(); }
        }

        bool IDbIdRef.IsSameDbId(IDbIdRef other)
        {
            throw new NotImplementedException();
        }

        int IDbIdRef.CompareDbId(IDbIdRef other)
        {
            throw new NotImplementedException();
        }

        int IDbIdRef.InternalGetIndex()
        {
            throw new NotImplementedException();
        }
    }
}
