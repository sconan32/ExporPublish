using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using Socona.Expor.Results.TextIO;

namespace Socona.Expor.Data.Models
{

    public class EMModel<V> : MeanModel<V> where V : IDataVector
    {
        /**
         * Cluster covariance matrix
         */
        private Matrix covarianceMatrix;

        /**
         * Constructor.
         * 
         * @param mean Mean vector
         * @param covarianceMatrix Covariance matrix
         */
        public EMModel(V mean, Matrix covarianceMatrix)
            : base(mean)
        {

            this.covarianceMatrix = covarianceMatrix;
        }


        public override void WriteToText(TextWriterStream sout, String label)
        {
            base.WriteToText(sout, label);
            sout.CommentPrintLine("Mean: " + sout.NormalizationRestore(this.GetMean()).ToString());
            sout.CommentPrintLine("Covariance Matrix: " + this.covarianceMatrix.ToString());
        }

        /**
         * @return covariance matrix
         */
        public Matrix GetCovarianceMatrix()
        {
            return covarianceMatrix;
        }

        /**
         * @param covarianceMatrix covariance matrix
         */
        public void SetCovarianceMatrix(Matrix covarianceMatrix)
        {
            this.covarianceMatrix = covarianceMatrix;
        }
    }

}
