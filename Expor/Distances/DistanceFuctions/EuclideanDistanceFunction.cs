using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Distances.DistanceFuctions
{

    public class EuclideanDistanceFunction :        LPNormDistanceFunction
    {
        /**
         * Static instance. Use this!
         */
        public static readonly EuclideanDistanceFunction STATIC = new EuclideanDistanceFunction();

        /**
         * Provides a Euclidean distance function that can compute the Euclidean
         * distance (that is a DoubleDistance) for FeatureVectors.
         * 
         * @deprecated Use static instance!
         */

        public EuclideanDistanceFunction() :
            base(2.0)
        {
        }

        /**
         * Provides the Euclidean distance between the given two vectors.
         * 
         * @return the Euclidean distance between the given two vectors as raw double
         *         value
         */

        public override double DoubleDistance(INumberVector o1, INumberVector o2)
        {
            var v1 = o1 as INumberVector;
            var v2 = o2 as INumberVector;
            int dim1 = v1.Count;
            if (dim1 != v2.Count)
            {
                throw new ArgumentException("Different dimensionality of FeatureVectors" +
                    "\n  first argument: " + v1.ToString() + "\n  second argument: " + v2.ToString() + "\n" + v1.Count + "!=" + v2.Count);
            }
            double sqrDist = 0;
            for (int i = 0; i < dim1; i++)
            {
                double delta = v1[i] - v2[i];
                sqrDist += delta * delta;
            }
            return Math.Sqrt(sqrDist);
        }


        public override double DoubleNorm(INumberVector v)
        {
            int dim = v.Count;
            double sqrDist = 0;
            for (int i = 1; i <= dim; i++)
            {
                double delta = v[i];
                sqrDist += delta * delta;
            }
            return Math.Sqrt(sqrDist);
        }

        protected double DoubleMinDistObject(ISpatialComparable mbr, INumberVector v)
        {
            int dim = mbr.Count;
            if (dim != v.Count)
            {
                throw new ArgumentException("Different dimensionality of objects\n  " +
                    "first argument: " + mbr.ToString() + "\n  " + "second argument: " +
                    v.ToString() + "\n" + dim + "!=" + v.Count);
            }

            double sqrDist = 0;
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

                double manhattanI = value - r;
                sqrDist += manhattanI * manhattanI;
            }
            return Math.Sqrt(sqrDist);
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

            double sqrDist = 0;
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
                sqrDist += manhattanI * manhattanI;
            }
            return Math.Sqrt(sqrDist);
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
            return "EuclideanDistance";
        }


        public override bool Equals(Object obj)
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
                //return EuclideanDistanceFunction.STATIC;
                return new EuclideanDistanceFunction();
            }


        }
    }
}
