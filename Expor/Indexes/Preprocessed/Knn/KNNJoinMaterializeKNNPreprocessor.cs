using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Algorithms;
using Socona.Expor.Data;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Indexes.Tree.Spatial;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Rstar;
using Socona.Expor.Utilities.DataStructures.Heap;
using Socona.Log;

namespace Socona.Expor.Indexes.Preprocessed.Knn
{

    /**
     * Class to materialize the kNN using a spatial join on an R-tree.
     * 
     * @author Erich Schubert
     * 
     * @param <V> vector type
     * @param <D> distance type
     */
    public class KNNJoinMaterializeKNNPreprocessor : AbstractMaterializeKNNPreprocessor<INumberVector>
    {
        /**
         * Logging class.
         */
        private static Logging LOG = Logging.GetLogger(typeof(KNNJoinMaterializeKNNPreprocessor));

        /**
         * Constructor.
         * 
         * @param relation Relation to index
         * @param distanceFunction Distance function
         * @param k k
         */
        public KNNJoinMaterializeKNNPreprocessor(IRelation relation, IDistanceFunction distanceFunction, int k) :
            base(relation, distanceFunction, k)
        {
        }


        protected override void Preprocess()
        {
            // Run KNNJoin
            var knnjoin = new KNNJoin<INumberVector, RStarTreeNode, ISpatialEntry>(distanceFunction, k);
            storage = knnjoin.Run(relation.GetDatabase(), relation);
        }


        protected override Logging GetLogger()
        {
            return LOG;
        }


        public override String LongName
        {
            get
            {
                return "knn-join materialized neighbors";
            }
        }


        public override String ShortName
        {
            get
            {
                return "knn-join";
            }
        }


        public  void LogStatistics()
        {
            // No statistics to log.
        }

        /**
         * The parameterizable factory.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.landmark
         * @apiviz.stereotype factory
         * @apiviz.uses AbstractMaterializeKNNPreprocessor oneway - - 芦create禄
         * 
         * @param <O> The object type
         * @param <D> The distance type
         */
        public new  class Factory : AbstractMaterializeKNNPreprocessor<INumberVector>.Factory
        {

            /**
           * Constructor.
           * 
           * @param k K
           * @param distanceFunction distance function
           */
            public Factory(int k, IDistanceFunction distanceFunction) :
                base(k, distanceFunction)
            {
            }


            public override IIndex Instantiate(IRelation relation)
            {
                return new KNNJoinMaterializeKNNPreprocessor(relation, distanceFunction, k);
            }

            /**
             * Parameterization class
             * 
             * @author Erich Schubert
             * 
             * @apiviz.exclude
             * 
             * @param <O> Object type
             * @param <D> Distance type
             */
            public new  class Parameterizer : AbstractMaterializeKNNPreprocessor<INumberVector>.Factory.Parameterizer
            {

                protected override object MakeInstance()
                {
                    return new KNNJoinMaterializeKNNPreprocessor.Factory(k, distanceFunction);
                }
            }
        }
    }
}
