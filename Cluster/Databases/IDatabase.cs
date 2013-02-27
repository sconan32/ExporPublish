using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Resluts;
using Socona.Clusterin.Utilities;
using Socona.Clustering.Databases.Relations;
using Socona.Clustering.Data.Types;
using Socona.Clustering.Databases.Queries.DistanceQueries;
using Socona.Clustering.Databases.Queries.SimilarityQueries;
using Socona.Clustering.Databases.Ids;

namespace Socona.Clustering.Databases
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
public interface IDatabase : IHierarchicalResult, InspectionUtilFrequentlyScanned {
  /**
   * Initialize the database, for example by loading the input data. (Since this
   * should NOT be done on construction time!)
   */
  public void Initialize();

  /**
   * Get all relations of a database.
   * 
   * @return All relations in the database
   */
  IList<IRelation<double>> GetRelations();

  /**
   * Get an object representation.
   * 
   * @param <O> Object type
   * @param restriction Type restriction
   * @param hints Optimizer hints
   * @return representation
   */
   IRelation<O> GetRelation<O>(ITypeInformation restriction,params Object hints) ;

  /**
   * Get the distance query for a particular distance function.
   * 
   * @param <O> Object type
   * @param <D> Distance result type
   * @param relation Relation used
   * @param distanceFunction Distance function to use
   * @param hints Optimizer hints
   * @return Instance to query the database with this distance
   */
   IDistanceQuery<O, D> GetDistanceQuery<O, D>(IRelation<O> relation, IDistanceFunction<O> distanceFunction, params Object hints);

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
  ISimilarityQuery<O, D> GetSimilarityQuery(IRelation<O> relation, SimilarityFunction<? super O, D> similarityFunction, params Object hints);

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
  <O, D extends Distance<D>> KNNQuery<O, D> getKNNQuery(DistanceQuery<O, D> distanceQuery, Object... hints);

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
  <O, D extends Distance<D>> RangeQuery<O, D> getRangeQuery(DistanceQuery<O, D> distanceQuery, Object... hints);

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
  <O, D extends Distance<D>> RKNNQuery<O, D> getRKNNQuery(DistanceQuery<O, D> distanceQuery, Object... hints);

  /**
   * Returns the DatabaseObject represented by the specified id.
   * 
   * @param id the id of the Object to be obtained from the Database
   * @return Bundle containing the objects' data
   */
  SingleObjectBundle getBundle(IDbId id);

  /**
   * Returns the DatabaseObject represented by the specified id.
   * 
   * @param id the id of the Object to be obtained from the Database
   * @return Bundle containing the object data
   * @throws ObjectNotFoundException when the DBID was not found.
   */
  // TODO: add
  // MultipleObjectsBundle getBundles(DBIDs id) throws ObjectNotFoundException;

  /**
   * Add a new index to the database.
   * 
   * @param index Index to add
   */
  public void addIndex(Index index);

  /**
   * Collection of known indexes
   */
  public ICollection<Index> getIndexes();

  /**
   * Remove a particular index
   * 
   * @param index Index to remove
   */
  public void RemoveIndex(Index index);

  /**
   * Adds a listener for the <code>DataStoreEvent</code> posted after the
   * content of the database changes.
   * 
   * @param l the listener to add
   * @see #removeDataStoreListener(DataStoreListener)
   * @see DataStoreListener
   * @see DataStoreEvent
   */
  void addDataStoreListener(DataStoreListener l);

  /**
   * Removes a listener previously added with
   * {@link #addDataStoreListener(DataStoreListener)}.
   * 
   * @param l the listener to remove
   * @see #addDataStoreListener(DataStoreListener)
   * @see DataStoreListener
   * @see DataStoreEvent
   */
  void removeDataStoreListener(DataStoreListener l);

  /**
   * Collects all insertion, deletion and update events until
   * {@link #flushDataStoreEvents()} is called.
   * 
   * @see DataStoreEvent
   */
  void accumulateDataStoreEvents();

  /**
   * Fires all collected insertion, deletion and update events as one
   * DataStoreEvent, i.e. notifies all registered DataStoreListener how the
   * content of the database has been changed since
   * {@link #accumulateDataStoreEvents()} has been called.
   * 
   * @see DataStoreListener
   * @see DataStoreEvent
   */
  void flushDataStoreEvents();
}
}
