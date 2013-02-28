using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Ids.Distance
{

    /**
     * Interface for kNN results.
     * 
     * To iterate over the results, use the following code:
     * 
     * <pre>
     * {@code
     * for (DistanceDbIdResultIter<D> iter = result.iter(); iter.valid(); iter.advance()) {
     *   // You can get the distance via: iter.getDistance();
     *   // Or use iter just like any other DbIdRef
     * }
     * }
     * </pre>
     * 
     * If you are only interested in the IDs of the objects, the following is also
     * sufficient:
     * 
     * <pre>
     * {@code
     * for (DbIdIter<D> iter = result.iter(); iter.valid(); iter.advance()) {
     *   // Use iter just like any other DbIdRef
     * }
     * }
     * </pre>
     * 
     * @author Erich Schubert
     * 
     * @apiviz.landmark
     * 
     * @apiviz.composedOf DistanceDbIdPair
     * 
     * @param <D> Distance type
     */
    public interface IKNNList : IDistanceDbIdList
    {


        /**
         * Get the K parameter (note: this may be less than the size of the list!)
         * 
         * @return K
         */
        int K { get; }



        /**
         * Get the distance to the k nearest neighbor, or maxdist otherwise.
         * 
         * @return Maximum distance
         */
        IDistanceValue KNNDistance { get; }
    }
}
