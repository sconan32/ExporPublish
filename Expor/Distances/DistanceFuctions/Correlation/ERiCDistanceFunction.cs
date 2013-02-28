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

    public class ERiCDistanceFunction :
        AbstractIndexBasedDistanceFunction<INumberVector>, IFilteredLocalPCABasedDistanceFunction<INumberVector>
    {
        /**
         * Logger for debug.
         */
        static Logging logger = Logging.GetLogger(typeof(PCABasedCorrelationDistanceFunction));

        /**
         * Parameter to specify the threshold for approximate linear dependencyin the
         * strong eigenvectors of q are approximately linear dependent from the strong
         * eigenvectors p if the following condition holds for all strong eigenvectors
         * q_i of q (lambda_q < lambda_p)in q_i' * M^check_p * q_i <= delta^2, must be
         * a double equal to or greater than 0.
         * <p>
         * Default valuein {@code 0.1}
         * </p>
         * <p>
         * Keyin {@code -ericdf.delta}
         * </p>
         */
        public static OptionDescription DELTA_ID = OptionDescription.GetOrCreate(
            "ericdf.delta", "Threshold for approximate linear dependencyin " +
            "the strong eigenvectors of q are approximately linear dependent " +
            "from the strong eigenvectors p if the following condition " +
            "holds for all stroneg eigenvectors q_i of q (lambda_q < lambda_p)in " +
            "q_i' * M^check_p * q_i <= delta^2.");

        /**
         * Parameter to specify the threshold for the maximum distance between two
         * approximately linear dependent subspaces of two objects p and q (lambda_q <
         * lambda_p) before considering them as parallel, must be a double equal to or
         * greater than 0.
         * <p>
         * Default valuein {@code 0.1}
         * </p>
         * <p>
         * Keyin {@code -ericdf.tau}
         * </p>
         */
        public static OptionDescription TAU_ID = OptionDescription.GetOrCreate("ericdf.tau",
            "Threshold for the maximum distance between two approximately linear " +
            "dependent subspaces of two objects p and q " +
            "(lambda_q < lambda_p) before considering them as parallel.");

        /**
         * Holds the value of {@link #DELTA_ID}.
         */
        private double delta;

        /**
         * Holds the value of {@link #TAU_ID}.
         */
        private double tau;

        /**
         * Constructor.
         * 
         * @param indexFactory Index factory.
         * @param delta Delta parameter
         * @param tau Tau parameter
         */
        public ERiCDistanceFunction(IIndexFactory indexFactory, double delta, double tau) :
            base(indexFactory)
        {
            this.delta = delta;
            this.tau = tau;
        }


        public BitDistance GetDistanceFactory()
        {
            return BitDistance.FACTORY;
        }


        public override IDistanceQuery Instantiate(IRelation database)
        {
            // We can't really avoid these warnings, due to a limitation in Java
            // Generics (AFAICT)

            IFilteredLocalPCAIndex indexinst = (IFilteredLocalPCAIndex)indexFactory.Instantiate(
                (IRelation)database);
            return new Instance(database, indexinst, this, delta, tau);
        }

        /**
         * Returns true, if the strong eigenvectors of the two specified pcas span up
         * the same space. Note, that the first pca must have equal ore more strong
         * eigenvectors than the second pca.
         * 
         * @param pca1 first PCA
         * @param pca2 second PCA
         * @return true, if the strong eigenvectors of the two specified pcas span up
         *         the same space
         */
        private bool ApproximatelyLinearDependent(PCAFilteredResult pca1, PCAFilteredResult pca2)
        {
            Matrix m1_czech = pca1.dissimilarityMatrix();
            Matrix v2_strong = pca2.AdapatedStrongEigenvectors();
            for (int i = 0; i < v2_strong.ColumnCount; i++)
            {
                Vector v2_i = v2_strong.Column(i);
                // check, if distance of v2_i to the space of pca_1 > delta
                // (i.e., if v2_i spans up a new dimension)
                double dist = Math.Sqrt(v2_i.TransposeTimes(v2_i) - v2_i.TransposeTimesTimes(m1_czech, v2_i));

                // if so, return false
                if (dist > delta)
                {
                    return false;
                }
            }

            return true;
        }

        /**
         * Computes the distance between two given DatabaseObjects according to this
         * distance function. Note, that the first pca must have equal or more strong
         * eigenvectors than the second pca.
         * 
         * @param v1 first DatabaseObject
         * @param v2 second DatabaseObject
         * @param pca1 first PCA
         * @param pca2 second PCA
         * @return the distance between two given DatabaseObjects according to this
         *         distance function
         */
        public BitDistance distance(INumberVector v1, INumberVector v2, PCAFilteredResult pca1, PCAFilteredResult pca2)
        {
            if (pca1.GetCorrelationDimension() < pca2.GetCorrelationDimension())
            {
                throw new InvalidOperationException("pca1.GetCorrelationDimension() < pca2.GetCorrelationDimension()in " + pca1.GetCorrelationDimension() + " < " + pca2.GetCorrelationDimension());
            }

            bool approximatelyLinearDependent;
            if (pca1.GetCorrelationDimension() == pca2.GetCorrelationDimension())
            {
                approximatelyLinearDependent = ApproximatelyLinearDependent(pca1, pca2) && ApproximatelyLinearDependent(pca2, pca1);
            }
            else
            {
                approximatelyLinearDependent = ApproximatelyLinearDependent(pca1, pca2);
            }

            if (!approximatelyLinearDependent)
            {
                return new BitDistance(true);
            }

            else
            {
                double affineDistance;

                if (pca1.GetCorrelationDimension() == pca2.GetCorrelationDimension())
                {
                    WeightedDistanceFunction df1 = new WeightedDistanceFunction(pca1.SimilarityMatrix());
                    WeightedDistanceFunction df2 = new WeightedDistanceFunction(pca2.SimilarityMatrix());
                    affineDistance = Math.Max(df1.DoubleDistance(v1, v2), df2.DoubleDistance(v1, v2));
                }
                else
                {
                    WeightedDistanceFunction df1 = new WeightedDistanceFunction(pca1.SimilarityMatrix());
                    affineDistance = df1.DoubleDistance(v1, v2);
                }

                if (affineDistance > tau)
                {
                    return new BitDistance(true);
                }

                return new BitDistance(false);
            }
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
            ERiCDistanceFunction other = (ERiCDistanceFunction)obj;
            return (this.delta == other.delta) && (this.tau == other.tau);
        }
        public override int GetHashCode()
        {
            return this.GetHashCode() ^ this.tau.GetHashCode();
        }

        /**
         * The actual instance bound to a particular database.
         * 
         * @author Erich Schubert
         */
        public new class Instance : AbstractIndexBasedDistanceFunction<INumberVector>.Instance,
            IFilteredLocalPCABasedDistanceFunctionInstance
        {
            /**
             * Holds the value of {@link #DELTA_ID}.
             */
            private double delta;

            /**
             * Holds the value of {@link #TAU_ID}.
             */
            private double tau;

            /**
             * Constructor.
             * 
             * @param database Database
             * @param index Index
             * @param parent Parent distance
             * @param delta Delta parameter
             * @param tau Tau parameter
             */
            public Instance(IRelation database, IFilteredLocalPCAIndex index,
                ERiCDistanceFunction parent, double delta, double tau) :
                base(database, index, parent)
            {
                this.delta = delta;
                this.tau = tau;
            }

            /**
             * Note, that the pca of o1 must have equal ore more strong eigenvectors
             * than the pca of o2.
             */

            public override IDistanceValue Distance(IDbIdRef id1, IDbIdRef id2)
            {
                PCAFilteredResult pca1 = (PCAFilteredResult)((IFilteredLocalPCAIndex)index).GetLocalProjection(id1);
                PCAFilteredResult pca2 = (PCAFilteredResult)((IFilteredLocalPCAIndex)index).GetLocalProjection(id2);
                INumberVector v1 = (INumberVector)relation[(id1)];
                INumberVector v2 = (INumberVector)relation[(id2)];
                return (parent as ERiCDistanceFunction).distance(v1, v2, pca1, pca2);
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
            double delta = 0.0;

            double tau = 0.0;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                ConfigIndexFactory(config, typeof(IFilteredLocalPCAIndexFactory),
                    typeof(KNNQueryFilteredPCAIndex.Factory));

                DoubleParameter deltaP = new DoubleParameter(DELTA_ID, new GreaterEqualConstraint<double>(0), 0.1);
                if (config.Grab(deltaP))
                {
                    delta = deltaP.GetValue();
                }

                DoubleParameter tauP = new DoubleParameter(TAU_ID, new GreaterEqualConstraint<double>(0), 0.1);
                if (config.Grab(tauP))
                {
                    tau = tauP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new ERiCDistanceFunction(factory, delta, tau);
            }
        }

   
        public override IDistanceValue DistanceFactory
        {
            get { throw new NotImplementedException(); }
        }
    }
}
