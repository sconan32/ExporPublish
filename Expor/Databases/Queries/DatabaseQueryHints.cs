using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Queries
{
    public static class DatabaseQueryHints
    {
        /**
         * Optimizer hint: request bulk support.
         */
        public static readonly String HINT_BULK = "need_bulk";

        /**
         * Optimizer hint: only a single request will be done - avoid expensive optimizations
         */
        public static readonly String HINT_SINGLE = "single_query";

        /**
         * Optimizer hint: no linear scans allowed - return null then!
         */
        public static readonly String HINT_OPTIMIZED_ONLY = "optimized";

        /**
         * Optimizer hint: heavy use - caching/preprocessing/approximation recommended
         */
        public static readonly String HINT_HEAVY_USE = "heavy";

        /**
         * Optimizer hint: exact - no approximations allowed!
         */
        public static readonly String HINT_EXACT = "exact";

        /**
         * Optimizer hint: no cache instances
         */
        public static readonly String HINT_NO_CACHE = "no-cache";
    }
}
