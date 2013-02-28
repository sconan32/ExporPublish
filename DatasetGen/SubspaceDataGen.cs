using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Extenstions;
namespace DatasetGen
{
    public class SubspaceDataGen : DataGen
    {
        private IList<int[]> subsapcelist;

        public IList<int[]> Subsapcelist
        {
            get { return subsapcelist; }
            set { subsapcelist = value; }
        }
        public override void GenerateDatasetToStream(Stream stream)
        {

            Random random = new Random(Seed);
            StreamWriter sw = new StreamWriter(stream);

            double range = Math.Abs(MaxValue - MinValue);
            double step = range / Clustercnt;
            double[] mubase = new double[Dimensioncnt];
            mubase = mubase.Select(t => random.NextDouble() * range + MinValue).ToArray();
            double twopercentrange = range * 0.02;
            double[] ClusterRatioCnt = new double[Clustercnt];
            int[] clusterObjCnt = new int[Clustercnt];
            int tki = 0;
            while (tki < Clustercnt - 1)
            {
                double rdcnt =  random.NextDouble() / 2;;
                double sumratio = ClusterRatioCnt.Sum();
                double realratio = rdcnt * (1.0 - sumratio);
                if(realratio > 1.0/(Clustercnt*2))
                {
                    ClusterRatioCnt[tki++] = realratio;
                }
            }

            clusterObjCnt = ClusterRatioCnt.Select(t => (int)(t * Datacnt)).ToArray();
            clusterObjCnt[Clustercnt - 1] = Datacnt - clusterObjCnt.Sum();
            var bitmasks = GenerateBitmask();
            for (int ki = 0; ki < Clustercnt; ki++)
            {
                double[] mu = new double[Dimensioncnt];
                double[] sigma = new double[Dimensioncnt];
                for (int di = 0; di < Dimensioncnt; di++)
                {
                    mu[di] = mubase[random.Next(Dimensioncnt)] + ki * step;
                    sigma[di] = Math.Min(Math.Abs(MaxValue - mu[di]), Math.Abs(mu[di] - MinValue)) / 2;
                }
                var list = GenerateSubspaceCluster(random, clusterObjCnt[ki], mu, sigma, bitmasks[ki]);

                foreach (var data in list)
                {
                    foreach (var num in data)
                    {
                        sw.Write(FormatUtil.Format(num));
                        sw.Write(", ");
                    }
                    sw.Write("Cluster-");
                    sw.Write(ki.ToString());
                    sw.Write(Environment.NewLine);
                }
            }
            sw.Close();
        }
        public IList<BitArray> GenerateBitmask()
        {
            List<BitArray> bits = new List<BitArray>();
            int ki = 0;
            for (ki = 0; ki < this.subsapcelist.Count; ki++)
            {
                BitArray ba = new BitArray(Dimensioncnt);
                foreach (var id in subsapcelist[ki])
                {
                    ba[id - 1] = true;
                }
                bits.Add(ba);
            }
            if (ki < Clustercnt)
            {
                while (ki++ < Clustercnt)
                {
                    bits.Add(new BitArray(Dimensioncnt));
                }
            }
            return bits;
        }
        public virtual IList<double[]> GenerateSubspaceCluster(Random random,
           int cnt, double[] mean, double[] sigma, BitArray bitmask)
        {
            Normal[] nds = new Normal[Dimensioncnt];
            ContinuousUniform[] cuds = new ContinuousUniform[Dimensioncnt];
            List<IEnumerator<double>> clusterT = new List<IEnumerator<double>>();
            List<IEnumerator<double>> noiseT = new List<IEnumerator<double>>();
            for (int di = 0; di < Dimensioncnt; di++)
            {
                nds[di] = new Normal(mean[di], sigma[di]);
                cuds[di] = new ContinuousUniform(MinValue, MaxValue);
                nds[di].RandomSource = random;
                cuds[di].RandomSource = random;
                clusterT.Add(nds[di].Samples().GetEnumerator());
                noiseT.Add(cuds[di].Samples().GetEnumerator());
            }

            List<double[]> res = new List<double[]>();
            for (int ci = 0; ci < cnt; ci++)
            {
                double[] tdata = new double[Dimensioncnt];
                for (int di = 0; di < Dimensioncnt; di++)
                {
                    if (bitmask[di])
                    {
                        clusterT[di].MoveNext();
                        while (clusterT[di].Current > MaxValue || clusterT[di].Current < MinValue)
                        {
                            clusterT[di].MoveNext();
                        }
                        tdata[di] = clusterT[di].Current;
                    }
                    else
                    {

                        noiseT[di].MoveNext();
                        while (noiseT[di].Current > MaxValue || noiseT[di].Current < MinValue)
                        {
                            noiseT[di].MoveNext();
                        }
                        tdata[di] = noiseT[di].Current;
                    }
                }
                res.Add(tdata);
            }
            return res;
        }
    }
}
