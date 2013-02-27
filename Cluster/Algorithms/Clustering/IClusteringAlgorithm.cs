using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Data.Model;
using Socona.Clustering.Databases;

namespace Socona.Clustering.Algorithms.Clustering
{
    public interface IClusteringAlgorithm<C> : IAlgorithm
    where C : ClusterModel
    {

        C Run(Database database);
    }
}
