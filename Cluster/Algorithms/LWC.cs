using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Clusters;
using Socona.Clustering.Datasets;

namespace Socona.Clustering.Algorithms
{
    /// <summary>
    /// LWC Algorithm
    /// Refer to:
    /// Hao Cheng, Kien A. Hua, and Khanh Vu. 2008. 
    /// Constrained locally weighted clustering. 
    /// Proc. VLDB Endow. 1, 1 (August 2008), 90-101. 
    /// DOI=10.1145/1453856.1453871 
    /// http://dx.doi.org/10.1145/1453856.1453871
    /// 
    /// Program Author: @sconan32
    /// </summary>
    class LWC:AbstractAlgorithm
    {
        protected List<SubspaceCluster> clusters = new List<SubspaceCluster>();
        protected List<int> CM;
        protected int numIter;
        protected double error;

        protected int seed;
        protected double epsilon;
        protected double threshold;
        protected int numClust;
        protected int maxIter;


        protected override void SetupArguments()
        {
            base.SetupArguments();

            epsilon = (double)Arguments.Get("epsilon");

            threshold = (double)Arguments.Get("threshold");

            object o = Arguments.Get("numclust");
            numClust = (int)o;
            maxIter = (int)Arguments.Get("maxiter");

            seed = (int)Arguments.Get("seed");
        }
        protected override void FetchResults()
        {
            PClustering pc = new PClustering();
            for (int i = 0; i < clusters.Count; i++)
            {
                pc.Add(clusters[i]);
            }
            Results.CM = CM;
            Results.Insert("pc", pc);
            Results.Insert("numiter", numIter);
            Results.Insert("error", error);
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
            //Random generator = new Random(seed);
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
            //set all weights to 1
            foreach (SubspaceCluster cc in clusters)
            {
                Schema schema = dataset.Schema;

                for (int i = 0; i < schema.Count; i++)
                {
                    cc.Weights[i] = 1 ;
                }
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
                tmp += rhs.Weights[j] * Math.Pow(
                    schema[j].CalcDistance(lhs[j], rhs.Center[j]), 
                    2);
            }

            return tmp;
        }
        protected virtual void Iterate()
        {
            double errorPre;
           
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

                errorPre = error;
                CalculateError();
                if (Math.Abs(errorPre - error) < threshold)
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
            double var, lambda ;
            Schema schema = dataset.Schema;
            Record c, r;
           

            for (int k = 0; k < clusters.Count; k++)
            {
                c = clusters[k].Center;
                 lambda = 0;
                for (int i = 0; i < schema.Count; i++)
                {
                    var = 0;
                    for (int j = 0; j < clusters[k].Count; j++)
                    {
                        var += Math.Pow(
                            schema[i].CalcDistance(
                            clusters[k][j][i], clusters[k].Center[i])
                            , 2.0);
                    }
                    lambda += Math.Log(var);

                }
                lambda = lambda / schema.Count;
                for (int j = 0; j < schema.Count; j++)
                {
                    var=0;
                    for (int i = 0; i < clusters[k].Count; i++)
                    {
                        var += Math.Pow(
                            schema[j].CalcDistance(clusters[k][i][j], clusters[k].Center[j]),
                            2.0);
                    }
                    var = Math.Log(var);
                    clusters[k].Weights[j] = Math.Exp(lambda - var);
                }
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
                   
                    clusters[k].Center[j] = temp / clusters[k].Count;
                }
            }
        }
        
        protected virtual void CalculateError()
        {
            Schema schema = dataset.Schema;
            double tmp = 0;
            for (int i = 0; i < clusters.Count; i++)
            {
                for (int j = 0; j < schema.Count; j++)
                {
                    tmp += clusters[i].Weights[j]*CalcDistance(dataset[i],clusters[i]);
                }

            }
            error = tmp;
        }
    }
}
