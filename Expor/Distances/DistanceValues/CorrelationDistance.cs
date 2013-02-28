using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Socona.Expor.Distances.DistanceValues
{

    public abstract class CorrelationDistance : AbstractDistanceValue
    {
        /**
         * The component separator used by correlation distances.
         * 
         * Note: Do NOT use regular expression syntax characters!
         */
        public static String SEPARATOR = "x";

        /**
         * The pattern used for correlation distances
         */
        public static Regex CORRELATION_DISTANCE_PATTERN = new Regex("\\d+" + Regex.Escape(SEPARATOR) + "\\d+(\\.\\d+)?([eE][-]?\\d+)?");

        /**
         * Generated SerialVersionUID.
         */
        //private static long serialVersionUID = 2829135841596857929L;

        /**
         * The correlation dimension.
         */
        protected int correlationValue;

        /**
         * The euclidean distance.
         */
        protected double euclideanValue;

        /**
         * Empty constructor for serialization purposes.
         */
        public CorrelationDistance()
        {
            // for serialization
        }

        /**
         * Constructs a new CorrelationDistance object consisting of the specified
         * correlation value and euclidean value.
         * 
         * @param correlationValue the correlation dimension to be represented by the
         *        CorrelationDistance
         * @param euclideanValue the euclidean distance to be represented by the
         *        CorrelationDistance
         */
        public CorrelationDistance(int correlationValue, double euclideanValue)
        {
            this.correlationValue = correlationValue;
            this.euclideanValue = euclideanValue;
        }

        /**
         * Returns a string representation of this CorrelationDistance.
         * 
         * @return the correlation value and the euclidean value separated by blank
         */

        public override String ToString()
        {
            return (correlationValue.ToString()) + SEPARATOR + (euclideanValue.ToString());
        }

        /**
         * Compares this CorrelationDistance with the given CorrelationDistance wrt
         * the represented correlation values. If both values are considered to be
         * equal, the euclidean values are compared. Subclasses may need to overwrite
         * this method if necessary.
         * 
         * @return the value of {@link Int32#CompareTo(Int32)}
         *         this.correlationValue.CompareTo(other.correlationValue)} if it is a
         *         non zero value, the value of {@link Double#compare(double,double)
         *         Double.compare(this.euclideanValue, other.euclideanValue)}
         *         otherwise
         */

        public override int CompareTo(IDistanceValue other)
        {
            int compare = (this.correlationValue).CompareTo(
                ((CorrelationDistance)other).CorrelationValue);
            if (compare != 0)
            {
                return compare;
            }
            else
            {
                return this.euclideanValue.CompareTo(
                    ((CorrelationDistance)other).EuclideanValue);
            }
        }


        public override int GetHashCode()
        {
            int result;
            long temp;
            result = correlationValue;
            temp = euclideanValue != +0.0d ? (long)euclideanValue : 0L;
            result = 29 * result + (int)(temp ^ (temp >> 32));
            return result;
        }



        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }
            CorrelationDistance other = (CorrelationDistance)obj;
            if (this.correlationValue != other.correlationValue)
            {
                return false;
            }
            if (this.euclideanValue != other.euclideanValue)
            {
                return false;
            }
            return true;
        }

        /**
         * Returns the correlation dimension between the objects.
         * 
         * @return the correlation dimension
         */
        public int CorrelationValue
        {
            get { return correlationValue; }
        }

        /**
         * Returns the euclidean distance between the objects.
         * 
         * @return the euclidean distance
         */
        public double EuclideanValue
        {
            get { return euclideanValue; }
        }

    }
}
