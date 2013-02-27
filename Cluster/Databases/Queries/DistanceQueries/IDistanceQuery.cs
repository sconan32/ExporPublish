using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Distances.DistanceValues;
using Socona.Clustering.Databases.Ids;
using Socona.Clustering.Distances.DistanceFuctions;
using Socona.Clustering.Databases.Relations;

namespace Socona.Clustering.Databases.Queries.DistanceQueries
{
   
       /**
 * A distance query serves as adapter layer for database and primitive distances.
 * 
 * @author Erich Schubert
 * 
 * @apiviz.landmark
 * @apiviz.has Distance
 * 
 * @param O Input object type
 * @param D Distance result type
 */
public interface IDistanceQuery<O, D >:IDatabaseQuery
    where D:IDistance
{
  /**
   * Returns the distance between the two objects specified by their object ids.
   * 
   * @param id1 first object id
   * @param id2 second object id
   * @return the distance between the two objects specified by their object ids
   */
  D Distance(IDbIdRef id1, IDbIdRef id2);

  /**
   * Returns the distance between the two objects specified by their object ids.
   * 
   * @param o1 first object
   * @param id2 second object id
   * @return the distance between the two objects specified by their object ids
   */
  D Distance(O o1, IDbIdRef id2);

  /**
   * Returns the distance between the two objects specified by their object ids.
   * 
   * @param id1 first object id
   * @param o2 second object
   * @return the distance between the two objects specified by their object ids
   */
  D Distance(IDbIdRef id1, O o2);

  /**
   * Returns the distance between the two objects specified by their object ids.
   * 
   * @param o1 first object
   * @param o2 second object
   * @return the distance between the two objects specified by their object ids
   */
  D Distance(O o1, O o2);

  /**
   * Method to get the distance functions factory.
   * 
   * @return Factory for distance objects
   */
  D GetDistanceFactory();

  /**
   * Get the inner distance function.
   * 
   * @return Distance function
   */
  IDistanceFunction<O> GetDistanceFunction();

  /**
   * Provides an infinite distance.
   * 
   * @return an infinite distance
   */
  D infiniteDistance();

  /**
   * Provides a null distance.
   * 
   * @return a null distance
   */
  D nullDistance();

  /**
   * Provides an undefined distance.
   * 
   * @return an undefined distance
   */
  D undefinedDistance();
  
  /**
   * Access the underlying data query.
   * 
   * @return data query in use
   */
  IRelation<O> getRelation();
}
}
