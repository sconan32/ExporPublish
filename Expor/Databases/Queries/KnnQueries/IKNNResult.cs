using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.KnnQueries
{

    public interface IKNNResult : IDistanceDbIdResult
        
    {
        /**
         * Size
         */

        int Size();

        /**
         * Get the K parameter (note: this may be less than the size of the list!)
         * 
         * @return K
         */
        int GetK();

        /**
         * Direct object access.
         * 
         * @param index
         */

        IDistanceResultPair Get(int index);

        /**
         * Get the distance to the k nearest neighbor, or maxdist otherwise.
         * 
         * @return Maximum distance
         */
         IDistanceValue GetKNNDistance();

        /**
         * View as ArrayDBIDs
         * 
         * @return Static DBIDs
         */
         IArrayDbIds AsDbIds();

        /**
         * View as list of distances
         * 
         * @return List of distances view
         */
         IList<IDistanceValue> AsDistanceList();
    }
}
