using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Bulk;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Insert;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Overflow;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Split;
using Socona.Expor.Persistent;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Rstar
{

    /**
     * Factory for regular R*-Trees.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.landmark factory
     * @apiviz.uses RStarTreeIndex oneway - - 芦create禄
     * 
     * @param <O> Object type
     */
    public class RStarTreeFactory<O> : AbstractRStarTreeFactory
    where O : INumberVector
    {
        /**
         * Constructor.
         * 
         * @param fileName
         * @param pageSize
         * @param cacheSize
         * @param bulkSplitter Bulk loading strategy
         * @param insertionStrategy the strategy to find the insertion child
         * @param nodeSplitter the strategy for splitting nodes.
         * @param overflowTreatment the strategy to use for overflow treatment
         * @param minimumFill the relative minimum fill
         */
        public RStarTreeFactory(String fileName, int pageSize, long cacheSize, IBulkSplit bulkSplitter,
            IInsertionStrategy insertionStrategy, ISplitStrategy nodeSplitter,
            IOverflowTreatment overflowTreatment, double minimumFill) :
            base(fileName, pageSize, cacheSize, bulkSplitter,
            insertionStrategy, nodeSplitter, overflowTreatment, minimumFill)
        {
        }


        public override IIndex Instantiate(IRelation relation)
        {
            IPageFile<RStarTreeNode> pagefile = MakePageFile<RStarTreeNode>(GetNodeClass());
            RStarTreeIndex<O> index = new RStarTreeIndex<O>(relation, pagefile);
            index.SetBulkStrategy(bulkSplitter);
            index.SetInsertionStrategy(insertionStrategy);
            index.SetNodeSplitStrategy(nodeSplitter);
            index.SetOverflowTreatment(overflowTreatment);
            index.SetMinimumFill(minimumFill);
            return index;
        }

        protected Type GetNodeClass()
        {
            return typeof(RStarTreeNode);
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : AbstractRStarTreeFactory.Parameterizer
        {

            protected override object MakeInstance()
            {
                return new RStarTreeFactory<O>(fileName, pageSize, cacheSize, bulkSplitter,
                    insertionStrategy, nodeSplitter, overflowTreatment, minimumFill);
            }

           
        }
    }
}
