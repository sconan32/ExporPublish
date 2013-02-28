//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Socona.Clustering.Utilities.DataStructures.ArrayLike
//{
  
///**
// * Adapter for using Trove TDoubleLists as array-like.
// * 
// * @author Erich Schubert
// * 
// * @apiviz.uses TDoubleList
// */
//public class TDoubleListAdapter :IArrayAdapter<Double, TDoubleList> {
//  /**
//   * Constructor.
//   */
//  protected TDoubleListAdapter() {
//    base();
//  }

//  @Override
//  public int size(TDoubleList array) {
//    return array.size();
//  }

//  @Override
//  public Double get(TDoubleList array, int off) throws IndexOutOfBoundsException {
//    return array.get(off);
//  }

//  @Override
//  public double getDouble(TDoubleList array, int off) throws IndexOutOfBoundsException {
//    return array.get(off);
//  }

//  @Override
//  public float getFloat(TDoubleList array, int off) throws IndexOutOfBoundsException {
//    return (float) array.get(off);
//  }

//  @Override
//  public int getInteger(TDoubleList array, int off) throws IndexOutOfBoundsException {
//    return (int) array.get(off);
//  }

//  @Override
//  public short getShort(TDoubleList array, int off) throws IndexOutOfBoundsException {
//    return (short) array.get(off);
//  }

//  @Override
//  public long getLong(TDoubleList array, int off) throws IndexOutOfBoundsException {
//    return (long) array.get(off);
//  }

//  @Override
//  public byte getByte(TDoubleList array, int off) throws IndexOutOfBoundsException {
//    return (byte) array.get(off);
//  }
//}
