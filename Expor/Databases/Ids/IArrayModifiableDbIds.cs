using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids
{

    /**
     * Array-oriented implementation of a modifiable DBID collection.
     * 
     * @author Erich Schubert
     */
    public interface IArrayModifiableDbIds : IModifiableDbIds, IArrayDbIds
    {
        /**
         * Sort the DBID set.
         */
        void Sort();

        /**
         * Sort the DBID set.
         * 
         * @param comparator Comparator to use
         */
        void Sort(IComparer<IDbId> comparer);
        void Sort(Comparison<IDbIdRef> comp);

        /**
         * Remove the i'th entry (starting at 0)
         * 
         * @param i Index
         * @return value removed
         */
        IDbId RemoveAt(int i);

        /**
         * Replace the i'th entry (starting at 0)
         * 
         * @param i Index
         * @param newval New value
         * @return previous value
         */
       // IDbId Set(int i, IDbIdRef newval);

        /**
         * Swap DBIDs add positions a and b.
         * 
         * @param a First position
         * @param b Second position
         */
        void Swap(int a, int b);
    }
}
