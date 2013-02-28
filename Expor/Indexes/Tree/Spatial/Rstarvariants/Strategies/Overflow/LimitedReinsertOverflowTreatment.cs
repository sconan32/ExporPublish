using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Reinsert;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Utilities;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Overflow
{

    [Reference(Authors = "N. Beckmann, H.-P. Kriegel, R. Schneider, B. Seeger",
        Title = "The R*-tree: an efficient and robust access method for points and rectangles",
        BookTitle = "Proceedings of the 1990 ACM SIGMOD International Conference on Management of Data, Atlantic City, NJ, May 23-25, 1990",
        Url = "http://dx.doi.org/10.1145/93597.98741")]
    public class LimitedReinsertOverflowTreatment : IOverflowTreatment
    {
        /**
         * Default insert strategy used by R*-tree
         */
        public static LimitedReinsertOverflowTreatment RSTAR_OVERFLOW =
            new LimitedReinsertOverflowTreatment(new CloseReinsert(0.3, SquaredEuclideanDistanceFunction.STATIC));

        /**
         * Bitset to keep track of levels a reinsert has been performed at.
         */
        private BitArray reinsertions = new BitArray(1000);

        /**
         * Strategy for the actual reinsertions
         */
        private IReinsertStrategy reinsertStrategy;

        /**
         * Constructor.
         * 
         * @param reinsertStrategy Reinsertion strategy
         */
        public LimitedReinsertOverflowTreatment(IReinsertStrategy reinsertStrategy) :
            base()
        {
            this.reinsertStrategy = reinsertStrategy;
        }


        public bool HandleOverflow<N, E>(AbstractRStarTree<N, E> tree, N node, IndexTreePath<E> path)
            where N : AbstractRStarTreeNode<N, E>
            where E : ISpatialEntry
        {
            int level = /* tree.GetHeight() - */(path.GetPathCount() - 1);
            // No reinsertions at root level
            if (path.GetPathCount() == 1)
            {
                return false;
            }
            // Earlier reinsertions at the same level
            if (reinsertions.Get(level))
            {
                return false;
            }

            reinsertions.Set(level, true);
            E entry = path.GetLastPathComponent().GetEntry();
            Debug.Assert(!entry.IsLeafEntry(), "Unexpected leaf entry");
            int[] cands = reinsertStrategy.ComputeReinserts(node as IEnumerable<ISpatialEntry>, NodeArrayAdapter.STATIC, entry);
            if (cands == null || cands.Length == 0)
            {
                return false;
            }
            tree.ReInsert(node, path, cands);
            return true;
        }


        public void Reinitialize()
        {
            reinsertions = new BitArray(1000);
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
             * Fast-insertion parameter. Optional.
             */
            public static OptionDescription REINSERT_STRATEGY_ID = OptionDescription.GetOrCreate(
                "rtree.reinsertion-strategy", "The strategy to select candidates for reinsertion.");

            /**
             * The actual reinsertion strategy
             */
            IReinsertStrategy reinsertStrategy = null;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                ObjectParameter<IReinsertStrategy> strategyP = new ObjectParameter<IReinsertStrategy>
                        (REINSERT_STRATEGY_ID, typeof(IReinsertStrategy), typeof(CloseReinsert));
                if (config.Grab(strategyP))
                {
                    reinsertStrategy = strategyP.InstantiateClass(config);
                }
            }


            protected override object MakeInstance()
            {
                return new LimitedReinsertOverflowTreatment(reinsertStrategy);
            }
        }
    }
}
