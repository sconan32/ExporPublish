using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Pairs
{

    public class DoubleIntPair : IComparable<DoubleIntPair>, IPair<Double, Int32>
    {
        /**
         * first value
         */
        public double first;

        /**
         * second value
         */
        public int second;

        /**
         * Constructor
         * 
         * @param first First value
         * @param second Second value
         */
        public DoubleIntPair(double first, int second)
        {

            this.first = first;
            this.second = second;
        }

        /**
         * Trivial equals implementation
         * 
         * @param obj Object to compare to
         */

        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }

            DoubleIntPair other = (DoubleIntPair)obj;
            return (this.first == other.first) && (this.second == other.second);
        }

        /**
         * Trivial hashCode implementation mixing the two integers.
         */

        public override int GetHashCode()
        {
            long firsthash = (long)(first);
            firsthash = firsthash ^ (firsthash >> 32);
            // primitive hash function mixing the two integers.
            // this number does supposedly not have any factors in common with 2^32
            return (int)(firsthash * 2654435761L + second);
        }

        /**
         * Implementation of comparable interface, sorting by first then second.
         * 
         * @param other Object to compare to
         * @return comparison result
         */

        public  int CompareTo(DoubleIntPair other)
        {
            int fdiff = this.first.CompareTo(other.first);
            if (fdiff != 0)
            {
                return fdiff;
            }
            return this.second - other.second;
        }

        /**
         * Implementation of comparableSwapped interface, sorting by second then
         * first.
         * 
         * @param other Object to compare to
         * @return comparison result
         */
        public int CompareSwappedTo(DoubleIntPair other)
        {
            int fdiff = this.second - other.second;
            if (fdiff != 0)
            {
                return fdiff;
            }
            return this.second.CompareTo(other.second);
        }

        /**
         * @deprecated use pair.first to avoid boxing!
         */


        public Double First
        {
            get { return first; }
            set { this.first = value; }
        }

        /**
         * @deprecated use pair.first to avoid boxing!
         */

        public int Second
        {
            get { return second; }

            /**
             * Set second value
             * 
             * @param second new value
             */
            set { this.second = value; }
        }

        /**
         * Comparator to compare by second component only
         */
        public static Comparison<DoubleIntPair> BYFIRST_COMPARATOR = new Comparison<DoubleIntPair>(
          (o1, o2) =>
          {
              return o1.first.CompareTo(o2.first);
          });


        /**
         * Comparator to compare by second component only
         */
        public static Comparison<DoubleIntPair> BYSECOND_COMPARATOR = new Comparison<DoubleIntPair>(
            (o1, o2) =>
            {
                return o1.second - o2.second;
            });

        /**
         * Comparator to compare by swapped components
         */
        public static Comparison<DoubleIntPair> SWAPPED_COMPARATOR = new Comparison<DoubleIntPair>(
            (o1, o2) =>
            {
                return o1.CompareSwappedTo(o2);
            });

    }
}
