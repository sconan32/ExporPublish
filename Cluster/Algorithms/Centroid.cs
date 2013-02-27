using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Distances;

namespace Socona.Clustering.Algorithms
{
    public  class Centroid:LW
    {
        protected override void SetupArguments()
        {
            base.SetupArguments();
            distance = new EuclideanDistance();
        }

        protected override void UpdateDm(int p, int q, int r)
        {
            double dist;
            double sp = clusterSize[p];
            double sq = clusterSize[q];

            foreach (var v in unmergedClusters)
            {
                if (v == r)
                {
                    continue;
                }
                dist = Math.Pow(dm[p, v], 2.0) * sp / (sp + sq) +
                    Math.Pow(dm[q, v], 2.0) * sq / (sp + sq) -
                    Math.Pow(dm[p, q], 2.0) * sp * sq / ((sp + sq) * (sp + sq));
                dm.Add(new KeyValuePair<int, int>(r, v), Math.Sqrt(dist));
            }
        }
    }
}
