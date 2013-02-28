using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Ids.Distance
{

    /**
     * Interface for kNN heaps.
     * 
     * To instantiate, use: {@link de.lmu.ifi.dbs.elki.database.ids.DbIdUtil#newHeap}!
     * 
     * @author Erich Schubert
     * 
     * @apiviz.landmark
     * 
     * @apiviz.uses KNNList - - 芦serializes to禄
     * @apiviz.composedOf DistanceDbIdPair
     * 
     * @param <D> Distance function
     */
    public interface IKNNHeap
    {
        /**
         * Serialize to a {@link KNNList}. This empties the heap!
         * 
         * @return KNNList with the heaps contents.
         */
        IKNNList ToKNNList();

        /**
         * Get the K parameter ("maxsize" internally).
         * 
         * @return K
         */
        //int GetK();
        int K { get; }

        /**
         * Get the distance to the k nearest neighbor, or maxdist otherwise.
         * 
         * @return Maximum distance
         */
        // IDistance GetKNNDistance();
        IDistanceValue KNNDistance { get; }
        /**
         * Add a distance-id pair to the heap unless the distance is too large.
         * 
         * Compared to the super.add() method, this often saves the pair construction.
         * 
         * @param distance Distance value
         * @param id ID number
         */
        void Insert(IDistanceValue distance, IDbIdRef id);

        /**
         * Current size of heap.
         * 
         * @return Heap size
         */
        // int Size();
        int Count { get; }

        /**
         * Test if the heap is empty.
         * 
         * @return true when empty.
         */
        bool IsEmpty();

        /**
         * Clear the heap.
         */
        void Clear();

        /**
         * Poll the <em>largest</em> element from the heap.
         * 
         * This is in descending order because of the heap structure. For a convenient
         * way to serialize the heap into a list that you can iterate in ascending
         * order, see {@link #toKNNList()}.
         * 
         * @return largest element
         */
        IDistanceDbIdPair Poll();

        /**
         * Peek at the <em>largest</em> element in the heap.
         * 
         * @return The current largest element.
         */
        IDistanceDbIdPair Peek();
    }
}
