using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Persistent
{

    public interface IPageFileStatistics
    {
        /**
         * Returns the read I/O-Accesses of this file.
         * 
         * @return Number of physical read I/O accesses
         */
         long GetReadOperations();

        /**
         * Returns the write I/O-Accesses of this file.
         * 
         * @return Number of physical write I/O accesses
         */
         long GetWriteOperations();

        /**
         * Resets the counters for page accesses of this file and flushes the cache.
         */
         void ResetPageAccess();

        /**
         * Get statistics for the inner page file, if present.
         * 
         * @return Inner page file statistics, or null.
         */
         IPageFileStatistics GetInnerStatistics();
    }

}
