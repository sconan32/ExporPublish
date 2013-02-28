using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.ArrayLike
{
    
/**
 * Utility class that allows plug-in use of various "array-like" types such as
 * lists in APIs that can take any kind of array to safe the cost of
 * reorganizing the objects into a real array.
 * 
 * @author Erich Schubert
 * 
 * @apiviz.landmark
 * @apiviz.composedOf ArrayAdapter
 */
public  class ArrayLikeUtil {
  /**
   * Static instance for lists
   */
  private static readonly ListArrayAdapter<Object> LISTADAPTER = new ListArrayAdapter<Object>();

  /**
   * Static instance for lists of numbers
   */
  private static readonly ListArrayAdapter<ValueType> NUMBERLISTADAPTER = new ListArrayAdapter<ValueType>();

  /**
   * Static instance
   */
  private readonly static IdentityArrayAdapter<object> IDENTITYADAPTER = new IdentityArrayAdapter<Object>();

  /**
   * Static instance
   */
  private static readonly FeatureVectorAdapter<ValueType> FEATUREVECTORADAPTER = new FeatureVectorAdapter<ValueType>();

  /**
   * Use a number vector in the array API.
   */
  private static readonly FeatureVectorAdapter<Double> NUMBERVECTORADAPTER = new FeatureVectorAdapter<Double>();

  /**
   * Use a double array in the array API.
   */
  public static readonly IArrayAdapter DOUBLEARRAYADAPTER = new DoubleArrayAdapter();

  /**
   * Use a float array in the array API.
   */
  public static readonly IArrayAdapter FLOATARRAYADAPTER = new FloatArrayAdapter();

  /**
   * Use a Trove double list as array.
   */
     public static readonly FeatureVectorAdapter<double> TDOUBLELISTADAPTER = new FeatureVectorAdapter<double>();

  /**
   * Use ArrayDBIDs as array.
   */
 // public static readonly ArrayDbIdsAdapter ARRAYDBIDADAPTER = new ArrayDBIDsAdapter();

  /**
   * Cast the static instance.
   * 
   * @param dummy Dummy variable, for type inference
   * @return Static instance
   */

 // public static <T> ArrayAdapter<T, List<? extends T>> listAdapter(List<? extends T> dummy) {
 //   return (ListArrayAdapter<T>) LISTADAPTER;
  //}

  /**
   * Cast the static instance.
   * 
   * @param dummy Dummy variable, for type inference
   * @return Static instance
   */

  //public static <T extends Number> NumberArrayAdapter<T, List<? extends T>> numberListAdapter(List<? extends T> dummy) {
  //  return (NumberListArrayAdapter<T>) NUMBERLISTADAPTER;
  //}

  /**
   * Get the static instance.
   * 
   * @param dummy Dummy object for type inference
   * @return Static instance
   */

  //public static <T> IdentityArrayAdapter<T> identityAdapter(T dummy) {
  //  return (IdentityArrayAdapter<T>) IDENTITYADAPTER;
  //}

  /**
   * Get the static instance.
   * 
   * @param prototype Prototype value, for type inference
   * @return Instance
   */

 // public static <F> FeatureVectorAdapter<F> featureVectorAdapter(FeatureVector<?, F> prototype) {
  //  return (FeatureVectorAdapter<F>) FEATUREVECTORADAPTER;
 // }

  /**
   * Get the static instance.
   * 
   * @param prototype Prototype value, for type inference
   * @return Instance
   */

  //public static <N extends Number> NumberVectorAdapter<N> numberVectorAdapter(NumberVector<?, N> prototype) {
  //  return (NumberVectorAdapter<N>) NUMBERVECTORADAPTER;
  //}

  /**
   * Get the adapter for double arrays.
   * 
   * @return double array adapter
   */
  //public static NumberArrayAdapter<Double, double[]> doubleArrayAdapter() {
  //  return DOUBLEARRAYADAPTER;
  //}

  /**
   * Returns the index of the maximum of the given values. If no value is bigger
   * than the first, the index of the first entry is returned.
   * 
   * @param <A> array type
   * @param array Array to inspect
   * @param adapter API adapter class
   * @return the index of the maximum in the given values
   * @throws IndexOutOfBoundsException if the length of the array is 0.
   */
  //public static <A> int getIndexOfMaximum(A array, NumberArrayAdapter<?, A> adapter)  {
   // readonly int size = adapter.size(array);
  //  int index = 0;
  //  double max = adapter.getDouble(array, 0);
 //   for(int i = 1; i < size; i++) {
 //     double val = adapter.getDouble(array, i);
 //     if(val > max) {
   //     max = val;
 //       index = i;
  //    }
 //   }
 //   return index;
//  }

  /**
   * Returns the index of the maximum of the given values. If no value is bigger
   * than the first, the index of the first entry is returned.
   * 
   * @param array Array to inspect
   * @return the index of the maximum in the given values
   * @throws IndexOutOfBoundsException if the length of the array is 0.
   */
  //public static int getIndexOfMaximum(double[] array)  {
  //  return getIndexOfMaximum(array, DOUBLEARRAYADAPTER);
  //}

  /**
   * Convert a numeric array-like to a <code>double[]</code>
   * 
   * @param array Array-like
   * @param adapter Adapter
   * @return primitive double array
   */
  public static  double[] ToPrimitiveDoubleArray<A>(IList<A> array, IArrayAdapter adapter) {
    double[] ret = new double[adapter.Size(array)];
    for(int i = 0; i < ret.Length; i++) {
      ret[i] =Convert.ToDouble(adapter.Get(array, i));
    }
    return ret;
  }

  /**
   * Convert a list of numbers to <code>double[]</code>.
   * 
   * @param array List of numbers
   * @return double array
   */
  //public static double[] toPrimitiveDoubleArray(List<? extends Number> array) {
  //  return toPrimitiveDoubleArray(array, NUMBERLISTADAPTER);
  //}

  /**
   * Convert a number vector to <code>double[]</code>.
   * 
   * @param obj Object to convert
   * @return primitive double array
   */
  //public static <N extends Number> double[] toPrimitiveDoubleArray(NumberVector<?, N> obj) {
  //  return toPrimitiveDoubleArray(obj, numberVectorAdapter(obj));
  //}

  /**
   * Convert a numeric array-like to a <code>float[]</code>
   * 
   * @param array Array-like
   * @param adapter Adapter
   * @return primitive float array
   */
  //public static <A> float[] toPrimitiveFloatArray(A array, NumberArrayAdapter<?, ? super A> adapter) {
  //  float[] ret = new float[adapter.size(array)];
  //  for(int i = 0; i < ret.length; i++) {
  //    ret[i] = adapter.getFloat(array, i);
  //  }
  //  return ret;
  //}

  /**
   * Convert a list of numbers to <code>float[]</code>.
   * 
   * @param array List of numbers
   * @return float array
   */
  //public static float[] toPrimitiveFloatArray(List<? extends Number> array) {
  //  return toPrimitiveFloatArray(array, NUMBERLISTADAPTER);
  //}

  /**
   * Convert a number vector to <code>float[]</code>.
   * 
   * @param obj Object to convert
   * @return primitive float array
   */
  //public static <N extends Number> float[] toPrimitiveFloatArray(NumberVector<?, N> obj) {
  //  return toPrimitiveFloatArray(obj, numberVectorAdapter(obj));
  //}
}
}
