using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.Extenstions;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
namespace Socona.Expor.Distances.DistanceFuctions.Subspace
{

    public class SubspaceLPNormDistanceFunction :
        AbstractDimensionsSelectingDoubleDistanceFunction<INumberVector>,
        ISpatialPrimitiveDoubleDistanceFunction
    {
        /**
         * Value of p
         */
        private double p;

        /**
         * Constructor.
         * 
         * @param dimensions Selected dimensions
         * @param p p value
         */
        public SubspaceLPNormDistanceFunction(double p, BitArray dimensions) :
            base(dimensions)
        {
            this.p = p;
        }

        /**
         * Get the value of p.
         * 
         * @return p
         */
        public double GetP()
        {
            return p;
        }

        public IDistanceValue Distance(ISpatialComparable spc1, ISpatialComparable spc2)
        {
            return this.Distance(spc1 as INumberVector, spc2 as INumberVector);
        }
        public double DoubleDistance(ISpatialComparable spc1, ISpatialComparable spc2)
        {
            if (spc1.Count != spc2.Count)
            {
                throw new ArgumentException("Different dimensionality of FeatureVectors\n  " +
                    "first argumentin " + spc1 + "\n  " + "second argumentin " + spc2);
            }

            double sqrDist = 0;
            for (int d = dimensions.NextSetBitIndex(0); d >= 0; d = dimensions.NextSetBitIndex(d + 1))
            {
                double delta = Math.Abs(spc1.GetMax(d) - spc2.GetMax(d));
                sqrDist += Math.Pow(delta, p);
            }
            return Math.Pow(sqrDist, 1.0 / p);
            
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
            return this.DoubleDistance((ISpatialComparable)v1, (ISpatialComparable)v2);
        }

        protected virtual double doubleMinDistObject(ISpatialComparable mbr, INumberVector v)
        {
            if (mbr.Count != v.Count)
            {
                throw new ArgumentException("Different dimensionality of objects\n  " + "first argumentin " + mbr.ToString() + "\n  " + "second argumentin " + v.ToString());
            }

            double sqrDist = 0;
            for (int d = dimensions.NextSetBitIndex(0); d >= 0; d = dimensions.NextSetBitIndex(d + 1))
            {
                double delta;
                double value = v[(d)];
                double omin = mbr.GetMin(d);
                if (value < omin)
                {
                    delta = omin - value;
                }
                else
                {
                    double omax = mbr.GetMax(d);
                    if (value > omax)
                    {
                        delta = value - omax;
                    }
                    else
                    {
                        continue;
                    }
                }
                sqrDist += Math.Pow(delta, p);
            }
            return Math.Pow(sqrDist, 1.0 / p);
        }


        public virtual double MinDoubleDistance(ISpatialComparable mbr1, ISpatialComparable mbr2)
        {
            if (mbr1.Count != mbr2.Count)
            {
                throw new ArgumentException("Different dimensionality of objects\n  " + "first argumentin " +
                    mbr1.ToString() + "\n  " + "second argumentin " + mbr2.ToString());
            }
            double sqrDist = 0;
            for (int d = dimensions.NextSetBitIndex(0); d >= 0; d = dimensions.NextSetBitIndex(d + 1))
            {
                double delta;
                double max1 = mbr1.GetMax(d);
                double min2 = mbr2.GetMin(d);
                if (max1 < min2)
                {
                    delta = min2 - max1;
                }
                else
                {
                    double min1 = mbr1.GetMin(d);
                    double max2 = mbr2.GetMax(d);
                    if (min1 > max2)
                    {
                        delta = min1 - max2;
                    }
                    else
                    { // The mbrs intersect!
                        continue;
                    }
                }
                sqrDist += Math.Pow(delta, p);
            }
            return Math.Pow(sqrDist, 1.0 / p);
        }


        public virtual IDistanceValue MinDistance(ISpatialComparable mbr1, ISpatialComparable mbr2)
        {
            return new DoubleDistanceValue(MinDoubleDistance(mbr1, mbr2));
        }


        public virtual DoubleDistanceValue Norm(INumberVector obj)
        {
            return new DoubleDistanceValue(DoubleNorm(obj));
        }


        public virtual double DoubleNorm(INumberVector obj)
        {
            double sqrDist = 0;
            for (int d = dimensions.NextSetBitIndex(0); d >= 0; d = dimensions.NextSetBitIndex(d + 1))
            {
                double delta = Math.Abs(obj[(d)]);
                sqrDist += Math.Pow(delta, p);
            }
            return Math.Pow(sqrDist, 1.0 / p);
        }


        public override IDistanceQuery Instantiate(IRelation database)
        {
            return new SpatialPrimitiveDistanceQuery(database, this);
        }


        public override ITypeInformation GetInputTypeRestriction()
        {
            return TypeUtil.NUMBER_VECTOR_FIELD;
        }


        public bool isMetric()
        {
            return true;
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : AbstractDimensionsSelectingDoubleDistanceFunction<INumberVector>.Parameterizer
        {
            /**
             * Value of p
             */
            private double p;


            protected override void MakeOptions(IParameterization config)
            {
                DoubleParameter paramP = new DoubleParameter(LPNormDistanceFunction.P_ID, new GreaterConstraint<double>(0));
                if (config.Grab(paramP))
                {
                    p = paramP.GetValue();
                }
                base.MakeOptions(config);
            }


            protected override object MakeInstance()
            {
                if (p == 2.0)
                {
                    return new SubspaceEuclideanDistanceFunction(dimensions);
                }
                if (p == 1.0)
                {
                    return new SubspaceManhattanDistanceFunction(dimensions);
                }
                return new SubspaceLPNormDistanceFunction(p, dimensions);
            }
        }

    }
}
