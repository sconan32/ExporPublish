using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Data.Spatial;
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
    [Reference(Authors = "S. Lloyd",
        Title = "Least squares quantization in PCM",
        BookTitle = "IEEE Transactions on Information Theory 28 (2): 129鈥�37.",
        Url = "http://dx.doi.org/10.1109/TIT.1982.1056489")]
    public class KMeansLloyd : AbstractKMeans, IClusteringAlgorithm
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(KMeansLloyd));

        /**
         * Constructor.
         * 
         * @param distanceFunction distance function
         * @param k k parameter
         * @param maxiter Maxiter parameter
         */
        public KMeansLloyd(IPrimitiveDistanceFunction<INumberVector> distanceFunction, int k,
            int maxiter, IKMeansInitialization<INumberVector> initializer) :

            base(distanceFunction, k, maxiter, initializer)
        {
        }

        /**
         * Run k-means
         * 
         * @param database Database
         * @param relation relation to use
         * @return result
         */
        public ClusterList Run(IDatabase database, IRelation relation)
        {
            if (relation.Count <= 0)
            {
                return new ClusterList("k-Means Clustering", "kmeans-clustering");
            }
            // Choose initial means
            IList<INumberVector> means = initializer.ChooseInitialMeans(relation, k, GetDistanceFunction());
            // Setup cluster assignment store
            IList<IModifiableDbIds> clusters = new List<IModifiableDbIds>();
            for (int i = 0; i < k; i++)
            {
                clusters.Add(DbIdUtil.NewHashSet(relation.Count / k));
            }

            for (int iteration = 0; maxiter <= 0 || iteration < maxiter; iteration++)
            {
                if (logger.IsVerbose)
                {
                    logger.Verbose("K-Means iteration " + (iteration + 1));
                }
                bool changed = AssignToNearestCluster(relation, means, clusters);
                // Stop if no cluster assignment changed.
                if (!changed)
                {
                    break;
                }
                // Recompute means.
                means = Means(clusters, means, relation);
            }
            // Wrap result
            INumberVector factory = DatabaseUtil.AssumeVectorField<INumberVector>(relation).GetFactory();
            ClusterList result = new ClusterList("k-Means Clustering", "kmeans-clustering");
            for (int i = 0; i < clusters.Count; i++)
            {
                MeanModel<INumberVector> model = new MeanModel<INumberVector>(factory.NewNumberVector(means[i].GetColumnVector().GetArrayRef()));
                result.AddCluster(new Cluster(clusters[i], model));
            }
            return result;
        }


        protected override Logging GetLogger()
        {
            return logger;
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : AbstractPrimitiveDistanceBasedAlgorithm<INumberVector>.Parameterizer
        {

            protected int maxiter;

            protected IKMeansInitialization<INumberVector> initializer;

            protected int k;

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
                return new KMeansLloyd(distanceFunction, k, maxiter, initializer);
            }

        }


    }
}
