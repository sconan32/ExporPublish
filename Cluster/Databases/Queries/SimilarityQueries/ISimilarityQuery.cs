using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Distances.DistanceValues;
using Socona.Clustering.Databases.Ids;
using Socona.Clustering.Databases.Relations;

namespace Socona.Clustering.Databases.Queries.SimilarityQueries
{
   
/**
 * A similarity query serves as adapter layer for database and primitive
 * similarity functions.
 * 
 * @author Erich Schubert
 * 
 * @apiviz.landmark
 * @apiviz.has Distance
 * 
 * @param O Input object type
 * @param D Distance result type
 */
public interface ISimilarityQuery<O, D > : IDatabaseQuery
    where D:IDistance
{
  /**
   * Returns the similarity between the two objects specified by their object
   * ids.
   * 
   * @param id1 first object id
   * @param id2 second object id
   * @return the similarity between the two objects specified by their object
   *         ids
   */
  public abstract D similarity(IDbIdRef id1, IDbIdRef id2);

  /**
   * Returns the similarity between the two objects specified by their object
   * ids.
   * 
   * @param o1 first object
   * @param id2 second object id
   * @return the similarity between the two objects specified by their object
   *         ids
   */
  public abstract D similarity(O o1, IDbIdRef id2);

  /**
   * Returns the similarity between the two objects specified by their object
   * ids.
   * 
   * @param id1 first object id
   * @param o2 second object
   * @return the similarity between the two objects specified by their object
   *         ids
   */
  public abstract D similarity(IDbIdRef id1, O o2);

  /**
   * Returns the similarity between the two objects specified by their object
   * ids.
   * 
   * @param o1 first object
   * @param o2 second object
   * @return the similarity between the two objects specified by their object
   *         ids
   */
  public abstract D similarity(O o1, O o2);

  /**
   * Method to get the distance functions factory.
   * 
   * @return Factory for distance objects
   */
  public abstract D getDistanceFactory();
  
  /**
   * Access the underlying data query.
   * 
   * @return data query in use
   */
  public abstract IRelation< O> GetRelation();
}
}
