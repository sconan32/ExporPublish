using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Databases.Queries.RangeQueries;
using Socona.Expor.Databases.Queries.RKnnQueries;
using Socona.Expor.Databases.Queries.SimilarityQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.SimilarityFunctions;

namespace Socona.Expor.Databases
{

    public sealed class QueryUtil
    {
        /**
         * Get a distance query for a given distance function, automatically choosing
         * a relation.
         * 
         * @param <O> Object type
         * @param <D> Distance type
         * @param database Database
         * @param distanceFunction Distance function
         * @param hints Optimizer hints
         * @return Distance query
         */
        public static IDistanceQuery GetDistanceQuery<O>(IDatabase database,
            IDistanceFunction distanceFunction,
            params Object[] hints)
            where O : ISpatialComparable
        {
            IRelation objectQuery = database.GetRelation(distanceFunction.GetInputTypeRestriction(), hints);
            return database.GetDistanceQuery(objectQuery, distanceFunction, hints);
        }

        /**
         * Get a similarity query, automatically choosing a relation.
         * 
         * @param <O> Object type
         * @param <D> Distance type
         * @param database Database
         * @param similarityFunction Similarity function
         * @param hints Optimizer hints
         * @return Similarity Query
         */
        public static ISimilarityQuery GetSimilarityQuery<O>(IDatabase database,
            ISimilarityFunction similarityFunction, params object[] hints)
        {
            IRelation objectQuery = database.GetRelation(similarityFunction.GetInputTypeRestriction(), hints);
            return database.GetSimilarityQuery(objectQuery, similarityFunction, hints);
        }

        /**
         * Get a KNN query object for the given distance function.
         * 
         * When possible, this will use an index, but it may default to an expensive
         * linear scan.
         * 
         * Hints include:
         * <ul>
         * <li>Integer: maximum value for k needed</li>
         * <li>{@link de.lmu.ifi.dbs.elki.database.query.DatabaseQuery#HINT_BULK} bulk
         * query needed</li>
         * </ul>
         * 
         * @param <O> Object type
         * @param <D> Distance type
         * @param database Database
         * @param distanceFunction Distance function
         * @param hints Optimizer hints
         * @return KNN Query object
         */
        public static IKNNQuery GetKNNQuery(IDatabase database, IDistanceFunction distanceFunction, params object[] hints)
        {
            IRelation relation = database.GetRelation(distanceFunction.GetInputTypeRestriction(), hints);
            IDistanceQuery distanceQuery = database.GetDistanceQuery(relation, distanceFunction, hints);
            return database.GetKNNQuery(distanceQuery, hints);
        }

        /**
         * Get a KNN query object for the given distance function.
         * 
         * When possible, this will use an index, but it may default to an expensive
         * linear scan.
         * 
         * Hints include:
         * <ul>
         * <li>Integer: maximum value for k needed</li>
         * <li>{@link de.lmu.ifi.dbs.elki.database.query.DatabaseQuery#HINT_BULK} bulk
         * query needed</li>
         * </ul>
         * @param relation Relation used
         * @param distanceFunction Distance function
         * @param hints Optimizer hints
         * 
         * @param <O> Object type
         * @param <D> Distance type
         * @return KNN Query object
         */
        public static IKNNQuery GetKNNQuery(IRelation relation, IDistanceFunction distanceFunction, params object[] hints)
        {
            IDatabase database = relation.GetDatabase();
            IDistanceQuery distanceQuery = database.GetDistanceQuery(relation, distanceFunction, hints);
            return database.GetKNNQuery(distanceQuery, hints);
        }

        /**
         * Get a range query object for the given distance function.
         * 
         * When possible, this will use an index, but it may default to an expensive
         * linear scan.
         * 
         * Hints include:
         * <ul>
         * <li>Range: maximum range requested</li>
         * <li>{@link de.lmu.ifi.dbs.elki.database.query.DatabaseQuery#HINT_BULK} bulk
         * query needed</li>
         * </ul>
         * 
         * @param <O> Object type
         * @param <D> Distance type
         * @param database Database
         * @param distanceFunction Distance function
         * @param hints Optimizer hints
         * @return KNN Query object
         */
        public static IRangeQuery GetRangeQuery(IDatabase database, IDistanceFunction distanceFunction, params object[] hints)
        {
            IRelation relation = database.GetRelation(distanceFunction.GetInputTypeRestriction(), hints);
            IDistanceQuery distanceQuery = database.GetDistanceQuery(relation, distanceFunction, hints);
            return database.GetRangeQuery(distanceQuery, hints);
        }

        /**
         * Get a range query object for the given distance function.
         * 
         * When possible, this will use an index, but it may default to an expensive
         * linear scan.
         * 
         * Hints include:
         * <ul>
         * <li>Range: maximum range requested</li>
         * <li>{@link de.lmu.ifi.dbs.elki.database.query.DatabaseQuery#HINT_BULK} bulk
         * query needed</li>
         * </ul>
         * @param relation Relation used
         * @param distanceFunction Distance function
         * @param hints Optimizer hints
         * 
         * @param <O> Object type
         * @param <D> Distance type
         * @return KNN Query object
         */
        public static IRangeQuery GetRangeQuery(IRelation relation, IDistanceFunction distanceFunction, params object[] hints)
        {
            IDatabase database = relation.GetDatabase();
            IDistanceQuery distanceQuery = database.GetDistanceQuery(relation, distanceFunction, hints);
            return database.GetRangeQuery(distanceQuery, hints);
        }

        /**
         * Get a rKNN query object for the given distance function.
         * 
         * When possible, this will use an index, but it may default to an expensive
         * linear scan.
         * 
         * Hints include:
         * <ul>
         * <li>Integer: maximum value for k needed</li>
         * <li>{@link de.lmu.ifi.dbs.elki.database.query.DatabaseQuery#HINT_BULK} bulk
         * query needed</li>
         * </ul>
         * @param relation Relation used
         * @param distanceFunction Dist
         * ance function
         * @param hints Optimizer hints
         * 
         * @param <O> Object type
         * @param <D> Distance type
         * @return RKNN Query object
         */
        public static IRKNNQuery GetRKNNQuery(IRelation relation, IDistanceFunction distanceFunction, params object[] hints)
        {
            IDatabase database = relation.GetDatabase();
            IDistanceQuery distanceQuery = database.GetDistanceQuery(relation, distanceFunction, hints);
            return database.GetRKNNQuery(distanceQuery, hints);
        }

        /**
         * Get a linear scan query for the given distance query.
         * 
         * @param <O> Object type
         * @param <D> Distance type
         * @param distanceQuery distance query
         * @return KNN query
         */
        public static IKNNQuery GetLinearScanKNNQuery(IDistanceQuery distanceQuery)
        {
            // Slight optimizations of linear scans
            if (distanceQuery is PrimitiveDistanceQuery<INumberVector>)
            {
                if (distanceQuery.DistanceFunction is IPrimitiveDoubleDistanceFunction<ISpatialComparable>)
                {
                    PrimitiveDistanceQuery<INumberVector> pdq = (PrimitiveDistanceQuery<INumberVector>)distanceQuery;

                    IKNNQuery knnQuery = new LinearScanRawDoubleDistanceKNNQuery((PrimitiveDistanceQuery<INumberVector>)pdq);

                    IKNNQuery castQuery = (IKNNQuery)knnQuery;
                    return castQuery;
                }
                else
                {
                    PrimitiveDistanceQuery<INumberVector> pdq = (PrimitiveDistanceQuery<INumberVector>)distanceQuery;
                    return new LinearScanPrimitiveDistanceKNNQuery(pdq);
                }
            }
            return new LinearScanKNNQuery<INumberVector>(distanceQuery);
        }

        /**
         * Get a linear scan query for the given distance query.
         * 
         * @param <O> Object type
         * @param <D> Distance type
         * @param distanceQuery distance query
         * @return Range query
         */
        public static IRangeQuery GetLinearScanRangeQuery(IDistanceQuery distanceQuery)
        {
            // Slight optimizations of linear scans
            if (distanceQuery is PrimitiveDistanceQuery<INumberVector>)
            {
                if (distanceQuery.DistanceFunction is IPrimitiveDoubleDistanceFunction<INumberVector>)
                {
                    PrimitiveDistanceQuery<INumberVector> pdq = (PrimitiveDistanceQuery<INumberVector>)distanceQuery;

                    IRangeQuery knnQuery = new LinearScanRawDoubleDistanceRangeQuery<INumberVector>((PrimitiveDistanceQuery<INumberVector>)pdq);

                    IRangeQuery castQuery = (IRangeQuery)knnQuery;
                    return castQuery;
                }
                else
                {
                    PrimitiveDistanceQuery<INumberVector> pdq = (PrimitiveDistanceQuery<INumberVector>)distanceQuery;
                    return new LinearScanPrimitiveDistanceRangeQuery<INumberVector>(pdq);
                }
            }
            return new LinearScanRangeQuery<INumberVector>(distanceQuery);
        }
    }
}
