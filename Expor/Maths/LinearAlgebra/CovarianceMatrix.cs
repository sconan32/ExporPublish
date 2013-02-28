using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Utilities;

namespace Socona.Expor.Maths.LinearAlgebra
{
    public class CovarianceMatrix
    {
        /**
         * The means
         */
        double[] mean;

        /**
         * The covariance matrix
         */
        double[,] elements;

        /**
         * Temporary storage, to avoid reallocations
         */
        double[] nmea;

        /**
         * The current weight
         */
        protected double wsum;

        /**
         * Constructor.
         * 
         * @param dim Dimensionality
         */
        public CovarianceMatrix(int dim)
        {

            this.mean = new double[dim];
            this.nmea = new double[dim];
            this.elements = new double[dim, dim];
            this.wsum = 0.0;
        }

        /**
         * Get the weight sum, to test whether the covariance matrix can be
         * materialized.
         * 
         * @return Weight sum.
         */
        public double GetWeight()
        {
            return wsum;
        }

        /**
         * Add a single value with weight 1.0
         * 
         * @param val Value
         */
        public void Put(double[] val)
        {
            Debug.Assert(val.Length == mean.Length);
            double nwsum = wsum + 1.0;
            // Compute new means
            for (int i = 0; i < mean.Length; i++)
            {
                double delta = val[i] - mean[i];
                nmea[i] = mean[i] + delta / nwsum;
            }
            // Update covariance matrix
            for (int i = 0; i < mean.Length; i++)
            {
                for (int j = i; j < mean.Length; j++)
                {
                    // We DO want to use the new mean once and the old mean once!
                    // It does not matter which one is which.
                    double delta = (val[i] - nmea[i]) * (val[j] - mean[j]);
                    elements[i, j] = elements[i, j] + delta;
                    // Optimize via symmetry
                    if (i != j)
                    {
                        elements[j, i] = elements[j, i] + delta;
                    }
                }
            }

            // Use new values.
            wsum = nwsum;
            Array.Copy(nmea, mean, nmea.Length);
            //System.arraycopy(nmea, 0, mean, 0, nmea.Length);
        }

        /**
         * Add data with a given weight.
         * 
         * @param val data
         * @param weight weight
         */
        public void Put(double[] val, double weight)
        {
            Debug.Assert(val.Length == mean.Length);
            double nwsum = wsum + weight;
            // Compute new means
            for (int i = 0; i < mean.Length; i++)
            {
                double delta = val[i] - mean[i];
                double rval = delta * weight / nwsum;
                nmea[i] = mean[i] + rval;
            }
            // Update covariance matrix
            for (int i = 0; i < mean.Length; i++)
            {
                for (int j = i; j < mean.Length; j++)
                {
                    // We DO want to use the new mean once and the old mean once!
                    // It does not matter which one is which.
                    double delta = (val[i] - nmea[i]) * (val[j] - mean[j]) * weight;
                    elements[i, j] = elements[i, j] + delta;
                    // Optimize via symmetry
                    if (i != j)
                    {
                        elements[j, i] = elements[j, i] + delta;
                    }
                }
            }

            // Use new values.
            wsum = nwsum;
            Array.Copy(nmea, mean, nmea.Length);
            //System.arraycopy(nmea, 0, mean, 0, nmea.Length);
        }

        /**
         * Add a single value with weight 1.0
         * 
         * @param val Value
         */
        public void Put(Vector val)
        {
            Put(val.GetArrayRef());
        }

        /**
         * Add data with a given weight.
         * 
         * @param val data
         * @param weight weight
         */
        public void Put(Vector val, double weight)
        {
            Put(val.GetArrayRef(), weight);
        }

        /**
         * Add a single value with weight 1.0
         * 
         * @param val Value
         */
        public void Put(INumberVector val)
        {
            Debug.Assert(val.Count == mean.Length);
            double nwsum = wsum + 1.0;
            // Compute new means
            for (int i = 0; i < mean.Length; i++)
            {
                double delta = val[i + 1] - mean[i];
                nmea[i] = mean[i] + delta / nwsum;
            }
            // Update covariance matrix
            for (int i = 0; i < mean.Length; i++)
            {
                for (int j = i; j < mean.Length; j++)
                {
                    // We DO want to use the new mean once and the old mean once!
                    // It does not matter which one is which.
                    double delta = (val[i + 1] - nmea[i]) * (val[j + 1] - mean[j]);
                    elements[i, j] = elements[i, j] + delta;
                    // Optimize via symmetry
                    if (i != j)
                    {
                        elements[j, i] = elements[j, i] + delta;
                    }
                }
            }
            // Use new values.
            wsum = nwsum;
            Array.Copy(nmea, mean, nmea.Length);
            //System.arraycopy(nmea, 0, mean, 0, nmea.Length);
        }

        /**
         * Add data with a given weight.
         * 
         * @param val data
         * @param weight weight
         */
        public void Put(INumberVector val, double weight)
        {
            Debug.Assert(val.Count == mean.Length);
            double nwsum = wsum + weight;
            // Compute new means
            for (int i = 0; i < mean.Length; i++)
            {
                double delta = val[i + 1] - mean[i];
                double rval = delta * weight / nwsum;
                nmea[i] = mean[i] + rval;
            }
            // Update covariance matrix
            for (int i = 0; i < mean.Length; i++)
            {
                for (int j = i; j < mean.Length; j++)
                {
                    // We DO want to use the new mean once and the old mean once!
                    // It does not matter which one is which.
                    double delta = (val[i + 1] - nmea[i]) * (val[j + 1] - mean[j]) * weight;
                    elements[i, j] = elements[i, j] + delta;
                    // Optimize via symmetry
                    if (i != j)
                    {
                        elements[j, i] = elements[j, i] + delta;
                    }
                }
            }
            // Use new values.
            wsum = nwsum;
            Array.Copy(nmea, mean, nmea.Length);
            //System.arraycopy(nmea, 0, mean, 0, nmea.Length);
        }

        /**
         * Get the mean as vector.
         * 
         * @return Mean vector
         */
        public Vector GetMeanVector()
        {
            return new Vector(mean);
        }

        /**
         * Get the mean as vector.
         * 
         * @return Mean vector
         */
        public F GetMeanVector<F>(IRelation relation) where F : INumberVector
        {
            return (F)DatabaseUtil.AssumeVectorField<F>(relation).GetFactory().NewNumberVector(mean);
        }

        /**
         * Obtain the covariance matrix according to the sample statistics: (n-1)
         * degrees of freedom.
         * 
         * This method duplicates the matrix contents, so it does allow further
         * updates. Use {@link #destroyToSampleMatrix()} if you do not need further
         * updates.
         * 
         * @return New matrix
         */
        public Matrix MakeSampleMatrix()
        {
            if (wsum <= 1.0)
            {
                throw new InvalidOperationException("Too few elements used to obtain a valid covariance matrix.");
            }
            Matrix mat = new Matrix(elements);
            return mat * (1.0 / (wsum - 1));
        }

        /**
         * Obtain the covariance matrix according to the population statistics: n
         * degrees of freedom.
         * 
         * This method duplicates the matrix contents, so it does allow further
         * updates. Use {@link #destroyToNaiveMatrix()} if you do not need further
         * updates.
         * 
         * @return New matrix
         */
        public Matrix MakeNaiveMatrix()
        {
            if (wsum <= 0.0)
            {
                throw new InvalidOperationException("Too few elements used to obtain a valid covariance matrix.");
            }
            Matrix mat = new Matrix(elements);
            return mat * (1.0 / wsum);
        }

        /**
         * Obtain the covariance matrix according to the sample statistics: (n-1)
         * degrees of freedom.
         * 
         * This method doesn't require matrix duplication, but will not allow further
         * updates, the object should be discarded. Use {@link #makeSampleMatrix()} if
         * you want to perform further updates.
         * 
         * @return New matrix
         */
        public Matrix DestroyToSampleMatrix()
        {
            if (wsum <= 1.0)
            {
                throw new InvalidOperationException("Too few elements used to obtain a valid covariance matrix.");
            }
            Matrix mat = new Matrix(elements) * (1.0 / (wsum - 1));
            this.elements = null;
            return mat;
        }

        /**
         * Obtain the covariance matrix according to the population statistics: n
         * degrees of freedom.
         * 
         * This method doesn't require matrix duplication, but will not allow further
         * updates, the object should be discarded. Use {@link #makeNaiveMatrix()} if
         * you want to perform further updates.
         * 
         * @return New matrix
         */
        public Matrix DestroyToNaiveMatrix()
        {
            if (wsum <= 0.0)
            {
                throw new InvalidOperationException("Too few elements used to obtain a valid covariance matrix.");
            }
            Matrix mat = new Matrix(elements) * (1.0 / wsum);
            this.elements = null;
            return mat;
        }

        /**
         * Static Constructor.
         * 
         * @param mat Matrix to use the columns of
         */
        public static CovarianceMatrix Make(Matrix mat)
        {
            CovarianceMatrix c = new CovarianceMatrix(mat.RowCount);
            int n = mat.ColumnCount;
            for (int i = 0; i < n; i++)
            {
                // TODO: avoid constructing the vector objects?
                c.Put(mat.Column(i));
            }
            return c;
        }

        /**
         * Static Constructor from a full relation.
         * 
         * @param relation Relation to use.
         */
        public static CovarianceMatrix Make(IRelation relation) 
        {
            CovarianceMatrix c = new CovarianceMatrix(DatabaseUtil.Dimensionality(relation));
            foreach (IDbId id in relation.GetDbIds())
            {
                //for(DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance()) {
                c.Put((double[])relation[id]);
            }
            return c;
        }

        /**
         * Static Constructor from a full relation.
         * 
         * @param relation Relation to use.
         * @param ids IDs to add
         */
        public static CovarianceMatrix Make(IRelation relation, IDbIds ids)
        {
            CovarianceMatrix c = new CovarianceMatrix(DatabaseUtil.Dimensionality(relation));
            foreach (IDbId id in ids)
            {
                //for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
                c.Put(((INumberVector)relation[id]).GetColumnVector().ToArray());
            }
            return c;
        }
    }
}
