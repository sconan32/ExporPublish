using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Algorithms
{
    public class Median:LW
    {
        protected override void SetupArguments()
        {
            base.SetupArguments();
            distance = new Clustering.Distances.EuclideanDistance();
        }
        protected override void UpdateDm(int p, int q, int r)
        {
            double dist;
            foreach (var v in unmergedClusters)
            {
                if (r == v)
                {
                    continue;
                }
                dist = 0.5 * Math.Pow(dm[p, v], 2.0) + 
                    0.5 * Math.Pow(dm[q, v], 2.0) -
                    0.25 * Math.Pow(dm[p, q], 2.0);
                dm.Add(new KeyValuePair<int, int>(r, v), Math.Sqrt(dist));
            }
        }
    }
}
