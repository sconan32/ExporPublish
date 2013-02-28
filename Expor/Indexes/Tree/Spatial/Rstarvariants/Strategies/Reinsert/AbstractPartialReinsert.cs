using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Utilities.DataStructures.ArrayLike;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Reinsert
{

    public abstract class AbstractPartialReinsert : IReinsertStrategy
    {
        /**
         * Amount of entries to reinsert
         */
        protected double reinsertAmount = 0.3;

        /**
         * Distance function to use for measuring
         */
        protected ISpatialPrimitiveDoubleDistanceFunction distanceFunction;

        /**
         * Constructor.
         * 
         * @param reinsertAmount Relative amount of objects to reinsert.
         * @param distanceFunction Distance function to use
         */
        public AbstractPartialReinsert(double reinsertAmount,
            ISpatialPrimitiveDoubleDistanceFunction distanceFunction)
        {

            this.reinsertAmount = reinsertAmount;
            this.distanceFunction = distanceFunction;
        }
        public abstract int[] ComputeReinserts(IEnumerable<ISpatialEntry> entries, IArrayAdapter getter, ISpatialComparable page);

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public abstract class Parameterizer : AbstractParameterizer
        {
            /**
             * Reinsertion share
             */
            public static OptionDescription REINSERT_AMOUNT_ID = OptionDescription.GetOrCreate(
                "rtree.reinsertion-amount", "The amount of entries to reinsert.");

            /**
             * Reinsertion share
             */
            public static OptionDescription REINSERT_DISTANCE_ID = OptionDescription.GetOrCreate(
                "rtree.reinsertion-distancce", "The distance function to compute reinsertion candidates by.");

            /**
             * The actual reinsertion strategy
             */
            protected double reinsertAmount = 0.3;

            /**
             * Distance function to use for measuring
             */
            protected ISpatialPrimitiveDoubleDistanceFunction distanceFunction;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                DoubleParameter reinsertAmountP = new DoubleParameter(REINSERT_AMOUNT_ID,
                    new IntervalConstraint<double>(
                        0.0, IntervalConstraint<double>.IntervalBoundary.OPEN,
                        0.5, IntervalConstraint<double>.IntervalBoundary.OPEN),
                        0.3);
                if (config.Grab(reinsertAmountP))
                {
                    reinsertAmount = reinsertAmountP.GetValue();
                }
                ObjectParameter<ISpatialPrimitiveDoubleDistanceFunction> distanceP =
                    new ObjectParameter<ISpatialPrimitiveDoubleDistanceFunction>
                    (REINSERT_DISTANCE_ID, typeof(ISpatialPrimitiveDoubleDistanceFunction),
                    typeof(SquaredEuclideanDistanceFunction));
                if (config.Grab(distanceP))
                {
                    distanceFunction = distanceP.InstantiateClass(config);
                }
            }
        }
    }

}
