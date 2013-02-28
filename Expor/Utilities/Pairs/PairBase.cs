using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Pairs
{

    public class Pair<FIRST, SECOND> : IPair<FIRST, SECOND>
    {
        /**
         * First value in pair
         */
        protected FIRST first;

        /**
         * Second value in pair
         */
        protected SECOND second;

        /**
         * Initialize pair
         * 
         * @param first first parameter
         * @param second second parameter
         */
        public Pair(FIRST first, SECOND second)
        {
            this.first = first;
            this.second = second;
        }

        /**
         * Canonical toString operator
         */

        public override String ToString()
        {
            return "Pair(" + (first != null ? first.ToString() : "null") + ", " + (second != null ? second.ToString() : "null") + ")";
        }

        /**
         * Getter for first
         * 
         * @return first element in pair
         */

        public FIRST GetFirst()
        {
            return first;
        }

        /**
         * Setter for first
         * 
         * @param first new value for first element
         */
        public void SetFirst(FIRST first)
        {
            this.first = first;
        }

        /**
         * Getter for second element in pair
         * 
         * @return second element in pair
         */

        public SECOND GetSecond()
        {
            return second;
        }

        /**
         * Setter for second
         * 
         * @param second new value for second element
         */
        public void SetSecond(SECOND second)
        {
            this.second = second;
        }

        /**
         * Create a new array of the given size (for generics)
         * 
         * @param <F> First class
         * @param <S> Second class
         * @param size array size
         * @return empty array of the new type.
         */
        public static Pair<F, S>[] NewPairArray<F, S>(int size)
        {
            //Class<Pair<F, S>> paircls = ClassGenericsUtil.uglyCastIntoSubclass(Pair.class);
            Type paircls = typeof(Pair<F, S>);
            return ClassGenericsUtil.NewArrayOfNull<Pair<F, S>>(size, paircls);
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
            if (!(obj is Pair<FIRST, SECOND>))
            {
                return false;
            }
            Pair<FIRST, SECOND> other = (Pair<FIRST, SECOND>)obj;
            // Handle "null" values appropriately
            if (this.first == null)
            {
                if (other.first != null)
                {
                    return false;
                }
            }
            else
            {
                if (!this.first.Equals(other.first))
                {
                    return false;
                }
            }
            if (this.second == null)
            {
                if (other.second != null)
                {
                    return false;
                }
            }
            else
            {
                if (!this.second.Equals(other.second))
                {
                    return false;
                }
            }
            return true;
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
            result = prime * result + ((first == null) ? 0 : first.GetHashCode());
            result = prime * result + ((second == null) ? 0 : second.GetHashCode());
            return (int)result;
        }

        public FIRST First
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

        public SECOND Second
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
