using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.KnnQueries
{

    public sealed class KNNUtil
    {
        /**
         * Sublist of an existing result to contain only the first k elements.
         * 
         * @author Erich Schubert
         * 
         * @param <D> Distance
         */
        internal class KNNSubList : Wintellect.PowerCollections.ListBase<IDistanceResultPair>, IKNNResult
        {
            /**
             * Parameter k
             */
            private readonly int k;

            /**
             * Actual size, including ties
             */
            private readonly int size;

            /**
             * Wrapped inner result.
             */
            private readonly IKNNResult inner;

            /**
             * Constructor.
             * 
             * @param inner Inner instance
             * @param k k value
             */
            public KNNSubList(IKNNResult inner, int k)
            {
                this.inner = inner;
                this.k = k;
                // Compute list size
                // TODO: optimize for double distances.
                {
                    IDistanceResultPair dist = inner.Get(k);
                    int i = k;
                    while (i + 1 < inner.Count)
                    {
                        if (dist.CompareByDistance(inner.Get(i + 1)) < 0)
                        {
                            break;
                        }
                        i++;
                    }
                    size = i;
                }
            }


            public int GetK()
            {
                return k;
            }


            public IDistanceResultPair Get(int index)
            {
                Debug.Assert(index < size, "Access beyond design size of list.");
                return inner.Get(index);
            }


            public IDistanceValue GetKNNDistance()
            {
                return inner.Get(k).GetDistance();
            }


            public IArrayDbIds AsDbIds()
            {
                return KNNUtil.AsDbIds(this);
            }

            public IList<IDistanceValue> AsDistanceList()
            {
                return KNNUtil.AsDistanceList(this);
            }




            public int Size()
            {
                return size;
            }

            public override void Clear()
            {
                throw new NotImplementedException();
            }

            public override int Count
            {
                get { throw new NotImplementedException(); }
            }

            public override void Insert(int index, IDistanceResultPair item)
            {
                throw new NotImplementedException();
            }

            public override void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            public override IDistanceResultPair this[int index]
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
        }
        /**
         * A view on the DbIds of the result
         * 
         * @author Erich Schubert
         */
        internal class DbIdView : IArrayDbIds
        {
            /**
             * The true list.
             */
            readonly IKNNResult parent;

            /**
             * Constructor.
             * 
             * @param parent Owner
             */
            public DbIdView(IKNNResult parent)
            {

                this.parent = parent;
            }


            public IDbId Get(int i)
            {
                return parent.Get(i).DbId;
            }

            public int Size()
            {
                return parent.Size();
            }


            public bool Contains(IDbIdRef o)
            {

                foreach (var id in parent)
                {
                    if (o == id)
                        return true;
                }
                return false;
            }


            public bool IsEmpty()
            {
                return parent.Count == 0;
            }

            /**
             * A binary search does not make sense here, as the (read-only) result is sorted by
             * distance, not DbId. Thus unsupported.
             */
            [Obsolete]
            public int BinarySearch(IDbIdRef key)
            {
                throw new InvalidOperationException("Since the result is usually not sorted, a binary Search does not make sense!");
            }

            public int Count
            {
                get { throw new NotImplementedException(); }
            }

            public IEnumerator<IDbId> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public IDbId this[int index]
            {
                get
                {
                    return parent.Get(index).DbId;
                }
                set
                {
                    throw new InvalidOperationException();
                }
            }

            IDbId IArrayDbIds.this[int index]
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            int IArrayDbIds.BinarySearch(IDbIdRef key)
            {
                throw new NotImplementedException();
            }

            IArrayDbIds IArrayDbIds.Slice(int begin, int end)
            {
                throw new NotImplementedException();
            }

            int IDbIds.Count
            {
                get { throw new NotImplementedException(); }
            }

            bool IDbIds.Contains(IDbIdRef o)
            {
                throw new NotImplementedException();
            }

            bool IDbIds.IsEmpty()
            {
                throw new NotImplementedException();
            }

            IEnumerator<IDbId> IEnumerable<IDbId>.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        /**
         * A view on the Distances of the result
         * 
         * @author Erich Schubert
         */
        internal class DistanceView<D> : Wintellect.PowerCollections.ListBase<D>, IList<D>
        where D : IDistanceValue
        {
            /**
             * The true list.
             */
            readonly IKNNResult parent;

            /**
             * Constructor.
             * 
             * @param parent Owner
             */
            public DistanceView(IKNNResult parent)
                : base()
            {

                this.parent = parent;
            }



            public override void Clear()
            {
                throw new NotImplementedException();
            }

            public override int Count
            {
                get { throw new NotImplementedException(); }
            }

            public override void Insert(int index, D item)
            {
                throw new NotImplementedException();
            }

            public override void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            public override D this[int index]
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
        }

        /**
         * View as ArrayDbIds
         * 
         * @param list Result to proxy
         * @return Static DbIds
         */
        public static IArrayDbIds AsDbIds(IKNNResult list)
        {
            return new DbIdView((IKNNResult)list);
        }

        /**
         * View as list of distances
         * 
         * @param list Result to proxy
         * @return List of distances view
         */
        public static IList<IDistanceValue> AsDistanceList(IKNNResult list)
        {
            return new DistanceView<IDistanceValue>(list);
        }

        /**
         * Get a subset of the KNN result.
         * 
         * @param list Existing list
         * @param k k
         * @return Subset
         */
        public static IKNNResult SubList<D>(IKNNResult list, int k)
        where D : IDistanceValue
        {
            if (k >= list.Count)
            {
                return list;
            }
            return new KNNSubList(list, k);
        }
    }
}
