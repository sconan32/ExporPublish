using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Utilities.Extenstions;
namespace Socona.Expor.Distances.DistanceFuctions.Subspace
{

    public class SubspaceManhattanDistanceFunction : SubspaceLPNormDistanceFunction
    {
        /**
         * Constructor.
         * 
         * @param dimensions Selected dimensions
         */
        public SubspaceManhattanDistanceFunction(BitArray dimensions) :
            base(1.0, dimensions)
        {
        }

        /**
         * Provides the Euclidean distance between two given feature vectors in the
         * selected dimensions.
         * 
         * @param v1 first feature vector
         * @param v2 second feature vector
         * @return the Euclidean distance between two given feature vectors in the
         *         selected dimensions
         */

        public double doubleDistance(INumberVector v1, INumberVector v2)
        {
            if (v1.Count != v2.Count)
            {
                throw new ArgumentException("Different dimensionality of FeatureVectors\n  " + "first argument: " + v1 + "\n  " + "second argument: " + v2);
            }

            double sum = 0;
            for (int d = dimensions.NextSetBitIndex(0); d >= 0; d = dimensions.NextSetBitIndex(d + 1))
            {
                sum += Math.Abs(v1[(d + 1)] - v2[(d + 1)]);
            }
            return sum;
        }

        protected override double doubleMinDistObject(ISpatialComparable mbr, INumberVector v)
        {
            if (mbr.Count != v.Count)
            {
                throw new ArgumentException("Different dimensionality of objects\n  " + 
                    "first argument: " + mbr.ToString() + "\n  " + "second argument: " + v.ToString());
            }

            double sum = 0;
            for (int d = dimensions.NextSetBitIndex(0); d >= 0; d = dimensions.NextSetBitIndex(d + 1))
            {
                double value = v[(d + 1)];
                double omin = mbr.GetMin(d + 1);
                if (value < omin)
                {
                    sum += omin - value;
                }
                else
                {
                    double omax = mbr.GetMax(d + 1);
                    if (value > omax)
                    {
                        sum += value - omax;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return sum;
        }


        public override double MinDoubleDistance(ISpatialComparable mbr1, ISpatialComparable mbr2)
        {
            if (mbr1.Count != mbr2.Count)
            {
                throw new ArgumentException("Different dimensionality of objects\n  " + "first argument: "
                    + mbr1.ToString() + "\n  " + "second argument: " + mbr2.ToString());
            }
            double sum = 0;
            for (int d = dimensions.NextSetBitIndex(0); d >= 0; d = dimensions.NextSetBitIndex(d + 1))
            {
                double max1 = mbr1.GetMax(d + 1);
                double min2 = mbr2.GetMin(d + 1);
                if (max1 < min2)
                {
                    sum += min2 - max1;
                }
                else
                {
                    double min1 = mbr1.GetMin(d + 1);
                    double max2 = mbr2.GetMax(d + 1);
                    if (min1 > max2)
                    {
                        sum += min1 - max2;
                    }
                    else
                    { // The mbrs intersect!
                        continue;
                    }
                }
            }
            return sum;
        }


        public override double DoubleNorm(INumberVector obj)
        {
            double sum = 0;
            for (int d = dimensions.NextSetBitIndex(0); d >= 0; d = dimensions.NextSetBitIndex(d + 1))
            {
                sum += Math.Abs(obj[(d + 1)]);
            }
            return sum;
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new  class Parameterizer :
            AbstractDimensionsSelectingDoubleDistanceFunction<INumberVector>.Parameterizer
        {

            protected override object MakeInstance()
            {
                return new SubspaceManhattanDistanceFunction(dimensions);
            }
        }
    }
}
