using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Distances.DistanceFuctions
{

    public class MaximumDistanceFunction : LPNormDistanceFunction
    {
        /**
         * Static instance.
         */
        public static readonly MaximumDistanceFunction STATIC = new MaximumDistanceFunction();

        /**
         * Provides a Maximum distance function that can compute the Manhattan
         * distance (that is a DoubleDistance) for FeatureVectors.
         * 
         * @deprecated Use static instance!
         */
        // [Obsolete]
        protected MaximumDistanceFunction()
            : base(Double.PositiveInfinity)
        {
        }


        public override double DoubleDistance(INumberVector v1, INumberVector v2)
        {
            int dim1 = v1.Count;
            if (dim1 != v2.Count)
            {
                throw new ArgumentException("Different dimensionality of FeatureVectors" + "\n  first argument: " + v1.ToString() + "\n  second argument: " + v2.ToString());
            }
            double max = 0;
            for (int i = 1; i <= dim1; i++)
            {
                double d = Math.Abs(v1[i] - v2[i]);
                max = Math.Max(d, max);
            }
            return max;
        }

        public override double DoubleNorm(INumberVector v)
        {
            int dim = v.Count;
            double max = 0;
            for (int i = 1; i <= dim; i++)
            {
                max = Math.Max(v[i], max);
            }
            return max;
        }


        public override double MinDoubleDistance(ISpatialComparable mbr1, ISpatialComparable mbr2)
        {
            int dim1 = mbr1.Count;
            if (dim1 != mbr2.Count)
            {
                throw new ArgumentException("Different dimensionality of objects.");
            }
            double max = 0;
            for (int i = 1; i <= dim1; i++)
            {
                double d;
                if (mbr1.GetMax(i) < mbr2.GetMin(i))
                {
                    d = mbr2.GetMin(i) - mbr1.GetMin(i);
                }
                else if (mbr1.GetMin(i) > mbr2.GetMax(i))
                {
                    d = mbr1.GetMin(i) - mbr2.GetMax(i);
                }
                else
                {
                    // The object overlap in this dimension.
                    continue;
                }
                max = Math.Max(d, max);
            }
            return max;
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
            return "MaximumDistance";
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
            if (this.GetType() == (obj.GetType()))
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
                return MaximumDistanceFunction.STATIC;
            }

        }


      

       
    }
}
