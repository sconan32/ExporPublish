using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Maths.LinearAlgebra.Pca
{
   
public class PCAFilteredResult : PCAResult ,IProjectionResult {
  /**
   * The strong eigenvalues.
   */
  private double[] strongEigenvalues;

  /**
   * The strong eigenvectors to their corresponding filtered eigenvalues.
   */
  private Matrix strongEigenvectors;

  /**
   * The weak eigenvalues.
   */
  private double[] weakEigenvalues;

  /**
   * The weak eigenvectors to their corresponding filtered eigenvalues.
   */
  private Matrix weakEigenvectors;

  /**
   * The amount of Variance explained by strong Eigenvalues
   */
  private double explainedVariance;

  /**
   * The selection matrix of the weak eigenvectors.
   */
  private Matrix e_hat;

  /**
   * The selection matrix of the strong eigenvectors.
   */
  private Matrix e_czech;

  /**
   * The similarity matrix.
   */
  private Matrix m_hat;

  /**
   * The dissimilarity matrix.
   */
  private Matrix m_czech;

  /**
   * The diagonal matrix of adapted strong eigenvalues: eigenvectors * e_czech.
   */
  private Matrix adapatedStrongEigenvectors = null;

  /**
   * Construct a result object for the filtered PCA result.
   * 
   * @param eigenPairs All EigenPairs
   * @param filteredEigenPairs filtered EigenPairs
   * @param big large value in selection matrix
   * @param small small value in selection matrix
   */

  public PCAFilteredResult(SortedEigenPairs eigenPairs, FilteredEigenPairs filteredEigenPairs, double big, double small) :
    base(eigenPairs){

    int dim = eigenPairs.GetEigenPair(0).Eigenvector.Count;

    double sumStrongEigenvalues = 0;
    double sumWeakEigenvalues = 0;
    {// strong eigenpairs
      IList<EigenPair> strongEigenPairs = filteredEigenPairs.GetStrongEigenPairs();
      strongEigenvalues = new double[strongEigenPairs.Count];
      strongEigenvectors = new Matrix(dim, strongEigenPairs.Count);
      int i = 0;
     // for(Iterator<EigenPair> it = strongEigenPairs.iterator(); it.hasNext(); i++) {
        foreach(var it in strongEigenPairs){

            EigenPair eigenPair = it;
        strongEigenvalues[i] = eigenPair.Eigenvalue;
        strongEigenvectors.SetColumn(i, eigenPair.Eigenvector);
        sumStrongEigenvalues += strongEigenvalues[i];
        i++;
      }
    }

    {// weak eigenpairs
      IList<EigenPair> weakEigenPairs = filteredEigenPairs.GetWeakEigenPairs();
      weakEigenvalues = new double[weakEigenPairs.Count];
      weakEigenvectors = new Matrix(dim, weakEigenPairs.Count);
      int i = 0;
     // for(Iterator<EigenPair> it = weakEigenPairs.iterator(); it.hasNext(); i++) {
        foreach(var it in weakEigenPairs){
            EigenPair eigenPair = it;//it.next();
        weakEigenvalues[i] = eigenPair.Eigenvalue;
        weakEigenvectors.SetColumn(i, eigenPair.Eigenvector);
        sumWeakEigenvalues += weakEigenvalues[i];
      }
    }
    explainedVariance = sumStrongEigenvalues / (sumStrongEigenvalues + sumWeakEigenvalues);
    int localdim = strongEigenvalues.Length;

    // selection Matrix for weak and strong EVs
    e_hat = new Matrix(dim, dim);
    e_czech = new Matrix(dim, dim);
    for(int d = 0; d < dim; d++) {
      if(d < localdim) {
        e_czech[d, d]= big;
        e_hat[d, d]= small;
      }
      else {
        e_czech[d, d]= small;
        e_hat[d, d]= big;
      }
    }

    Matrix V = Eigenvectors;
    m_hat = (V*e_hat).TimesTranspose(V);
    m_czech =( V*e_czech).TimesTranspose(V);
  }

  /**
   * Returns the matrix of strong eigenvectors after passing the eigen pair
   * filter.
   * 
   * @return the matrix of eigenvectors
   */
  public  Matrix GetStrongEigenvectors() {
    return strongEigenvectors;
  }

  /**
   * Returns the strong eigenvalues of the object after passing the eigen pair
   * filter.
   * 
   * @return the eigenvalues
   */
  public  double[] GetStrongEigenvalues() {
    return strongEigenvalues;
  }

  /**
   * Returns the matrix of weak eigenvectors after passing the eigen pair
   * filter.
   * 
   * @return the matrix of eigenvectors
   */
  public  Matrix GetWeakEigenvectors() {
    return weakEigenvectors;
  }

  /**
   * Returns the weak eigenvalues of the object after passing the eigen pair
   * filter.
   * 
   * @return the eigenvalues
   */
  public  double[] GetWeakEigenvalues() {
    return weakEigenvalues;
  }

  /**
   * Get correlation (subspace) dimensionality
   * 
   * @return length of strong eigenvalues
   */
  
  public  int GetCorrelationDimension() {
    return strongEigenvalues.Length;
  }

  /**
   * Returns explained variance
   * 
   * @return the variance explained by the strong Eigenvectors
   */
  public double GetExplainedVariance() {
    return explainedVariance;
  }

  /**
   * Returns the selection matrix of the weak eigenvectors (E_hat) of
   * the object to which this PCA belongs to.
   * 
   * @return the selection matrix of the weak eigenvectors E_hat
   */
  public Matrix selectionMatrixOfWeakEigenvectors() {
    return e_hat;
  }

  /**
   * Returns the selection matrix of the strong eigenvectors (E_czech)
   * of this LocalPCA.
   * 
   * @return the selection matrix of the weak eigenvectors E_czech
   */
  public Matrix selectionMatrixOfStrongEigenvectors() {
    return e_czech;
  }

  /**
   * Returns the similarity matrix (M_hat) of this LocalPCA.
   * 
   * @return the similarity matrix M_hat
   */
  
  public  Matrix SimilarityMatrix() {
    return m_hat;
  }

  /**
   * Returns the dissimilarity matrix (M_czech) of this LocalPCA.
   * 
   * @return the dissimilarity matrix M_hat
   */
  public Matrix dissimilarityMatrix() {
    return m_czech;
  }

  /**
   * Returns the adapted strong eigenvectors.
   * 
   * @return the adapted strong eigenvectors
   */
  public Matrix AdapatedStrongEigenvectors() {
    if (adapatedStrongEigenvectors == null) {
       Matrix ev = Eigenvectors;
      adapatedStrongEigenvectors = ev*(e_czech)*(Matrix.Identity(ev.RowCount, strongEigenvalues.Length));
    }
    return adapatedStrongEigenvectors;
  }
}
}
