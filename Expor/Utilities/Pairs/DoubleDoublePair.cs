using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Pairs
{

    public class DoubleDoublePair : IComparable<DoubleDoublePair>, IPair<Double, Double>
    {
        /**
         * first value
         */
        public double first;

        /**
         * second value
         */
        public double second;

        /**
         * Constructor
         * 
         * @param first First value
         * @param second Second value
         */
        public DoubleDoublePair(double first, double second)
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

            DoubleDoublePair other = (DoubleDoublePair)obj;
            return (this.first == other.first) && (this.second == other.second);
        }

        /**
         * Trivial hashCode implementation mixing the two integers.
         */

        public override int GetHashCode()
        {
            // convert to longs
            return first.GetHashCode() ^ second.GetHashCode();
        }

        /**
         * Implementation of comparable interface, sorting by first then second.
         * 
         * @param other Object to compare to
         * @return comparison result
         */

        public int CompareTo(DoubleDoublePair other)
        {
            int fdiff = this.first.CompareTo(other.first);
            if (fdiff != 0)
            {
                return fdiff;
            }
            return this.second.CompareTo(other.second);
        }

        /**
         * Implementation of comparableSwapped interface, sorting by second then
         * first.
         * 
         * @param other Object to compare to
         * @return comparison result
         */
        public int CompareSwappedTo(DoubleDoublePair other)
        {
            int fdiff = this.second.CompareTo(other.second);
            if (fdiff != 0)
            {
                return fdiff;
            }
            return this.first.CompareTo(other.first);
        }

        /**
         * @deprecated use pair.first to avoid boxing!
         */
        [Obsolete]
        public Double GetFirst()
        {
            return first;
        }

        /**
         * Set first value
         * 
         * @param first new value
         */

        public void SetFirst(double first)
        {
            this.first = first;
        }

        /**
         * @deprecated use pair.first to avoid boxing!
         */
        [Obsolete]
        public Double GetSecond()
        {
            return second;
        }

        /**
         * Set second value
         * 
         * @param second new value
         */
        public void SetSecond(double second)
        {
            this.second = second;
        }


        public override String ToString()
        {
            return "DoubleDoublePair(" + first + "," + second + ")";
        }

        /**
         * Comparator to compare by second component only
         */
        public class ByFirstComparer : Comparer<DoubleDoublePair>
        {

            public override int Compare(DoubleDoublePair o1, DoubleDoublePair o2)
            {
                return o1.first.CompareTo(o2.first);
            }
        }
        public static ByFirstComparer BYFIRST_COMPARATER = new ByFirstComparer();

        /**
         * Comparator to compare by second component only
         */
        public class BySecondComparer : Comparer<DoubleDoublePair>
        {
            public override int Compare(DoubleDoublePair o1, DoubleDoublePair o2)
            {
                return o1.second.CompareTo(o2.second);
            }
        }
        public static BySecondComparer BYSECOND_COMPARER = new BySecondComparer();
        /**
         * Comparator to compare by swapped components
         */
        public class SwappedComparer : Comparer<DoubleDoublePair>
        {
            public override int Compare(DoubleDoublePair o1, DoubleDoublePair o2)
            {
                return o1.CompareSwappedTo(o2);
            }
        }
        public static SwappedComparer SWAPPED_COMPARER = new SwappedComparer();

        public double First
        {
            get
            {
                return first;
            }
            set
            {
                first = value;
            }
        }

        public double Second
        {
            get
            {
                return second;
            }
            set
            {
                second = value;
            }
        }
    }
}
