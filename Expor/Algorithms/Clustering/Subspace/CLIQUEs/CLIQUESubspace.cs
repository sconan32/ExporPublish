using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Utilities.Pairs;
using Socona.Expor.Utilities.Extenstions;

namespace Socona.Expor.Algorithms.Clustering.Subspace.CLIQUEs
{

    public class CLIQUESubspace<V> : Subspace<V>
        where V : IDataVector
    {
        /**
         * The dense units belonging to this subspace.
         */
        private List<CLIQUEUnit<V>> denseUnits;

        /**
         * The coverage of this subspace, which is the number of all feature vectors
         * that fall inside the dense units of this subspace.
         */
        private int coverage;

        /**
         * Creates a new one-dimensional subspace of the original data space.
         * 
         * @param dimension the dimension building this subspace
         */
        public CLIQUESubspace(int dimension) :
            base(dimension)
        {
            denseUnits = new List<CLIQUEUnit<V>>();
            coverage = 0;
        }

        /**
         * Creates a new k-dimensional subspace of the original data space.
         * 
         * @param dimensions the dimensions building this subspace
         */
        public CLIQUESubspace(BitArray dimensions) :
            base(dimensions)
        {
            denseUnits = new List<CLIQUEUnit<V>>();
            coverage = 0;
        }

        /**
         * Adds the specified dense unit to this subspace.
         * 
         * @param unit the unit to be Added.
         */
        public void AddDenseUnit(CLIQUEUnit<V> unit)
        {
            ICollection<Interval> intervals = unit.GetIntervals();
            foreach (Interval interval in intervals)
            {
                if (!GetDimensions().Get(interval.GetDimension()))
                {
                    throw new ArgumentException("Unit " + unit + "cannot be Added to this subspace, because of wrong dimensions!");
                }
            }

            GetDenseUnits().Add(unit);
            coverage += unit.numberOfFeatureVectors();
        }

        /**
         * Determines all clusters in this subspace by performing a depth-first search
         * algorithm to find connected dense units.
         * 
         * @return the clusters in this subspace and the corresponding cluster models
         */
        public List<IPair<Subspace<V>, IModifiableDbIds>> determineClusters()
        {
            List<IPair<Subspace<V>, IModifiableDbIds>> clusters = new List<IPair<Subspace<V>, IModifiableDbIds>>();

            foreach (CLIQUEUnit<V> unit in GetDenseUnits())
            {
                if (!unit.isAssigned())
                {
                    IModifiableDbIds cluster = DbIdUtil.NewHashSet();
                    CLIQUESubspace<V> model = new CLIQUESubspace<V>(GetDimensions());
                    clusters.Add(new Pair<Subspace<V>, IModifiableDbIds>(model, cluster));
                    dfs(unit, cluster, model);
                }
            }
            return clusters;
        }

        /**
         * Depth-first search algorithm to find connected dense units in this subspace
         * that build a cluster. It starts with a unit, assigns it to a cluster and
         * finds all units it is connected to.
         * 
         * @param unit the unit
         * @param cluster the IDs of the feature vectors of the current cluster
         * @param model the model of the cluster
         */
        public void dfs(CLIQUEUnit<V> unit, IModifiableDbIds cluster, CLIQUESubspace<V> model)
        {
            cluster.AddDbIds(unit.GetIds());
            unit.markAsAssigned();
            model.AddDenseUnit(unit);

            for (int dim = GetDimensions().NextSetBitIndex(0); dim >= 0; dim = GetDimensions().NextSetBitIndex(dim + 1))
            {
                CLIQUEUnit<V> left = leftNeighbor(unit, dim);
                if (left != null && !left.isAssigned())
                {
                    dfs(left, cluster, model);
                }

                CLIQUEUnit<V> right = rightNeighbor(unit, dim);
                if (right != null && !right.isAssigned())
                {
                    dfs(right, cluster, model);
                }
            }
        }

        /**
         * Returns the left neighbor of the given unit in the specified dimension.
         * 
         * @param unit the unit to determine the left neighbor for
         * @param dim the dimension
         * @return the left neighbor of the given unit in the specified dimension
         */
        public CLIQUEUnit<V> leftNeighbor(CLIQUEUnit<V> unit, int dim)
        {
            Interval i = unit.GetInterval(dim);

            foreach (CLIQUEUnit<V> u in GetDenseUnits())
            {
                if (u.containsLeftNeighbor(i))
                {
                    return u;
                }
            }
            return null;
        }

        /**
         * Returns the right neighbor of the given unit in the specified dimension.
         * 
         * @param unit the unit to determine the right neighbor for
         * @param dim the dimension
         * @return the right neighbor of the given unit in the specified dimension
         */
        public CLIQUEUnit<V> rightNeighbor(CLIQUEUnit<V> unit, int dim)
        {
            Interval i = unit.GetInterval(dim);

            foreach (CLIQUEUnit<V> u in GetDenseUnits())
            {
                if (u.containsRightNeighbor(i))
                {
                    return u;
                }
            }
            return null;
        }

        /**
         * Returns the coverage of this subspace, which is the number of all feature
         * vectors that fall inside the dense units of this subspace.
         * 
         * @return the coverage of this subspace
         */
        public int GetCoverage()
        {
            return coverage;
        }

        /**
         * @return the denseUnits
         */
        public List<CLIQUEUnit<V>> GetDenseUnits()
        {
            return denseUnits;
        }

        /**
         * Joins this subspace and its dense units with the specified subspace and its
         * dense units. The join is only successful if both subspaces have the first
         * k-1 dimensions in common (where k is the number of dimensions) and the last
         * dimension of this subspace is less than the last dimension of the specified
         * subspace.
         * 
         * @param other the subspace to join
         * @param all the overall number of feature vectors
         * @param tau the density threshold for the selectivity of a unit
         * @return the join of this subspace with the specified subspace if the join
         *         condition is fulfilled, null otherwise.
         * @see de.lmu.ifi.dbs.elki.data.Subspace#joinLastDimensions
         */
        public CLIQUESubspace<V> join(CLIQUESubspace<V> other, double all, double tau)
        {
            BitArray dimensions = JoinLastDimensions(other);
            if (dimensions == null)
            {
                return null;
            }

            CLIQUESubspace<V> s = new CLIQUESubspace<V>(dimensions);
            foreach (CLIQUEUnit<V> u1 in this.GetDenseUnits())
            {
                foreach (CLIQUEUnit<V> u2 in other.GetDenseUnits())
                {
                    CLIQUEUnit<V> u = u1.join(u2, all, tau);
                    if (u != null)
                    {
                        s.AddDenseUnit(u);
                    }
                }
            }
            if (s.GetDenseUnits().Count <= 0)
            {
                return null;
            }
            return s;
        }

        /**
         * Calls the base method and Adds Additionally the coverage, and the dense
         * units of this subspace.
         */

        public override String ToString(String pre)
        {
            StringBuilder result = new StringBuilder();
            result.Append(base.ToString(pre));
            result.Append("\n").Append(pre).Append("Coverage: ").Append(coverage);
            result.Append("\n").Append(pre).Append("Units: " + "\n");
            foreach (CLIQUEUnit<V> denseUnit in GetDenseUnits())
            {
                result.Append(pre).Append("   ").Append(denseUnit.ToString()).Append("   ").Append(denseUnit.GetIds().Count).Append(" objects\n");
            }
            return result.ToString();
        }

        /**
         * A partial comparator for CLIQUESubspaces based on their coverage. The
         * CLIQUESubspaces are reverse ordered by the values of their coverage.
         * 
         * Note: this comparator provides an ordering that is inconsistent with
         * Equals.
         * 
         * @author Elke Achtert
         */
        public class CoverageComparator : IComparer<CLIQUESubspace<V>>
        {
            /**
             * Compares the two specified CLIQUESubspaces for order. Returns a negative
             * integer, zero, or a positive integer if the coverage of the first
             * subspace is greater than, equal to, or less than the coverage of the
             * second subspace. I.e. the subspaces are reverse ordered by the values of
             * their coverage.
             * 
             * Note: this comparator provides an ordering that is inconsistent with
             * Equals.
             * 
             * @param s1 the first subspace to compare
             * @param s2 the second subspace to compare
             * @return a negative integer, zero, or a positive integer if the coverage
             *         of the first subspace is greater than, equal to, or less than the
             *         coverage of the second subspace
             */

            public int Compare(CLIQUESubspace<V> s1, CLIQUESubspace<V> s2)
            {
                return -(s1.GetCoverage() - s2.GetCoverage());
            }
        }
    }

}
