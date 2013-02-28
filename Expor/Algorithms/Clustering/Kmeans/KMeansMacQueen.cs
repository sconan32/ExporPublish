using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Algorithms.Clustering.KMeans;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.Algorithms.Clustering.KMeans
{

    [Title("K-Means")]
    [Description("Finds a partitioning into k clusters.")]
    [Reference(Authors = "J. MacQueen", Title = "Some Methods for Classification and Analysis of Multivariate Observations", BookTitle = "5th Berkeley Symp. Math. Statist. Prob., Vol. 1, 1967, pp 281-297", Url = "http://projecteuclid.org/euclid.bsmsp/1200512992")]
    public class KMeansMacQueen : AbstractKMeans, IClusteringAlgorithm
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(KMeansMacQueen));

        /**
         * Constructor.
         * 
         * @param distanceFunction distance function
         * @param k k parameter
         * @param maxiter Maxiter parameter
         */
        public KMeansMacQueen(IPrimitiveDistanceFunction<INumberVector> distanceFunction, int k, int maxiter,
            IKMeansInitialization<INumberVector> initializer) :
            base(distanceFunction, k, maxiter, initializer)
        {
        }

        /**
         * Run k-means
         * 
         * @param database Database
         * @param relation relation to use
         * @return Clustering result
         */
        public ClusterList Run(IDatabase database, IRelation relation)
        {
            if (relation.Count <= 0)
            {
                return new ClusterList("k-Means Clustering", "kmeans-clustering");
            }
            // Choose initial means
            IList<INumberVector> means = new List<INumberVector>(k);
            foreach (INumberVector nv in initializer.ChooseInitialMeans(relation, k, GetDistanceFunction()))
            {
                means.Add(nv.GetColumnVector());
            }
            // Initialize cluster and assign objects
            List<IModifiableDbIds> clusters = new List<IModifiableDbIds>();
            for (int i = 0; i < k; i++)
            {
                clusters.Add(DbIdUtil.NewHashSet(relation.Count / k));
            }
            AssignToNearestCluster(relation, means, clusters);
            // Initial recomputation of the means.
            means = Means(clusters, means, relation);

            // Refine result
            for (int iteration = 0; maxiter <= 0 || iteration < maxiter; iteration++)
            {
                if (logger.IsVerbose)
                {
                    logger.Verbose("K-Means iteration " + (iteration + 1));
                }
                bool changed = MacQueenIterate(relation, means, clusters);
                if (!changed)
                {
                    break;
                }
            }
            INumberVector factory = DatabaseUtil.AssumeVectorField<INumberVector>(relation).GetFactory();
            ClusterList result = new ClusterList("k-Means Clustering", "kmeans-clustering");
            for (int i = 0; i < clusters.Count; i++)
            {
                IDbIds ids = clusters[(i)];
                MeanModel<INumberVector> model = new MeanModel<INumberVector>(
                    factory.NewNumberVector(means[(i)].GetColumnVector().GetArrayRef()));
                result.AddCluster(new Cluster(ids, model));
            }
            return result;
        }


        protected override Logging GetLogger()
        {
            return logger;
        }

        /**
         * IParameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : AbstractPrimitiveDistanceBasedAlgorithm<INumberVector>.Parameterizer
        {
            protected int k;

            protected int maxiter;

            protected IKMeansInitialization<INumberVector> initializer;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                IntParameter kP = new IntParameter(K_ID, new GreaterConstraint<int>(0));
                if (config.Grab(kP))
                {
                    k = kP.GetValue();
                }

                ObjectParameter<IKMeansInitialization<INumberVector>> initialP =
                    new ObjectParameter<IKMeansInitialization<INumberVector>>(INIT_ID,
                    typeof(IKMeansInitialization<INumberVector>),
                    typeof(RandomlyGeneratedInitialMeans<INumberVector>));
                if (config.Grab(initialP))
                {
                    initializer = initialP.InstantiateClass(config);
                }

                IntParameter maxiterP = new IntParameter(MAXITER_ID, new GreaterEqualConstraint<int>(0), 0);
                if (config.Grab(maxiterP))
                {
                    maxiter = maxiterP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new KMeansMacQueen(distanceFunction, k, maxiter, initializer);
            }
        }
    }
}
