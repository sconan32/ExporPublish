using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.DataSources.Filters;
using Socona.Expor.DataSources.Filters.Normalization;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Results;
using Socona.Expor.Results.TextIO;
using Socona.Log;

namespace Socona.Expor.Data.Models
{

    public class CorrelationAnalysisSolution<V> : ITextWriteable, IResult, IModel
    where V : INumberVector
    {
        /**
         * Stores the solution equations.
         */
        private LinearEquationSystem linearEquationSystem;

        /**
         * Number Format for output accuracy.
         */
        private NumberFormatInfo nf;

        /**
         * The dimensionality of the correlation.
         */
        private int correlationDimensionality;

        /**
         * The standard deviation within this solution.
         */
        private double standardDeviation;

        /**
         * The weak eigenvectors of the hyperplane induced by the correlation.
         */
        private Matrix weakEigenvectors;

        /**
         * The strong eigenvectors of the hyperplane induced by the correlation.
         */
        private Matrix strongEigenvectors;

        /**
         * The similarity matrix of the pca.
         */
        private Matrix similarityMatrix;

        /**
         * The Centroid if the objects belonging to the hyperplane induced by the
         * correlation.
         */
        private Vector Centroid;

        /**
         * Provides a new CorrelationAnalysisSolution holding the specified matrix.
         * <p/>
         * 
         * @param solution the linear equation system describing the solution
         *        equations
         * @param db the database containing the objects
         * @param strongEigenvectors the strong eigenvectors of the hyperplane induced
         *        by the correlation
         * @param weakEigenvectors the weak eigenvectors of the hyperplane induced by
         *        the correlation
         * @param similarityMatrix the similarity matrix of the underlying distance
         *        computations
         * @param Centroid the Centroid if the objects belonging to the hyperplane
         *        induced by the correlation
         */
        public CorrelationAnalysisSolution(LinearEquationSystem solution, IRelation db,
            Matrix strongEigenvectors, Matrix weakEigenvectors, Matrix similarityMatrix, Vector Centroid) :
            this(solution, db, strongEigenvectors, weakEigenvectors, similarityMatrix, Centroid,
            NumberFormatInfo.GetInstance(new CultureInfo("en-US")))
        {
        }

        /**
         * Provides a new CorrelationAnalysisSolution holding the specified matrix and
         * number Format.
         * 
         * @param solution the linear equation system describing the solution
         *        equations
         * @param db the database containing the objects
         * @param strongEigenvectors the strong eigenvectors of the hyperplane induced
         *        by the correlation
         * @param weakEigenvectors the weak eigenvectors of the hyperplane induced by
         *        the correlation
         * @param similarityMatrix the similarity matrix of the underlying distance
         *        computations
         * @param Centroid the Centroid if the objects belonging to the hyperplane
         *        induced by the correlation
         * @param nf the number Format for output accuracy
         */
        public CorrelationAnalysisSolution(LinearEquationSystem solution, IRelation db, Matrix strongEigenvectors,
            Matrix weakEigenvectors, Matrix similarityMatrix, Vector Centroid, NumberFormatInfo nf)
        {
            this.linearEquationSystem = solution;
            this.correlationDimensionality = strongEigenvectors.ColumnCount;
            this.strongEigenvectors = strongEigenvectors;
            this.weakEigenvectors = weakEigenvectors;
            this.similarityMatrix = similarityMatrix;
            this.Centroid = Centroid;
            this.nf = nf;

            // determine standard deviation
            double variance = 0;
            IDbIds ids = db.GetDbIds();
            //for(DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
            foreach (var iter in ids)
            {
                double distance = Distance(((V)db[iter]).GetColumnVector());
                variance += distance * distance;
            }
            standardDeviation = Math.Sqrt(variance / ids.Count);
        }

        /**
         * Returns the linear equation system for printing purposes. If normalization
         * is null the linear equation system is returned, otherwise the linear
         * equation system will be transformed according to the normalization.
         * 
         * @param normalization the normalization, can be null
         * @return the linear equation system for printing purposes
         * @throws NonNumericFeaturesException if the linear equation system is not
         *         compatible with values initialized during normalization
         */
        public LinearEquationSystem GetNormalizedLinearEquationSystem(INormalization<V> normalization)
        {
            if (normalization != null)
            {
                LinearEquationSystem lq = normalization.Transform(linearEquationSystem);
                lq.SolveByTotalPivotSearch();
                return lq;
            }
            else
            {
                return linearEquationSystem;
            }
        }

        /**
         * Return the correlation dimensionality.
         * 
         * @return the correlation dimensionality
         */
        public int GetCorrelationDimensionality()
        {
            return correlationDimensionality;
        }

        /**
         * Returns the distance of NumberVector p from the hyperplane underlying this
         * solution.
         * 
         * @param p a vector in the space underlying this solution
         * @return the distance of p from the hyperplane underlying this solution
         */
        public double Distance(V p)
        {
            return Distance(p.GetColumnVector());
        }

        /**
         * Returns the distance of Matrix p from the hyperplane underlying this
         * solution.
         * 
         * @param p a vector in the space underlying this solution
         * @return the distance of p from the hyperplane underlying this solution
         */
        private double Distance(Vector p)
        {
            // TODO: Is there a particular reason not to do this:
            // return p.minus(Centroid).projection(weakEigenvectors).euclideanNorm(0);
            // V_affin = V + a
            // dist(p, V_affin) = d(p-a, V) = ||p - a - proj_V(p-a) ||
            Vector p_minus_a = p - (Centroid);
            Vector proj = p_minus_a.Projection(strongEigenvectors);
            return (p_minus_a - proj).EuclideanLength();
        }

        /**
         * Returns the error vectors after projection.
         * 
         * @param p a vector in the space underlying this solution
         * @return the error vectors
         */
        public Vector errorVector(V p)
        {
            return p.GetColumnVector().MinusEquals(Centroid).Projection(weakEigenvectors);
        }

        /**
         * Returns the data vectors after projection.
         * 
         * @param p a vector in the space underlying this solution
         * @return the data projections
         */
        public Matrix dataProjections(V p)
        {
            Vector centered = p.GetColumnVector().MinusEquals(Centroid);
            Matrix sum = new Matrix(p.Count, strongEigenvectors.ColumnCount);
            for (int i = 0; i < strongEigenvectors.ColumnCount; i++)
            {
                Vector v_i = strongEigenvectors.Column(i);
                v_i.TimesEquals(centered.TransposeTimes(v_i));
                sum.SetColumn(i, v_i);
            }
            return sum;
        }

        /**
         * Returns the data vectors after projection.
         * 
         * @param p a vector in the space underlying this solution
         * @return the error vectors
         */
        public Vector dataVector(V p)
        {
            return p.GetColumnVector().MinusEquals(Centroid).Projection(strongEigenvectors);
        }

        /**
         * Returns the standard deviation of the distances of the objects belonging to
         * the hyperplane underlying this solution.
         * 
         * @return the standard deviation of this solution
         */
        public double GetStandardDeviation()
        {
            return standardDeviation;
        }

        /**
         * Returns the strong eigenvectors.
         * 
         * @return the strong eigenvectors
         */
        public Matrix GetStrongEigenvectors()
        {
            return strongEigenvectors;
        }

        /**
         * Returns the weak eigenvectors.
         * 
         * @return the weak eigenvectors
         */
        public Matrix GetWeakEigenvectors()
        {
            return weakEigenvectors;
        }

        /**
         * Returns the similarity matrix of the pca.
         * 
         * @return the similarity matrix of the pca
         */
        public Matrix GetSimilarityMatrix()
        {
            return similarityMatrix;
        }

        /**
         * Returns the Centroid of this model.
         * 
         * @return the Centroid of this model
         */
        public Vector GetCentroid()
        {
            return Centroid;
        }

        /**
         * Text output of the equation system
         */

        public void WriteToText(TextWriterStream sout, String label)
        {
            if (label != null)
            {
                sout.CommentPrintLine(label);
            }
            sout.CommentPrintLine("Model class: " + this.GetType().Name);
            try
            {
                if (GetNormalizedLinearEquationSystem(null) != null)
                {
                    // TODO: more elegant way of doing normalization here?
                    /*if(out is TextWriterStreamNormalizing) {
                      TextWriterStreamNormalizing<V> nout = (TextWriterStreamNormalizing<V>) out;
                      LinearEquationSystem lq = GetNormalizedLinearEquationSystem(nout.GetNormalization());
                      out.CommentPrint("Linear Equation System: ");
                      out.CommentPrintLn(lq.equationsToString(nf));
                    } else { */
                    LinearEquationSystem lq = GetNormalizedLinearEquationSystem(null);
                    sout.CommentPrint("Linear Equation System: ");
                    sout.CommentPrintLine(lq.EquationsToString(nf));
                    //}
                }
            }
            catch (NonNumericFeaturesException e)
            {
                Logging.GetLogger(this.GetType()).Error(e);
            }
        }


        public String LongName
        {
            get { return "Correlation Analysis Solution"; }
        }


        public String ShortName
        {
            get { return "correlationanalysissolution"; }
        }
    }
}
