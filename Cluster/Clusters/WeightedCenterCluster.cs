using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Datasets;

namespace Socona.Clustering.Clusters
{
    class WeightedCenterCluster:CenterCluster
    {
        public List<double> Weights { get; set; }

        public WeightedCenterCluster(Record center)
            :base(center)
        {
            Weights = new List<double>(center.Schema.Count);
        }
    }
}
