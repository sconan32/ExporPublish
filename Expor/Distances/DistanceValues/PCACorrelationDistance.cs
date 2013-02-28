using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Socona.Expor.Distances.DistanceValues
{
    public class PCACorrelationDistance : CorrelationDistance
    {
        /**
         * The static factory instance
         */
        public static PCACorrelationDistance FACTORY = new PCACorrelationDistance();

        /**
         * Serial
         */
        // private static long serialVersionUID = 1L;

        /**
         * Empty constructor for serialization purposes.
         */
        public PCACorrelationDistance()
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
        public PCACorrelationDistance(int correlationValue, double euclideanValue) :
            base(correlationValue, euclideanValue)
        {
        }

        /**
         * Provides a distance suitable to this DistanceFunction based on the given
         * pattern.
         * 
         * @param val A pattern defining a distance suitable to this
         *        DistanceFunction
         * @return a distance suitable to this DistanceFunction based on the given
         *         pattern
         * @throws IllegalArgumentException if the given pattern is not compatible
         *         with the requirements of this DistanceFunction
         */

        public override IDistanceValue ParseString(String val)
        {
            if (val.Equals(INFINITY_PATTERN))
            {
                return Infinity;
            }
            if (TestInputPattern(val))
            {
                String[] values = SEPARATOR.Split(val.ToCharArray());
                return new PCACorrelationDistance(int.Parse(values[0]), Double.Parse(values[1]));
            }
            else
            {
                throw new ArgumentException("Given pattern \"" + val +
                    "\" does not match required pattern \"" + RequiredInputPattern + "\"");
            }
        }


        public override Regex GetPattern()
        {
            return CORRELATION_DISTANCE_PATTERN;
        }

        /**
         * Provides an infinite distance.
         * 
         * @return an infinite distance
         */

        public override IDistanceValue Infinity
        {
            get { return new PCACorrelationDistance(int.MaxValue, Double.PositiveInfinity); }
        }

        /**
         * Provides a null distance.
         * 
         * @return a null distance
         */

        public override IDistanceValue Empty
        {
            get { return new PCACorrelationDistance(0, 0.0); }
        }

        /**
         * Provides an undefined distance.
         * 
         * @return an undefined distance
         */

        public override IDistanceValue Undefined
        {
            get { return new PCACorrelationDistance(-1, Double.NaN); }
        }


        public PCACorrelationDistance Plus(PCACorrelationDistance distance)
        {
            return new PCACorrelationDistance(this.correlationValue + distance.CorrelationValue,
                this.euclideanValue + distance.EuclideanValue);
        }


        public PCACorrelationDistance Minus(PCACorrelationDistance distance)
        {
            return new PCACorrelationDistance(this.correlationValue - distance.CorrelationValue,
                this.euclideanValue - distance.EuclideanValue);
        }


        public override bool IsInfinity
        {
            get { return correlationValue == int.MaxValue || euclideanValue == Double.PositiveInfinity; }
        }


        public override bool IsEmpty
        {
            get { return correlationValue == 0 || euclideanValue == 0.0; }
        }


        public override bool IsUndefined
        {
            get { return correlationValue == -1 && Double.IsNaN(euclideanValue); }
        }

       


        public override int CompareTo(IDistanceValue obj)
        {
            throw new NotImplementedException();
        }

        public override double ToDouble()
        {
            throw new NotImplementedException();
        }
    }
}
