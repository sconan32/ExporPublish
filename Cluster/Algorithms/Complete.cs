﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Algorithms
{
    public class Complete:LW
    {
        protected override void UpdateDm(int p, int q, int r)
        {
            double dist;
            foreach (int s in unmergedClusters)
            {
                if (s == r)
                {
                    continue;
                }
                dist = Math.Max(dm[p, s], dm[q, s]);
                dm.Add(new KeyValuePair<int, int>(r, s), dist);
            }
        }
    }
}
