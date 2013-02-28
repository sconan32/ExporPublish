using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries.DistanceQueries;

namespace Socona.Expor.Algorithms.Clustering.KMeans
{

    public interface IKMedoidsInitialization
    {
        /**
         * Choose initial means
         * 
         * @param k Parameter k
         * @param distanceFunction Distance function
         * @return List of chosen means for k-means
         */
        IDbIds ChooseInitialMedoids(int k, IDistanceQuery distanceFunction);
    }
}
