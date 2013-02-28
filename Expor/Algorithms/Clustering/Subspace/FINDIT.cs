using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Int32DbIds;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.DataStructures.Heap;
using Socona.Expor.Utilities.Extenstions;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;
using Wintellect.PowerCollections;

namespace Socona.Expor.Algorithms.Clustering.Subspace
{
    public class FINDIT : AbstractAlgorithm, ISubspaceClusteringAlgorithm
    {
        private static Logging logger = Logging.GetLogger(typeof(FINDIT));

        private static OptionDescription C_MINSIZE_ID = OptionDescription.GetOrCreate("findit.cminsize",
          "The Minimun size of a cluster");
        private static OptionDescription D_MINDIST_ID = OptionDescription.GetOrCreate("findit.dmindist",
            "the minimun that merge two clusters");
        private static OptionDescription EPSILON_ID = OptionDescription.GetOrCreate("findit.epsilon",
            "The probability threshold");
        protected static BitArray allOneMask;

        protected static readonly int eCount = 25;
        protected int sMinSize;
        protected int cMinSize;
        protected double[] epsilons;

        protected int dMinDist;
        protected double delta;
        protected double valuerange;


        protected IModifiableDbIds samples;
        protected List<INumberVector> medoids;
        protected int dbColumnCnt = 0;
        protected INumberVector factory;

        //Test Best Epsilon

        double bestsoundness = 0;



        protected IList<INumberVector> means = null;
        protected IList<Subspace<INumberVector>> subspaces = null;
        protected IList<IModifiableDbIds> clusters = null;
        protected IModifiableDbIds outliers = null;

        public FINDIT(int cminsize, int dmindist, double delta)
            : base()
        {

            this.cMinSize = cminsize;
            this.dMinDist = dmindist;
            this.delta = delta;

        }
        public virtual ClusterList Run(IDatabase database, IRelation relation)
        {

            Initialize(database, relation);
            int numRecords = relation.Count;
            int ssize = ChernoffBounds(numRecords, sMinSize);
            int mminsize = 1;
            int msize = ChernoffBounds(numRecords, mminsize);
            List<MedoidCluster> bestmclust = new List<MedoidCluster>();
            int bestEidx = -1;
            samples = SelectSamples(relation, ssize);
            medoids = SelectMedoids(relation, msize);

            TestBestEpsilon(relation, bestmclust, ref bestEidx);

            //Step4 Assign all points
            AssignAllPoints(relation, bestmclust, bestEidx);
            ClusterList result = new ClusterList("FINDIT", "FINDIT-clustering");
            INumberVector factory = DatabaseUtil.AssumeVectorField<INumberVector>(relation).GetFactory();
            for (int i = 0; i < clusters.Count; i++)
            {
                SubspaceModel<INumberVector> model = new SubspaceModel<INumberVector>(subspaces[i],
                    factory.NewNumberVector(means[i].GetColumnVector().GetArrayRef()));
                result.AddCluster(new Cluster(clusters[i], model));
            }
            result.AddCluster(new Cluster(ids: outliers, noise: true, model: ClusterModel.CLUSTER));
            return result;
        }
        protected virtual void AssignAllPoints(IRelation relation, List<MedoidCluster> bestmclust, int bestEidx)
        {
            if (logger.IsDebugging)
            {
                logger.Debug("Step 4: Assign all points");
            }
            for (int mci = 0; mci < bestmclust.Count; mci++)
            {
                means.Add(bestmclust[mci].FirstMedoid);
                clusters.Add(DbIdUtil.NewHashSet());
                subspaces.Add(new Subspace<INumberVector>(bestmclust[mci].ClusterSubspace));
            }
            var bestE = epsilons[bestEidx];
            
            foreach (var id in relation)
            {
                int mindod = dbColumnCnt;
                int nearmcidx = -1;
                var vi = relation[id] as INumberVector;
                for (int mci = 0; mci < bestmclust.Count; mci++)
                {
                    var mcisubspace = bestmclust[mci].ClusterSubspace;
                    for (int mi = 0; mi < bestmclust[mci].Medoids.Count(); mi++)
                    {
                        var miv = bestmclust[mci].Medoids.ElementAt(mi);
                        var curdode = Dode(bestE, allOneMask, vi, mcisubspace, miv);
                        if (Dode(bestE, mcisubspace, miv, allOneMask, vi) == 0 &&
                            curdode < mindod)
                        {
                            mindod = curdode;
                            nearmcidx = mci;
                        }
                    }
                }
                if (nearmcidx >= 0)
                {
                    clusters[nearmcidx].Add(id);
                }
                else
                {
                    outliers.Add(id);
                }
            }
            logger.Debug("Outliers: " + FormatUtil.Format(outliers));
        }
        protected virtual void DimensionVoting(IRelation relation, int ei, int vnearcnt, List<MedoidCluster> mcls)
        {
            //PHASE 1: determine support dbColumnCnt
            //Determine key dimension for points in M

            if (logger.IsDebugging)
            {
                logger.Debug("Step 1: determine support dimension");
            }
            double curepsilon = epsilons[ei];

            //Calc how many Yes voter can tell a related dim
            double prob = 2 * curepsilon / valuerange;
            int x = 0;
            MathNet.Numerics.Distributions.Binomial b =
                new MathNet.Numerics.Distributions.Binomial(prob, vnearcnt);

            for (x = vnearcnt; x >= 0; x--)
            {
                if (b.CumulativeDistribution(x) < 1 - 1e-3)
                {
                    break;
                }
            }
            logger.Debug("x=" + x.ToString());

            Heap<Tuple<IDbId, double>> vnear =
                new Heap<Tuple<IDbId, double>>(vnearcnt, (t1, t2) => { return t1.Item2.CompareTo(t2.Item2); });

            foreach (INumberVector mv in medoids)
            {
                vnear.Clear();
                foreach (var sid in samples)
                {
                    var dist = Dode(curepsilon, allOneMask, mv, allOneMask, relation[sid] as INumberVector);
                    vnear.Offer(new Tuple<IDbId, double>(sid, dist));
                }

                //Calc Support dim
                var voteresult = new int[dbColumnCnt];

                for (int vi = 0; vi < vnear.Count; vi++)
                {
                    INumberVector viv = relation[vnear[vi].Item1] as INumberVector;
                    for (int di = 0; di < dbColumnCnt; di++)
                    {
                        if (Math.Abs(mv[di] - viv[di]) - curepsilon <= float.Epsilon)
                        {
                            voteresult[di]++;
                        }
                    }
                }

                //tell support dim
                BitArray bitmask = new BitArray(dbColumnCnt);
                for (int di = 0; di < dbColumnCnt; di++)
                {
                    if (voteresult[di] > x)
                    {
                        bitmask.Set(di, true);
                    }
                }
                mcls.Add(new MedoidCluster(mv, bitmask));
            }
        }
        protected virtual void TestBestEpsilon(IRelation relation,
            List<MedoidCluster> mclusters, ref int bestEidx)
        {
            int vnearcnt = 10 + sMinSize / 2;
            //Test Best Epsilon
            double[] evec = new double[dbColumnCnt];
            List<MedoidCluster> bestmclust = null;
            List<MedoidCluster> mcls = new List<MedoidCluster>();
            //double bestsoundness = 0;
            for (int ei = 15; ei < eCount; ei++)
            {
                double curepsilon = epsilons[ei];
                if (logger.IsDebugging)
                {
                    logger.Debug(string.Format("Start Testing Epsilon = {0}", curepsilon));
                }

                //Set<int> curoutliers = new Set<int>();
                mcls.Clear();

                //PHASE 1: determine support dimension
                DimensionVoting(relation, ei, vnearcnt, mcls);
                //PHASE 2.1: Assign points to nearest 
                AssignSamplesToMedoids(relation, ei, mcls);
                //PHASE 2.2 : Clustering Ms using PAC
                ClusterMedoids(relation, curepsilon, mcls);
                //Step 2.3 Medoid cluster tuning.
                MedoidClusterTuning(mcls);
                //TODO:Here need refine the mcluster
                //STEP 3 clac soundness
                double sound = ComputeSoundness(mcls);
                if (sound > bestsoundness)
                {
                    bestsoundness = sound;
                    bestEidx = ei;
                    bestmclust = mcls;

                }
                if (logger.IsDebugging)
                {
                    logger.Debug(string.Format(
                        "\tBest E-Idx: {0}, Sound: {1} | Current E-Idx: {2}, Sound: {3}",
                        bestEidx, bestsoundness, ei, sound));
                }
            }//end for epsilon
            mclusters.AddRange(bestmclust);
        }
        protected virtual double ComputeSoundness(List<MedoidCluster> mcls)
        {
            if (logger.IsDebugging)
            {
                logger.Debug("Step 3: clac soundness");
            }
            double sound = 0;
            for (int mci = 0; mci < mcls.Count; mci++)
            {
                sound += mcls[mci].Count * mcls[mci].ClusterSubspace.Cardinality();
            }
            return sound;
        }
        protected virtual void MedoidClusterTuning(List<MedoidCluster> mcls)
        {
            if (logger.IsDebugging)
            {
                logger.Debug("Step 2.3 Medoid cluster tuning.");
            }

            for (int mci = 0; mci < mcls.Count; mci++)
            {
                var curmc = mcls[mci];
                BitArray curmcmask = new BitArray(dbColumnCnt);
                IDbIds ids = curmc.Members;
                int[] dcorcnt = new int[dbColumnCnt];
                int[] dcnt = new int[dbColumnCnt];
                foreach (var mi in curmc.Medoids)
                {
                    for (int di = 0; di < dbColumnCnt; di++)
                    {
                        if (curmc.BitMaskFor(mi)[di])
                        {
                            dcorcnt[di] += curmc.MemberCount(mi);
                        }
                        dcnt[di] += curmc.MemberCount(mi);
                    }
                }
                for (int di = 0; di < dbColumnCnt; di++)
                {
                    curmcmask[di] = (double)dcorcnt[di] / (double)dcnt[di] > 0.9;
                }
                curmc.ClusterSubspace = curmcmask;
            }
        }
        protected virtual void ClusterMedoids(IRelation relation, double curepsilon, List<MedoidCluster> mcls)
        {
            if (logger.IsDebugging)
            {
                logger.Debug("Step 2.2 : Clustering Ms using PAC");
            }
            OrderedBag<Tuple<MedoidCluster, MedoidCluster, double>> distances = new OrderedBag<Tuple<MedoidCluster, MedoidCluster, double>>(
                comparison: (t1, t2) =>
                {
                    int isEqual = t1.Item3.CompareTo(t2.Item3);
                    if (isEqual != 0)
                        return isEqual;
                    else
                    {
                        return (t1.Item1.Count + t1.Item2.Count).CompareTo(
                            t2.Item1.Count + t2.Item2.Count);
                    }
                });


            for (int mi1 = 0; mi1 < mcls.Count; mi1++)
            {
                var m1v = mcls[mi1].FirstMedoid;
                BitArray m1mask = mcls[mi1].BitMaskFor(m1v);
                for (int mi2 = mi1 + 1; mi2 < mcls.Count; mi2++)
                {
                    var m2v = mcls[mi2].FirstMedoid;
                    var m2mask = mcls[mi2].BitMaskFor(m2v);
                    var maxmedoiddod = Math.Max(
                                DodeMedoid(curepsilon, m1mask, m1v, m2mask, m2v),
                                DodeMedoid(curepsilon, m2mask, m2v, m1mask, m1v)
                            );
                    if (maxmedoiddod <= dMinDist)
                    {
                        distances.Add(new Tuple<MedoidCluster, MedoidCluster, double>(
                               mcls[mi1], mcls[mi2], maxmedoiddod));
                    }
                }
            }

            Set<MedoidCluster> deletedset = new Set<MedoidCluster>();
            while (distances.Count > 0 && distances[0].Item3 <= dMinDist)
            {
                var dist = distances.GetFirst();
                distances.RemoveFirst();
                //merge mc[i] and mc[j]
                var mc1 = dist.Item1;
                var mc2 = dist.Item2;

                if (deletedset.Contains(mc1) || deletedset.Contains(mc2))
                {
                    continue;
                }
                mcls.Remove(mc1);
                mcls.Remove(mc2);
                deletedset.Add(mc1);
                deletedset.Add(mc2);

                //logger.Debug(distances.Take(6));
                var newMc = mc1.Union(mc2);
                for (int mci1 = 0; mci1 < mcls.Count; mci1++)
                {
                    var mcdist = DodeMedoidCluster(curepsilon, relation, mcls[mci1], newMc);
                    distances.Add(new Tuple<MedoidCluster, MedoidCluster, double>
                        (mcls[mci1], newMc, mcdist));
                }
                mcls.Add(newMc);
            }
        }
        protected virtual void AssignSamplesToMedoids(IRelation relation, int ei,
            List<MedoidCluster> mcls)
        {
            Set<IDbId> outliers = new Set<IDbId>();
            //PHASE 2.1: Assign points to nearest M
            if (logger.IsDebugging)
            {
                logger.Debug("Step  2.1: Assign points to nearest M");
            }
            var curepsilon = epsilons[ei];

            foreach (var sid in samples)
            {
                int minmidx = -1;
                int minmdod = int.MaxValue;
                INumberVector curs = relation[sid] as INumberVector;
                for (int mi = 0; mi < mcls.Count; mi++)
                {
                    INumberVector curm = mcls[mi].FirstMedoid;
                    var bitm = mcls[mi].BitMaskFor(curm);
                    var dodeMiToSi = Dode(curepsilon, bitm, curm, allOneMask, curs);
                    if (dodeMiToSi == 0)
                    {
                        var dodeSiToMi = Dode(curepsilon, allOneMask, curs, bitm, curm);
                        var curmdod = Math.Max(dodeMiToSi, dodeSiToMi);
                        if (curmdod < minmdod)
                        {
                            minmidx = mi;
                            minmdod = curmdod;
                        }
                    }
                }
                if (minmidx >= 0)
                {
                    var mcl = mcls[minmidx];
                    mcl.AddMember(mcl.FirstMedoid, sid);
                }
                else
                {
                    //it is outlier.
                    outliers.Add(sid);
                }
            }
            logger.Debug("Outliers:" + FormatUtil.Format(outliers));
        }
        protected int Dode(double epsilon, BitArray frommask, INumberVector from, BitArray tomask, INumberVector to)
        {
            BitArray bf = new BitArray(frommask);
            BitArray bt = new BitArray(tomask);
            BitArray subdims = bf.And(bt);
            int smallthane = 0;
            int setbit = 0;
            for (setbit = subdims.NextSetBitIndex(0); setbit >= 0; setbit = subdims.NextSetBitIndex(setbit + 1))
            {
                if (Math.Abs(from[setbit] - to[setbit]) <= epsilon)
                {
                    ++smallthane;
                }
            }
            return frommask.Cardinality() - smallthane;
        }
        protected double DodeMedoidCluster(double epsilon, IRelation relation,
          MedoidCluster mc1, MedoidCluster mc2)
        {
            double sumupside = 0;
            int mcfromcnt = mc1.Count;
            int mctocnt = mc2.Count;

            for (int smii = 0; smii < mc1.MedoidCount; smii++)
            {
                var curm1 = mc1.MedoidAt(smii);
                var memfcnt = mc1.MemberCount(curm1);

                double sumj = 0;
                for (int smji = 0; smji < mc2.MedoidCount; smji++)
                {
                    var curm2 = mc2.MedoidAt(smji);
                    var memtcnt = mc2.MemberCount(curm2);

                    var distij = DodeMedoid(epsilon, mc1.BitMaskFor(curm1), curm1,
                       mc2.BitMaskFor(curm2), curm2);
                    var tij = memfcnt * memtcnt * distij;
                    sumj += tij;
                }
                sumupside += sumj;
            }
            double sumdownside = mcfromcnt * mctocnt;
            if (sumdownside == 0)
            {
                return double.PositiveInfinity;
            }
            return sumupside / sumdownside;
        }
        protected int DodeMedoid(double epsilon, BitArray frommask, INumberVector from, BitArray tomask, INumberVector to)
        {
            int smallthane = 0;
            BitArray bf = new BitArray(frommask);
            BitArray bt = new BitArray(tomask);
            BitArray subdims = bf.And(bt);
            int setbit = 0;
            for (setbit = subdims.NextSetBitIndex(0);
                setbit >= 0;
                setbit = subdims.NextSetBitIndex(setbit + 1))
            {
                if (Math.Abs(from[setbit] - to[setbit]) <= epsilon)
                {
                    ++smallthane;
                }
            }
            if (smallthane <= 2)
            {
                return frommask.Cardinality();
            }
            return Math.Max(frommask.Cardinality(), tomask.Cardinality()) - smallthane;
        }

        protected virtual void Initialize(IDatabase db, IRelation relation)
        {

            bestsoundness = 0;
            means = new List<INumberVector>();
            clusters = new List<IModifiableDbIds>();
            subspaces = new List<Subspace<INumberVector>>();
            outliers = DbIdUtil.NewHashSet();

            dbColumnCnt = DatabaseUtil.Dimensionality(relation);
            this.factory = DatabaseUtil.AssumeVectorField<INumberVector>(relation).GetFactory();

            this.sMinSize = (int)(relation.Count * 0.015);

            double max = 0;
            double min = int.MaxValue;

            allOneMask = new BitArray(dbColumnCnt);
            allOneMask = allOneMask.Not();

            foreach (var id in relation)
            {
                DoubleVector data = relation[id] as DoubleVector;
                var tmax = data.GetValues().Max();
                var tmin = data.GetValues().Min();
                if (tmax > max)
                {
                    max = tmax;
                }
                if (tmin < min)
                {
                    min = tmin;
                }
            }
            epsilons = new double[eCount];

            valuerange = max - min;
            logger.Debug("Value Range of Dataset is " + valuerange.ToString());
            for (int i = 0; i < eCount; i++)
            {
                epsilons[i] = (max - min) * (i + 1) / 100;
            }




        }

        protected int ChernoffBounds(int dataCount, int minSize)
        {
            int k = dataCount / cMinSize;
            int p = 1;
            int e = minSize;
            double t = Math.Pow(Math.Log(1.0 / delta), 2) + 2 * e * Math.Log(1.0 / delta);
            int res = (int)(e * k * p +
                k * p * Math.Log(1.0 / delta) + k * p * Math.Sqrt(t));
            return res;
        }
        protected virtual List<INumberVector> SelectMedoids(IRelation relation, int size)
        {
            List<INumberVector> list = new List<INumberVector>();
            IModifiableDbIds dbids = DbIdUtil.RandomSample(relation.GetDbIds(), size, null);
            foreach (var id in dbids)
            {
                list.Add(relation.VectorAt(id));
            }
            return list;
        }
        protected virtual IModifiableDbIds SelectSamples(IRelation relation, int size)
        {
            return DbIdUtil.RandomSample(relation.GetDbIds(), size, null);
        }
        public override Data.Types.ITypeInformation[] GetInputTypeRestriction()
        {
            return TypeUtil.Array(TypeUtil.NUMBER_VECTOR_FIELD);
        }
        protected override Log.Logging GetLogger()
        {
            return logger;
        }

        protected class MedoidCluster
        {
            Dictionary<INumberVector, Tuple<INumberVector, IDbIds, BitArray>> store;
            BitArray clusterSubspace;

            public BitArray ClusterSubspace
            {
                get { return clusterSubspace; }
                set { clusterSubspace = value; }
            }

            public MedoidCluster(INumberVector medoid, BitArray bitmask)
                : this(medoid, DbIdUtil.NewHashSet(), bitmask)
            { }
            public MedoidCluster(INumberVector medoid, IDbIds members, BitArray bitmask)
            {
                store = new Dictionary<INumberVector, Tuple<INumberVector, IDbIds, BitArray>>();
                store.Add(medoid,
                    new Tuple<INumberVector, IDbIds, BitArray>(medoid, members, bitmask));
            }
            protected MedoidCluster(Dictionary<INumberVector, Tuple<INumberVector, IDbIds, BitArray>> store)
            {
                this.store = new Dictionary<INumberVector, Tuple<INumberVector, IDbIds, BitArray>>(store);
            }
            public void AddMedoid(INumberVector medoid, BitArray bitmask)
            {
                AddMedoid(medoid, DbIdUtil.NewHashSet(), bitmask);
            }
            public void AddMedoid(INumberVector medoid, IDbIds members, BitArray bitmask)
            {
                if (store.ContainsKey(medoid))
                {
                    logger.Warning("Key Repeated, Old keyvaluepair will be deleted");
                    store.Remove(medoid);
                }
                store.Add(medoid, new Tuple<INumberVector, IDbIds, BitArray>(
                    medoid, members, bitmask));
            }
            public void AddMember(INumberVector medoid, IDbId member)
            {
                var value = store[medoid];
                DbIdUtil.EnsureModifiable(value.Item2).Add(member);
                //  store[medoid] = value;
            }
            public BitArray BitMaskFor(INumberVector medoid)
            {
                var value = store[medoid];
                return value.Item3;
            }
            public INumberVector FirstMedoid
            {
                get { return store.Keys.ElementAt(0); }
            }
            public int Count
            {
                get { return store.Count + store.Select(t => t.Value.Item2.Count).Sum(); }
            }
            public INumberVector MedoidAt(int index)
            {
                return store.Keys.ElementAt(index);
            }
            public int MedoidCount
            {
                get { return store.Count; }
            }
            public int MemberCount(INumberVector modoid)
            {
                if (store.ContainsKey(modoid))
                {
                    return store[modoid].Item2.Count + 1;
                }
                return 0;
            }
            public IEnumerable<INumberVector> Medoids
            {
                get { return store.Keys; }
            }
            public IDbIds Members
            {
                get
                {
                    IModifiableDbIds ids = DbIdUtil.NewHashSet();
                    store.ToList().ForEach(t => ids.AddDbIds(t.Value.Item2));
                    return ids;
                }
            }
            public IDbIds MemberFor(INumberVector medoid)
            {
                var value = store[medoid];
                return value.Item2;
            }
            public MedoidCluster Union(MedoidCluster mc2)
            {
                Dictionary<INumberVector, Tuple<INumberVector, IDbIds, BitArray>> news = new Dictionary<INumberVector, Tuple<INumberVector, IDbIds, BitArray>>(this.store);
                foreach (var item in mc2.store)
                {
                    if (!news.ContainsKey(item.Key))
                    {
                        news.Add(item.Key, item.Value);
                    }
                    else
                    {
                        logger.Warning("Duplicate Medoids!");
                    }
                }
                return new MedoidCluster(news);
            }
            public override string ToString()
            {
                return "{Msize=" + this.Medoids.Count() + ", Ssize=" + this.Members.Count + "}";
            }
        }

        public class Parameterizer : AbstractParameterizer
        {
            //protected IDistance epsilon = null;
            protected double delta = 0;
            protected int cminsize = 0;
            protected int dmindist = 0;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                DoubleParameter epsilonP = new DoubleParameter(EPSILON_ID,
                    new IntervalConstraint<double>(0.0, IntervalConstraint<double>.IntervalBoundary.OPEN,
                        1.0, IntervalConstraint<double>.IntervalBoundary.OPEN), defaultValue: 0.01);
                if (config.Grab(epsilonP))
                {
                    delta = epsilonP.GetValue();
                }

                IntParameter cminsizeP = new IntParameter(C_MINSIZE_ID, new GreaterConstraint<int>(0));
                if (config.Grab(cminsizeP))
                {
                    cminsize = cminsizeP.GetValue();
                }
                IntParameter sminsizeP = new IntParameter(D_MINDIST_ID, new GreaterConstraint<int>(0));
                if (config.Grab(sminsizeP))
                {
                    dmindist = sminsizeP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new FINDIT(cminsize, dmindist, delta);
            }


        }
    }
}
