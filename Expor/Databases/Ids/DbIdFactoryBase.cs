using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Ids.Int32DbIds;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Ids
{
    public abstract class DbIdFactoryBase : IDbIdFactory
    {
        /**
        * Static DbId factory to use.
        */
        public static IDbIdFactory FACTORY = new TrivialDbIdFactory();


        abstract public IDbId ImportInt32(int id);

        abstract public IDbId GenerateSingleDbId();

        abstract public void DeallocateSingleDbId(IDbId id);

        abstract public IDbIdRange GenerateStaticDbIdRange(int size);

        abstract public void DeallocateDbIdRange(IDbIdRange range);

        abstract public IDbIdPair MakePair(IDbIdRef id1, IDbIdRef id2);

        abstract public IArrayModifiableDbIds NewArray();

        abstract public IHashSetModifiableDbIds NewHashSet();

        abstract public IArrayModifiableDbIds NewArray(int size);

        abstract public IHashSetModifiableDbIds NewHashSet(int size);

        abstract public IArrayModifiableDbIds NewArray(IDbIds existing);

        abstract public IHashSetModifiableDbIds NewHashSet(IDbIds existing);

        abstract public Persistent.IByteBufferSerializer GetDbIdSerializer();

        abstract public Persistent.IFixedSizeByteBufferSerializer<IDbId> GetDbIdSerializerStatic();

        abstract public Type GetTypeRestriction();

        abstract public int Compare(IDbIdRef a, IDbIdRef b);

        abstract public bool IsEqual(IDbIdRef a, IDbIdRef b);

        abstract public Distance.IDistanceDbIdPair NewDistancePair(IDistanceValue val, IDbIdRef id);
        abstract public Distance.IDoubleDistanceDbIdPair NewDistancePair(double val, IDbIdRef id);
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
        public abstract IKNNHeap NewHeap(IDistanceValue factory, int k);

        /**
         * Build a new heap from a given list.
         * 
         * @param exist Existing result
         * @param <D> Distance type
         * @return New heap
         */
        public abstract IKNNHeap NewHeap(IKNNList exist);

        /**
         * Create an appropriate heap for double distances.
         * 
         * @param k K value
         * @return New heap of size k, appropriate for this distance type.
         */
        public abstract IDoubleDistanceKNNHeap NewDoubleDistanceHeap(int k);
    }
}
