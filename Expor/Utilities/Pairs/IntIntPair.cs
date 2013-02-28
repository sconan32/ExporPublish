using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Pairs
{

    public class IntIntPair : IComparable<IntIntPair>, IPair<int, int>
    {
        /**
         * first value
         */
        public int first;

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
        public IntIntPair(int first, int second)
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

            IntIntPair other = (IntIntPair)obj;
            return (this.first == other.first) && (this.second == other.second);
        }

        /**
         * Trivial hashCode implementation mixing the two integers.
         */

        public override int GetHashCode()
        {
            // primitive hash function mixing the two integers.
            // this number does supposedly not have any factors in common with 2^32
            return (int)(first * 2654435761L + second);
        }

        /**
         * Implementation of comparable interface, sorting by first then second.
         * 
         * @param other Object to compare to
         * @return comparison result
         */

        public int CompareTo(IntIntPair other)
        {
            int fdiff = this.first - other.first;
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
        public int CompareSwappedTo(IntIntPair other)
        {
            int fdiff = this.second - other.second;
            if (fdiff != 0)
            {
                return fdiff;
            }
            return this.first - other.first;
        }

        /**
         * @deprecated use pair.first to avoid boxing!
         */

        [Obsolete]
        public int GetFirst()
        {
            return first;
        }

        /**
         * Set first value
         * 
         * @param first new value
         */
        public void SetFirst(int first)
        {
            this.first = first;
        }

        /**
         * @deprecated use pair.first to avoid boxing!
         */

        [Obsolete]
        public Int32 GetSecond()
        {
            return second;
        }

        /**
         * Set second value
         * 
         * @param second new value
         */
        public void SetSecond(int second)
        {
            this.second = second;
        }

        
        public override String ToString()
        {
            return "(" + first + ", " + second + ")";
        }

        public class ByFirstComparer : Comparer<IntIntPair>
        {

            public override int Compare(IntIntPair o1, IntIntPair o2)
            {
                return o1.first.CompareTo(o2.first);
            }
        }
        public static ByFirstComparer BYFIRST_COMPARATER = new ByFirstComparer();

        /**
         * Comparator to compare by second component only
         */
        public class BySecondComparer : Comparer<IntIntPair>
        {
            public override int Compare(IntIntPair o1, IntIntPair o2)
            {
                return o1.second.CompareTo(o2.second);
            }
        }
        public static BySecondComparer BYSECOND_COMPARER = new BySecondComparer();
        /**
         * Comparator to compare by swapped components
         */
        public class SwappedComparer : Comparer<IntIntPair>
        {
            public override int Compare(IntIntPair o1, IntIntPair o2)
            {
                return o1.CompareSwappedTo(o2);
            }
        }
        public static SwappedComparer SWAPPED_COMPARER = new SwappedComparer();

        public int First
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

        public int Second
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
