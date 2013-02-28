using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Maths.LinearAlgebra.Pca;
using Socona.Expor.Results.TextIO;
using Socona.Expor.Utilities;

namespace Socona.Expor.Data.Models
{

    public class CorrelationModel<V> : BaseModel, ITextWriteable
    {
        /**
         * The computed PCA result of this cluster.
         */
        private PCAFilteredResult pcaresult;

        /**
         * The centroid of this cluster.
         */
        private V centroid;

        /**
         * Constructor
         * 
         * @param pcaresult PCA result
         * @param centroid Centroid
         */
        public CorrelationModel(PCAFilteredResult pcaresult, V centroid) :
            base()
        {
            this.pcaresult = pcaresult;
            this.centroid = centroid;
        }

        /**
         * Get assigned PCA result
         * 
         * @return PCA result
         */
        public PCAFilteredResult GetPCAResult()
        {
            return pcaresult;
        }

        /**
         * Assign new PCA result
         * 
         * @param pcaresult PCA result
         */
        public void SetPCAResult(PCAFilteredResult pcaresult)
        {
            this.pcaresult = pcaresult;
        }

        /**
         * Get assigned for Centroid
         * 
         * @return centroid
         */
        public V GetCentroid()
        {
            return centroid;
        }

        /**
         * Assign new Centroid
         * 
         * @param centroid Centroid
         */
        public void setCentroid(V centroid)
        {
            this.centroid = centroid;
        }

        /**
         * Implementation of {@link TextWriteable} interface
         * 
         * @param label unused parameter
         */

        public override void WriteToText(TextWriterStream sout, String label)
        {
            sout.CommentPrintLine(TextWriterStream.SER_MARKER + " " + typeof(CorrelationModel<V>).Name);
            sout.CommentPrintLine("Centroidin " + sout.NormalizationRestore(GetCentroid()).ToString());
            sout.CommentPrintLine("Strong Eigenvectorsin");
            String strong = GetPCAResult().GetStrongEigenvectors().ToString();
            while (strong.EndsWith("\n"))
            {
                strong = strong.Substring(0, strong.Length - 1);
            }
            sout.CommentPrintLine(strong);
            sout.CommentPrintLine("Weak Eigenvectorsin");
            String weak = GetPCAResult().GetWeakEigenvectors().ToString();
            while (weak.EndsWith("\n"))
            {
                weak = weak.Substring(0, weak.Length - 1);
            }
            sout.CommentPrintLine(weak);
            sout.CommentPrintLine("Eigenvaluesin " + FormatUtil.Format(GetPCAResult().Eigenvalues, " ", 2));
        }
    }

}
