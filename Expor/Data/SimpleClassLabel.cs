using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Data
{

    public class SimpleClassLabel : ClassLabel
    {
        /**
         * Holds the String designating the label.
         */
        private String label;

        /**
         * Constructor.
         * 
         * @param label Label
         */
        public SimpleClassLabel(String label)
            : base()
        {

            this.label = label;
        }

        /**
         * The ordering of two SimpleClassLabels is given by the ordering on the
         * Strings they represent.
         * <p/>
         * That is, the result equals <code>this.label.compareTo(o.label)</code>.
         */

        public override int CompareTo(ClassLabel o)
        {
            SimpleClassLabel other = (SimpleClassLabel)o;
            return this.label.CompareTo(other.label);
        }

        /**
         * The hash code of a simple class label is the hash code of the String
         * represented by the ClassLabel.
         */

        public override int GetHashCode()
        {
            return label.GetHashCode();
        }

        /**
         * Any ClassLabel should ensure a natural ordering that is consistent with
         * equals. Thus, if <code>this.compareTo(o)==0</code>, then
         * <code>this.equals(o)</code> should be <code>true</code>.
         * 
         * @param o an object to test for equality w.r.t. this ClassLabel
         * @return true, if <code>this==obj || this.compareTo(o)==0</code>, false
         *         otherwise
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
            if (!base.Equals(o))
            {
                return false;
            }
            SimpleClassLabel that = (SimpleClassLabel)o;

            return label.Equals(that.label);
        }

        /**
         * Returns a new instance of the String covered by this SimpleClassLabel.
         * 
         * @return a new instance of the String covered by this SimpleClassLabel
         */

        public override String ToString()
        {
            return label;
        }

        /**
         * Factory class
         * 
         * @author Erich Schubert
         * 
         * @apiviz.has SimpleClassLabel - - 芦creates禄
         * @apiviz.stereotype factory
         */
        public class Factory : ClassLabel.Factory<SimpleClassLabel>
        {

            public override SimpleClassLabel MakeFromString(String lbl)
            {
                SimpleClassLabel l = existing[(lbl)];
                if (l == null)
                {
                    l = new SimpleClassLabel(lbl);
                    existing[lbl] = l;
                }
                return l;
            }
        }
    }
}
