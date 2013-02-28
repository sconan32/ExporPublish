using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Extenstions;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Distances.DistanceFuctions.Subspace
{

    public abstract class AbstractDimensionsSelectingDoubleDistanceFunction<V> :
        AbstractPrimitiveDistanceFunction<V>, IPrimitiveDoubleDistanceFunction<V>,
        IDimensionSelectingSubspaceDistanceFunction
    {
        /**
         * Dimensions parameter.
         */
        public static OptionDescription DIMS_ID = OptionDescription.GetOrCreate("distance.dims",
            "a comma separated array of integer values, where 1 <= d_i <= the dimensionality of the feature space specifying the dimensions to be considered for distance computation. If this parameter is not set, no dimensions will be considered, i.e. the distance between two objects is always 0.");

        /**
         * The dimensions to be considered for distance computation.
         */
        protected BitArray dimensions;

        /**
         * Constructor.
         * 
         * @param dimensions
         */
        public AbstractDimensionsSelectingDoubleDistanceFunction(BitArray dimensions) :
            base()
        {
            this.dimensions = dimensions;
        }


        public override IDistanceValue Distance(V o1, V o2)
        {
            return new DoubleDistanceValue(DoubleDistance(o1, o2));
        }

        public abstract double DoubleDistance(V o1, V o2);
        public BitArray SelectedDimensions
        {
            get { return dimensions; }
            set
            {
                if (this.dimensions == null)
                {
                    this.dimensions = value;
                    return;
                }
                this.dimensions = this.dimensions.Xor(this.dimensions);
                this.dimensions.Or(value);
            }
        }


        public override IDistanceValue DistanceFactory
        {
            get { return DoubleDistanceValue.STATIC; }
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
            return this.dimensions.Equals(
                ((AbstractDimensionsSelectingDoubleDistanceFunction<V>)obj).dimensions);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ dimensions.GetHashCode();
        }
        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public abstract class Parameterizer : AbstractParameterizer
        {
            protected BitArray dimensions = null;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                dimensions = new BitArray(1000);
                IntListParameter dimsP = new IntListParameter(DIMS_ID, new ListGreaterEqualConstraint<Int32>(1), true);
                if (config.Grab(dimsP))
                {
                    foreach (int d in dimsP.GetValue())
                    {
                        dimensions.Set(d - 1, true);
                    }
                }
            }
        }



    }
}
