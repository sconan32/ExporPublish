using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Maths;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;


namespace Socona.Expor.Algorithms.Clustering.KMeans
{

    public class KMedoidsEM
        : AbstractDistanceBasedAlgorithm<IDataVector>, IClusteringAlgorithm
    
    {
        /**
         * The logger for this class.
         */
        private static readonly Logging logger = Logging.GetLogger(typeof(KMedoidsEM));

        /**
         * Holds the value of {@link AbstractKMeans#K_ID}.
         */
        protected int k;

        /**
         * Holds the value of {@link AbstractKMeans#MAXITER_ID}.
         */
        protected int maxiter;

        /**
         * Method to choose initial means.
         */
        protected IKMedoidsInitialization initializer;

        /**
         * Constructor.
         * 
         * @param distanceFunction distance function
         * @param k k parameter
         * @param maxiter Maxiter parameter
         * @param initializer Function to generate the initial means
         */
        public KMedoidsEM(IPrimitiveDistanceFunction<IDataVector> distanceFunction, int k, int maxiter, IKMedoidsInitialization initializer)
            : base(distanceFunction)
        {

            this.k = k;
            this.maxiter = maxiter;
            this.initializer = initializer;
        }

        /**
         * Run k-medoids
         * 
         * @param database Database
         * @param relation relation to use
         * @return result
         */
        public ClusterList Run(IDatabase database, IRelation relation)
        {
            if (relation.Count <= 0)
            {
                return new ClusterList("k-Medoids Clustering", "kmedoids-clustering");
            }
            IDistanceQuery distQ = database.GetDistanceQuery(relation, GetDistanceFunction(), null);
            // Choose initial medoids
            IArrayModifiableDbIds medoids = DbIdUtil.NewArray(initializer.ChooseInitialMedoids(k, distQ));
            // Setup cluster assignment store
            IList<IModifiableDbIds> clusters = new List<IModifiableDbIds>();
            for (int i = 0; i < k; i++)
            {
                clusters.Add(DbIdUtil.NewHashSet(relation.Count / k));
            }
            Mean[] mdists = Mean.NewArray(k);

            // Initial assignment to nearest medoids
            // TODO: reuse this information, from the build phase, when possible?
            assignToNearestCluster(medoids, mdists, clusters, distQ);

            // Swap phase
            bool changed = true;
            while (changed)
            {
                changed = false;
                // Try to swap the medoid with a better cluster member:
                for (int i = 0; i < k; i++)
                {
                    IDbId med = medoids[(i)];
                    IDbId best = null;
                    Mean bestm = mdists[i];
                    var cl = clusters[i];
                    foreach (var dbid in cl)
                    {
                        if (med.IsSameDbId(dbid))
                        {
                            continue;
                        }
                        Mean mdist = new Mean();

                        foreach (var dbid2 in cl)
                        {
                            mdist.Put((distQ.Distance(dbid, dbid2) as DoubleDistanceValue).DoubleValue());
                        }
                        if (mdist.GetMean() < bestm.GetMean())
                        {
                            best = dbid.DbId;
                            bestm = mdist;
                        }
                    }
                    if (best != null && !med.IsSameDbId(best))
                    {
                        changed = true;
                        medoids[i]= best;
                        mdists[i] = bestm;
                    }
                }
                // Reassign
                if (changed)
                {
                    assignToNearestCluster(medoids, mdists, clusters, distQ);
                }
            }

            // Wrap result
            ClusterList result = new ClusterList("k-Medoids Clustering", "kmedoids-clustering");
            for (int i = 0; i < clusters.Count; i++)
            {
                MedoidModel model = new MedoidModel(medoids[i]);
                result.AddCluster(new Cluster(clusters[i], model));
            }
            return result;
        }

        /**
         * Returns a list of clusters. The k<sup>th</sup> cluster contains the ids of
         * those FeatureVectors, that are nearest to the k<sup>th</sup> mean.
         * 
         * @param means a list of k means
         * @param mdist Mean distances
         * @param clusters cluster assignment
         * @param distQ distance query
         * @return true when the object was reassigned
         */
        protected bool assignToNearestCluster(IArrayDbIds means, Mean[] mdist, IList<IModifiableDbIds> clusters, IDistanceQuery distQ)
        {
            bool changed = false;

            double[] dists = new double[k];
            var rel = distQ.Relation;
            foreach (var dbid in rel)
            {
                int minIndex = 0;
                double mindist = Double.PositiveInfinity;
                for (int i = 0; i < k; i++)
                {
                    dists[i] = (distQ.Distance(dbid, means[i]) as DoubleDistanceValue).DoubleValue();
                    if (dists[i] < mindist)
                    {
                        minIndex = i;
                        mindist = dists[i];
                    }
                }
                if (clusters[minIndex].Add(dbid))
                {
                    changed = true;
                    mdist[minIndex].Put(mindist);
                    // Remove from previous cluster
                    // TODO: keep a list of cluster assignments to save this search?
                    for (int i = 0; i < k; i++)
                    {
                        if (i != minIndex)
                        {
                            if (clusters[i].Remove(dbid))
                            {
                                mdist[minIndex].Put(dists[i], -1);
                                break;
                            }
                        }
                    }
                }
            }
            return changed;
        }

        public override ITypeInformation[] GetInputTypeRestriction()
        {
            return TypeUtil.Array(GetDistanceFunction().GetInputTypeRestriction());
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
        public new class Parameterizer : AbstractPrimitiveDistanceBasedAlgorithm<IDataVector>.Parameterizer
        {
            protected int k;

            protected int maxiter;

            protected IKMedoidsInitialization initializer;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                IntParameter kP = new IntParameter(AbstractKMeans.K_ID, new GreaterConstraint<int>(0));
                if (config.Grab(kP))
                {
                    k = kP.GetValue();
                }

                ObjectParameter<IKMedoidsInitialization> initialP =
                    new ObjectParameter<IKMedoidsInitialization>(AbstractKMeans.INIT_ID,
                        typeof(IKMedoidsInitialization), typeof(PAMInitialMeans<IDataVector>));
                if (config.Grab(initialP))
                {
                    initializer = initialP.InstantiateClass(config);
                }

                IntParameter maxiterP = new IntParameter(AbstractKMeans.MAXITER_ID, new GreaterEqualConstraint<int>(0), 0);
                if (config.Grab(maxiterP))
                {
                    maxiter = maxiterP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new KMedoidsEM(distanceFunction, k, maxiter, initializer);
            }

        }






    }
}
