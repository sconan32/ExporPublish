using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Socona.Expor.Distances.DistanceValues
{

    /**
     * Provides a Distance for a double-valued distance.
     * 
     * @author Elke Achtert
     */
    public class DoubleDistanceValue : NumberDistanceValue<Double>
    {
        /**
         * The static factory instance
         */
        public static readonly DoubleDistanceValue STATIC = new DoubleDistanceValue();

        /**
         * The actual value.
         */
        double value;

        /**
         * Generated serialVersionUID.
         */
        //private static readonly long serialVersionUID = 3711413449321214862L;

        /**
         * Empty constructor for serialization purposes.
         */
        public DoubleDistanceValue()
            : base()
        {

        }

        /**
         * Constructs a new DoubleDistance object that represents the double argument.
         * 
         * @param value the value to be represented by the DoubleDistance.
         */
        public DoubleDistanceValue(double value)
            : base()
        {

            this.value = value;
        }


        //public override NumberDistance<double> FromDouble(double val)
        //{
        //    return new DoubleDistance(val);
        //}


        public DoubleDistanceValue Plus(DoubleDistanceValue distance)
        {
            return new DoubleDistanceValue(this.value + distance.value);
        }


        public DoubleDistanceValue Minus(DoubleDistanceValue distance)
        {
            return new DoubleDistanceValue(this.value - distance.value);
        }

        /**
         * Returns a new distance as the product of this distance and the given
         * distance.
         * 
         * @param distance the distance to be multiplied with this distance
         * @return a new distance as the product of this distance and the given
         *         distance
         */
        public DoubleDistanceValue Times(DoubleDistanceValue distance)
        {
            return new DoubleDistanceValue(this.value * distance.value);
        }

        /**
         * Returns a new distance as the product of this distance and the given double
         * value.
         * 
         * @param lambda the double value this distance should be multiplied with
         * @return a new distance as the product of this distance and the given double
         *         value
         */
        public DoubleDistanceValue Times(double lambda)
        {
            return new DoubleDistanceValue(this.value * lambda);
        }


        public override Double Value
        {
            get { return this.value; }
            set { this.value = value; }

        }
        public override double DoubleValue()
        {
            return value;
        }





        public int CompareTo(DoubleDistanceValue other)
        {
            return this.value.CompareTo(other.value);
        }


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
            double delta = System.Math.Abs(value - ((DoubleDistanceValue)o).value);
            return delta < Double.MinValue;
        }

        /**
         * An infinite DoubleDistance is based on {@link Double#POSITIVE_INFINITY
         * Double.POSITIVE_INFINITY}.
         */

        public override IDistanceValue Infinity
        {
            get { return new DoubleDistanceValue(Double.PositiveInfinity); }
        }

        /**
         * A null DoubleDistance is based on 0.
         */

        public override IDistanceValue Empty
        {
            get { return new DoubleDistanceValue(0.0); }
        }

        /**
         * An undefined DoubleDistance is based on {@link Double#NaN Double.NaN}.
         */

        public override IDistanceValue Undefined
        {
            get { return new DoubleDistanceValue(Double.NaN); }
        }

        /**
         * As pattern is required a String defining a Double.
         */

        public override IDistanceValue ParseString(String val)
        {
            if (val.Equals(INFINITY_PATTERN))
            {
                return Infinity;
            }
            if (TestInputPattern(val))
            {
                return new DoubleDistanceValue(Double.Parse(val));
            }
            else
            {
                throw new ArgumentException("Given pattern \"" + val + "\" does not match required pattern \"" + RequiredInputPattern + "\"");
            }
        }


        public override bool IsInfinity
        {
            get { return Double.IsInfinity(value); }
        }


        public override bool IsEmpty
        {
            get { return (value == 0.0); }
        }


        public override bool IsUndefined
        {
            get { return Double.IsNaN(value); }
        }


        public override Regex GetPattern()
        {
            return DOUBLE_PATTERN;
        }


        public override int GetHashCode()
        {

            long bits = BitConverter.DoubleToInt64Bits(value);
            return (int)(bits ^ (bits >> 32));
        }



        public override long LongValue()
        {
            throw new NotImplementedException();
        }


        public override int CompareTo(IDistanceValue obj)
        {
            return this.value.CompareTo(((DoubleDistanceValue)obj).value);
        }


    }
}
