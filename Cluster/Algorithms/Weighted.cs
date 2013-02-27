using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Algorithms
{
    public class Weighted:LW
    {
        protected override void UpdateDm(int p, int q, int r)
        {
            double dist;
            foreach (var v in unmergedClusters)
            {
                if (r == v)
                {
                    continue;
                }
                dist = (dm[p, v] + dm[q, v]) / 2;
                dm.Add(new KeyValuePair<int, int>(r, v), dist);
            }
        }
    }
}
