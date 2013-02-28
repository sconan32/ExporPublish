using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;


namespace DatasetGen
{
    public class DataGen
    {
        private int seed;

        public int Seed
        {
            get { return seed; }
            set { seed = value; }
        }
        private int datacnt;

        public int Datacnt
        {
            get { return datacnt; }
            set { datacnt = value; }
        }
        private int dimensioncnt;

        public int Dimensioncnt
        {
            get { return dimensioncnt; }
            set { dimensioncnt = value; }
        }
        private int clustercnt;

        public int Clustercnt
        {
            get { return clustercnt; }
            set { clustercnt = value; }
        }

        private double minValue;

        public double MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }
        private double maxValue;

        public double MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }

        public virtual void GenerateDatasetToStream(Stream stream)
        {

            Random random = new Random(seed);
            StreamWriter sw = new StreamWriter(stream);

            double range = Math.Abs(maxValue - minValue);
            double step = range / clustercnt;
            double[] mubase = new double[dimensioncnt];
            mubase = mubase.Select(t => random.NextDouble() * range + minValue).ToArray();
            double twopercentrange = range * 0.02;
            double[] ClusterRatioCnt = new double[clustercnt];
            int[] clusterObjCnt = new int[clustercnt];
            int tki = 0;
            while (tki < clustercnt - 1)
            {
                double rdcnt = 0;
                double sumratio = ClusterRatioCnt.Sum();
                while (rdcnt < 1.0 / clustercnt / 2 ||
                    rdcnt > 1.0 / clustercnt * 2 ||
                    sumratio > (1.0 - 1 / clustercnt / 2))
                {
                    rdcnt = random.NextDouble();
                    sumratio = ClusterRatioCnt.Sum() + rdcnt;
                }
                ClusterRatioCnt[tki++] = rdcnt;
            }

            clusterObjCnt = ClusterRatioCnt.Select(t => (int)t * datacnt).ToArray();
            clusterObjCnt[clustercnt - 1] = datacnt - clusterObjCnt.Sum();
            for (int ki = 0; ki < clustercnt; ki++)
            {
                double[] mu = new double[dimensioncnt];
                double[] sigma = new double[dimensioncnt];
                for (int di = 0; di < dimensioncnt; di++)
                {
                    mu[di] = mubase[di] + ki * step;
                    sigma[di] = di * Math.Sqrt(range) * (random.NextDouble() + 0.2);
                }
                var list = GenerateCluster(random, clusterObjCnt[ki], mu, sigma);

                foreach (var data in list)
                {
                    foreach (var num in data)
                    {
                        sw.Write(num);
                        sw.Write(",");
                    }
                    sw.Write("Cluster-");
                    sw.Write(ki.ToString());
                    sw.Write(Environment.NewLine);
                }
            }
            sw.Close();
        }
        protected virtual IList<Double[]> GenerateCluster(Random random, int cnt, double[] mean, double[] sigma)
        {
            Normal[] nds = new Normal[dimensioncnt];

            List<IEnumerator<double>> clusterT = new List<IEnumerator<double>>();
            for (int di = 0; di < dimensioncnt; di++)
            {
                nds[di] = new Normal(mean[di], sigma[di]);
                nds[di].RandomSource = random;
                clusterT.Add(nds[di].Samples().GetEnumerator());
            }

            List<double[]> res = new List<double[]>();
            for (int ci = 0; ci < cnt; ci++)
            {
                double[] tdata = new double[dimensioncnt];
                for (int di = 0; di < dimensioncnt; di++)
                {
                    clusterT[di].MoveNext();
                    tdata[di] = clusterT[di].Current;
                }
                res.Add(tdata);
            }
            return res;
        }


    }
}
