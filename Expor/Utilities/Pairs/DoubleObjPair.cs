using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Pairs
{

    public class DoubleObjPair<O> : IPair<Double, O>, IComparable<DoubleObjPair<O>>
    {
        /**
         * Double value
         */
        public double first;

        /**
         * Second object value
         */
        public O second;

        /**
         * Constructor.
         * 
         * @param first First value
         * @param second Second value
         */
        public DoubleObjPair(double first, O second)
        {
            this.first = first;
            this.second = second;
        }

        /**
         * @deprecated use pair.first to avoid boxing!
         */
        [Obsolete]
        public Double GetFirst()
        {
            return first;
        }


        public O GetSecond()
        {
            return second;
        }


        public int CompareTo(DoubleObjPair<O> o)
        {
            return first.CompareTo(o.first);
        }


        public override bool Equals(Object obj)
        {
            if (!(obj is DoubleObjPair<O>))
            {
                // TODO: allow comparison with arbitrary pairs?
                return false;
            }
            DoubleObjPair<O> other = (DoubleObjPair<O>)obj;
            if (first != other.first)
            {
                return false;
            }
            if (second == null)
            {
                return (other.second == null);
            }
            return second.Equals(other.second);
        }

        public override int GetHashCode()
        {
            return this.first.GetHashCode() ^ this.second.GetHashCode();
        }
        /**
         * Canonical toString operator
         */

        public override String ToString()
        {
            return "Pair(" + first + ", " + (second != null ? second.ToString() : "null") + ")";
        }

        public double First
        {
            get
            {
                return first;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public O Second
        {
            get
            {
                return second;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }
    }
}
