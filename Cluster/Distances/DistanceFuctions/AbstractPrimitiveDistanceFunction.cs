using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Distances.DistanceValues;
using Socona.Clustering.Databases.Relations;
using Socona.Clustering.Databases.Queries.DistanceQueries;

namespace Socona.Clustering.Distances.DistanceFuctions
{
  
/**
 * AbstractDistanceFunction provides some methods valid for any extending class.
 * 
 * @author Arthur Zimek
 * 
 * @param <O> the type of objects to compute the distances in between
 * @param <D> the type of Distance used
 */
public abstract class AbstractPrimitiveDistanceFunction<O, D >:IPrimitiveDistanceFunction<O, D>
    where D:IDistance
{
  /**
   * Provides an abstract DistanceFunction.
   */
  public AbstractPrimitiveDistanceFunction() {
    // EMPTY
  }

  
  abstract override public D distance(O o1, O o2);

  
  abstract override public  D getDistanceFactory();

  
  public override bool isSymmetric() {
    // Assume symmetric by default!
    return true;
  }

  
  public override bool isMetric() {
      // Do NOT assume triangle equation by default!
    return false;
  }

  /**
   * Instantiate with a database to get the actual distance query.
   * 
   * @param relation Representation
   * @return Actual distance query.
   */
  
  public override  IDistanceQuery<T, D> instantiate<T>(IRelation<T> relation) {
    return new PrimitiveDistanceQuery<T, D>(relation, this);
  }
}
}
