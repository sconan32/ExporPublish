using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Utilities;
using Socona.Clustering.Clusters;
using Socona.Clustering.Patterns;

namespace Socona.Clustering.Algorithms
{
    public abstract class LW:AbstractAlgorithm
    {
        public LW()
        {
            dm = new NNMap<double>();
            forest = new Dictionary<int, HClustering>();
            clusterSize = new Dictionary<int, int>();
            unmergedClusters = new SortedSet<int>();
        }
        protected override void SetupArguments()
        {
            base.SetupArguments();
            distance = Arguments.Distance;

        }
        protected override void PerformClustering()
        {
            CreateDm();
            InitForest();
            Linkage();
        }
        protected override void FetchResults()
        {
            int n = dataset.Count;
            Results.Insert("hc", new HClustering(forest[2 * n - 2].Root));
        }
        protected virtual void CreateDm()
        {
            int n = dataset.Count;
            for (int i = 0; i < n - 1; ++i)
            {
                for (int j = 1 + i; j < n; j++)
                {
                    dm.Add(new KeyValuePair<int, int>(i, j), 
                        distance.CalcDistance(dataset[i], dataset[j]));
                }
            }
        }
        protected virtual void InitForest()
        {
            int n = dataset.Count;
            for (int s = 0; s < n; s++)
            {
                INode pln = new LeafNode(dataset[s], s, null);
                pln.Level = 0;
                HClustering phc = new HClustering(pln);

                forest.Add(s, phc);
                clusterSize.Add(s, 1);
                unmergedClusters.Add(s);
            }
        }
        protected virtual void Linkage()
        {
            int n = dataset.Count;

            SortedSet<int>.Enumerator sse;
            double min, tmp;

            int m, s1=-1, s2=-1;
            for (int s = 0; s < n - 1; ++s)
            {
                min = double.MaxValue;
                List<int> nvTemp=new List<int>(unmergedClusters);
                m = nvTemp.Count;
                for (int i = 0; i < m; ++i)
                {
                    for (int j = i + 1; j < m; j++)
                    {
                        tmp = dm[nvTemp[i], nvTemp[j]];
                        if (tmp < min)
                        {
                            min = tmp;
                            s1 = nvTemp[i];
                            s2 = nvTemp[j];
                        }
                    }
                }
                INode node = forest[s1].JoinWith(forest[s2], min);
                node.Id = n + s;
                node.Level = s + 1;
                HClustering phc = new HClustering(node);
                forest.Add(n + s, phc);
                clusterSize.Add(n + s, clusterSize[s1] + clusterSize[s2]);
                unmergedClusters.Remove(s1);
                unmergedClusters.Remove(s2);
                unmergedClusters.Add(n + s);
                UpdateDm(s1, s2, n + s);

            }
        }
        protected abstract void UpdateDm(int p, int q, int r);

        protected NNMap<double> dm;
        protected SortedSet<int> unmergedClusters;
        protected Dictionary<int, HClustering> forest;
        protected Dictionary<int, int> clusterSize;
        protected Distances.Distance distance;
    }
}
