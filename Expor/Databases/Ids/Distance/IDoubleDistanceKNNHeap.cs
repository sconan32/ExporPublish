using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Ids.Distance
{

    /**
     * Interface for kNN heaps storing double distances and DBIDs.
     * 
     * @author Erich Schubert
     */
    public interface IDoubleDistanceKNNHeap : IKNNHeap
    {
        /**
         * Add a distance-id pair to the heap unless the distance is too large.
         * 
         * Compared to the super.add() method, this often saves the pair construction.
         * 
         * @param distance Distance value
         * @param id ID number
         * @return updated k-distance
         */
        double Insert(double distance, IDbIdRef id);


        /**
         * Add a distance-id pair to the heap unless the distance is too large.
         * 
         * Use for existing pairs.
         * 
         * @param e Existing distance pair
         */
        void Insert(IDoubleDistanceDbIdPair e);

        /**
         * {@inheritDoc}
         * 
         * @deprecated if you know your distances are double-valued, you should be
         *             using the primitive type.
         */

        void Insert(DoubleDistanceValue dist, IDbIdRef id);

        /**
         * Get the distance to the k nearest neighbor, or maxdist otherwise.
         * 
         * @return Maximum distance
         */
        double DoubleKNNDistance { get; }

    }

}
