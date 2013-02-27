using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Clusters;
using Socona.Clustering.Datasets;

namespace Socona.Clustering.Algorithms
{
    public class FSC:AbstractAlgorithm
    {

        List<SubspaceCluster> clusters=new List<SubspaceCluster> ();
        List<int> CM;
        protected int numIter;
        protected double dObj;

        protected int seed;
        protected double alpha;
        protected double epsilon;
        protected double threshold;
        protected int numClust;
        protected int maxIter;


        protected override void SetupArguments()
        {
            base.SetupArguments();

            epsilon = (double)Arguments.Get("epsilon");

            threshold = (double)Arguments.Get("threshold");

            alpha = (double)Arguments.Get("alpha");
            object o = Arguments.Get("numclust");
            numClust = (int)o;

            maxIter = (int)Arguments.Get("maxiter");

            seed = (int)Arguments.Get("seed");
        }
        protected override void FetchResults()
        {
            PClustering pc=new PClustering ();
            for (int i = 0; i < clusters.Count; i++)
            {
                pc.Add(clusters[i]);
            }
            Results.CM = CM;
            Results.Insert("pc", pc);
            Results.Insert("numiter", numIter);
            Results.Insert("obj", dObj);
        }
        protected override void PerformClustering()
        {
            Initialize();
            Iterate();
        }
        protected virtual void Initialize()
        {
            int numRecords = dataset.Count;
            List<int> index = new List<int>(numRecords);
            CM = new List<int>(numRecords);
            for (int i = 0; i < numRecords; i++)
            {
                CM.Add(i);
                index.Add(i);
            }
            Random generator = new Random(seed);
            for (int i = 0; i < numClust; i++)
            {
                MathNet.Numerics.Distributions.DiscreteUniform uni =
                    new MathNet.Numerics.Distributions.DiscreteUniform(0, numRecords - i - 1);
                int r = uni.Sample();
                Record cr = dataset[r].Clone() as Record;
                SubspaceCluster c = new SubspaceCluster(cr);
                c.Id = i;
                clusters.Add(c);
                index.Remove(r);
            }
            int s = -1;
            double min, dist;

            for (int i = 0; i < numRecords; i++)
            {
                min = double.MaxValue;
                for (int j = 0; j < numClust; j++)
                {
                    dist = CalcDistance(dataset[i], clusters[j]);
                    if (dist < min)
                    {
                        s = j;
                        min = dist;
                    }
                }
                clusters[s].Add(dataset[i]);
                CM[i] = s;
            }


        }
        protected virtual double CalcDistance(Record lhs, SubspaceCluster rhs)
        {
            double tmp = 0;
            Schema schema = dataset.Schema;

            for (int j = 0; j < schema.Count; j++)
            {
                tmp += Math.Pow(rhs.Weights[j], alpha) * Math.Pow(schema[j].CalcDistance(lhs[j], rhs.Center[j]), 2);
            }

            return tmp;
        }
        protected virtual void Iterate()
        {
            double objPre;
            UpdateWeight();
            UpdateCenter();

            numIter = 1;
            while (true)
            {
                
                int s = 0;
                double min, dist;
                for (int i = 0; i < dataset.Count; i++)
                {
                    min = double.MaxValue;
                    for (int k = 0; k < clusters.Count; k++)
                    {
                        dist = CalcDistance(dataset[i], clusters[k]);
                        if (min > dist)
                        {
                            min = dist;
                            s = k;
                        }
                    }
                    if (CM[i] != s)
                    {
                        clusters[CM[i]].Remove(dataset[i]);
                        clusters[s].Add(dataset[i]);
                        CM[i] = s;
                        
                    }
                }
                UpdateWeight();
                UpdateCenter();

                objPre = dObj;
                CalculateObj();
                if (Math.Abs(objPre - dObj) < threshold)
                {
                    break;
                }

                numIter++;
                if (numIter > maxIter)
                {
                    break;
                }
            }
        }
        protected virtual void UpdateWeight()
        {
            double var, sum;
            Schema schema = dataset.Schema;
            Record c, r;
            List<double> w = new List<double>(schema.Count);

            for (int k = 0; k < clusters.Count; k++)
            {
                c = clusters[k].Center;
                sum = 0;
                for (int j = 0; j < schema.Count; j++)
                {
                    var = 0;
                    for (int i = 0; i < clusters[k].Count; i++)
                    {
                        r = clusters[k][i];
                        var += Math.Pow(schema[j].CalcDistance(r[j], c[j]), 2.0);

                    }
                    w.Add(Math.Pow(1 / (var + epsilon), 1 / (alpha - 1)));
                    sum += w[j];
                }
                for (int j = 0; j < schema.Count; j++)
                {
                    clusters[k].Weights[j] = w[j] / sum;
                }
                w.Clear();
            }

        }
        protected virtual void UpdateCenter()
        {
            double temp;
            Schema schema = dataset.Schema;
            for (int k = 0; k < clusters.Count; k++)
            {
                for (int j = 0; j < schema.Count; j++)
                {
                    temp = 0;
                    for (int i = 0; i < clusters[k].Count; ++i)
                    {
                        Record rec = clusters[k][i];
                        temp += schema[j].GetContinuousValue(rec[j]);
                    }
                    //schema[j].SetContinuousValue(
                    //    clusters[k].Center[j], temp / clusters[k].Count);
                    clusters[k].Center[j] = temp / clusters[k].Count;
                }
            }
        }
        protected virtual void CalculateObj()
        {
            double tmp = 0;
            for (int i = 0; i < dataset.Count; i++)
            {
                tmp += CalcDistance(dataset[i], clusters[CM[i]]);

            }
            dObj = tmp;
        }
    }
}
