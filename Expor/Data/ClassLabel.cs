using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Socona.Expor.Utilities;

namespace Socona.Expor.Data
{

    /**
     * A ClassLabel to identify a certain class of objects that is to discern from
     * other classes by a classifier.
     * 
     * @author Arthur Zimek
     */
    public abstract class ClassLabel : IComparable<ClassLabel>
    {
        /**
         * ClassLabels need an empty constructor for dynamic access. Subsequently, the
         * init method must be called.
         */
        protected ClassLabel()
        {
            // Initialized from factory
        }

        /**
         * Any ClassLabel should ensure a natural ordering that is consistent with
         * equals. Thus, if <code>this.compareTo(o)==0</code>, then
         * <code>this.equals(o)</code> should be <code>true</code>.
         * 
         * @param obj an object to test for equality w.r.t. this ClassLabel
         * @return true, if <code>this==obj || this.compareTo(o)==0</code>, false
         *         otherwise
         */

        public override bool Equals(Object obj)
        {
            if (!(obj is ClassLabel))
            {
                return false;
            }
            return this == obj || this.CompareTo((ClassLabel)obj) == 0;
        }

        /**
         * Any ClassLabel requires a method to represent the label as a String. If
         * <code>ClassLabel a.equals((ClassLabel) b)</code>, then also
         * <code>a.toString().equals(b.toString())</code> should hold.
         * 
         * @see java.lang.Object#toString()
         */

        public override abstract String ToString();

        /**
         * Returns the hashCode of the String-representation of this ClassLabel.
         * 
         * @see java.lang.Object#hashCode()
         */

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /**
         * Class label factory
         * 
         * @author Erich Schubert
         * 
         * @apiviz.has ClassLabel - - «creates»
         * @apiviz.stereotype factory
         */
        public abstract class Factory<L> : IInspectionUtilFrequentlyScanned
            where L : ClassLabel
        {
            /**
             * Set for reusing the same objects.
             */
            protected Dictionary<String, L> existing = new Dictionary<String, L>();

            /**
             * Convert a string into a class label
             * 
             * @param lbl String to convert
             * @return Class label instance.
             */
            public abstract L MakeFromString(String lbl);
        }

        public abstract int CompareTo(ClassLabel other);
    }
}
