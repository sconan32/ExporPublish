using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Clusters;
using Socona.Clustering.Datasets;

namespace Socona.Clustering.Algorithms
{
    public class Kmean:AbstractAlgorithm
    {
        protected double error;
        protected int numIter;
        protected int numClust;
        protected int maxIter;
        protected int seed;
        protected Distances.Distance distance;

        protected List<int> CM;
        protected List<CenterCluster> clusters=new List<CenterCluster> ();

        protected virtual void Initialize()
        {
            int numRecords = dataset.Count;
            List<int> index=new List<int> (numRecords);
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
                CenterCluster c = new CenterCluster(cr);
                c.Id = i;
                clusters.Add(c);
                index.Remove(r);
            }
            int s=-1;
            double min, dist;

            for (int i = 0; i < numRecords; i++)
            {
                min = double.MaxValue;
                for (int j = 0; j < numClust; j++)
                {
                    dist = distance.CalcDistance(dataset[i], clusters[j].Center);
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
        protected virtual void Iterate()
        {
            bool changed = true;
            UpdateCenter();

            numIter = 1;
            while (changed)
            {
                changed = false;
                int s=0;
                double min, dist;
                for (int i = 0; i < dataset.Count; i++)
                {
                    min = double.MaxValue;
                    for (int k = 0; k < clusters.Count; k++)
                    {
                        dist = distance.CalcDistance(dataset[i], clusters[k].Center);
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
                        changed = true;
                    }
                }
                UpdateCenter();
                numIter++;
                if (numIter > maxIter)
                {
                    break;
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
                    //schema[j].SetContinuousValue(
                    //    clusters[k].Center[j], temp / clusters[k].Count);
                    clusters[k].Center[j] = temp / clusters[k].Count;
                }
            }
        }
        protected override void SetupArguments()
        {
            base.SetupArguments();

            distance = Arguments.Distance;
            numClust = Convert.ToInt32(Arguments.Get("numclust"));
            maxIter = Convert.ToInt32(Arguments.Get("maxiter"));
            seed = Convert.ToInt32(Arguments.Get("seed"));

        }
        protected override void PerformClustering()
        {
            Initialize();
            Iterate();
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
            error = 0.0;
            for (int i = 0; i < dataset.Count; i++)
            {
                error += distance.CalcDistance(dataset[i], clusters[CM[i]].Center);
            }
            Results.Insert("error", error);
            Results.Insert("numiter", numIter);
        }
      

    }
}
