using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results;
using Socona.Expor.Utilities;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.SimilarityQueries;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.SimilarityFunctions;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Databases.Queries.RangeQueries;
using Socona.Expor.Databases.Queries.RKnnQueries;
using Socona.Expor.Indexes;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Data;

namespace Socona.Expor.Databases
{
    /**
   * Database specifies the requirements for any database implementation. Note
   * that any implementing class is supposed to provide a constructor without
   * parameters for dynamic instantiation.
   * 
   * @author Elke Achtert
   * 
   * @apiviz.landmark
   * @apiviz.has DistanceQuery oneway - - provides
   * @apiviz.has KNNQuery oneway - - provides
   * @apiviz.has RangeQuery oneway - - provides
   * @apiviz.has RKNNQuery oneway - - provides
   * @apiviz.has Relation oneway - - contains
   * @apiviz.has Index oneway - - manages
   * @apiviz.uses DataStoreListener oneway - - invokes
   */
    public interface IDatabase: IHierarchicalResult, IInspectionUtilFrequentlyScanned
    {
        /**
         * Initialize the database, for example by loading the input data. (Since this
         * should NOT be done on construction time!)
         */
        void Initialize();

        /**
         * Get all relations of a database.
         * 
         * @return All relations in the database
         */
        IList<IRelation> GetRelations();


        /// <summary>
        /// Get an object representation.
        /// </summary>
        /// <param name="restriction">Type restriction</param>
        /// <param name="hints">Optimizer hints</param>
        /// <returns>representation</returns>
        IRelation GetRelation(ITypeInformation restriction, params Object[] hints);

        /**
         * Get the distance query for a particular distance function.
         * @param relation Relation used
         * @param distanceFunction Distance function to use
         * @param hints Optimizer hints
         * @return Instance to query the database with this distance
         */
        IDistanceQuery GetDistanceQuery(IRelation relation, IDistanceFunction distanceFunction, params Object[] hints);
           


        /**
         * Get the similarity query for a particular similarity function.
         * 
         * @param <O> Object type
         * @param <D> Similarity result type
         * @param relation Relation used
         * @param similarityFunction Similarity function to use
         * @param hints Optimizer hints
         * @return Instance to query the database with this similarity
         */
        ISimilarityQuery GetSimilarityQuery(IRelation relation, ISimilarityFunction similarityFunction, params Object[] hints);

        /**
         * Get a KNN query object for the given distance query.
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
         * @param distanceQuery Distance query
         * @param hints Optimizer hints
         * @return KNN Query object
         */
        IKNNQuery GetKNNQuery(IDistanceQuery distanceQuery, params Object[] hints);

        /**
         * Get a range query object for the given distance query.
         * 
         * When possible, this will use an index, but it may default to an expensive
         * linear scan.
         * 
         * Hints include:
         * <ul>
         * <li>Distance object: Maximum query range</li>
         * <li>{@link de.lmu.ifi.dbs.elki.database.query.DatabaseQuery#HINT_BULK} bulk
         * query needed</li>
         * </ul>
         * 
         * @param <O> Object type
         * @param <D> Distance type
         * @param distanceQuery Distance query
         * @param hints Optimizer hints
         * @return KNN Query object
         */
        IRangeQuery GetRangeQuery(IDistanceQuery distanceQuery, params Object[] hints);

        /**
         * Get a rKNN query object for the given distance query.
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
         * @param distanceQuery Distance query
         * @param hints Optimizer hints
         * @return KNN Query object
         */
        IRKNNQuery GetRKNNQuery(IDistanceQuery distanceQuery, params Object[] hints);

        /**
         * Returns the DatabaseObject represented by the specified id.
         * 
         * @param id the id of the Object to be obtained from the Database
         * @return Bundle containing the objects' data
         */
        SingleObjectBundle GetBundle(IDbId id);

        /**
         * Returns the DatabaseObject represented by the specified id.
         * 
         * @param id the id of the Object to be obtained from the Database
         * @return Bundle containing the object data
         * @throws ObjectNotFoundException when the DBID was not found.
         */
        // TODO: add
        MultipleObjectsBundle GetBundles(IDbIds id);

        /**
         * Add a new index to the database.
         * 
         * @param index Index to add
         */
        void AddIndex(IIndex index);

        /**
         * Collection of known indexes
         */
        ICollection<IIndex> GetIndexes();

        /**
         * Remove a particular index
         * 
         * @param index Index to remove
         */
        void RemoveIndex(IIndex index);

        /**
         * Adds a listener for the <code>DataStoreEvent</code> posted after the
         * content of the database changes.
         * 
         * @param l the listener to add
         * @see #removeDataStoreListener(DataStoreListener)
         * @see DataStoreListener
         * @see DataStoreEvent
         */
        //  void AddDataStoreListener(DataStoreListener l);

        /**
         * Removes a listener previously added with
         * {@link #addDataStoreListener(DataStoreListener)}.
         * 
         * @param l the listener to remove
         * @see #addDataStoreListener(DataStoreListener)
         * @see DataStoreListener
         * @see DataStoreEvent
         */
        //  void RemoveDataStoreListener(DataStoreListener l);

        /**
         * Collects all insertion, deletion and update events until
         * {@link #flushDataStoreEvents()} is called.
         * 
         * @see DataStoreEvent
         */
        // void AccumulateDataStoreEvents();

        /**
         * Fires all collected insertion, deletion and update events as one
         * DataStoreEvent, i.e. notifies all registered DataStoreListener how the
         * content of the database has been changed since
         * {@link #accumulateDataStoreEvents()} has been called.
         * 
         * @see DataStoreListener
         * @see DataStoreEvent
         */
       // void FlushDataStoreEvents();

        event EventHandler<ObjectsChangeEventArgs> ObjectsInserted;

    }
    public class ObjectsChangeEventArgs : EventArgs
    {
        public IArrayDbIds DbIds { get; set; }
    }
}
