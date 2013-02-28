using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{
    public class Int32DbIdPair : IDbIdPair
    {
        /**
         * First value in pair
         */
        public int first;

        /**
         * Second value in pair
         */
        public int second;

        /**
         * Initialize pair
         * 
         * @param first first parameter
         * @param second second parameter
         */
        public Int32DbIdPair(int first, int second)
        {
            this.first = first;
            this.second = second;
        }

        /**
         * Canonical toString operator
         */

        public override String ToString()
        {
            return "Pair(" + first + ", " + second + ")";
        }

        /**
         * Getter for first
         * 
         * @return first element in pair
         */

        public IDbId GetFirst()
        {
            return new Int32DbId(first);
        }


        /**
         * Getter for second element in pair
         * 
         * @return second element in pair
         */

        public IDbId GetSecond()
        {
            return new Int32DbId(second);
        }

        /**
         * Simple equals statement.
         * 
         * This Pair equals another Object if they are identical or if the other
         * Object is also a Pair and the {@link #first} and {@link #second} element of
         * this Pair equal the {@link #first} and {@link #second} element,
         * respectively, of the other Pair.
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
            if (!(obj is Int32DbIdPair))
            {
                return false;
            }
            Int32DbIdPair other = (Int32DbIdPair)obj;
            return (this.first == other.first) && (this.second == other.second);
        }

        /**
         * Canonical hash function, mixing the two hash values.
         */

        public override int GetHashCode()
        {
            // primitive hash function mixing the two integers.
            // this number does supposedly not have any factors in common with 2^32
            long prime = 2654435761L;
            long result = 1;
            result = prime * result + first;
            result = prime * result + second;
            return (int)result;
        }

        public IDbId First
        {
            get
            {
                return new Int32DbId(first);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IDbId Second
        {
            get
            {
                return new Int32DbId(second);
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
