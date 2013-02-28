using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Distances.DistanceFuctions
{

    public class LPNormDistanceFunction : AbstractVectorDoubleDistanceNorm, ISpatialPrimitiveDoubleDistanceFunction
    {
        /**
         * OptionDescription for the "p" parameter
         */
        public static OptionDescription P_ID = OptionDescription.GetOrCreate("lpnorm.p", "the degree of the L-P-Norm (positive number)");

        /**
         * Keeps the currently set p.
         */
        private double p;

        /**
         * Constructor, internal version.
         * 
         * @param p Parameter p
         */
        public LPNormDistanceFunction(double p)
            : base()
        {

            this.p = p;
        }

        /**
         * Returns the distance between the specified FeatureVectors as a LP-Norm for
         * the currently set p.
         * 
         * @param v1 first FeatureVector
         * @param v2 second FeatureVector
         * @return the distance between the specified FeatureVectors as a LP-Norm for
         *         the currently set p
         */

        public override double DoubleDistance(INumberVector o1, INumberVector o2)
        {
            var v1 = o1 as INumberVector;
            var v2 = o2 as INumberVector;
            int dim1 = v1.Count;
            if (dim1 != v2.Count)
            {
                throw new ArgumentException("Different dimensionality of FeatureVectors\n  first argument: " + v1.ToString() + "\n  second argument: " + v2.ToString());
            }

            double sqrDist = 0;
            for (int i = 0; i < dim1; i++)
            {
                double delta = System.Math.Abs(v1[i] - v2[i]);
                sqrDist += System.Math.Pow(delta, p);
            }
            return System.Math.Pow(sqrDist, 1.0 / p);
        }


        public override double DoubleNorm(INumberVector v)
        {
            int dim = v.Count;
            double sqrDist = 0;
            for (int i = 1; i <= dim; i++)
            {
                double delta = v[i];
                sqrDist += System.Math.Pow(delta, p);
            }
            return System.Math.Pow(sqrDist, 1.0 / p);
        }

        /**
         * Get the functions p parameter.
         * 
         * @return p
         */
        public double GetP()
        {
            return p;
        }


        public virtual double MinDoubleDistance(ISpatialComparable mbr1, ISpatialComparable mbr2)
        {
            // Optimization for the simplest case
            if (mbr1 is INumberVector)
            {
                if (mbr2 is INumberVector)
                {
                    return DoubleDistance((INumberVector)mbr1, (INumberVector)mbr2);
                }
            }
            // TODO: optimize for more simpler cases: obj vs. rect?
            int dim1 = mbr1.Count;
            if (dim1 != mbr2.Count)
            {
                throw new ArgumentException("Different dimensionality of objects\n  " + "first argument: " + mbr1.ToString() + "\n  " + "second argument: " + mbr2.ToString());
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
                sumDist += System.Math.Pow(manhattanI, p);
            }
            return System.Math.Pow(sumDist, 1.0 / p);
        }


        public virtual IDistanceValue MinDistance(ISpatialComparable mbr1, ISpatialComparable mbr2)
        {
            return new DoubleDistanceValue(MinDoubleDistance(mbr1, mbr2));
        }


        public override bool IsMetric
        {
            get
            {

                return (p >= 1);
            }

        }
        public override String ToString()
        {
            return "L_" + p + " Norm";
        }


        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is LPNormDistanceFunction)
            {
                return this.p == ((LPNormDistanceFunction)obj).p;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override IDistanceQuery Instantiate(IRelation relation)
        {
            return new SpatialPrimitiveDistanceQuery(relation, (ISpatialPrimitiveDistanceFunction)this);
        }
        IDistanceValue IPrimitiveDistanceFunction<ISpatialComparable>.Distance(ISpatialComparable mbr1, ISpatialComparable mbr2)
        {
            return this.Distance((INumberVector)mbr1, (INumberVector)mbr2);
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public class Parameterizer : AbstractParameterizer
        {
            /**
             * The value of p.
             */
            protected double p = 0.0;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                DoubleParameter paramP = new DoubleParameter(P_ID, new GreaterConstraint<double>(0));
                if (config.Grab(paramP))
                {
                    p = paramP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                if (p == 1.0)
                {
                    return ManhattanDistanceFunction.STATIC;
                }
                if (p == 2.0)
                {
                    return EuclideanDistanceFunction.STATIC;
                    // return new EuclideanDistanceFunction();
                }
                if (p == Double.PositiveInfinity)
                {
                    return MaximumDistanceFunction.STATIC;
                }
                return new LPNormDistanceFunction(p);
            }



        }

        public override bool IsSymmetric
        {
            get { throw new NotImplementedException(); }
        }

        public override IDistanceValue DistanceFactory
        {
            get { return DoubleDistanceValue.STATIC; }
        }


    }
}
