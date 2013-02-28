using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Distances.DistanceFuctions.Subspace
{

    public class DimensionSelectingDistanceFunction :
        AbstractPrimitiveDistanceFunction<INumberVector>, ISpatialPrimitiveDoubleDistanceFunction
    {
        /**
         * Parameter for dimensionality.
         */
        public static OptionDescription DIM_ID = OptionDescription.GetOrCreate(
            "dim", "an integer between 1 and the dimensionality of the " +
            "feature space 1 specifying the dimension to be considered " + "for distance computation.");

        /**
         * The dimension to be considered for distance computation.
         */
        private int dim;

        /**
         * Constructor.
         * 
         * @param dim Dimension
         */
        public DimensionSelectingDistanceFunction(int dim) :
            base()
        {
            this.dim = dim;
        }

        /**
         * Computes the distance between two given DatabaseObjects according to this
         * distance function.
         * 
         * @param v1 first DatabaseObject
         * @param v2 second DatabaseObject
         * @return the distance between two given DatabaseObjects according to this
         *         distance function
         */
        public IDistanceValue Distance(ISpatialComparable o1, ISpatialComparable o2)
        {
            return CalculateDistance((INumberVector)o1, (INumberVector)o2);
        }
        public override IDistanceValue Distance(INumberVector o1, INumberVector o2)
        {
            return this.CalculateDistance(o1, o2);
        }
        public double DoubleDistance(ISpatialComparable spc1, ISpatialComparable spc2)
        {
            return CalcDoubleDistance(spc1 as INumberVector, spc2 as INumberVector);
        }
        public double CalcDoubleDistance(INumberVector v1, INumberVector v2)
        {
            if (dim > v1.Count || dim > v2.Count)
            {
                throw new ArgumentException("Specified dimension to be considered " +
                    "is larger that dimensionality of FeatureVectors:" + "\n  first argument: " + v1.ToString() + "\n  second argument: " + v2.ToString() + "\n  dimension: " + dim);
            }

            double manhattan = v1[dim] - v2[dim];
            return Math.Abs(manhattan);
        }


        public double MinDoubleDistance(ISpatialComparable mbr1, ISpatialComparable mbr2)
        {
            if (dim > mbr1.Count || dim > mbr2.Count)
            {
                throw new ArgumentException("Specified dimension to be considered " + "is larger that dimensionality of FeatureVectors:" + "\n  first argument: " + mbr1.ToString() + "\n  second argument: " + mbr2.ToString() + "\n  dimension: " + dim);
            }

            double m1, m2;
            if (mbr1.GetMax(dim) < mbr2.GetMin(dim))
            {
                m1 = mbr1.GetMax(dim);
                m2 = mbr2.GetMin(dim);
            }
            else if (mbr1.GetMin(dim) > mbr2.GetMax(dim))
            {
                m1 = mbr1.GetMin(dim);
                m2 = mbr2.GetMax(dim);
            }
            else
            { // The mbrs intersect!
                m1 = 0;
                m2 = 0;
            }
            double manhattan = m1 - m2;

            return Math.Abs(manhattan);
        }


        public DoubleDistanceValue CalculateDistance(INumberVector o1, INumberVector o2)
        {
            return new DoubleDistanceValue(CalcDoubleDistance(o1, o2));
        }


        public IDistanceValue MinDistance(ISpatialComparable mbr1, ISpatialComparable mbr2)
        {
            return new DoubleDistanceValue(MinDoubleDistance(mbr1, mbr2));
        }




        public override ITypeInformation GetInputTypeRestriction()
        {
            return VectorTypeInformation.Get(typeof(INumberVector), dim, int.MinValue);
        }


        public override IDistanceValue DistanceFactory
        {
            get { return DoubleDistanceValue.STATIC; }
        }


        public override IDistanceQuery Instantiate(IRelation database)
        {
            return new SpatialPrimitiveDistanceQuery(database, this);
        }


        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            return this.dim == ((DimensionSelectingDistanceFunction)obj).dim;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.dim.GetHashCode();
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
            protected int dim = 0;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                IntParameter dimP = new IntParameter(DIM_ID, new GreaterEqualConstraint<int>(1));
                if (config.Grab(dimP))
                {
                    dim = dimP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new DimensionSelectingDistanceFunction(dim);
            }
        }
    }
}
