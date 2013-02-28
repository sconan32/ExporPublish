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

    public class SubspaceEuclideanDistanceFunction : SubspaceLPNormDistanceFunction
    {
        /**
         * Constructor.
         * 
         * @param dimensions Selected dimensions
         */
        public SubspaceEuclideanDistanceFunction(BitArray dimensions) :
            base(2.0, dimensions)
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

        public override double DoubleDistance(INumberVector v1, INumberVector v2)
        {
            if (v1.Count != v2.Count)
            {
                throw new ArgumentException("Different dimensionality of FeatureVectors\n  " + "first argumentin " + v1 + "\n  " + "second argumentin " + v2);
            }

            double sqrDist = 0;
            for (int d = dimensions.NextSetBitIndex(0); d >= 0; d = dimensions.NextSetBitIndex(d + 1))
            {
                double delta = v1[(d + 1)] - v2[(d + 1)];
                sqrDist += delta * delta;
            }
            return Math.Sqrt(sqrDist);
        }


        protected override double doubleMinDistObject(ISpatialComparable mbr, INumberVector v)
        {
            if (mbr.Count != v.Count)
            {
                throw new ArgumentException("Different dimensionality of objects\n  " + "first argumentin " + mbr.ToString() + "\n  " + "second argumentin " + v.ToString());
            }

            double sqrDist = 0;
            for (int d = dimensions.NextSetBitIndex(0); d >= 0; d = dimensions.NextSetBitIndex(d + 1))
            {
                double delta;
                double value = v[(d + 1)];
                double omin = mbr.GetMin(d + 1);
                if (value < omin)
                {
                    delta = omin - value;
                }
                else
                {
                    double omax = mbr.GetMax(d + 1);
                    if (value > omax)
                    {
                        delta = value - omax;
                    }
                    else
                    {
                        continue;
                    }
                }
                sqrDist += delta * delta;
            }
            return Math.Sqrt(sqrDist);
        }


        public override double MinDoubleDistance(ISpatialComparable mbr1, ISpatialComparable mbr2)
        {
            if (mbr1.Count != mbr2.Count)
            {
                throw new ArgumentException("Different dimensionality of objects\n  " + "first argumentin " + mbr1.ToString() + "\n  " + "second argumentin " + mbr2.ToString());
            }
            double sqrDist = 0;
            for (int d = dimensions.NextSetBitIndex(0); d >= 0; d = dimensions.NextSetBitIndex(d + 1))
            {
                double delta;
                double max1 = mbr1.GetMax(d + 1);
                double min2 = mbr2.GetMin(d + 1);
                if (max1 < min2)
                {
                    delta = min2 - max1;
                }
                else
                {
                    double min1 = mbr1.GetMin(d + 1);
                    double max2 = mbr2.GetMax(d + 1);
                    if (min1 > max2)
                    {
                        delta = min1 - max2;
                    }
                    else
                    { // The mbrs intersect!
                        continue;
                    }
                }
                sqrDist += delta * delta;
            }
            return Math.Sqrt(sqrDist);
        }


        public override double DoubleNorm(INumberVector obj)
        {
            double sqrDist = 0;
            for (int d = dimensions.NextSetBitIndex(0); d >= 0; d = dimensions.NextSetBitIndex(d + 1))
            {
                double delta = obj[(d + 1)];
                sqrDist += delta * delta;
            }
            return Math.Sqrt(sqrDist);
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : 
            AbstractDimensionsSelectingDoubleDistanceFunction<INumberVector>.Parameterizer
        {

            protected override object MakeInstance()
            {
                return new SubspaceEuclideanDistanceFunction(dimensions);
            }
        }
    }
}
