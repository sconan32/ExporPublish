using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Algorithms.Clustering.Subspace.CLIQUEs;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Expor.Utilities.Pairs;
using Socona.Log;

namespace Socona.Expor.Algorithms.Clustering.Subspace
{
    [Title("CLIQUE: Automatic Subspace Clustering of High Dimensional Data for Data Mining Applications")]
    [Description("Grid-based algorithm to identify dense clusters in subspaces of maximum dimensionality.")]
    [Reference(Authors = "R. Agrawal, J. Gehrke, D. Gunopulos, P. Raghavan",
        Title = "Automatic Subspace Clustering of High Dimensional Data for Data Mining Applications",
        BookTitle = "Proc. SIGMOD Conference, Seattle, WA, 1998",
        Url = "http://dx.doi.org/10.1145/276304.276314")]
    public class CLIQUE : AbstractAlgorithm, ISubspaceClusteringAlgorithm
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(CLIQUE));

        /**
         * Parameter to specify the number of intervals (units) in each dimension,
         * must be an integer greater than 0.
         * <p>
         * Key: {@code -clique.xsi}
         * </p>
         */
        public static OptionDescription XSI_ID = OptionDescription.GetOrCreate("clique.xsi", 
            "The number of intervals (units) in each dimension.");
       
        /**
         * Parameter to specify the density threshold for the selectivity of a unit,
         * where the selectivity is the fraction of total feature vectors contained in
         * this unit, must be a double greater than 0 and less than 1.
         * <p>
         * Key: {@code -clique.tau}
         * </p>
         */
        public static OptionDescription TAU_ID = OptionDescription.GetOrCreate("clique.tau", 
            "The density threshold for the selectivity of a unit, where the selectivity is" + 
            "the fraction of total feature vectors contained in this unit.");

        /**
         * Flag to indicate that only subspaces with large coverage (i.e. the fraction
         * of the database that is covered by the dense units) are selected, the rest
         * will be pruned.
         * <p>
         * Key: {@code -clique.prune}
         * </p>
         */
        public static OptionDescription PRUNE_ID = OptionDescription.GetOrCreate("clique.prune", 
            "Flag to indicate that only subspaces with large coverage " + "(i.e. the fraction of the database that is covered by the dense units) " + "are selected, the rest will be pruned.");

        /**
         * Holds the value of {@link #XSI_ID}.
         */
        private int xsi;

        /**
         * Holds the value of {@link #TAU_ID}.
         */
        private double tau;

        /**
         * Holds the value of {@link #PRUNE_ID}.
         */
        private bool prune;

        /**
         * Constructor.
         * 
         * @param xsi Xsi value
         * @param tau Tau value
         * @param prune Prune flag
         */
        public CLIQUE(int xsi, double tau, bool prune) :
            base()
        {
            this.xsi = xsi;
            this.tau = tau;
            this.prune = prune;
        }

        /**
         * Performs the CLIQUE algorithm on the given database.
         * 
         * @param relation Data relation to process
         * @return Clustering result
         */
        public ClusterList Run(IRelation relation)
        {
            // 1. Identification of subspaces that contain clusters
            // TODO: use step logging.
            if (logger.IsVerbose)
            {
                logger.Verbose("*** 1. Identification of subspaces that contain clusters ***");
            }
            IDictionary<int, List<CLIQUESubspace<INumberVector>>> dimensionToDenseSubspaces = new SortedDictionary<int, List<CLIQUESubspace<INumberVector>>>();
            List<CLIQUESubspace<INumberVector>> denseSubspaces = FindOneDimensionalDenseSubspaces(relation);
            dimensionToDenseSubspaces[0] = denseSubspaces;
            if (logger.IsVerbose)
            {
                logger.Verbose("    1-dimensional dense subspaces: " + denseSubspaces.Count);
            }
            if (logger.IsDebugging)
            {
                foreach (CLIQUESubspace<INumberVector> s in denseSubspaces)
                {
                    logger.Debug(s.ToString("      "));
                }
            }

            int dimensionality = DatabaseUtil.Dimensionality(relation);
            for (int k = 2; k <= dimensionality && denseSubspaces.Count > 0; k++)
            {
                denseSubspaces = findDenseSubspaces(relation, denseSubspaces);
                dimensionToDenseSubspaces[k - 1] = denseSubspaces;
                if (logger.IsVerbose)
                {
                    logger.Verbose("    " + k + "-dimensional dense subspaces: " + denseSubspaces.Count);
                }
                if (logger.IsDebugging)
                {
                    foreach (CLIQUESubspace<INumberVector> s in denseSubspaces)
                    {
                        logger.Debug(s.ToString("      "));
                    }
                }
            }

            // 2. Identification of clusters
            if (logger.IsVerbose)
            {
                logger.Verbose("*** 2. Identification of clusters ***");
            }
            // build result
            int numClusters = 1;
            ClusterList result = new ClusterList("CLIQUE clustering", "clique-clustering");
            foreach (int dim in dimensionToDenseSubspaces.Keys)
            {
                List<CLIQUESubspace<INumberVector>> subspaces = dimensionToDenseSubspaces[(dim)];
                IList<IPair<Subspace<INumberVector>, IModifiableDbIds>> modelsAndClusters = DetermineClusters(subspaces);

                if (logger.IsVerbose)
                {
                    logger.Verbose("    " + (dim + 1) + "-dimensional clusters: " + modelsAndClusters.Count);
                }

                foreach (IPair<Subspace<INumberVector>, IModifiableDbIds> modelAndCluster in modelsAndClusters)
                {
                    Cluster newCluster = new Cluster(modelAndCluster.Second);
                    newCluster.Model = (new SubspaceModel<INumberVector>(modelAndCluster.First,
                        DatabaseUtil.Centroid<INumberVector>(relation, modelAndCluster.Second)));
                    newCluster.Name = ("cluster_" + numClusters++);
                    result.AddCluster(newCluster);
                }
            }

            return result;
        }

        /**
         * Determines the clusters in the specified dense subspaces.
         * 
         * @param denseSubspaces the dense subspaces in reverse order by their
         *        coverage
         * @return the clusters in the specified dense subspaces and the corresponding
         *         cluster models
         */
        private IList<IPair<Subspace<INumberVector>, IModifiableDbIds>> DetermineClusters(List<CLIQUESubspace<INumberVector>> denseSubspaces)
        {
            List<IPair<Subspace<INumberVector>, IModifiableDbIds>> clusters = new List<IPair<Subspace<INumberVector>, IModifiableDbIds>>();

            foreach (CLIQUESubspace<INumberVector> subspace in denseSubspaces)
            {
                List<IPair<Subspace<INumberVector>, IModifiableDbIds>> clustersInSubspace = subspace.determineClusters();
                if (logger.IsDebugging)
                {
                    logger.Debug("Subspace " + subspace + " clusters " + clustersInSubspace.Count);
                }
                clusters.AddRange(clustersInSubspace);
            }
            return clusters;
        }

        /**
         * Determines the one dimensional dense subspaces and performs a pruning if
         * this option is chosen.
         * 
         * @param database the database to run the algorithm on
         * @return the one dimensional dense subspaces reverse ordered by their
         *         coverage
         */
        private List<CLIQUESubspace<INumberVector>> FindOneDimensionalDenseSubspaces(IRelation database)
        {
            List<CLIQUESubspace<INumberVector>> denseSubspaceCandidates = FindOneDimensionalDenseSubspaceCandidates(database);

            if (prune)
            {
                return PruneDenseSubspaces(denseSubspaceCandidates);
            }

            return denseSubspaceCandidates;
        }

        /**
         * Determines the {@code k}-dimensional dense subspaces and performs a pruning
         * if this option is chosen.
         * 
         * @param database the database to run the algorithm on
         * @param denseSubspaces the {@code (k-1)}-dimensional dense subspaces
         * @return a list of the {@code k}-dimensional dense subspaces sorted in
         *         reverse order by their coverage
         */
        private List<CLIQUESubspace<INumberVector>> findDenseSubspaces(IRelation database, List<CLIQUESubspace<INumberVector>> denseSubspaces)
        {
            List<CLIQUESubspace<INumberVector>> denseSubspaceCandidates = FindDenseSubspaceCandidates(database, denseSubspaces);

            if (prune)
            {
                return PruneDenseSubspaces(denseSubspaceCandidates);
            }

            return denseSubspaceCandidates;
        }

        /**
         * Initializes and returns the one dimensional units.
         * 
         * @param database the database to run the algorithm on
         * @return the created one dimensional units
         */
        private ICollection<CLIQUEUnit<INumberVector>> InitOneDimensionalUnits(IRelation database)
        {
            int dimensionality = DatabaseUtil.Dimensionality(database);
            // initialize minima and maxima
            double[] minima = new double[dimensionality];
            double[] maxima = new double[dimensionality];
            for (int d = 0; d < dimensionality; d++)
            {
                maxima[d] = -Double.MaxValue;
                minima[d] = Double.MaxValue;
            }
            // update minima and maxima
            //for (DbIdIter it = database.iterDbIds(); it.valid(); it.advance())
            foreach (var it in database.GetDbIds())
            {
                INumberVector featureVector = (INumberVector)database[(it)];
                UpdateMinMax(featureVector, minima, maxima);
            }
            for (int i = 0; i < maxima.Length; i++)
            {
                maxima[i] += 0.0001;
            }

            // determine the unit length in each dimension
            double[] unit_lengths = new double[dimensionality];
            for (int d = 0; d < dimensionality; d++)
            {
                unit_lengths[d] = (maxima[d] - minima[d]) / xsi;
            }

            if (logger.IsDebugging)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("   minima: ").Append(FormatUtil.Format(minima, ", ", 2));
                msg.Append("\n   maxima: ").Append(FormatUtil.Format(maxima, ", ", 2));
                msg.Append("\n   unit lengths: ").Append(FormatUtil.Format(unit_lengths, ", ", 2));
                logger.Debug(msg.ToString());
            }

            // determine the boundaries of the units
            double[,] unit_bounds = new double[xsi + 1, dimensionality];
            for (int x = 0; x <= xsi; x++)
            {
                for (int d = 0; d < dimensionality; d++)
                {
                    if (x < xsi)
                    {
                        unit_bounds[x, d] = minima[d] + x * unit_lengths[d];
                    }
                    else
                    {
                        unit_bounds[x, d] = maxima[d];
                    }
                }
            }
            if (logger.IsDebugging)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("   unit bounds ").Append(FormatUtil.Format(new Matrix(unit_bounds), "   "));
                logger.Debug(msg.ToString());
            }

            // build the 1 dimensional units
            List<CLIQUEUnit<INumberVector>> units = new List<CLIQUEUnit<INumberVector>>((xsi * dimensionality));
            for (int x = 0; x < xsi; x++)
            {
                for (int d = 0; d < dimensionality; d++)
                {
                    units.Add(new CLIQUEUnit<INumberVector>(new Interval(d, unit_bounds[x, d], unit_bounds[x + 1, d])));
                }
            }

            if (logger.IsDebugging)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("   total number of 1-dim units: ").Append(units.Count);
                logger.Debug(msg.ToString());
            }

            return units;
        }

        /**
         * Updates the minima and maxima array according to the specified feature
         * vector.
         * 
         * @param featureVector the feature vector
         * @param minima the array of minima
         * @param maxima the array of maxima
         */
        private void UpdateMinMax(INumberVector featureVector, double[] minima, double[] maxima)
        {
            if (minima.Length != featureVector.Count)
            {
                throw new ArgumentException("FeatureVectors differ in length.");
            }
            for (int d = 0; d < featureVector.Count; d++)
            {
                if ((featureVector[(d)]) > maxima[d])
                {
                    maxima[d] = (featureVector[(d)]);
                }
                if ((featureVector[(d)]) < minima[d])
                {
                    minima[d] = (featureVector[(d)]);
                }
            }
        }

        /**
         * Determines the one-dimensional dense subspace candidates by making a pass
         * over the database.
         * 
         * @param database the database to run the algorithm on
         * @return the one-dimensional dense subspace candidates reverse ordered by
         *         their coverage
         */
        private List<CLIQUESubspace<INumberVector>> FindOneDimensionalDenseSubspaceCandidates(IRelation database)
        {
            ICollection<CLIQUEUnit<INumberVector>> units = InitOneDimensionalUnits(database);
            ICollection<CLIQUEUnit<INumberVector>> denseUnits = new List<CLIQUEUnit<INumberVector>>();
            IDictionary<int, CLIQUESubspace<INumberVector>> denseSubspaces = new Dictionary<int, CLIQUESubspace<INumberVector>>();

            // identify dense units
            double total = database.Count;
            //for (DbIdIter it = database.iterDbIds(); it.valid(); )
            foreach (var it in database.GetDbIds())
            {
                INumberVector featureVector = (INumberVector)database[(it)];
                IDbId id = it.DbId;

                foreach (CLIQUEUnit<INumberVector> unit in units)
                {
                    unit.AddFeatureVector(id, featureVector);
                    if (unit.selectivity(total) >= tau)
                    {
                        denseUnits.Add(unit);
                        // Add the dense unit to its subspace
                        int dim = unit.GetIntervals().ElementAt(0).GetDimension();
                        CLIQUESubspace<INumberVector> subspace_d = denseSubspaces[dim];
                        if (subspace_d == null)
                        {
                            subspace_d = new CLIQUESubspace<INumberVector>(dim);
                            denseSubspaces[dim] = subspace_d;
                        }
                        subspace_d.AddDenseUnit(unit);
                    }
                }
            }

            if (logger.IsDebugging)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("   number of 1-dim dense units: ").Append(denseUnits.Count);
                msg.Append("\n   number of 1-dim dense subspace candidates: ").Append(denseSubspaces.Count);
                logger.Debug(msg.ToString());
            }

            List<CLIQUESubspace<INumberVector>> subspaceCandidates = new List<CLIQUESubspace<INumberVector>>(denseSubspaces.Values);
            subspaceCandidates.Sort(new CLIQUESubspace<INumberVector>.CoverageComparator());
            return subspaceCandidates;
        }

        /**
         * Determines the {@code k}-dimensional dense subspace candidates from the
         * specified {@code (k-1)}-dimensional dense subspaces.
         * 
         * @param database the database to run the algorithm on
         * @param denseSubspaces the {@code (k-1)}-dimensional dense subspaces
         * @return a list of the {@code k}-dimensional dense subspace candidates
         *         reverse ordered by their coverage
         */
        private List<CLIQUESubspace<INumberVector>> FindDenseSubspaceCandidates(IRelation database, List<CLIQUESubspace<INumberVector>> denseSubspaces)
        {
            // sort (k-1)-dimensional dense subspace according to their dimensions
            List<CLIQUESubspace<INumberVector>> denseSubspacesByDimensions = new List<CLIQUESubspace<INumberVector>>(denseSubspaces);
            denseSubspacesByDimensions.Sort(new Subspace<INumberVector>.DimensionComparator());

            // determine k-dimensional dense subspace candidates
            double all = database.Count;
            List<CLIQUESubspace<INumberVector>> denseSubspaceCandidates = new List<CLIQUESubspace<INumberVector>>();

            while (denseSubspacesByDimensions.Count > 0)
            {
                CLIQUESubspace<INumberVector> s1 = denseSubspacesByDimensions[(0)];
                denseSubspacesByDimensions.RemoveAt(0);
                foreach (CLIQUESubspace<INumberVector> s2 in denseSubspacesByDimensions)
                {
                    CLIQUESubspace<INumberVector> s = s1.join(s2, all, tau);
                    if (s != null)
                    {
                        denseSubspaceCandidates.Add(s);
                    }
                }
            }

            // sort reverse by coverage
            denseSubspaceCandidates.Sort(new CLIQUESubspace<INumberVector>.CoverageComparator());
            return denseSubspaceCandidates;
        }

        /**
         * Performs a MDL-based pruning of the specified dense subspaces as described
         * in the CLIQUE algorithm.
         * 
         * @param denseSubspaces the subspaces to be pruned sorted in reverse order by
         *        their coverage
         * @return the subspaces which are not pruned reverse ordered by their
         *         coverage
         */
        private List<CLIQUESubspace<INumberVector>> PruneDenseSubspaces(List<CLIQUESubspace<INumberVector>> denseSubspaces)
        {
            int[][] means = ComputeMeans(denseSubspaces);
            double[][] diffs = ComputeDiffs(denseSubspaces, means[0], means[1]);
            double[] codeLength = new double[denseSubspaces.Count];
            double minCL = Double.MaxValue;
            int min_i = -1;

            for (int i = 0; i < denseSubspaces.Count; i++)
            {
                int mi = means[0][i];
                int mp = means[1][i];
                double log_mi = mi == 0 ? 0 : Math.Log(mi) / Math.Log(2);
                double log_mp = mp == 0 ? 0 : Math.Log(mp) / Math.Log(2);
                double diff_mi = diffs[0][i];
                double diff_mp = diffs[1][i];
                codeLength[i] = log_mi + diff_mi + log_mp + diff_mp;

                if (codeLength[i] <= minCL)
                {
                    minCL = codeLength[i];
                    min_i = i;
                }
            }
            //TODO:这里可能有陷阱，C#和JAVA这个子串的逻辑不同
            return denseSubspaces.GetRange(0, min_i + 1);
        }

        /**
         * The specified sorted list of dense subspaces is divided into the selected
         * set I and the pruned set P. For each set the mean of the cover fractions is
         * computed.
         * 
         * @param denseSubspaces the dense subspaces in reverse order by their
         *        coverage
         * @return the mean of the cover fractions, the first value is the mean of the
         *         selected set I, the second value is the mean of the pruned set P.
         */
        private int[][] ComputeMeans(List<CLIQUESubspace<INumberVector>> denseSubspaces)
        {
            int n = denseSubspaces.Count - 1;

            int[] mi = new int[n + 1];
            int[] mp = new int[n + 1];

            double resultMI = 0;
            double resultMP = 0;

            for (int i = 0; i < denseSubspaces.Count; i++)
            {
                resultMI += denseSubspaces[(i)].GetCoverage();
                resultMP += denseSubspaces[(n - i)].GetCoverage();
                mi[i] = (int)Math.Ceiling(resultMI / (i + 1));
                if (i != n)
                {
                    mp[n - 1 - i] = (int)Math.Ceiling(resultMP / (i + 1));
                }
            }

            int[][] result = new int[2][];
            result[0] = mi;
            result[1] = mp;

            return result;
        }

        /**
         * The specified sorted list of dense subspaces is divided into the selected
         * set I and the pruned set P. For each set the difference from the specified
         * mean values is computed.
         * 
         * @param denseSubspaces denseSubspaces the dense subspaces in reverse order
         *        by their coverage
         * @param mi the mean of the selected sets I
         * @param mp the mean of the pruned sets P
         * @return the difference from the specified mean values, the first value is
         *         the difference from the mean of the selected set I, the second
         *         value is the difference from the mean of the pruned set P.
         */
        private double[][] ComputeDiffs(List<CLIQUESubspace<INumberVector>> denseSubspaces, int[] mi, int[] mp)
        {
            int n = denseSubspaces.Count - 1;

            double[] diff_mi = new double[n + 1];
            double[] diff_mp = new double[n + 1];

            double resultMI = 0;
            double resultMP = 0;

            for (int i = 0; i < denseSubspaces.Count; i++)
            {
                double diffMI = Math.Abs(denseSubspaces[i].GetCoverage() - mi[i]);
                resultMI += diffMI == 0.0 ? 0 : Math.Log(diffMI) / Math.Log(2);
                double diffMP = (i != n) ? Math.Abs(denseSubspaces[n - i].GetCoverage() - mp[n - 1 - i]) : 0;
                resultMP += diffMP == 0.0 ? 0 : Math.Log(diffMP) / Math.Log(2);
                diff_mi[i] = resultMI;
                if (i != n)
                {
                    diff_mp[n - 1 - i] = resultMP;
                }
            }
            double[][] result = new double[2][];
            result[0] = diff_mi;
            result[1] = diff_mp;

            return result;
        }


        public override ITypeInformation[] GetInputTypeRestriction()
        {
            return TypeUtil.Array(TypeUtil.NUMBER_VECTOR_FIELD);
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
        public class Parameterizer : AbstractParameterizer
        {
            protected int xsi;

            protected double tau;

            protected bool prune;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                IntParameter xsiP = new IntParameter(XSI_ID, new GreaterConstraint<Int32>(0));
                if (config.Grab(xsiP))
                {
                    xsi = xsiP.GetValue();
                }

                DoubleParameter tauP = new DoubleParameter(TAU_ID, new IntervalConstraint<double>(0,
                    IntervalConstraint<double>.IntervalBoundary.OPEN, 1, IntervalConstraint<double>.IntervalBoundary.OPEN));
                if (config.Grab(tauP))
                {
                    tau = tauP.GetValue();
                }

                BoolParameter pruneF = new BoolParameter(PRUNE_ID);
                if (config.Grab(pruneF))
                {
                    prune = pruneF.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new CLIQUE(xsi, tau, prune);
            }


        }
    }
}
