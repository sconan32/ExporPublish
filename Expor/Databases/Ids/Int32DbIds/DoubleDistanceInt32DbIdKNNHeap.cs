using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.DataStructures.Heap;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{

    /**
     * Class to efficiently manage a kNN heap.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.has DoubleDistanceInt32DbIdKNNList
     * @apiviz.composedOf DoubleInt32MaxHeap
     */
    public class DoubleDistanceInt32DbIdKNNHeap : IDoubleDistanceKNNHeap
    {
        /**
         * k for this heap.
         */
        protected int k;

        /**
         * The main heap.
         */
        private DoubleInt32MaxHeap heap;

        /**
         * List to track ties.
         */
        private int[] ties;

        /**
         * Number of element in ties list.
         */
        private int numties = 0;

        /**
         * Current maximum value.
         */
        private double kdist = Double.PositiveInfinity;

        /**
         * Initial size of ties array.
         */
        private static int INITIAL_TIES_SIZE = 11;

        /**
         * Constructor.
         * 
         * @param k Size of knn.
         */
        public DoubleDistanceInt32DbIdKNNHeap(int k) :
            base()
        {
            this.k = k;
            this.heap = new DoubleInt32MaxHeap(k);
            this.ties = new int[INITIAL_TIES_SIZE];
        }


        public int K
        {
            get { return k; }
        }



        public IDistanceValue KNNDistance
        {
            get
            {
                if (heap.Count < k)
                {
                    return (DoubleDistanceValue)DoubleDistanceValue.STATIC.Infinity;
                }
                return new DoubleDistanceValue(kdist);
            }
        }


        public double DoubleKNNDistance
        {
            get { return kdist; }
        }



        public void Insert(DoubleDistanceValue distance, IDbIdRef id)
        {
            double dist = distance.DoubleValue();
            int iid = id.InternalGetIndex();
            if (heap.Count < k)
            {
                heap.Add(dist, iid);
                if (heap.Count >= k)
                {
                    kdist = heap.PeekKey();
                }
                return;
            }
            // Tied with top:
            if (dist >= kdist)
            {
                if (dist == kdist)
                {
                    AddToTies(iid);
                }
                return;
            }
            // Old top element: (kdist, previd)
            UpdateHeap(dist, iid);
        }



        public void Insert(IDistanceValue distance, IDbIdRef id)
        {
            if (distance is DoubleDistanceValue)
            { Insert((DoubleDistanceValue)distance, id); }
        }


        public double Insert(double distance, IDbIdRef id)
        {
            int iid = id.InternalGetIndex();
            if (heap.Count < k)
            {
                heap.Add(distance, iid);
                if (heap.Count >= k)
                {
                    kdist = heap.PeekKey();
                }
                return kdist;
            }
            // Tied with top:
            if (distance >= kdist)
            {
                if (distance == kdist)
                {
                    AddToTies(iid);
                }
                return kdist;
            }
            // Old top element: (kdist, previd)
            UpdateHeap(distance, iid);
            return kdist;
        }


        public void Insert(IDoubleDistanceDbIdPair e)
        {
            double distance = e.DoubleDistance();
            int iid = e.InternalGetIndex();
            if (heap.Count < k)
            {
                heap.Add(distance, iid);
                if (heap.Count >= k)
                {
                    kdist = heap.PeekKey();
                }
                return;
            }
            // Tied with top:
            if (distance >= kdist)
            {
                if (distance == kdist)
                {
                    AddToTies(iid);
                }
                return;
            }
            // Old top element: (kdist, previd)
            UpdateHeap(distance, iid);
        }

        /**
         * Do a full update for the heap.
         * 
         * @param distance Distance
         * @param iid Object id
         */
        private void UpdateHeap(double distance, int iid)
        {
            double prevdist = kdist;
            int previd = heap.PeekValue();
            heap.ReplaceTopElement(distance, iid);
            kdist = heap.PeekKey();
            // If the kdist improved, zap ties.
            if (kdist < prevdist)
            {
                numties = 0;
            }
            else
            {
                AddToTies(previd);
            }
        }

        /**
         * Ensure the ties array has capacity for at least one more element.
         * 
         * @param id Id to add
         */
        private void AddToTies(int id)
        {
            if (ties.Length == numties)
            {
                var newties = new int[(ties.Length << 1) + 1];
                Array.Copy(ties, newties, ties.Length);
                ties = newties; // grow.
            }
            ties[numties] = id;
            ++numties;
        }


        public IDistanceDbIdPair Poll()
        {
            DoubleDistanceInt32DbIdPair ret;
            if (numties > 0)
            {
                ret = new DoubleDistanceInt32DbIdPair(kdist, ties[numties - 1]);
                --numties;
            }
            else
            {
                ret = new DoubleDistanceInt32DbIdPair(heap.PeekKey(), heap.PeekValue());
                heap.Poll();
            }
            return ret;
        }

        /**
         * Pop the topmost element.
         */
        protected void Pop()
        {
            if (numties > 0)
            {
                --numties;
            }
            else
            {
                heap.Poll();
            }
        }


        public IDistanceDbIdPair Peek()
        {
            if (numties > 0)
            {
                return new DoubleDistanceInt32DbIdPair(kdist, ties[numties - 1]);
            }
            return new DoubleDistanceInt32DbIdPair(heap.PeekKey(), heap.PeekValue());
        }


        public int Count
        {
            get { return heap.Count + numties; }
        }


        public bool IsEmpty()
        {
            return heap.Count == 0;
        }


        public void Clear()
        {
            heap.Clear();
            numties = 0;
        }


        public IKNNList ToKNNList()
        {
            int hsize = heap.Count;
            DoubleDistanceInt32DbIdKNNList ret = new DoubleDistanceInt32DbIdKNNList(k, hsize + numties);
            // Add ties:

            for (int i = 0; i < numties; i++)
            {
                ret.Add(new DoubleDistanceInt32DbIdPair(kdist, ties[i]));
                //  ret.dists[hsize + i] = kdist;
                //   ret.ids[hsize + i] = ties[i];
            }
            for (int j = hsize - 1; j >= 0; j--)
            {
                ret.Add(new DoubleDistanceInt32DbIdPair(heap.PeekKey(), heap.PeekValue()));
                //   ret.dists[j] = heap.PeekKey();
                //ret.ids[j] = heap.PeekValue();
                heap.Poll();
            }
            Debug.Assert(ret.Count == hsize + numties);
            ret.Sort();
            return ret;
        }

        /**
         * Peek the topmost distance.
         * 
         * @return distance
         */
        protected double PeekDistance()
        {
            if (numties > 0)
            {
                return kdist;
            }
            else
            {
                return heap.PeekKey();
            }
        }

        /**
         * Peek the topmost internal ID.
         * 
         * @return internal id
         */
        protected int PeekInternalDbId()
        {
            if (numties > 0)
            {
                return ties[numties - 1];
            }
            return heap.PeekValue();
        }


    }

}
