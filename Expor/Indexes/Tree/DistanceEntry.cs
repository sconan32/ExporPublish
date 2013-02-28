using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Indexes.Tree
{

    public class DistanceEntry<E> : IComparable<DistanceEntry<E>>
    where E : IEntry
    {
        /**
         * The entry of the Index.
         */
        private E entry;

        /**
         * The distance value belonging to the entry.
         */
        private IDistanceValue distance;

        /**
         * The index of the entry in its parent's child array.
         */
        private int index;

        /**
         * Constructs a new DistanceEntry object with the specified parameters.
         * 
         * @param entry the entry of the Index
         * @param distance the distance value belonging to the entry
         * @param index the index of the entry in its parent' child array
         */
        public DistanceEntry(E entry, IDistanceValue distance, int index)
        {
            this.entry = entry;
            this.distance = distance;
            this.index = index;
        }

        /**
         * Returns the entry of the Index.
         * 
         * @return the entry of the Index
         */
        public E GetEntry()
        {
            return entry;
        }

        /**
         * Returns the distance value belonging to the entry.
         * 
         * @return the distance value belonging to the entry
         */
        public IDistanceValue GetDistance()
        {
            return distance;
        }

        /**
         * Returns the index of this entry in its parents child array.
         * 
         * @return the index of this entry in its parents child array
         */
        public int GetIndex()
        {
            return index;
        }

        /**
         * Compares this object with the specified object for order.
         * 
         * @param o the Object to be compared.
         * @return a negative integer, zero, or a positive integer as this object is
         *         less than, equal to, or greater than the specified object.
         * @throws ClassCastException if the specified object's type prevents it from
         *         being compared to this Object.
         */

        public  int CompareTo(DistanceEntry<E> o)
        {
            int comp = distance.CompareTo(o.distance);
            if (comp != 0)
            {
                return comp;
            }

            //return entry.getEntryID().compareTo(o.entry.getEntryID());
            return 0;
        }

        /**
         * Returns a string representation of the object.
         * 
         * @return a string representation of the object.
         */

        public override String ToString()
        {
            return "" + entry.ToString() + "(" + distance + ")";
        }
    }
}
