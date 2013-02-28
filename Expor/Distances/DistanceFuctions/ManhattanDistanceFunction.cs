using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Distances.DistanceFuctions
{

    /**
     * Manhattan distance function to compute the Manhattan distance for a pair of
     * FeatureVectors.
     * 
     * @author Arthur Zimek
     */
    // TODO: add spatial!
    public class ManhattanDistanceFunction : LPNormDistanceFunction
    {
        /**
         * The static instance to use.
         */
        public static readonly ManhattanDistanceFunction STATIC = new ManhattanDistanceFunction();

        /**
         * Provides a Manhattan distance function that can compute the Manhattan
         * distance (that is a DoubleDistance) for FeatureVectors.
         * 
         * @deprecated Use static instance!
         */
      //  [Obsolete]
        protected  ManhattanDistanceFunction()
            : base(1.0)
        {
        }

        /**
         * Compute the Manhattan distance
         * 
         * @param v1 first vector
         * @param v2 second vector
         * @return Manhattan distance value
         */

        public override double DoubleDistance(INumberVector v1, INumberVector v2)
        {
            int dim = v1.Count;
            if (dim != v2.Count)
            {
                throw new ArgumentException("Different dimensionality of FeatureVectors" + "\n  first argument: " + v1.ToString() + "\n  second argument: " + v2.ToString());
            }
            double sum = 0;
            for (int i = 0; i < dim; i++)
            {
                sum += Math.Abs(v1[i] - v2[i]);
            }
            return sum;
        }

        /**
         * Returns the Manhattan norm of the given vector.
         * 
         * @param v the vector to compute the norm of
         * @return the Manhattan norm of the given vector
         */

        public override double DoubleNorm(INumberVector v)
        {
            int dim = v.Count;
            double sum = 0;
            for (int i = 1; i <= dim; i++)
            {
                sum += Math.Abs(v[i]);
            }
            return sum;
        }

        private double DoubleMinDistObject(ISpatialComparable mbr, INumberVector v)
        {
            int dim = mbr.Count;
            if (dim != v.Count)
            {
                throw new ArgumentException("Different dimensionality of objects\n  " + "first argument: " + mbr.ToString() + "\n  " + "second argument: " + v.ToString() + "\n" + dim + "!=" + v.Count);
            }

            double sumDist = 0;
            for (int d = 1; d <= dim; d++)
            {
                double value = v[d];
                double r;
                if (value < mbr.GetMin(d))
                {
                    r = mbr.GetMin(d);
                }
                else if (value > mbr.GetMax(d))
                {
                    r = mbr.GetMax(d);
                }
                else
                {
                    r = value;
                }

                double manhattanI = Math.Abs(value - r);
                sumDist += manhattanI;
            }
            return sumDist;
        }

        public override double MinDoubleDistance(ISpatialComparable mbr1, ISpatialComparable mbr2)
        {
            // Some optimizations for simpler cases.
            if (mbr1 is INumberVector)
            {
                if (mbr2 is INumberVector)
                {
                    return DoubleDistance((INumberVector)mbr1, (INumberVector)mbr2);
                }
                else
                {
                    return DoubleMinDistObject(mbr2, (INumberVector)mbr1);
                }
            }
            else if (mbr2 is INumberVector)
            {
                return DoubleMinDistObject(mbr1, (INumberVector)mbr2);
            }
            int dim1 = mbr1.Count;
            if (dim1 != mbr2.Count)
            {
                throw new ArgumentException("Different dimensionality of objects\n  " +
                    "first argument: " + mbr1.ToString() + "\n  " + "second argument: " + mbr2.ToString());
            }

            double sumDist = 0;
            for (int d = 1; d <= dim1; d++)
            {
                double m1, m2;
                if (mbr1.GetMax(d) < mbr2.GetMin(d))
                {
                    m1 = mbr2.GetMin(d);
                    m2 = mbr1.GetMax(d);
                }
                else if (mbr1.GetMin(d) > mbr2.GetMax(d))
                {
                    m1 = mbr1.GetMin(d);
                    m2 = mbr2.GetMax(d);
                }
                else
                { // The mbrs intersect!
                    continue;
                }
                double manhattanI = m1 - m2;
                sumDist += manhattanI;
            }
            return sumDist;
        }
        public override bool IsMetric
        {
            get
            {
                return true;
            }
        }


        public override String ToString()
        {
            return "ManhattanDistance";
        }

        public override bool Equals(object obj)
        {

            if (obj == null)
            {
                return false;
            }
            if (obj == this)
            {
                return true;
            }
            if (this.GetType() == obj.GetType())
            {
                return true;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : AbstractParameterizer
        {

            protected override object MakeInstance()
            {
                return ManhattanDistanceFunction.STATIC;
            }

        }
    }
}
