using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Socona.Expor.Algorithms.Clustering.Trivial;
using Socona.Expor.Constraints.Pairwise;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.DataSources;
using Socona.Expor.DataSources.Parsers;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Utilities.DataStructures.Heap;

namespace DatasetGen
{
    public class ConstraintsGen
    {
        StaticArrayDatabase db;
        string fileName;
        int count;
        Random random;
        public ConstraintsGen(string dbfile, int count)
        {
            this.fileName = dbfile;
            this.count = count;
            random = new Random();
            db = new StaticArrayDatabase(
                new FileBasedDatabaseConnection(null,
                    new NumberVectorLabelParser(DoubleVector.STATIC), File.OpenRead(fileName)
                    ),
                null);

            db.Initialize();
        }

        public void GenerateConstraintsToStream(Stream stream)
        {
            StreamWriter sw = new StreamWriter(stream);
            EuclideanDistanceFunction edf = EuclideanDistanceFunction.STATIC;
            ByLabelClustering blc = new ByLabelClustering();
            IRelation data = db.GetRelation(TypeUtil.NUMBER_VECTOR_FIELD);
            Debug.Assert(data != null);
            ClusterList res = blc.Run(db) as ClusterList;
            var clusters = res.GetToplevelClusters();
            Heap<Tuple<IDbId, IDbId, double, Cluster>>[] inClusterBigDistance =
                new Heap<Tuple<IDbId, IDbId, double, Cluster>>[clusters.Count];
            for (int ki = 0; ki < clusters.Count; ki++)
            {
                inClusterBigDistance[ki] = new Heap<Tuple<IDbId, IDbId, double, Cluster>>(count, (t1, t2) => -t1.Item3.CompareTo(t2.Item3));
            }
            for (int ki = 0; ki < clusters.Count; ki++)
            {
                var clus = clusters[ki];
                for (int i1 = 0; i1 < clus.Ids.Count; i1++)
                {
                    var id1 = clus.Ids.ElementAt(i1);
                    for (int i2 = i1 + 1; i2 < clus.Ids.Count; i2++)
                    {
                        var id2 = clus.Ids.ElementAt(i2);
                        var dist = edf.DoubleDistance((INumberVector)data[id1],(INumberVector) data[id2]);

                        inClusterBigDistance[ki].Offer(new Tuple<IDbId, IDbId, double, Cluster>(
                            id1, id2, dist, clus));

                    }
                }
            }
            List<MustLink> mls = new List<MustLink>();

            List<CannotLink> cls = new List<CannotLink>();
            Heap<Tuple<IDbId, IDbId, double, Cluster>> clcandi =
                   new Heap<Tuple<IDbId, IDbId, double, Cluster>>(count * 8, (t1, t2) =>- t1.Item3.CompareTo(t2.Item3));

            for (int ki = 0; ki < clusters.Count; ki++)
            {

                int mladdcnt = 0;
                foreach (var tup in inClusterBigDistance[ki].ToSortedArrayList())
                {
                    int clccnt = 0;
                    foreach (var clu in clusters)
                    {
                        if (clu == tup.Item4)
                        {
                            continue;
                        }
                        foreach (var id in clu.Ids)
                        {
                            var dist1 = edf.DoubleDistance((INumberVector)data[id],(INumberVector) data[tup.Item1]);
                            var dist2 = edf.DoubleDistance((INumberVector)data[id],(INumberVector) data[tup.Item2]);
                            if (dist1 > tup.Item3)
                            {
                                clcandi.Offer(new Tuple<IDbId, IDbId, double, Cluster>(tup.Item1, id, dist1, clu));
                                clccnt++;
                            }
                            if (dist2 > tup.Item3)
                            {
                                clcandi.Offer(new Tuple<IDbId, IDbId, double, Cluster>(tup.Item2, id, dist2, clu));
                                clccnt++;
                            }
                            if (clccnt > clusters.Count)
                            {
                                break;
                            }
                        }
                    }
                    var cllefts = clcandi.Where(t => t.Item1.Equals(tup.Item1));
                    var clrights = clcandi.Where(t => t.Item1.Equals(tup.Item2));
                    if (cllefts != null && clrights != null && cllefts.Count() > 0 && clrights.Count() > 0)
                    {
                        if (mls.Count < count && mladdcnt++ <= count / clusters.Count)
                        {
                            var nml = new MustLink(tup.Item1, tup.Item2);
                            int clcount = 0;
                            if (!mls.Contains(nml))
                            {
                                //mls.Add(nml);
                                CannotLink tcl1 = null, tcl2 = null;
                                var ids = cllefts.Select(t => t.Item2).ToArray();
                                RandomDbIdArray(ids);
                                foreach (var id in ids)
                                {
                                    tcl1 = new CannotLink(tup.Item1, id);
                                    if (!cls.Contains(tcl1))
                                    {
                                        clcount++;
                                        break;
                                    }
                                }
                                ids = clrights.Select(t => t.Item2).ToArray();
                                RandomDbIdArray(ids);
                                foreach (var id in ids)
                                {
                                    tcl2 = new CannotLink(tup.Item2, id);
                                    if (!cls.Contains(tcl2))
                                    {
                                        clcount++;
                                        break;
                                    }
                                }
                                if (clcount == 2)
                                {
                                    mls.Add(nml);
                                    cls.Add(tcl1);
                                    cls.Add(tcl2);
                                }
                            }
                        }
                    }
                }
            }


            if (mls.Count < count)
            {
                //Show warning
            }
            foreach (var must in mls)
            {
                sw.Write(must.First.ToString());
                sw.Write(", ");
                sw.Write(must.Second.ToString());
                sw.Write(", M");
                sw.Write(Environment.NewLine);
            }
            foreach (var cannot in cls)
            {
                sw.Write(cannot.First.ToString());
                sw.Write(", ");
                sw.Write(cannot.Second.ToString());
                sw.Write(", C");
                sw.Write(Environment.NewLine);
            }
            sw.Close();
        }
        private void RandomDbIdArray(IDbId[] array)
        {
            for (int i = 0; i < array.Length * 2; i++)
            {
                int t = random.Next(array.Length);
                IDbId tmp = array[t];
                array[t] = array[0];
                array[0] = tmp;
            }
        }
    }
}
