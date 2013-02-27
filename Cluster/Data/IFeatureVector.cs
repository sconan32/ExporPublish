using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Utilities.DataStructures;

namespace Socona.Clustering.Data
{
    
/**
 * Generic FeatureVector class that can contain any type of data (i.e. numerical
 * or categorical attributes). See {@link NumberVector} for vectors that
 * actually store numerical features.
 * 
 * @author Erich Schubert
 * 
 * @param <V> Vector class
 * @param <D> Data type
 */
public interface IFeatureVector< D> {
  /**
   * The dimensionality of the vector space where of this FeatureVector of V is
   * an element.
   * 
   * @return the number of dimensions of this FeatureVector of V
   */
  int GetDimensionality();

  /**
   * Returns the value in the specified dimension.
   * 
   * @param dimension the desired dimension, where 1 &le; dimension &le;
   *        <code>this.getDimensionality()</code>
   * @return the value in the specified dimension
   */
  D GetValue(int dimension);

  /**
   * Returns a String representation of the FeatureVector of V as a line that is
   * suitable to be printed in a sequential file.
   * 
   * @return a String representation of the FeatureVector of V
   */
  
  override String ToString();

  /**
   * Returns a new FeatureVector of V for the given values.
   * 
   * @param array the values of the featureVector
   * @param adapter adapter class
   * @param <A> Array type
   * @return a new FeatureVector of V for the given values
   */
  IFeatureVector<D> newFeatureVector<A> (A array, IArrayAdapter<D, A> adapter);
}
}
