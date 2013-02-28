using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Databases.Queries.RangeQueries;
using Socona.Expor.Distances.DistanceFuctions;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Queries
{

    public class RStarTreeUtil
    {
        /**
         * Get an RTree range query, using an optimized double implementation when
         * possible.
         * 
         * @param <O> Object type
         * @param <D> Distance type
         * @param tree Tree to query
         * @param distanceQuery distance query
         * @param hints Optimizer hints
         * @return Query object
         */

        public static IRangeQuery GetRangeQuery<N, E>(AbstractRStarTree<N, E> tree,
            ISpatialDistanceQuery distanceQuery, params Object[] hints)
            where N : AbstractRStarTreeNode<N, E>
            where E : ISpatialEntry
        {
            // Can we support this distance function - spatial distances only!
            ISpatialPrimitiveDistanceFunction df =
                (ISpatialPrimitiveDoubleDistanceFunction)distanceQuery.DistanceFunction;
            // Can we use an optimized query?
            if (df is ISpatialPrimitiveDoubleDistanceFunction)
            {
                IDistanceQuery dqc = (IDistanceQuery)(distanceQuery);
                ISpatialPrimitiveDoubleDistanceFunction dfc = (ISpatialPrimitiveDoubleDistanceFunction)(df);
                IRangeQuery q = new DoubleDistanceRStarTreeRangeQuery<INumberVector, N, E>(tree, dqc, dfc);
                return (IRangeQuery)q;
            }
            return new GenericRStarTreeRangeQuery<INumberVector, N, E>(tree, distanceQuery);
        }

        /**
         * Get an RTree knn query, using an optimized double implementation when
         * possible.
         * 
         * @param <O> Object type
         * @param <D> Distance type
         * @param tree Tree to query
         * @param distanceQuery distance query
         * @param hints Optimizer hints
         * @return Query object
         */

        public static IKNNQuery GetKNNQuery<N, E>(AbstractRStarTree<N, E> tree,
            ISpatialDistanceQuery distanceQuery, params Object[] hints)
            where N : AbstractRStarTreeNode<N, E>
            where E : ISpatialEntry
        {
            // Can we support this distance function - spatial distances only!
            ISpatialPrimitiveDistanceFunction df = (ISpatialPrimitiveDistanceFunction)distanceQuery.DistanceFunction;
            // Can we use an optimized query?
            if (df is ISpatialPrimitiveDoubleDistanceFunction)
            {
                IDistanceQuery dqc = (IDistanceQuery)(distanceQuery);
                ISpatialPrimitiveDoubleDistanceFunction dfc = (ISpatialPrimitiveDoubleDistanceFunction)(df);
                IKNNQuery q = new DoubleDistanceRStarTreeKNNQuery<INumberVector, N, E>(tree, dqc, dfc);
                return (IKNNQuery)q;
            }
            return new GenericRStarTreeKNNQuery<INumberVector, N, E>(tree, distanceQuery);
        }
    }
}
