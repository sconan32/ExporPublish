using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Socona.Expor.Distances.DistanceValues
{

    /**
     * An abstract distance implements equals conveniently for any extending class.
     * At the same time any extending class is to implement hashCode properly.
     * 
     * See {@link de.lmu.ifi.dbs.elki.distance.DistanceUtil} for related utility
     * functions such as <code>min</code>, <code>max</code>.
     * 
     * @author Arthur Zimek
     * @see de.lmu.ifi.dbs.elki.distance.DistanceUtil
     * @param <D> the () type of Distance used
     */
    public abstract class AbstractDistanceValue : IDistanceValue
    {
        /**
         * Indicates an infinity pattern.
         */
        public static readonly String INFINITY_PATTERN = "inf";

        /**
         * Pattern for parsing and validating double values
         */
        public static readonly Regex DOUBLE_PATTERN = new Regex("(\\d+|\\d*\\.\\d+)?([eE][-]?\\d+)?");

        /**
         * Pattern for parsing and validating integer values
         */
        public static readonly Regex INTEGER_PATTERN = new Regex("\\d+");


        /**
         * Returns true if <code>this == o</code> has the value <code>true</code> or o
         * is not null and o is of the same class as this instance and
         * <code>this.compareTo(o)</code> is 0, false otherwise.
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

            return this.CompareTo((AbstractDistanceValue)o) == 0;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /**
         * Get the pattern accepted by this distance
         * 
         * @return Pattern
         */
        public abstract Regex GetPattern();


        public String RequiredInputPattern
        {
            get { return GetPattern().ToString(); }
        }

        /**
         * Test a string value against the input pattern.
         * 
         * @param value String value to test
         * @return Match result
         */
        public virtual bool TestInputPattern(String value)
        {
            return GetPattern().IsMatch(value);
        }


        public virtual bool IsInfinity
        {
            get { return this.Equals(this.Infinity); }
        }


        public virtual bool IsEmpty
        {
            get { return this.Equals(this.Empty); }
        }


        public virtual bool IsUndefined
        {
            get { return this.Equals(this.Undefined); }
        }


        public abstract IDistanceValue ParseString(string pattern);


        public abstract IDistanceValue Infinity { get; }


        public abstract IDistanceValue Empty { get; }


        public abstract IDistanceValue Undefined { get; }

        public abstract double ToDouble();

        public int CompareTo(object obj)
        {
            return CompareTo((IDistanceValue)obj);
        }

        public abstract int CompareTo(IDistanceValue other);

    }
}
