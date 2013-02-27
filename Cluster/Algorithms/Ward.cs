using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Algorithms
{
    public class Ward:LW
    {
        protected override void SetupArguments()
        {
            base.SetupArguments();
            distance = new Distances.EuclideanDistance();
        }
        protected override void UpdateDm(int p, int q, int r)
        {
            double dist;
            double sp = clusterSize[p];
            double sq = clusterSize[q];

            foreach (var v in unmergedClusters)
            {
                if(r==v)
                {
                    continue;
                }
                double sv=clusterSize[v];
                double st=sp+sq+sv;
                dist = Math.Pow(dm[p, v], 2.0) * (sp + sv) / st +
                    Math.Pow(dm[q, v], 2.0) * (sq + sv) / st -
                    Math.Pow(dm[p, q], 2.0) * sv / st;
                dm.Add(new KeyValuePair<int, int>(r, v), Math.Sqrt(dist));
            }

        }
    }
}
