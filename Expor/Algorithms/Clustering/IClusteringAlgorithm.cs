using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Databases;
using Socona.Expor.Results;

namespace Socona.Expor.Algorithms.Clustering
{
    public interface IClusteringAlgorithm : IAlgorithm
   
    {

        new IResult  Run(IDatabase database);
    }
}
