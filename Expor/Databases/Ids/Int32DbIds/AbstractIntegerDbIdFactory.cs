using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Ids.Generic;
using Socona.Expor.Databases.Ids.Int32DbIds;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{

    /**
     * Abstract base class for DbId factories.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.uses Int32DbId oneway - - 芦create禄
     * @apiviz.uses Int32DbIdPair oneway - - 芦create禄
     * @apiviz.uses Int32DbIdRange oneway - - 芦create禄
     * @apiviz.uses TroveHashSetModifiableDbIds oneway - - 芦create禄
     * @apiviz.uses Int32ArrayDbIds oneway - - 芦create禄
     */
    public abstract class AbstractInt32DbIdFactory : DbIdFactoryBase
    {
        /**
         * Invalid ID.
         */
        IDbId invalid = new Int32DbId(Int32.MinValue);


        public override IDbId ImportInt32(int id)
        {
            return new Int32DbId(id);
        }


        public void AssignVar(IDbIdVar var, int val)
        {
            if (var is Int32DbIdVar)
            {
                ((Int32DbIdVar)var).InternalSetIndex(val);
            }
            else
            {
                var.Set(new Int32DbId(val));
            }
        }


        public override int Compare(IDbIdRef a, IDbIdRef b)
        {
            int inta = a.InternalGetIndex();
            int intb = b.InternalGetIndex();
            return (inta < intb ? -1 : (inta == intb ? 0 : 1));
        }


        public override bool IsEqual(IDbIdRef a, IDbIdRef b)
        {
            return a.InternalGetIndex() == b.InternalGetIndex();
        }


        public String ToString(IDbIdRef id)
        {
            return (id.InternalGetIndex()).ToString();
        }


        public IDbIdVar NewVar(IDbIdRef val)
        {
            return new Int32DbIdVar(val);
        }


        public override IArrayModifiableDbIds NewArray()
        {
            return new ArrayModifiableInt32DbIds();
        }


        public override IHashSetModifiableDbIds NewHashSet()
        {
            return new TroveHashSetModifiableDbIds();
        }


        public override IArrayModifiableDbIds NewArray(int size)
        {
            return new ArrayModifiableInt32DbIds(size);
        }


        public override IHashSetModifiableDbIds NewHashSet(int size)
        {
            return new TroveHashSetModifiableDbIds(size);
        }


        public override IArrayModifiableDbIds NewArray(IDbIds existing)
        {
            return new ArrayModifiableInt32DbIds(existing);
        }


        public override IHashSetModifiableDbIds NewHashSet(IDbIds existing)
        {
            return new TroveHashSetModifiableDbIds(existing);
        }


        public IDbIdPair NewPair(IDbIdRef first, IDbIdRef second)
        {
            return new Int32DbIdPair(first.InternalGetIndex(), second.InternalGetIndex());
        }


        public IDoubleDbIdPair NewPair(double val, IDbIdRef id)
        {
            return new Int32DoubleDbIdPair(val, id.InternalGetIndex());
        }



        public override IDistanceDbIdPair NewDistancePair(IDistanceValue val, IDbIdRef id)
        {
            if (val is DoubleDistanceValue)
            {
                return (IDistanceDbIdPair)new DoubleDistanceInt32DbIdPair(((DoubleDistanceValue)val).DoubleValue(),
                    id.InternalGetIndex());
            }
            return new DistanceInt32DbIdPair(val, id.InternalGetIndex());
        }


        public override IDoubleDistanceDbIdPair NewDistancePair(double val, IDbIdRef id)
        {
            return new DoubleDistanceInt32DbIdPair(val, id.InternalGetIndex());
        }



        public override IKNNHeap NewHeap(IDistanceValue factory, int k)
        {
            if (factory is DoubleDistanceValue)
            {
                return (IKNNHeap)NewDoubleDistanceHeap(k);
            }
            return new DistanceDbIdPairKNNHeap(k);
        }



        public override IKNNHeap NewHeap(IKNNList exist)
        {
            if (exist is IDoubleDistanceKNNList)
            {
                IDoubleDistanceKNNHeap heap = NewDoubleDistanceHeap(exist.K);
                // Insert backwards, as this will produce a proper heap
                for (int i = exist.Count - 1; i >= 0; i--)
                {
                    heap.Insert((IDoubleDistanceDbIdPair)exist[(i)]);
                }
                return (IKNNHeap)heap;
            }
            else
            {
                DistanceDbIdPairKNNHeap heap = new DistanceDbIdPairKNNHeap(exist.K);
                // Insert backwards, as this will produce a proper heap
                for (int i = exist.Count - 1; i >= 0; i--)
                {
                    heap.Insert(exist[(i)]);
                }
                return heap;
            }
        }


        public override IDoubleDistanceKNNHeap NewDoubleDistanceHeap(int k)
        {
            // TODO: benchmark threshold!
            if (k > 1000)
            {
                return new DoubleDistanceInt32DbIdKNNHeap(k);
            }
            return new DoubleDistanceInt32DbIdSortedKNNList(k);
        }


        //public ByteBufferSerializer<DbId> getDbIdSerializer() {
        //  return Int32DbId.DYNAMIC_SERIALIZER;
        //}


        //public FixedSizeByteBufferSerializer<DbId> getDbIdSerializerStatic() {
        //  return Int32DbId.STATIC_SERIALIZER;
        //}


        public override Type GetTypeRestriction()
        {
            return typeof(Int32DbId);
        }


        public IDbIdRef Invalid()
        {
            return invalid;
        }
    }

}
