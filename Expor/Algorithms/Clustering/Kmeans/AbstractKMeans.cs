using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Utilities.DataStructures;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Algorithms.Clustering.KMeans
{

    public abstract class AbstractKMeans : AbstractPrimitiveDistanceBasedAlgorithm<INumberVector>, IKMeans
    {
        /**
* Parameter to specify the initialization method
*/
        public static readonly OptionDescription INIT_ID = OptionDescription.GetOrCreate("kmeans.initialization", "Method to choose the initial means.");

        /**
         * Parameter to specify the number of clusters to find, must be an integer
         * greater than 0.
         */
        public static readonly OptionDescription K_ID = OptionDescription.GetOrCreate("kmeans.k", "The number of clusters to find.");

        /**
         * Parameter to specify the number of clusters to find, must be an integer
         * greater or equal to 0, where 0 means no limit.
         */
        public static readonly OptionDescription MAXITER_ID = OptionDescription.GetOrCreate("kmeans.maxiter", "The maximum number of iterations to do. 0 means no limit.");

        /**
         * Parameter to specify the random generator seed.
         */
        public static readonly OptionDescription SEED_ID = OptionDescription.GetOrCreate("Kmeans.seed", "The random number generator seed.");
        /**
         * Holds the value of {@link #K_ID}.
         */
        protected int k;

        /**
         * Holds the value of {@link #MAXITER_ID}.
         */
        protected int maxiter;

        /**
         * Method to choose initial means.
         */
        protected IKMeansInitialization<INumberVector> initializer;

        /**
         * Constructor.
         * 
         * @param distanceFunction distance function
         * @param k k parameter
         * @param maxiter Maxiter parameter
         * @param initializer Function to generate the initial means
         */
        public AbstractKMeans(IPrimitiveDistanceFunction<INumberVector> distanceFunction, int k, int maxiter,
            IKMeansInitialization<INumberVector> initializer) :
            base(distanceFunction)
        {
            this.k = k;
            this.maxiter = maxiter;
            this.initializer = initializer;
        }

        /**
         * Returns a list of clusters. The k<sup>th</sup> cluster contains the ids of
         * those FeatureVectors, that are nearest to the k<sup>th</sup> mean.
         * 
         * @param relation the database to cluster
         * @param means a list of k means
         * @param clusters cluster assignment
         * @return true when the object was reassigned
         */
        protected bool AssignToNearestCluster(IRelation relation, IList<INumberVector> means, IList<IModifiableDbIds> clusters)
        {
            bool changed = false;

            if (GetDistanceFunction() is IPrimitiveDoubleDistanceFunction<INumberVector>)
            {

                IPrimitiveDoubleDistanceFunction<INumberVector> df = (IPrimitiveDoubleDistanceFunction<INumberVector>)GetDistanceFunction();
                //  for(DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance()) {
                for (int ii = 0; ii < relation.GetDbIds().Count; ii++)
                {
                    var iditer = relation.GetDbIds().ElementAt(ii);
                    double mindist = Double.PositiveInfinity;
                    INumberVector fv = (INumberVector)relation[iditer];
                    int minIndex = 0;
                    for (int i = 0; i < k; i++)
                    {
                        double dist = df.DoubleDistance(fv, means[i]);
                        if (dist < mindist)
                        {
                            minIndex = i;
                            mindist = dist;
                        }
                    }
                    if (clusters[(minIndex)].Add(iditer))
                    {
                        changed = true;
                        // Remove from previous cluster
                        // TODO: keep a list of cluster assignments to save this search?
                        for (int i = 0; i < k; i++)
                        {
                            if (i != minIndex)
                            {
                                if (clusters[i].Remove(iditer))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                IPrimitiveDistanceFunction<INumberVector> df = GetDistanceFunction();
                //for (DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance())
                for (int ii = 0; ii < relation.GetDbIds().Count; ii++)
                {
                    var iditer = relation.GetDbIds().ElementAt(ii);
                    IDistanceValue mindist = df.DistanceFactory.Infinity;
                    INumberVector fv = (INumberVector)relation[(iditer)];
                    int minIndex = 0;
                    for (int i = 0; i < k; i++)
                    {
                        IDistanceValue dist = df.Distance(fv, means[i]);
                        if (dist.CompareTo(mindist) < 0)
                        {
                            minIndex = i;
                            mindist = dist;
                        }
                    }
                    if (clusters[(minIndex)].Add(iditer))
                    {
                        changed = true;
                        // Remove from previous cluster
                        // TODO: keep a list of cluster assignments to save this search?
                        for (int i = 0; i < k; i++)
                        {
                            if (i != minIndex)
                            {
                                if (clusters[i].Remove(iditer))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return changed;
        }


        public override ITypeInformation[] GetInputTypeRestriction()
        {
            return TypeUtil.Array(TypeUtil.NUMBER_VECTOR_FIELD);
        }

        /**
         * Returns the mean vectors of the given clusters in the given database.
         * 
         * @param clusters the clusters to compute the means
         * @param means the recent means
         * @param database the database containing the vectors
         * @return the mean vectors of the given clusters in the given database
         */
        protected IList<INumberVector> Means(IList<IModifiableDbIds> clusters, IList<INumberVector> means, IRelation database)
        {
            IList<INumberVector> newMeans = new List<INumberVector>(k);
            for (int i = 0; i < k; i++)
            {
                IModifiableDbIds list = clusters[i];
                Vector mean = null;
                if (list.Count > 0)
                {
                    double s = 1.0 / list.Count;
                    //DbIdIter iter = list.iter();
                    // Debug.Assert(iter.valid());
                    int ii = 0;
                    IDbId iter = list.ElementAt(ii);

                    mean = ((INumberVector)database[(iter)]).GetColumnVector().TimesEquals(s);

                    for (ii++; ii < list.Count; ii++)
                    {
                        mean.PlusTimesEquals(((INumberVector)database[(iter)]).GetColumnVector(), s);
                    }
                }
                else
                {
                    mean = means[i].GetColumnVector();
                }
                newMeans.Add(mean);
            }
            return newMeans;
        }

        /**
         * Returns the median vectors of the given clusters in the given database.
         * 
         * @param clusters the clusters to compute the means
         * @param medians the recent medians
         * @param database the database containing the vectors
         * @return the mean vectors of the given clusters in the given database
         */
        protected List<INumberVector> Medians(IList<IDbIds> clusters, IList<INumberVector> medians, IRelation database)
        {
            int dim = medians[(0)].Count;
            VectorUtil.SortDbIdsBySingleDimension sorter = new VectorUtil.SortDbIdsBySingleDimension(database, 0);
            List<INumberVector> newMedians = new List<INumberVector>(k);
            for (int i = 0; i < k; i++)
            {
                IArrayModifiableDbIds list = DbIdUtil.NewArray(clusters[i]);
                if (list.Count > 0)
                {
                    Vector mean = new Vector(dim);
                    for (int d = 0; d < dim; d++)
                    {
                        sorter.Dimension = (d + 1);
                        IDbId id = QuickSelectUtil.Median(list, sorter);
                        mean[d] = ((INumberVector)database[id])[d + 1];
                    }
                    newMedians.Add(mean);
                }
                else
                {
                    newMedians.Add((INumberVector)medians[i]);
                }
            }
            return newMedians;
        }

        /**
         * Compute an incremental update for the mean
         * 
         * @param mean Mean to update
         * @param vec Object vector
         * @param ne.Count (New).Count of cluster
         * @param op Cluster.Count change / Weight change
         */
        protected void IncrementalUpdateMean(INumberVector mean, INumberVector vec, int newsize, double op)
        {
            if (newsize == 0)
            {
                return; // Keep old mean
            }
            Vector delta = vec.GetColumnVector();
            // Compute difference from mean
            delta.MinusEquals(mean.GetColumnVector());
            delta.TimesEquals(op / newsize);
            mean = mean.GetColumnVector().PlusEquals(delta);
        }

        /**
         * Perform a MacQueen style iteration.
         * 
         * @param relation Relation
         * @param means Means
         * @param clusters Clusters
         * @return true when the means have changed
         */
        protected bool MacQueenIterate(IRelation relation, IList<INumberVector> means, IList<IModifiableDbIds> clusters)
        {
            bool changed = false;

            if (GetDistanceFunction() is IPrimitiveDoubleDistanceFunction<INumberVector>)
            {
                // Raw distance function

                IPrimitiveDoubleDistanceFunction<INumberVector> df = (IPrimitiveDoubleDistanceFunction<INumberVector>)GetDistanceFunction();

                // Incremental update
                // for (DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance())
                foreach (var iditer in relation.GetDbIds())
                {
                    double mindist = Double.PositiveInfinity;
                    INumberVector fv = (INumberVector)relation[(iditer)];
                    int minIndex = 0;
                    for (int i = 0; i < k; i++)
                    {
                        double dist = df.DoubleDistance(fv, means[i]);
                        if (dist < mindist)
                        {
                            minIndex = i;
                            mindist = dist;
                        }
                    }
                    // Update the cluster mean incrementally:
                    for (int i = 0; i < k; i++)
                    {
                        IModifiableDbIds ci = clusters[i];
                        if (i == minIndex)
                        {
                            if (ci.Add(iditer))
                            {
                                IncrementalUpdateMean(means[i], fv, ci.Count, +1);
                                changed = true;
                            }
                        }
                        else if (ci.Remove(iditer))
                        {
                            IncrementalUpdateMean(means[i], fv, ci.Count + 1, -1);
                            changed = true;
                        }
                    }
                }
            }
            else
            {
                // Raw distance function
                IPrimitiveDistanceFunction<INumberVector> df = GetDistanceFunction();

                // Incremental update
                // for (DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance())
                foreach (var iditer in relation.GetDbIds())
                {
                    IDistanceValue mindist = df.DistanceFactory.Infinity;
                    INumberVector fv = (INumberVector)relation[(iditer)];
                    int minIndex = 0;
                    for (int i = 0; i < k; i++)
                    {
                        IDistanceValue dist = df.Distance(fv, (INumberVector)means[i]);
                        if (dist.CompareTo(mindist) < 0)
                        {
                            minIndex = i;
                            mindist = dist;
                        }
                    }
                    // Update the cluster mean incrementally:
                    for (int i = 0; i < k; i++)
                    {
                        IModifiableDbIds ci = clusters[i];
                        if (i == minIndex)
                        {
                            if (ci.Add(iditer))
                            {
                                IncrementalUpdateMean(means[i], fv, ci.Count, +1);
                                changed = true;
                            }
                        }
                        else if (ci.Remove(iditer))
                        {
                            IncrementalUpdateMean(means[i], fv, ci.Count + 1, -1);
                            changed = true;
                        }
                    }
                }
            }
            return changed;
        }
    }
}
