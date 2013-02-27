using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Datasets;

namespace Socona.Clustering.Clusters
{
    public class SubspaceCluster:CenterCluster
    {
        public List<double> Weights { get; protected set; }

        public SubspaceCluster(Record center)
            : base(center)
        {
            Weights = new List<double>(center.Schema.Count);
            for (int i = 0; i < center.Schema.Count;i++ )
            {
                Weights.Add((double)1/center.Schema.Count);
            }
        }
        public double At(int index)
        {
            return Weights[index];
        }
        public void SetAt(int index, double value)
        {
            Weights[index] = value;
        }
    }
}
