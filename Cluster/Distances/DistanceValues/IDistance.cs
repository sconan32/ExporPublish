using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Socona.Clustering.Distances.DistanceValues
{
   
/**
 * The interface Distance defines the requirements of any instance class.
 * 
 * See {@link de.lmu.ifi.dbs.elki.distance.DistanceUtil} for related utility
 * functions such as <code>min</code>, <code>max</code>.
 * 
 * @author Arthur Zimek
 * 
 * @see de.lmu.ifi.dbs.elki.distance.DistanceUtil
 * 
 * @apiviz.landmark
 * 
 * @param <D> the type of Distance used
 */

public interface IDistance : IComparable,ISerializable {
  /**
   * Returns a new distance as sum of this distance and the given distance.
   * 
   * @param distance the distance to be added to this distance
   * @return a new distance as sum of this distance and the given distance
   */
  IDistance Plus(IDistance distance);

  /**
   * Returns a new Distance by subtracting the given distance from this
   * distance.
   * 
   * @param distance the distance to be subtracted from this distance
   * @return a new Distance by subtracting the given distance from this distance
   */
  IDistance Minus(IDistance distance);

  /**
   * Any implementing class should implement a proper toString-method for
   * printing the result-values.
   * 
   * @return String a human-readable representation of the Distance
   */

  override String ToString();

  /**
   * Provides a measurement suitable to this measurement function based on the
   * given pattern.
   * 
   * @param pattern a pattern defining a similarity suitable to this measurement
   *        function
   * @return a measurement suitable to this measurement function based on the
   *         given pattern
   * @throws IllegalArgumentException if the given pattern is not compatible
   *         with the requirements of this measurement function
   */
  IDistance ParseString(String pattern);

  /**
   * Returns a String as description of the required input format.
   * 
   * @return a String as description of the required input format
   */
  String RequiredInputPattern();

  /**
   * Returns the number of Bytes this distance uses if it is written to an
   * external file.
   * 
   * @return the number of Bytes this distance uses if it is written to an
   *         external file
   */
  int ExternalizableSize();

  /**
   * Provides an infinite distance.
   * 
   * @return an infinite distance
   */
  IDistance InfiniteDistance();

  /**
   * Provides a null distance.
   * 
   * @return a null distance
   */
  IDistance NullDistance();

  /**
   * Provides an undefined distance.
   * 
   * @return an undefined distance
   */
  IDistance UndefinedDistance();

  /**
   * Returns true, if the distance is an infinite distance, false otherwise.
   * 
   * @return true, if the distance is an infinite distance, false otherwise
   */
  bool IsInfiniteDistance();

  /**
   * Returns true, if the distance is a null distance, false otherwise.
   * 
   * @return true, if the distance is a null distance, false otherwise
   */
  bool IsNullDistance();

  /**
   * Returns true, if the distance is an undefined distance, false otherwise.
   * 
   * @return true, if the distance is an undefined distance, false otherwise
   */
  bool IsUndefinedDistance();
}
}
