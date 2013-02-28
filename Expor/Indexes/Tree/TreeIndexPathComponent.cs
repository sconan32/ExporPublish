using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Indexes.Tree
{

    public class TreeIndexPathComponent<E> where E : IEntry
    {
        /**
         * The entry of this component.
         */
        private E entry;

        /**
         * The index of this component in its parent.
         */
        private Int32 index;

        /**
         * Creates a new IndexPathComponent.
         * 
         * @param entry the entry of the component
         * @param index index of the component in its parent
         */
        public TreeIndexPathComponent(E entry, Int32 index)
        {
            this.entry = entry;
            this.index = index;
        }

        /**
         * Returns the entry of the component.
         * 
         * @return the entry of the component
         */
        public E GetEntry()
        {
            return entry;
        }

        /**
         * Returns the index of the component in its parent.
         * 
         * @return the index of the component in its parent
         */
        public Int32 GetIndex()
        {
            return index;
        }

        /**
         * Returns <code>true</code> if <code>this == o</code> has the value
         * <code>true</code> or o is not null and o is of the same class as this
         * instance and if the entry of this component Equals the entry of the o
         * argument, <code>false</code> otherwise.
         */


        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }

            TreeIndexPathComponent<E> that = (TreeIndexPathComponent<E>)o;
            return (this.entry.Equals(that.entry));
        }

        /**
         * Returns a hash code for this component. The hash code of a
         * TreeIndexPathComponent is defined to be the hash code of its entry.
         * 
         * @return the hash code of the entry of this component
         */

        public override int GetHashCode()
        {
            return entry.GetHashCode();
        }

        /**
         * Returns a string representation of this component.
         * 
         * @return a string representation of the entry of this component followd by
         *         the index of this component in its parent
         */

        public override String ToString()
        {
            return entry.ToString() + " [" + index + "]";
        }
    }

}
