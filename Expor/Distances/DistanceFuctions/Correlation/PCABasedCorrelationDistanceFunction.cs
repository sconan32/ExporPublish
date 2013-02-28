using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Indexes;
using Socona.Expor.Indexes.Preprocessed.LocalPca;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Maths.LinearAlgebra.Pca;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.Distances.DistanceFuctions.Correlation
{

    public class PCABasedCorrelationDistanceFunction : AbstractIndexBasedDistanceFunction<INumberVector>,
        IFilteredLocalPCABasedDistanceFunction<INumberVector>, IFilteredLocalPCAIndex
    {
        /**
         * Logger for debug.
         */
        static Logging logger = Logging.GetLogger(typeof(PCABasedCorrelationDistanceFunction));

        /**
         * Parameter to specify the threshold of a distance between a vector q and a
         * given space that indicates that q Adds a new dimension to the space, must
         * be a double equal to or greater than 0.
         * <p>
         * Default valuein {@code 0.25}
         * </p>
         * <p>
         * Keyin {@code -pcabasedcorrelationdf.delta}
         * </p>
         */
        public static OptionDescription DELTA_ID = OptionDescription.GetOrCreate(
            "pcabasedcorrelationdf.delta",
            "Threshold of a distance between a vector q and a given space that indicates that " +
            "q Adds a new dimension to the space.");

        /**
         * Holds the value of {@link #DELTA_ID}.
         */
        private double delta;

        /**
         * Constructor
         * 
         * @param indexFactory index factory
         * @param delta Delta parameter
         */
        public PCABasedCorrelationDistanceFunction(IIndexFactory indexFactory, double delta) :
            base(indexFactory)
        {
            this.delta = delta;
        }


        public PCACorrelationDistance GetDistanceFactory()
        {
            return PCACorrelationDistance.FACTORY;
        }


        public override IDistanceQuery Instantiate(IRelation database)
        {
            // We can't really avoid these warnings, due to a limitation in Java
            // Generics (AFAICT)

            IFilteredLocalPCAIndex indexinst = (IFilteredLocalPCAIndex)indexFactory.Instantiate((IRelation)database);
            return new Instance(database, indexinst, delta, this);
        }


        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            PCABasedCorrelationDistanceFunction other = (PCABasedCorrelationDistanceFunction)obj;
            return (this.delta == other.delta);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode()^this.delta.GetHashCode();
        }
        /**
         * The actual instance bound to a particular database.
         * 
         * @author Erich Schubert
         */
        public new class Instance :
            AbstractIndexBasedDistanceFunction<INumberVector>.Instance,
            IFilteredLocalPCABasedDistanceFunctionInstance
        {
            /**
             * Delta value
             */
            double delta;

            /**
             * Constructor.
             * 
             * @param database Database
             * @param index Index to use
             * @param delta Delta
             * @param distanceFunction Distance function
             */
            public Instance(IRelation database, IFilteredLocalPCAIndex index,
                double delta, PCABasedCorrelationDistanceFunction distanceFunction) :
                base(database, index, distanceFunction)
            {
                this.delta = delta;
            }


            public override IDistanceValue Distance(IDbIdRef id1, IDbIdRef id2)
            {
                PCAFilteredResult pca1 =
                    (PCAFilteredResult)((IFilteredLocalPCAIndex)index).GetLocalProjection(id1);
                PCAFilteredResult pca2 =
                    (PCAFilteredResult)((IFilteredLocalPCAIndex)index).GetLocalProjection(id2);
                INumberVector dv1 = (INumberVector)relation[(id1)];
                INumberVector dv2 = (INumberVector)relation[(id2)];

                int correlationDistance = CorrelationDistance(pca1, pca2, dv1.Count);
                double euclideanDistance = EuclideanDistance(dv1, dv2);

                return new PCACorrelationDistance(correlationDistance, euclideanDistance);
            }

            /**
             * Computes the correlation distance between the two subspaces defined by
             * the specified PCAs.
             * 
             * @param pca1 first PCA
             * @param pca2 second PCA
             * @param dimensionality the dimensionality of the data space
             * @return the correlation distance between the two subspaces defined by the
             *         specified PCAs
             */
            public int CorrelationDistance(PCAFilteredResult pca1, PCAFilteredResult pca2, int dimensionality)
            {
                // TODO nur in eine Richtung?
                // pca of rv1
                Matrix v1 = pca1.Eigenvectors.Copy();
                Matrix v1_strong = pca1.AdapatedStrongEigenvectors().Copy();
                Matrix e1_czech = pca1.selectionMatrixOfStrongEigenvectors().Copy();
                int lambda1 = pca1.GetCorrelationDimension();

                // pca of rv2
                Matrix v2 = pca2.Eigenvectors.Copy();
                Matrix v2_strong = pca2.AdapatedStrongEigenvectors().Copy();
                Matrix e2_czech = pca2.selectionMatrixOfStrongEigenvectors().Copy();
                int lambda2 = pca2.GetCorrelationDimension();

                // for all strong eigenvectors of rv2
                Matrix m1_czech = pca1.dissimilarityMatrix();
                for (int i = 0; i < v2_strong.ColumnCount; i++)
                {
                    Vector v2_i = v2_strong.Column(i);
                    // check, if distance of v2_i to the space of rv1 > delta
                    // (i.e., if v2_i spans up a new dimension)
                    double dist = Math.Sqrt(v2_i.TransposeTimes(v2_i) - v2_i.TransposeTimesTimes(m1_czech, v2_i));

                    // if so, insert v2_i into v1 and adjust v1
                    // and compute m1_czech new, increase lambda1
                    if (lambda1 < dimensionality && dist > delta)
                    {
                        adjust(v1, e1_czech, v2_i, lambda1++);
                        m1_czech = (v1 * e1_czech).TimesTranspose(v1);
                    }
                }

                // for all strong eigenvectors of rv1
                Matrix m2_czech = pca2.dissimilarityMatrix();
                for (int i = 0; i < v1_strong.ColumnCount; i++)
                {
                    Vector v1_i = v1_strong.Column(i);
                    // check, if distance of v1_i to the space of rv2 > delta
                    // (i.e., if v1_i spans up a new dimension)
                    double dist = Math.Sqrt(v1_i.TransposeTimes(v1_i) -( v1_i.TransposeTimes(m2_czech)*(v1_i))[(0)]);

                    // if so, insert v1_i into v2 and adjust v2
                    // and compute m2_czech new , increase lambda2
                    if (lambda2 < dimensionality && dist > delta)
                    {
                        adjust(v2, e2_czech, v1_i, lambda2++);
                        m2_czech = (v2 * e2_czech).TimesTranspose(v2);
                    }
                }

                int correlationDistance = Math.Max(lambda1, lambda2);

                // TODO delta einbauen
                // Matrix m_1_czech = pca1.dissimilarityMatrix();
                // double dist_1 = normalizedDistance(dv1, dv2, m1_czech);
                // Matrix m_2_czech = pca2.dissimilarityMatrix();
                // double dist_2 = normalizedDistance(dv1, dv2, m2_czech);
                // if (dist_1 > delta || dist_2 > delta) {
                // correlationDistance++;
                // }

                return correlationDistance;
            }

            /**
             * Inserts the specified vector into the given orthonormal matrix
             * <code>v</code> at column <code>corrDim</code>. After insertion the matrix
             * <code>v</code> is orthonormalized and column <code>corrDim</code> of
             * matrix <code>e_czech</code> is set to the <code>corrDim</code>-th unit
             * vector.
             * 
             * @param v the orthonormal matrix of the eigenvectors
             * @param e_czech the selection matrix of the strong eigenvectors
             * @param vector the vector to be inserted
             * @param corrDim the column at which the vector should be inserted
             */
            private void adjust(Matrix v, Matrix e_czech, Vector vector, int corrDim)
            {
                int dim = v.RowCount;

                // set e_czech[corrDim][corrDim] in= 1
                e_czech[corrDim, corrDim] = 1;

                // normalize v
                Vector v_i = (Vector)vector.Clone();
                Vector sum = new Vector(dim);
                for (int k = 0; k < corrDim; k++)
                {
                    Vector v_k = v.Column(k);
                    sum.PlusTimesEquals(v_k, v_i.TransposeTimes(v_k));
                }
                v_i.MinusEquals(sum);
                v_i.Normalize();
                v.SetColumn(corrDim, v_i);
            }

            /**
             * Computes the Euclidean distance between the given two vectors.
             * 
             * @param dv1 first FeatureVector
             * @param dv2 second FeatureVector
             * @return the Euclidean distance between the given two vectors
             */
            private double EuclideanDistance(INumberVector dv1, INumberVector dv2)
            {
                if (dv1.Count != dv2.Count)
                {
                    throw new ArgumentException("Different dimensionality of FeatureVectors\n  first argumentin " + dv1.ToString() + "\n  second argumentin " + dv2.ToString());
                }

                double sqrDist = 0;
                for (int i = 1; i <= dv1.Count; i++)
                {
                    double manhattanI = dv1[(i)] - dv2[(i)];
                    sqrDist += manhattanI * manhattanI;
                }
                return Math.Sqrt(sqrDist);
            }
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer :
            AbstractIndexBasedDistanceFunction<INumberVector>.Parameterizer
        {
            protected double delta = 0.0;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                ConfigIndexFactory(config, typeof(IFilteredLocalPCAIndexFactory),
                    typeof(KNNQueryFilteredPCAIndex.Factory));

                DoubleParameter param = new DoubleParameter(DELTA_ID, new GreaterEqualConstraint<double>(0), 0.25);
                if (config.Grab(param))
                {
                    delta = param.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new PCABasedCorrelationDistanceFunction(factory, delta);
            }
        }

        public override IDistanceValue DistanceFactory
        {
            get { throw new NotImplementedException(); }
        }



        public IProjectionResult GetLocalProjection(IDbIdRef id)
        {
            throw new NotImplementedException();
        }

        public Persistent.IPageFileStatistics GetPageFileStatistics()
        {
            throw new NotImplementedException();
        }

        public void Insert(IDbId id)
        {
            throw new NotImplementedException();
        }

        public void InsertAll(IDbIds ids)
        {
            throw new NotImplementedException();
        }

        public bool Delete(IDbId id)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll(IDbIds ids)
        {
            throw new NotImplementedException();
        }

        public string LongName
        {
            get { throw new NotImplementedException(); }
        }

        public string ShortName
        {
            get { throw new NotImplementedException(); }
        }

     
    }
}
