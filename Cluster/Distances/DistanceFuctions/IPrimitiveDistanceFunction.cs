using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Data.Types;

namespace Socona.Clustering.Distances.DistanceFuctions
{
  
/**
 * Primitive distance function that is defined on some kind of object.
 * 
 * @author Erich Schubert
 * 
 * @apiviz.landmark
 * 
 * @param <O> input object type
 * @param <D> distance result type
 */
public interface IPrimitiveDistanceFunction<O, D>: IDistanceFunction<O, D> {
  /**
   * Computes the distance between two given DatabaseObjects according to this
   * distance function.
   * 
   * @param o1 first DatabaseObject
   * @param o2 second DatabaseObject
   * @return the distance between two given DatabaseObjects according to this
   *         distance function
   */
  D distance(O o1, O o2);
  

  override  SimpleTypeInformation<T> getInputTypeRestriction<T>();  
}
}
