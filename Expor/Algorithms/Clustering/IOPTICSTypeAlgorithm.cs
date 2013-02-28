using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Results.Optics;

namespace Socona.Expor.Algorithms.Clustering
{

    public interface IOPTICSTypeAlgorithm : IAlgorithm
    {

       //ClusterOrderResult Run(IDatabase database);

        /**
         * Get the minpts value used. Needed for OPTICS Xi etc.
         * 
         * @return minpts value
         */
         int GetMinPts();

        /**
         * Get the distance factory. Needed for type checking (i.e. is number distance)
         * 
         * @return distance factory
         */
         IDistanceValue GetDistanceFactory();
    }
}
