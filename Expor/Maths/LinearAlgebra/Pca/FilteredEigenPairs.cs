using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Maths.LinearAlgebra.Pca
{
    
public class FilteredEigenPairs {
  /**
   * The weak eigenpairs.
   */
  private  IList<EigenPair> weakEigenPairs;

  /**
   * The strong eigenpairs.
   */
  private  IList<EigenPair> strongEigenPairs;

  /**
   * Creates a new object that encapsulates weak and strong eigenpairs
   * that have been filtered out by an eigenpair filter
   *
   * @param weakEigenPairs the weak eigenpairs
   * @param strongEigenPairs the strong eigenpairs
   */
  public FilteredEigenPairs(IList<EigenPair> weakEigenPairs, IList<EigenPair> strongEigenPairs) {
    this.weakEigenPairs = weakEigenPairs;
    this.strongEigenPairs = strongEigenPairs;
  }

  /**
   * Returns the weak eigenpairs (no copy).
   * @return the weak eigenpairs
   */
  public IList<EigenPair> GetWeakEigenPairs() {
    return weakEigenPairs;
  }

  /**
   * Counts the strong eigenpairs.
   * @return number of strong eigenpairs
   */
  public int countWeakEigenPairs() {
    return strongEigenPairs.Count;
  }

  /**
   * Returns the strong eigenpairs (no copy).
   * @return the strong eigenpairs
   */
  public IList<EigenPair> GetStrongEigenPairs() {
    return strongEigenPairs;
  }
  
  /**
   * Counts the strong eigenpairs.
   * @return number of strong eigenpairs
   */
  public int countStrongEigenPairs() {
    return strongEigenPairs.Count;
  }

  /**
   * Returns a string representation of the object.
   *
   * @return a string representation of the object.
   */
  
  public override String ToString() {
    return "weak EP: " + weakEigenPairs + "\nstrong EP: " + strongEigenPairs;
  }
}

}
