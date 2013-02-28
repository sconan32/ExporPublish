using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Ids.Int32DbIds;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Persistent;

namespace Socona.Expor.Databases.Ids
{

    /**
     * Factory interface for generating DbIds. See {@link #FACTORY} for the static
     * instance to use.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.stereotype factory
     * @apiviz.uses DbId oneway - - «create»
     * @apiviz.uses DbIds oneway - - «create»
     * @apiviz.uses DbIdPair oneway - - «create»
     * @apiviz.uses DbIdRange oneway - - «create»
     * @apiviz.uses ArrayModifiableDbIds oneway - - «create»
     * @apiviz.uses HashSetModifiableDbIds oneway - - «create»
     * @apiviz.uses HashSetModifiableDbIds oneway - - «create»
     * @apiviz.has ByteBufferSerializer oneway - - provides
     */
    public interface IDbIdFactory
    {

        /**
         * Import an integer ID
         * 
         * @param id Int32 ID to import
         * @return DbId
         */
        IDbId ImportInt32(int id);

        /**
         * Generate a single DbId
         * 
         * @return A single DbId
         */
        IDbId GenerateSingleDbId();

        /**
         * Return a single DbId for reuse.
         * 
         * @param id DbId to deallocate
         */
        void DeallocateSingleDbId(IDbId id);

        /**
         * Generate a static DbId range.
         * 
         * @param size Requested size
         * @return DbId range
         */
        IDbIdRange GenerateStaticDbIdRange(int size);

        /**
         * Deallocate a static DbId range.
         * 
         * @param range Range to deallocate
         */
        void DeallocateDbIdRange(IDbIdRange range);

        /**
         * Make a DbId pair from two existing DbIds.
         * 
         * @param id1 first DbId
         * @param id2 second DbId
         * 
         * @return new pair.
         */

        IDbIdPair MakePair(IDbIdRef id1, IDbIdRef id2);

        /**
         * Make a new (modifiable) array of DbIds.
         * 
         * @return New array
         */
        IArrayModifiableDbIds NewArray();

        /**
         * Make a new (modifiable) hash set of DbIds.
         * 
         * @return New hash set
         */
        IHashSetModifiableDbIds NewHashSet();

        /**
         * Make a new (modifiable) array of DbIds.
         * 
         * @param size Size hint
         * @return New array
         */
        IArrayModifiableDbIds NewArray(int size);

        /**
         * Make a new (modifiable) hash set of DbIds.
         * 
         * @param size Size hint
         * @return New hash set
         */
        IHashSetModifiableDbIds NewHashSet(int size);

        /**
         * Make a new (modifiable) array of DbIds.
         * 
         * @param existing existing DbIds to use
         * @return New array
         */
        IArrayModifiableDbIds NewArray(IDbIds existing);

        /**
         * Make a new (modifiable) hash set of DbIds.
         * 
         * @param existing existing DbIds to use
         * @return New hash set
         */
        IHashSetModifiableDbIds NewHashSet(IDbIds existing);

        /**
         * Get a serializer for DbIds
         * 
         * @return DbId serializer 
         */
        IByteBufferSerializer GetDbIdSerializer();

        /**
         * Get a serializer for DbIds with static size
         * 
         * @return DbId serializer
         */
        IFixedSizeByteBufferSerializer<IDbId> GetDbIdSerializerStatic();

        /**
         * Get type restriction
         * 
         * @return type restriction for DbIds
         */
        Type GetTypeRestriction();


        /**
         * Compare two DbIds, for sorting.
         * 
         * @param a First
         * @param b Second
         * @return Comparison result
         */
        int Compare(IDbIdRef a, IDbIdRef b);

        /**
         * Compare two DbIds, for equality testing.
         * 
         * @param a First
         * @param b Second
         * @return Comparison result
         */
        bool IsEqual(IDbIdRef a, IDbIdRef b);
        /**
         * Make a new distance-DbId pair.
         * 
         * @param val Distance value
         * @param id Object ID
         * @param <D> Distance type
         * @return New pair
         */
        IDistanceDbIdPair NewDistancePair(IDistanceValue val, IDbIdRef id);

        /**
         * Make a new distance-DbId pair.
         * 
         * @param val Distance value
         * @param id Object ID
         * @return New pair
         */
        IDoubleDistanceDbIdPair NewDistancePair(double val, IDbIdRef id);


        /**
         * Create an appropriate heap for the distance function.
         * 
         * This will use a double heap if appropriate.
         * 
         * @param factory distance prototype
         * @param k K value
         * @param <D> distance type
         * @return New heap of size k, appropriate for this distance type.
         */
        IKNNHeap NewHeap(IDistanceValue factory, int k);

        /**
         * Build a new heap from a given list.
         * 
         * @param exist Existing result
         * @param <D> Distance type
         * @return New heap
         */
        IKNNHeap NewHeap(IKNNList exist);

        /**
         * Create an appropriate heap for double distances.
         * 
         * @param k K value
         * @return New heap of size k, appropriate for this distance type.
         */
        IDoubleDistanceKNNHeap NewDoubleDistanceHeap(int k);

    }
}
