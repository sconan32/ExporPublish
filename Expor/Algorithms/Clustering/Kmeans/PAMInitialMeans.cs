using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Databases.DataStore;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Maths;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Algorithms.Clustering.KMeans
{
    public class PAMInitialMeans<V> : IKMeansInitialization<V>, IKMedoidsInitialization
    {
        /**
         * Constructor.
         */
        public PAMInitialMeans()
        {

        }


        public IList<V> ChooseInitialMeans(IRelation relation, int k, IPrimitiveDistanceFunction<V> distanceFunction)
        {
            // Get a distance query
            if (!(distanceFunction.DistanceFactory is NumberDistanceValue<V>))
            {
                throw new AbortException("PAM initialization can only be used with numerical distances.");
            }

            IPrimitiveDistanceFunction<V> distF = (IPrimitiveDistanceFunction<V>)distanceFunction;
            IDistanceQuery distQ = relation.GetDatabase().GetDistanceQuery(relation, distF, null);
            IDbIds medids = ChooseInitialMedoids(k, distQ);
            IList<V> medoids = new List<V>(k);
            foreach (var id in medids)
            {
                medoids.Add((V)relation[id]);
            }

            return medoids;
        }


        public IDbIds ChooseInitialMedoids(int k, IDistanceQuery distQ2)
        {
            if (!(distQ2.DistanceFactory is NumberDistanceValue<V>))
            {
                throw new AbortException("PAM initialization can only be used with numerical distances.");
            }

            IDistanceQuery distQ = (IDistanceQuery)distQ2;
            IDbIds ids = distQ.Relation.GetDbIds();

            IArrayModifiableDbIds medids = DbIdUtil.NewArray(k);
            double best = Double.PositiveInfinity;
            Mean mean = new Mean(); // Mean is numerically more stable than sum.
            IWritableDoubleDataStore mindist = null;

            // First mean is chosen by having the smallest distance sum to all others.
            {
                IDbId bestid = null;
                IWritableDoubleDataStore bestd = null;
                foreach (var dbid in ids)
                {
                    IWritableDoubleDataStore newd = DataStoreUtil.MakeDoubleStorage(ids, DataStoreHints.Hot | DataStoreHints.Temp);
                    mean.Reset();
                    foreach (var dbid2 in ids)
                    {
                        double d = (distQ.Distance(dbid, dbid2) as DoubleDistanceValue).DoubleValue();
                        mean.Put(d);
                        newd[dbid2] = d;
                    }
                    if (mean.GetMean() < best)
                    {
                        best = mean.GetMean();
                        bestid = dbid.DbId;
                        if (bestd != null)
                        {
                            bestd.Destroy();
                        }
                        bestd = newd;
                    }
                    else
                    {
                        newd.Destroy();
                    }
                }
                medids.Add(bestid);
                mindist = bestd;
            }
            Debug.Assert(mindist != null);

            // Subsequent means optimize the full criterion.
            for (int i = 1; i < k; i++)
            {
                IDbId bestid = null;
                IWritableDoubleDataStore bestd = null;
                foreach (var dbid in ids)
                {
                    IDbId id = dbid.DbId;
                    if (medids.Contains(id))
                    {
                        continue;
                    }
                    IWritableDoubleDataStore newd = DataStoreUtil.MakeDoubleStorage(
                        ids, DataStoreHints.Hot | DataStoreHints.Temp);
                    mean.Reset();
                    foreach (var dbid2 in ids)
                    {
                        IDbId other = dbid2.DbId;
                        double dn = (distQ.Distance(dbid, dbid2) as DoubleDistanceValue).DoubleValue();
                        double v = Math.Min(dn, (double)mindist[(other)]);
                        mean.Put(v);
                        newd[other] = v;
                    }
                    Debug.Assert(mean.GetCount() == ids.Count);
                    if (mean.GetMean() < best)
                    {
                        best = mean.GetMean();
                        bestid = id;
                        if (bestd != null)
                        {
                            bestd.Destroy();
                        }
                        bestd = newd;
                    }
                    else
                    {
                        newd.Destroy();
                    }
                }
                if (bestid == null)
                {
                    throw new AbortException("No median found that improves the criterion function?!?");
                }
                medids.Add(bestid);
                mindist.Destroy();
                mindist = bestd;
            }

            mindist.Destroy();
            return medids;
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public class Parameterizer : AbstractParameterizer
        {

            protected override object MakeInstance()
            {
                return new PAMInitialMeans<V>();
            }


        }
    }
}
