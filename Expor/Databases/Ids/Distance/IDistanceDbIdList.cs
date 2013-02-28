using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Distance
{

    /**
     * Collection of objects and their distances.
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
     * @apiviz.has DistanceDbIdListIter
     * 
     * @param <D> Distance type
     */
    public interface IDistanceDbIdList : ICollection<IDistanceDbIdPair>
    {

        /**
         * Access a single pair.
         * 
         * @param off Offset
         * @return Pair
         */
        IDistanceDbIdPair this[int off] { get; }
        IDbIds ToDbIds();

        void RemoveAt(int index);
        //  int Count { get; }

        //  void Add(IDistanceDbIdPair pair);

        // bool IsReadOnly { get; }
    }
}
