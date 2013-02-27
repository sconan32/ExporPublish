using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Data.Types;
using Socona.Clustering.Distances.DistanceValues;
using Socona.Clustering.Databases.Queries.SimilarityQueries;
using Socona.Clustering.Databases.Relations;
using Socona.Clustering.Utilities.Options;

namespace Socona.Clustering.Distances.SimilarityFunctions
{
    
/**
 * Interface SimilarityFunction describes the requirements of any similarity
 * function.
 * 
 * @author Elke Achtert
 * 
 * @apiviz.landmark
 * @apiviz.has Distance
 * 
 * @param <O> object type
 * @param <D> distance type
 */
public interface ISimilarityFunction<O> : IParameterizable {
  /**
   * Is this function symmetric?
   * 
   * @return {@code true} when symmetric
   */
  bool IsSymmetric();

  /**
   * Get the input data type of the function.
   */
  ITypeInformation GetInputTypeRestriction();

  /**
   * Get a distance factory.
   * 
   * @return distance factory
   */
  IDistance GetDistanceFactory();

  /**
   * Instantiate with a representation to get the actual similarity query.
   * 
   * @param relation Representation to use
   * @return Actual distance query.
   */
  public  ISimilarityQuery<T,D> Instantiate<T,D>(IRelation<T> relation);
}
}
