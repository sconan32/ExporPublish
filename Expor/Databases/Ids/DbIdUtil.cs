using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Ids.Generic;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Persistent;
using Socona.Expor.Utilities.DataStructures.Heap;

namespace Socona.Expor.Databases.Ids
{

    public sealed class DbIdUtil
    {
        /**
         * Static - no public constructor.
         */
        private DbIdUtil()
        {
            // Never called.
        }

        /**
         * Final, global copy of empty IDbIds.
         */
        public static readonly EmptyDbIds EMPTYDBIDS = new EmptyDbIds();

        /**
         * Import an Int32 DbId.
         * 
         * @param id Int32 ID
         * @return DbId
         */
        public static IDbId ImportInt32(int id)
        {
            return DbIdFactoryBase.FACTORY.ImportInt32(id);
        }

        /**
         * Get a serializer for IDbIds
         * 
         * @return DbId serializer
         */
        public IByteBufferSerializer GetDbIdSerializer()
        {
            return DbIdFactoryBase.FACTORY.GetDbIdSerializer();
        }

        /**
         * Get a serializer for IDbIds with static size
         * 
         * @return DbId serializer
         */
        public IByteBufferSerializer GetDbIdSerializerStatic()
        {
            return DbIdFactoryBase.FACTORY.GetDbIdSerializerStatic();
        }

        /**
         * Generate a single DbId
         * 
         * @return A single DbId
         */
        public static IDbId GenerateSingleDbId()
        {
            return DbIdFactoryBase.FACTORY.GenerateSingleDbId();
        }

        /**
         * Return a single DbId for reuse.
         * 
         * @param id DbId to deallocate
         */
        public static void DeallocateSingleDbId(IDbId id)
        {
            DbIdFactoryBase.FACTORY.DeallocateSingleDbId(id);
        }

        /**
         * Generate a static DbId range.
         * 
         * @param size Requested size
         * @return DbId range
         */
        public static IDbIdRange GenerateStaticDbIdRange(int size)
        {
            return DbIdFactoryBase.FACTORY.GenerateStaticDbIdRange(size);
        }

        /**
         * Deallocate a static DbId range.
         * 
         * @param range Range to deallocate
         */
        public static void DeallocateDbIdRange(IDbIdRange range)
        {
            DbIdFactoryBase.FACTORY.DeallocateDbIdRange(range);
        }

        /**
         * Make a new (modifiable) array of IDbIds.
         * 
         * @return New array
         */
        public static IArrayModifiableDbIds NewArray()
        {
            return DbIdFactoryBase.FACTORY.NewArray();
        }

        /**
         * Make a new (modifiable) hash set of IDbIds.
         * 
         * @return New hash set
         */
        public static IHashSetModifiableDbIds NewHashSet()
        {
            return DbIdFactoryBase.FACTORY.NewHashSet();
        }

        /**
         * Make a new (modifiable) array of IDbIds.
         * 
         * @param size Size hint
         * @return New array
         */
        public static IArrayModifiableDbIds NewArray(int size)
        {
            return DbIdFactoryBase.FACTORY.NewArray(size);
        }

        /**
         * Make a new (modifiable) hash set of IDbIds.
         * 
         * @param size Size hint
         * @return New hash set
         */
        public static IHashSetModifiableDbIds NewHashSet(int size)
        {
            return DbIdFactoryBase.FACTORY.NewHashSet(size);
        }

        /**
         * Make a new (modifiable) array of IDbIds.
         * 
         * @param existing Existing IDbIds
         * @return New array
         */
        public static IArrayModifiableDbIds NewArray(IDbIds existing)
        {
            return DbIdFactoryBase.FACTORY.NewArray(existing);
        }

        /**
         * Make a new (modifiable) hash set of IDbIds.
         * 
         * @param existing Existing IDbIds
         * @return New hash set
         */
        public static IHashSetModifiableDbIds NewHashSet(IDbIds existing)
        {
            return DbIdFactoryBase.FACTORY.NewHashSet(existing);
        }

        /**
         * Compute the set intersection of two sets.
         * 
         * @param first First set
         * @param second Second set
         * @return result.
         */
        // TODO: optimize?
        public static IModifiableDbIds Intersection(IDbIds first, IDbIds second)
        {
            if (first.Count > second.Count)
            {
                return Intersection(second, first);
            }
            IModifiableDbIds inter = NewHashSet(first.Count);
            foreach (var dbid in first)
            {
                if (second.Contains(dbid))
                {
                    inter.Add(dbid);
                }
            }
            return inter;
        }

        /**
         * Compute the set symmetric intersection of two sets.
         * 
         * @param first First set
         * @param second Second set
         * @param firstonly OUTPUT: elements only in first. MUST BE EMPTY
         * @param intersection OUTPUT: elements in intersection. MUST BE EMPTY
         * @param secondonly OUTPUT: elements only in second. MUST BE EMPTY
         */
        // TODO: optimize?
        public static void SymmetricIntersection(IDbIds first, IDbIds second,
            IHashSetModifiableDbIds firstonly, IHashSetModifiableDbIds intersection,
            IHashSetModifiableDbIds secondonly)
        {
            if (first.Count > second.Count)
            {
                SymmetricIntersection(second, first, secondonly, intersection, firstonly);
                return;
            }
            Debug.Assert(firstonly.Count == 0, "OUTPUT set should be empty!");
            Debug.Assert(intersection.Count == 0, "OUTPUT set should be empty!");
            Debug.Assert(secondonly.Count == 0, "OUTPUT set should be empty!");
            // Initialize with second
            secondonly.AddDbIds(second);
            foreach (var dbid in first)
            {
                // Try to remove
                if (secondonly.Remove(dbid))
                {
                    intersection.Add(dbid);
                }
                else
                {
                    firstonly.Add(dbid);
                }
            }
        }

        /**
         * Returns the union of the two specified collection of IDs.
         * 
         * @param ids1 the first collection
         * @param ids2 the second collection
         * @return the union of ids1 and ids2 without duplicates
         */
        public static IModifiableDbIds Union(IDbIds ids1, IDbIds ids2)
        {
            IModifiableDbIds result = DbIdUtil.NewHashSet(Math.Max(ids1.Count, ids2.Count));
            result.AddDbIds(ids1);
            result.AddDbIds(ids2);
            return result;
        }

        /**
         * Returns the difference of the two specified collection of IDs.
         * 
         * @param ids1 the first collection
         * @param ids2 the second collection
         * @return the difference of ids1 minus ids2
         */
        public static IModifiableDbIds Difference(IDbIds ids1, IDbIds ids2)
        {
            IModifiableDbIds result = DbIdUtil.NewHashSet(ids1);
            result.RemoveDbIds(ids2);
            return result;
        }

        /**
         * Wrap an existing IDbIds collection to be unmodifiable.
         * 
         * @param existing Existing collection
         * @return Unmodifiable collection
         */
        public static IStaticDbIds MakeUnmodifiable(IDbIds existing)
        {
            if (existing is IStaticDbIds)
            {
                return (IStaticDbIds)existing;
            }
            if (existing is IArrayDbIds)
            {
                return new UnmodifiableArrayDbIds((IArrayDbIds)existing);
            }
            else
            {
                return new UnmodifiableDbIds(existing);
            }
        }

        /**
         * Ensure that the given IDbIds are array-indexable.
         * 
         * @param ids
         * @return Array IDbIds.
         */
        public static IArrayDbIds EnsureArray(IDbIds ids)
        {
            if (ids is IArrayDbIds)
            {
                return (IArrayDbIds)ids;
            }
            else
            {
                return NewArray(ids);
            }
        }

        /**
         * Ensure that the given IDbIds support fast "contains" operations.
         * 
         * @param ids
         * @return Array IDbIds.
         */
        public static ISetDbIds EnsureSet(IDbIds ids)
        {
            if (ids is ISetDbIds)
            {
                return (ISetDbIds)ids;
            }
            else
            {
                return NewHashSet(ids);
            }
        }

        /**
         * Ensure modifiable
         * 
         * @param ids
         * @return Array IDbIds.
         */
        public static IModifiableDbIds EnsureModifiable(IDbIds ids)
        {
            if (ids is IModifiableDbIds)
            {
                return (IModifiableDbIds)ids;
            }
            else
            {
                if (ids is IArrayDbIds)
                {
                    return NewArray(ids);
                }
                if (ids is IHashSetDbIds)
                {
                    return NewHashSet(ids);
                }
                return NewArray(ids);
            }
        }

        /**
         * Make a DbId pair.
         * 
         * @param id1 first ID
         * @param id2 second ID
         * 
         * @return DbId pair
         */
        public static IDbIdPair NewPair(IDbIdRef id1, IDbIdRef id2)
        {
            return DbIdFactoryBase.FACTORY.MakePair(id1, id2);
        }

        /**
         * Produce a random sample of the given IDbIds
         * 
         * @param source Original IDbIds
         * @param k k Parameter
         * @param seed Random generator seed
         * @return new IDbIds
         */
        public static IModifiableDbIds RandomSample(IDbIds source, int k, int seed)
        {
            return RandomSample(source, k, (long)seed);
        }

        /**
         * Produce a random sample of the given IDbIds
         * 
         * @param source Original IDbIds
         * @param k k Parameter
         * @param seed Random generator seed
         * @return new IDbIds
         */
        public static IModifiableDbIds RandomSample(IDbIds source, int k, long? seed)
        {
            if (k <= 0 || k > source.Count)
            {
                throw new ArgumentException("Illegal value for size of random sample: " + k + " > " + source.Count + " or < 0");
            }
            Random random;
            if (seed != null)
            {
                random = new Random((int)seed);
            }
            else
            {
                random = new Random();
            }
            // TODO: better balancing for different sizes
            // Two methods: constructive vs. destructive
            if (k < source.Count / 2)
            {
                IArrayDbIds aids = DbIdUtil.EnsureArray(source);
                IHashSetModifiableDbIds sample = DbIdUtil.NewHashSet(k);
                while (sample.Count < k)
                {
                    sample.Add(aids[random.Next(aids.Count)]);
                }
                return sample;
            }
            else
            {
                IArrayModifiableDbIds sample = DbIdUtil.NewArray(source);
                while (sample.Count > k)
                {
                    // Element to remove
                    int idx = random.Next(sample.Count);
                    // Remove last element
                    IDbId last = sample.RemoveAt(sample.Count - 1);
                    // Replace target element:
                    if (idx < sample.Count)
                    {
                        sample[idx] = last;
                    }
                }
                return sample;
            }
        }

        /**
         * Get a subset of the KNN result.
         * 
         * @param list Existing list
         * @param k k
         * @param <D> distance type
         * @return Subset
         */

        public static IKNNList SubList(IKNNList list, int k)
        {
            if (k >= list.Count)
            {
                return list;
            }
            if (list is IDoubleDistanceKNNList)
            {
                return (IKNNList)new DoubleDistanceKNNSubList((IDoubleDistanceKNNList)list, k);
            }
            return new KNNSubList(list, k);
        }

        /**
         * Test two DbIds for equality.
         * 
         * @param id1 First ID
         * @param id2 Second ID
         * @return Comparison result
         */
        public static bool IsEqual(IDbIdRef id1, IDbIdRef id2)
        {
            return DbIdFactoryBase.FACTORY.IsEqual(id1, id2);
        }
        /**
      * Compare two DbIds.
      * 
      * @param id1 First ID
      * @param id2 Second ID
      * @return Comparison result
      */
        public static int Compare(IDbIdRef id1, IDbIdRef id2)
        {
            return DbIdFactoryBase.FACTORY.Compare(id1, id2);
        }

        /**
         * Create an appropriate heap for the distance type.
         * 
         * This will use a double heap if appropriate.
         * 
         * @param distancetype distance prototype
         * @param k K value
         * @param <D> distance type
         * @return New heap of size k, appropriate for this distance type.
         */
        public static IKNNHeap NewHeap(IDistanceValue distancetype, int k)
        {
            return DbIdFactoryBase.FACTORY.NewHeap(distancetype, k);
        }

        /**
         * Create an appropriate heap for double distances.
         * 
         * @param k K value
         * @return New heap of size k, appropriate for this distance type.
         */
        public static IDoubleDistanceKNNHeap NewDoubleDistanceHeap(int k)
        {
            return DbIdFactoryBase.FACTORY.NewDoubleDistanceHeap(k);
        }

        /**
         * Build a new heap from a given list.
         * 
         * @param exist Existing result
         * @param <D> Distance type
         * @return New heap
         */
        public static IKNNHeap NewHeap(IKNNList exist)
        {
            return DbIdFactoryBase.FACTORY.NewHeap(exist);
        }
                /**
         * Dereference a DBID reference.
         * 
         * @param ref DBID reference
         * @return DBID
         */
        public static IDbId Deref(IDbIdRef _ref)
        {
            if (_ref is IDbId)
            {
                return (IDbId)_ref;
            }
            return ImportInt32(_ref.InternalGetIndex());
        }


    }
}
