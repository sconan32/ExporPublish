using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Maths.Geometry;
using Socona.Expor.Results.Outliers;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Evaluation
{
    /// <summary>
    /// Compute ROC (Receiver Operating Characteristics) curves.
    /// </summary>
    public class ROC
    {
        /**
         * Compute a ROC curve given a set of positive IDs and a sorted list of
         * (comparable, ID)s, where the comparable object is used to decided when two
         * objects are interchangeable.
         * 
         * @param <C> Reference type
         * @param size Database size
         * @param ids Collection of positive IDs, should support efficient contains()
         * @param nei List of neighbors along with some comparable object to detect
         *        'same positions'.
         * @return area under curve
         */
        public static XYCurve MaterializeROC<C, T>(int size, ISet<T> ids, IEnumerator<IPair<C, T>> nei)
        where C : IComparable
        {
            int postot = ids.Count, negtot = size - postot;
            int poscnt = 0, negcnt = 0;
            XYCurve curve = new XYCurve("True Negative Rate", "True Positive Rate", postot + 2);

            // start in bottom left
            curve.Add(0.0, 0.0);

            C prevval = default(C);
            while (nei.Current != null)
            {
                // Analyze next point
                IPair<C, T> cur = nei.Current;
                // positive or negative match?
                if (ids.Contains(cur.Second))
                {
                    poscnt += 1;
                }
                else
                {
                    negcnt += 1;
                }
                // defer calculation for ties
                if ((prevval != null) && (prevval.CompareTo(cur.First) == 0))
                {
                    continue;
                }
                // Add a new point.
                curve.AddAndSimplify(negcnt / (double)negtot, poscnt / (double)postot);
                prevval = cur.First;
            }
            // Ensure we end up in the top right corner.
            // Simplification will skip this if we already were.
            curve.AddAndSimplify(1.0, 1.0);
            return curve;
        }

        /**
         * Compute a ROC curve given a set of positive IDs and a sorted list of
         * (comparable, ID)s, where the comparable object is used to decided when two
         * objects are interchangeable.
         * 
         * @param <C> Reference type
         * @param size Database size
         * @param ids Collection of positive IDs, should support efficient contains()
         * @param nei List of neighbors along with some comparable object to detect
         *        'same positions'.
         * @return area under curve
         */
        public static XYCurve MaterializeROC<C>(int size, ISetDbIds ids, IEnumerator<IPair<C, IDbId>> nei)
        {
            int postot = ids.Count, negtot = size - postot;
            int poscnt = 0, negcnt = 0;
            XYCurve curve = new XYCurve("True Negative Rate", "True Positive Rate", postot + 2);

            // start in bottom left
            curve.Add(0.0, 0.0);

            C prevval = default(C);
            while (nei.Current != null)
            {
                // Rates at *previous* data point. Because of tie handling strategy!
                double trueneg = negcnt / (double)negtot;
                double truepos = poscnt / (double)postot;
                // Analyze next point
                IPair<C, IDbId> cur = nei.Current;
                // positive or negative match?
                if (ids.Contains(cur.Second))
                {
                    poscnt += 1;
                }
                else
                {
                    negcnt += 1;
                }
                // defer calculation for ties
                if ((prevval != null) && (prevval.Equals(cur.First)))
                {
                    continue;
                }
                // Add point for *previous* result (since we are no longer tied with it)
                curve.AddAndSimplify(trueneg, truepos);
                prevval = cur.First;
            }
            // Ensure we end up in the top right corner.
            // Simplification will skip this if we already were.
            curve.AddAndSimplify(1.0, 1.0);
            return curve;
        }

        /**
         * This adapter can be used for an arbitrary collection of Integers, and uses
         * that id1.compareTo(id2) != 0 for id1 != id2 to satisfy the comparability.
         * 
         * Note that of course, no id should occur more than once.
         * 
         * The ROC values would be incorrect then anyway!
         * 
         * @author Erich Schubert
         */
        public class SimpleAdapter : IEnumerator<IDbIdPair>
        {
            /**
             * Original Iterator
             */
            private IEnumerator<IDbId> iter;
            private IDbIdPair curPair;
            /**
             * Constructor
             * 
             * @param iter Iterator for object IDs
             */
            public SimpleAdapter(IEnumerator<IDbId> iter)
            {

                this.iter = iter;
            }





            public bool MoveNext()
            {
                if (iter.Current != null)
                {
                    curPair = DbIdUtil.NewPair(iter.Current, iter.Current);
                    iter.MoveNext();
                    return true;
                }
                return false;

            }



            public IDbIdPair Current
            {
                get { return curPair; }
            }

            public void Dispose()
            {

            }

            object System.Collections.IEnumerator.Current
            {
                get { return curPair; }
            }


            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        /**
         * This adapter can be used for an arbitrary collection of Integers, and uses
         * that id1.compareTo(id2) != 0 for id1 != id2 to satisfy the comparability.
         * 
         * Note that of course, no id should occur more than once.
         * 
         * The ROC values would be incorrect then anyway!
         * 
         * @author Erich Schubert
         * @param <D> Distance type
         */
        public class DistanceResultAdapter : IEnumerator<IPair<IDistanceValue, IDbId>>
        {
            /**
             * Original Iterator
             */
            private IEnumerator<IDistanceResultPair> iter;
            private IPair<IDistanceValue, IDbId> curPair;
            /**
             * Constructor
             * 
             * @param iter Iterator for distance results
             */
            public DistanceResultAdapter(IEnumerator<IDistanceResultPair> iter)
            {

                this.iter = iter;
            }



            public IPair<IDistanceValue, IDbId> Current
            {
                get { return curPair; }
            }

            public void Dispose()
            {

            }

            object System.Collections.IEnumerator.Current
            {
                get { return curPair; }
            }

            public bool MoveNext()
            {
                if (iter.MoveNext())
                {
                    curPair = new Pair<IDistanceValue, IDbId>(iter.Current.GetDistance(), iter.Current.Second);
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        /**
         * This adapter can be used for an arbitrary collection of Integers, and uses
         * that id1.compareTo(id2) != 0 for id1 != id2 to satisfy the comparability.
         * 
         * Note that of course, no id should occur more than once.
         * 
         * The ROC values would be incorrect then anyway!
         * 
         * @author Erich Schubert
         */
        public class OutlierScoreAdapter : IEnumerator<DoubleObjPair<IDbId>>
        {
            /**
             * Original Iterator
             */
            private IEnumerator<IDbId> iter;
            DoubleObjPair<IDbId> curPair;
            /**
             * Outlier score
             */
            private IRelation scores;

            /**
             * Constructor.
             * 
             * @param o Result
             */
            public OutlierScoreAdapter(OutlierResult o)
            {

                this.iter = o.GetOrdering().Iter(o.GetScores().GetDbIds()).GetEnumerator();
                this.scores = o.GetScores();
            }



            public bool MoveNext()
            {
                if (iter.MoveNext())
                {
                    IDbId id = this.iter.Current.DbId;
                    curPair = new DoubleObjPair<IDbId>((double)scores[id], id);
                    return true;

                }
                return false;

            }



            public DoubleObjPair<IDbId> Current
            {
                get { return curPair; }
            }

            public void Dispose()
            {

            }

            object System.Collections.IEnumerator.Current
            {
                get { return curPair; }
            }


            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        /**
         * Compute a ROC curves Area-under-curve for a QueryResult and a Cluster.
         * 
         * @param <D> Distance type
         * @param size Database size
         * @param clus Cluster object
         * @param nei Query result
         * @return area under curve
         */
        public static double ComputeROCAUCDistanceResult(int size,
         Data.Cluster clus, IEnumerable<IDistanceResultPair> nei)
        {
            // TODO: ensure the collection has efficient "contains".
            return ROC.ComputeROCAUCDistanceResult(size, clus.Ids, nei);
        }

        /**
         * Compute a ROC curves Area-under-curve for a QueryResult and a Cluster.
         * 
         * @param <D> Distance type
         * @param size Database size
         * @param ids Collection of positive IDs, should support efficient contains()
         * @param nei Query Result
         * @return area under curve
         */
        public static double ComputeROCAUCDistanceResult(int size, IDbIds ids,
            IEnumerable<IDistanceResultPair> nei)
        {
            // TODO: do not materialize the ROC, but introduce an iterator interface
            XYCurve roc = MaterializeROC(size, DbIdUtil.EnsureSet(ids), new DistanceResultAdapter(nei.GetEnumerator()));
            return XYCurve.AreaUnderCurve(roc);
        }

        /**
         * Compute a ROC curves Area-under-curve for a QueryResult and a Cluster.
         * 
         * @param size Database size
         * @param ids Collection of positive IDs, should support efficient contains()
         * @param nei Query Result
         * @return area under curve
         */
        public static double ComputeROCAUCSimple(int size, IDbIds ids, IDbIds nei)
        {
            // TODO: do not materialize the ROC, but introduce an iterator interface
            XYCurve roc = MaterializeROC(size, DbIdUtil.EnsureSet(ids), new SimpleAdapter(nei.GetEnumerator()));
            return XYCurve.AreaUnderCurve(roc);
        }
    }
}
