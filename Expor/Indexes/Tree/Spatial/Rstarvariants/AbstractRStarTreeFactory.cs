using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Types;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Bulk;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Insert;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Overflow;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Split;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants
{

    /**
     * Abstract factory for R*-Tree based trees.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.stereotype factory
     * @apiviz.uses AbstractRStarTree oneway - - 芦create禄
     * 
     * @param <O> Object type
     * @param <N> Node type
     * @param <E> Entry type
     * @param <I> Index type
     */
    public abstract class AbstractRStarTreeFactory : TreeIndexFactory
    {
        /**
         * Fast-insertion parameter. Optional.
         */
        public static OptionDescription INSERTION_STRATEGY_ID =
            OptionDescription.GetOrCreate("rtree.insertionstrategy", "The strategy to use for object insertion.");

        /**
         * Split strategy parameter. Optional.
         */
        public static OptionDescription SPLIT_STRATEGY_ID =
            OptionDescription.GetOrCreate("rtree.splitstrategy", "The strategy to use for node splitting.");

        /**
         * Parameter for bulk strategy
         */
        public static OptionDescription BULK_SPLIT_ID =
            OptionDescription.GetOrCreate("spatial.bulkstrategy", "The class to perform the bulk split with.");

        /**
         * Parameter for the relative minimum fill.
         */
        public static OptionDescription MINIMUM_FILL_ID =
            OptionDescription.GetOrCreate("rtree.minimum-fill", "Minimum relative fill required for data pages.");

        /**
         * Overflow treatment.
         */
        public static OptionDescription OVERFLOW_STRATEGY_ID =
            OptionDescription.GetOrCreate("rtree.overflowtreatment", "The strategy to use for handling overflows.");

        /**
         * Strategy to find the insertion node with.
         */
        protected IInsertionStrategy insertionStrategy;

        /**
         * The strategy for bulk load.
         */
        protected IBulkSplit bulkSplitter;

        /**
         * The strategy for splitting nodes
         */
        protected ISplitStrategy nodeSplitter;

        /**
         * Overflow treatment strategy
         */
        protected IOverflowTreatment overflowTreatment;

        /**
         * Relative minimum fill
         */
        protected double minimumFill;

        /**
         * Constructor.
         * 
         * @param fileName
         * @param pageSize
         * @param cacheSize
         * @param bulkSplitter the strategy to use for bulk splitting
         * @param insertionStrategy the strategy to find the insertion child
         * @param nodeSplitter the strategy to use for splitting nodes
         * @param overflowTreatment the strategy to use for overflow treatment
         * @param minimumFill the relative minimum fill
         */
        public AbstractRStarTreeFactory(String fileName, int pageSize, long cacheSize,
            IBulkSplit bulkSplitter, IInsertionStrategy insertionStrategy,
            ISplitStrategy nodeSplitter, IOverflowTreatment overflowTreatment, double minimumFill) :
            base(fileName, pageSize, cacheSize)
        {
            this.insertionStrategy = insertionStrategy;
            this.bulkSplitter = bulkSplitter;
            this.nodeSplitter = nodeSplitter;
            this.overflowTreatment = overflowTreatment;
            this.minimumFill = minimumFill;
        }


        public override ITypeInformation GetInputTypeRestriction()
        {
            return TypeUtil.NUMBER_VECTOR_FIELD;
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new abstract class Parameterizer : TreeIndexFactory.Parameterizer
        {
            /**
             * Insertion strategy
             */
            protected IInsertionStrategy insertionStrategy = null;

            /**
             * The strategy for splitting nodes
             */
            protected ISplitStrategy nodeSplitter = null;

            /**
             * Bulk loading strategy
             */
            protected IBulkSplit bulkSplitter = null;

            /**
             * Overflow treatment strategy
             */
            protected IOverflowTreatment overflowTreatment = null;

            /**
             * Relative minimum fill
             */
            protected double minimumFill;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                ObjectParameter<IInsertionStrategy> insertionStrategyP =
                    new ObjectParameter<IInsertionStrategy>(INSERTION_STRATEGY_ID,
                        typeof(IInsertionStrategy), typeof(CombinedInsertionStrategy));
                if (config.Grab(insertionStrategyP))
                {
                    insertionStrategy = insertionStrategyP.InstantiateClass(config);
                }
                ObjectParameter<ISplitStrategy> splitStrategyP =
                    new ObjectParameter<ISplitStrategy>(SPLIT_STRATEGY_ID,
                        typeof(ISplitStrategy), typeof(TopologicalSplitter));
                if (config.Grab(splitStrategyP))
                {
                    nodeSplitter = splitStrategyP.InstantiateClass(config);
                }
                DoubleParameter minimumFillP = new DoubleParameter(MINIMUM_FILL_ID,
                    new IntervalConstraint<double>(0.0, IntervalConstraint<double>.IntervalBoundary.OPEN,
                        0.5, IntervalConstraint<double>.IntervalBoundary.OPEN), 0.4);
                if (config.Grab(minimumFillP))
                {
                    minimumFill = minimumFillP.GetValue();
                }
                ObjectParameter<IOverflowTreatment> overflowP =
                    new ObjectParameter<IOverflowTreatment>(OVERFLOW_STRATEGY_ID, typeof(IOverflowTreatment),
                        typeof(LimitedReinsertOverflowTreatment));
                if (config.Grab(overflowP))
                {
                    overflowTreatment = overflowP.InstantiateClass(config);
                }
                ConfigBulkLoad(config);
            }

            /**
             * Configure the bulk load parameters.
             * 
             * @param config Parameterization
             */
            protected void ConfigBulkLoad(IParameterization config)
            {
                ObjectParameter<IBulkSplit> bulkSplitP = new ObjectParameter<IBulkSplit>(BULK_SPLIT_ID,
                    typeof(IBulkSplit), true);
                if (config.Grab(bulkSplitP))
                {
                    bulkSplitter = bulkSplitP.InstantiateClass<IBulkSplit>(config);
                }
            }


            protected override abstract object MakeInstance();
        }
    }
}
