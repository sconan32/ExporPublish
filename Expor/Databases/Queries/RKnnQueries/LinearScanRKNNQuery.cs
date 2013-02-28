using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.RKnnQueries
{

    public class LinearScanRKNNQuery<O> : AbstractRKNNQuery<O>, ILinearScanQuery
        where O:IDataVector
    {
        /**
         * KNN query we use.
         */
        protected readonly IKNNQuery knnQuery;

        /**
         * Constructor.
         * 
         * @param distanceQuery Distance function to use
         * @param knnQuery kNN query to use.
         * @param maxk k to use
         */
        public LinearScanRKNNQuery(IDistanceQuery distanceQuery, IKNNQuery knnQuery, Int32 maxk) :
            base(distanceQuery)
        {
            this.knnQuery = knnQuery;
        }


        public override IList<IDistanceResultPair> GetRKNNForObject(O obj, int k)
        {
            List<IDistanceResultPair> rNNlist = new List<IDistanceResultPair>();

            IArrayDbIds allIDs = DbIdUtil.EnsureArray(relation.GetDbIds());
            IList<IKNNList> kNNLists = knnQuery.GetKNNForBulkDbIds(allIDs, k);

            int i = 0;
            foreach (var id in allIDs)
            {
                IKNNList knn = kNNLists[i];
                int last = Math.Min(k - 1, knn.Count - 1);
                IDistanceValue dist = distanceQuery.Distance(obj, id);
                if (last < k - 1 || dist.CompareTo(knn[last].Distance) < 1)
                {
                    rNNlist.Add(new GenericDistanceResultPair(dist, id));
                }
                i++;
            }
            rNNlist.Sort();
            return rNNlist;
        }


        public override IList<IDistanceResultPair> GetRKNNForDbId(IDbIdRef id, int k)
        {
            List<IDistanceResultPair> rNNList = new List<IDistanceResultPair>();

            IArrayDbIds allIDs = DbIdUtil.EnsureArray(relation.GetDbIds());
            IList<IKNNList> kNNList = knnQuery.GetKNNForBulkDbIds(allIDs, k);

            int i = 0;
            foreach (var id2 in allIDs)
            {
                IKNNList knn = kNNList[i];
                foreach (IDistanceResultPair n in (IEnumerable<IDistanceDbIdPair>)knn)
                {
                    if (n.IsSameDbId(id))
                    {
                        rNNList.Add(new GenericDistanceResultPair(n.GetDistance(), id2.DbId));
                    }
                }
                i++;
            }
            rNNList.Sort();
            return rNNList;
        }


        public override IList<IList<IDistanceResultPair>> GetRKNNForBulkDbIds(IArrayDbIds ids, int k)
        {
            List<IList<IDistanceResultPair>> rNNList =
                new List<IList<IDistanceResultPair>>(ids.Count);
            for (int i = 0; i < ids.Count; i++)
            {
                rNNList.Add(new List<IDistanceResultPair>());
            }

            IArrayDbIds allIDs = DbIdUtil.EnsureArray(relation.GetDbIds());
            IList<IKNNList> kNNList = knnQuery.GetKNNForBulkDbIds(allIDs, k);

            int ii = 0;
            foreach (var id2 in allIDs)
            {
                IDbId qid = id2;
                IKNNList knn = kNNList[ii];
                foreach (IDistanceDbIdPair n in (IEnumerable<IDistanceDbIdPair>)knn)
                {
                    int j = 0;
                    foreach (var id3 in ids)
                    {
                        if (n.DbId.IsSameDbId(id3))
                        {
                            IList<IDistanceResultPair> rNN = rNNList[j];
                            rNN.Add(new GenericDistanceResultPair(n.Distance, qid));
                        }
                        j++;
                    }
                }
                ii++;
            }
            for (int j = 0; j < ids.Count; j++)
            {
                IList<IDistanceResultPair> rNN = rNNList[j];
                (rNN as List<IDistanceResultPair>).Sort();
            }
            return rNNList;
        }
    }
}
