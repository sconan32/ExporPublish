using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Concurrent;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Persistent;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{

    /**
     * Trivial IDbId management, that never reuses IDs and just gives them out in sequence.
     * Statically allocated IDbId ranges are given positive values,
     * Dynamically allocated DbIds are given negative values.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.landmark
     * @apiviz.stereotype factory
     * @apiviz.uses Int32DbId oneway - - «create»
     * @apiviz.uses Int32DbIdPair oneway - - «create»
     * @apiviz.uses Int32DbIdRange oneway - - «create»
     * @apiviz.uses TroveArrayModifiableDbIds oneway - - «create»
     * @apiviz.uses TroveHashSetModifiableDbIds oneway - - «create»
     */
    public class TrivialDbIdFactory : AbstractInt32DbIdFactory
    {
        /**
         * Keep track of the smallest dynamic IDbId offset not used
         */
        AtomicInt32 next = new AtomicInt32(1);

        /**
         * Constructor
         */
        public TrivialDbIdFactory()
            : base()
        {

        }


        public override IDbId GenerateSingleDbId()
        {
            int id = next.GetAndIncrement();
            if (id == Int32.MaxValue)
            {
                throw new AbortException("IDbId allocation error - too many objects allocated!");
            }
            IDbId ret = new Int32DbId(id);
            return ret;
        }


        public override void DeallocateSingleDbId(IDbId id)
        {
            // ignore.
        }


        public override IDbIdRange GenerateStaticDbIdRange(int size)
        {
            int start = next.GetAndAdd(size);
            if (start > next.Get())
            {
                throw new AbortException("IDbId range allocation error - too many objects allocated!");
            }
            IDbIdRange alloc = new Int32DbIdRange(start, size);
            return alloc;
        }


        public override void DeallocateDbIdRange(IDbIdRange range)
        {
            // ignore.
        }


        public override IDbId ImportInt32(int id)
        {
            return new Int32DbId(id);
        }


        public override IArrayModifiableDbIds NewArray()
        {
            return new TroveArrayModifiableDbIds();
        }


        public override IHashSetModifiableDbIds NewHashSet()
        {
            return new TroveHashSetModifiableDbIds();
        }


        public override IArrayModifiableDbIds NewArray(int size)
        {
            return new TroveArrayModifiableDbIds(size);
        }


        public override IHashSetModifiableDbIds NewHashSet(int size)
        {
            return new TroveHashSetModifiableDbIds(size);
        }


        public override IArrayModifiableDbIds NewArray(IDbIds existing)
        {
            return new TroveArrayModifiableDbIds(existing);
        }


        public override IHashSetModifiableDbIds NewHashSet(IDbIds existing)
        {
            return new TroveHashSetModifiableDbIds(existing);
        }


        public override IDbIdPair MakePair(IDbIdRef first, IDbIdRef second)
        {
            return new Int32DbIdPair(first.Int32Id, second.Int32Id);
        }


        public override IByteBufferSerializer GetDbIdSerializer()
        {
            return new Int32DbId.DynamicSerializer();
        }


        public override IFixedSizeByteBufferSerializer<IDbId> GetDbIdSerializerStatic()
        {
            return new Int32DbId.StaticSerializer();
        }


        public override Type GetTypeRestriction()
        {
            return typeof(Int32DbId);
        }



    }
}
