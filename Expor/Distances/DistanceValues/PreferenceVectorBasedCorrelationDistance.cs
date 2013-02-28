using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Socona.Expor.Distances.DistanceValues
{

    public class PreferenceVectorBasedCorrelationDistance :
        CorrelationDistance
    {
        /**
         * The static factory instance
         */
        public static PreferenceVectorBasedCorrelationDistance FACTORY = new PreferenceVectorBasedCorrelationDistance();

        /**
         * Serial version
         */
        //private static long serialVersionUID = 1;

        /**
         * The dimensionality of the feature space (needed for serialization).
         */
        private int dimensionality;

        /**
         * The common preference vector of the two objects defining this distance.
         */
        private BitArray commonPreferenceVector;

        /**
         * Empty constructor for serialization purposes.
         */
        public PreferenceVectorBasedCorrelationDistance() :
            base()
        {
        }

        /**
         * Constructs a new CorrelationDistance object.
         * 
         * @param dimensionality the dimensionality of the feature space (needed for
         *        serialization)
         * @param correlationValue the correlation dimension to be represented by the
         *        CorrelationDistance
         * @param euclideanValue the euclidean distance to be represented by the
         *        CorrelationDistance
         * @param commonPreferenceVector the common preference vector of the two
         *        objects defining this distance
         */
        public PreferenceVectorBasedCorrelationDistance(int dimensionality, int correlationValue,
            double euclideanValue, BitArray commonPreferenceVector) :
            base(correlationValue, euclideanValue)
        {
            this.dimensionality = dimensionality;
            this.commonPreferenceVector = commonPreferenceVector;
        }

        /**
         * Returns the common preference vector of the two objects defining this
         * distance.
         * 
         * @return the common preference vector
         */
        public BitArray CommonPreferenceVector
        {
            get { return commonPreferenceVector; }
        }

        /**
         * Returns a string representation of this
         * PreferenceVectorBasedCorrelationDistance.
         * 
         * @return the correlation value, the euclidean value and the common
         *         preference vector separated by blanks
         */

        public override String ToString()
        {
            return base.ToString() + SEPARATOR + commonPreferenceVector.ToString();
        }

        /**
         * @throws ArgumentException if the dimensionality values and common
         *         preference vectors of this distance and the specified distance are
         *         not equal
         */

        public PreferenceVectorBasedCorrelationDistance Plus(PreferenceVectorBasedCorrelationDistance distance)
        {
            if (this.dimensionality != distance.dimensionality)
            {
                throw new ArgumentException("The dimensionality values of this distance " +
                    "and the specified distance need to be equal.\n" + "this.dimensionality     " +
                    this.dimensionality + "\n" + "distance.dimensionality " + distance.dimensionality + "\n");
            }

            if (!this.commonPreferenceVector.Equals(distance.commonPreferenceVector))
            {
                throw new ArgumentException("The common preference vectors of this distance " +
                    "and the specified distance need to be equal.\n" + "this.commonPreferenceVector     " +
                    this.commonPreferenceVector + "\n" + "distance.commonPreferenceVector " +
                    distance.commonPreferenceVector + "\n");
            }

            return new PreferenceVectorBasedCorrelationDistance(dimensionality,
                CorrelationValue + distance.CorrelationValue,
                EuclideanValue + distance.EuclideanValue,
                (BitArray)commonPreferenceVector.Clone());
        }

        /**
         * @throws ArgumentException if the dimensionality values and common
         *         preference vectors of this distance and the specified distance are
         *         not equal
         */

        public PreferenceVectorBasedCorrelationDistance Minus(PreferenceVectorBasedCorrelationDistance distance)
        {
            if (this.dimensionality != distance.dimensionality)
            {
                throw new ArgumentException("The dimensionality values of this distance " +
                    "and the specified distance need to be equal.\n" + "this.dimensionality     " +
                    this.dimensionality + "\n" + "distance.dimensionality " + distance.dimensionality + "\n");
            }

            if (!this.commonPreferenceVector.Equals(distance.commonPreferenceVector))
            {
                throw new ArgumentException("The common preference vectors of this distance " +
                    "and the specified distance need to be equal.\n" + "this.commonPreferenceVector     " +
                    this.commonPreferenceVector + "\n" + "distance.commonPreferenceVector " +
                    distance.commonPreferenceVector + "\n");
            }

            return new PreferenceVectorBasedCorrelationDistance(dimensionality,
                CorrelationValue - distance.CorrelationValue,
                EuclideanValue - distance.EuclideanValue,
                (BitArray)commonPreferenceVector.Clone());
        }

        /**
         * Checks if the dimensionality values of this distance and the specified
         * distance are equal. If the check fails an ArgumentException is
         * thrown, otherwise
         * {@link CorrelationDistance#CompareTo(CorrelationDistance)
         * CorrelationDistance#CompareTo(distance)} is returned.
         * 
         * @return the value of
         *         {@link CorrelationDistance#CompareTo(CorrelationDistance)
         *         CorrelationDistance#CompareTo(distance)}
         * @throws ArgumentException if the dimensionality values of this
         *         distance and the specified distance are not equal
         */

        public int CompareTo(PreferenceVectorBasedCorrelationDistance distance)
        {
            if (this.dimensionality >= 0 && distance.dimensionality >= 0 &&
                this.dimensionality != distance.dimensionality)
            {
                throw new ArgumentException("The dimensionality values of this distance " +
                    "and the specified distance need to be equal.\n" + "this.dimensionality     " +
                    this.dimensionality + "\n" + "distance.dimensionality " + distance.dimensionality + "\n");
            }

            return base.CompareTo(distance);
        }

       
        public override Regex GetPattern()
        {
            return CORRELATION_DISTANCE_PATTERN;
        }


        public PreferenceVectorBasedCorrelationDistance parseString(String pattern)
        {
            if (pattern.Equals(INFINITY_PATTERN))
            {
                return (PreferenceVectorBasedCorrelationDistance)Infinity;
            }
            if (TestInputPattern(pattern))
            {
                String[] values = SEPARATOR.Split(pattern.ToCharArray());
                return new PreferenceVectorBasedCorrelationDistance(
                    -1, Int32.Parse(values[0]), Double.Parse(values[1]), new BitArray(1000));
            }
            else
            {
                throw new ArgumentException("Given pattern \"" + pattern + "\" does not match required pattern \"" +
                    RequiredInputPattern + "\"");
            }
        }


        public override IDistanceValue Infinity
        {
            get
            {
                return new PreferenceVectorBasedCorrelationDistance(
                -1, Int32.MaxValue, Double.PositiveInfinity, new BitArray(1000));
            }
        }


        public override IDistanceValue Empty
        {
            get { return new PreferenceVectorBasedCorrelationDistance(-1, 0, 0, new BitArray(1000)); }
        }


        public override IDistanceValue Undefined
        {
            get { return new PreferenceVectorBasedCorrelationDistance(-1, -1, Double.NaN, new BitArray(1000)); }
        }

       
        public override IDistanceValue ParseString(string pattern)
        {
            return parseString(pattern);
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
