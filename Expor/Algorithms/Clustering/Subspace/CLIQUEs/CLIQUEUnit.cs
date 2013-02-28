using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Algorithms.Clustering.Subspace.CLIQUEs
{

    public class CLIQUEUnit<V> where V : IDataVector
    {
        /**
         * The one-dimensional intervals of which this unit is build.
         */
        private SortedSet<Interval> intervals;

        /**
         * Provides a mapping of particular dimensions to the intervals of which this
         * unit is build.
         */
        private IDictionary<int, Interval> dimensionToInterval;

        /**
         * The ids of the feature vectors this unit contains.
         */
        private IModifiableDbIds ids;

        /**
         * Flag that indicates if this unit is already assigned to a cluster.
         */
        private bool assigned;

        /**
         * Creates a new k-dimensional unit for the given intervals.
         * 
         * @param intervals the intervals belonging to this unit
         * @param ids the ids of the feature vectors belonging to this unit
         */
        public CLIQUEUnit(SortedSet<Interval> intervals, IModifiableDbIds ids)
        {
            this.intervals = intervals;

            dimensionToInterval = new Dictionary<int, Interval>();
            foreach (Interval interval in intervals)
            {
                dimensionToInterval[interval.GetDimension()] = interval;
            }

            this.ids = ids;

            assigned = false;
        }

        /**
         * Creates a new one-dimensional unit for the given interval.
         * 
         * @param interval the interval belonging to this unit
         */
        public CLIQUEUnit(Interval interval)
        {
            intervals = new SortedSet<Interval>();
            intervals.Add(interval);

            dimensionToInterval = new Dictionary<int, Interval>();
            dimensionToInterval[interval.GetDimension()] = interval;

            ids = DbIdUtil.NewHashSet();

            assigned = false;
        }

        /**
         * Returns true, if the intervals of this unit contain the specified feature
         * vector.
         * 
         * @param vector the feature vector to be tested for containment
         * @return true, if the intervals of this unit contain the specified feature
         *         vector, false otherwise
         */
        public bool Contains(V vector)
        {
            foreach (Interval interval in intervals)
            {
                double value =(double) vector.Get(interval.GetDimension() );
                if (interval.GetMin() > value || value >= interval.GetMax())
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * Adds the id of the specified feature vector to this unit, if this unit
         * contains the feature vector.
         * 
         * @param id Vector id
         * @param vector the feature vector to be Added
         * @return true, if this unit contains the specified feature vector, false
         *         otherwise
         */
        public bool AddFeatureVector(IDbId id, V vector)
        {
            if (Contains(vector))
            {
                ids.Add(id);
                return true;
            }
            return false;
        }

        /**
         * Returns the number of feature vectors this unit contains.
         * 
         * @return the number of feature vectors this unit contains
         */
        public int numberOfFeatureVectors()
        {
            return ids.Count;
        }

        /**
         * Returns the selectivity of this unit, which is defined as the fraction of
         * total feature vectors contained in this unit.
         * 
         * @param total the total number of feature vectors
         * @return the selectivity of this unit
         */
        public double selectivity(double total)
        {
            return ids.Count / total;
        }

        /**
         * Returns a sorted set of the intervals of which this unit is build.
         * 
         * @return a sorted set of the intervals of which this unit is build
         */
        public SortedSet<Interval> GetIntervals()
        {
            return intervals;
        }

        /**
         * Returns the interval of the specified dimension.
         * 
         * @param dimension the dimension of the interval to be returned
         * @return the interval of the specified dimension
         */
        public Interval GetInterval(int dimension)
        {
            return dimensionToInterval[(dimension)];
        }

        /**
         * Returns true if this unit contains the left neighbor of the specified
         * interval.
         * 
         * @param i the interval
         * @return true if this unit contains the left neighbor of the specified
         *         interval, false otherwise
         */
        public bool containsLeftNeighbor(Interval i)
        {
            Interval interval = dimensionToInterval[(i.GetDimension())];
            if (interval == null)
            {
                return false;
            }
            return interval.GetMax() == i.GetMin();
        }

        /**
         * Returns true if this unit contains the right neighbor of the specified
         * interval.
         * 
         * @param i the interval
         * @return true if this unit contains the right neighbor of the specified
         *         interval, false otherwise
         */
        public bool containsRightNeighbor(Interval i)
        {
            Interval interval = dimensionToInterval[(i.GetDimension())];
            if (interval == null)
            {
                return false;
            }
            return interval.GetMin() == i.GetMax();
        }

        /**
         * Returns true if this unit is already assigned to a cluster.
         * 
         * @return true if this unit is already assigned to a cluster, false
         *         otherwise.
         */
        public bool isAssigned()
        {
            return assigned;
        }

        /**
         * Marks this unit as assigned to a cluster.
         */
        public void markAsAssigned()
        {
            this.assigned = true;
        }

        /**
         * Returns the ids of the feature vectors this unit contains.
         * 
         * @return the ids of the feature vectors this unit contains
         */
        public IDbIds GetIds()
        {
            return ids;
        }

        /**
         * Joins this unit with the specified unit.
         * 
         * @param other the unit to be joined
         * @param all the overall number of feature vectors
         * @param tau the density threshold for the selectivity of a unit
         * @return the joined unit if the selectivity of the join result is equal or
         *         greater than tau, null otherwise
         */
        public CLIQUEUnit<V> join(CLIQUEUnit<V> other, double all, double tau)
        {
            Interval i1 = this.intervals.Last();
            Interval i2 = other.intervals.Last();
            if (i1.GetDimension() >= i2.GetDimension())
            {
                return null;
            }

            IEnumerator<Interval> it1 = this.intervals.GetEnumerator();
            IEnumerator<Interval> it2 = other.intervals.GetEnumerator();
            SortedSet<Interval> resultIntervals = new SortedSet<Interval>();
            for (int i = 0; i < this.intervals.Count - 1; i++)
            {
                it1.MoveNext();
                it2.MoveNext();
                i1 = it1.Current;
                i2 = it2.Current;
                if (!i1.Equals(i2))
                {
                    return null;
                }
                resultIntervals.Add(i1);
            }
            resultIntervals.Add(this.intervals.Last());
            resultIntervals.Add(other.intervals.Last());

            IHashSetModifiableDbIds resultIDs = DbIdUtil.NewHashSet(this.ids);
            resultIDs.RetainAll(other.ids);

            if (resultIDs.Count / all >= tau)
            {
                return new CLIQUEUnit<V>(resultIntervals, resultIDs);
            }

            return null;
        }

        /**
         * Returns a string representation of this unit that contains the intervals of
         * this unit.
         * 
         * @return a string representation of this unit
         */

        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (Interval interval in intervals)
            {
                result.Append(interval).Append(" ");
            }

            return result.ToString();
        }
    }
}
