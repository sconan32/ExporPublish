using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Datasets;

namespace Socona.Clustering.Clusters
{
    public class CenterCluster:Cluster
    {
        public Record Center { get; set; }

        public CenterCluster(Record center)
        {
            Center = center;
        }
    }
}
