using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Persistent
{

    public sealed class PageFileUtil
    {
        /**
         * Append the page file statistics to the output buffer.
         * 
         * @param buffer Buffer to Append to
         */
        public static void AppendPageFileStatistics(StringBuilder buffer, IPageFileStatistics statistics)
        {
            if (statistics != null)
            {
                buffer.Append("Page File Layer: ").Append(statistics.GetType()).Append("\n");
                buffer.Append("Read Operations: ").Append(statistics.GetReadOperations()).Append("\n");
                buffer.Append("Write Operations: ").Append(statistics.GetWriteOperations()).Append("\n");
                IPageFileStatistics inner = statistics.GetInnerStatistics();
                if (inner != null)
                {
                    AppendPageFileStatistics(buffer, inner);
                }
            }
        }

        /**
         * Get the number of (logical) read operations (without caching).
         * 
         * @param statistics Statistics.
         * @return logical read operations.
         */
        public static long GetLogicalReadOperations(IPageFileStatistics statistics)
        {
            return statistics.GetReadOperations();
        }

        /**
         * Get the number of (logical) write operations (without caching).
         * 
         * @param statistics Statistics.
         * @return logical write operations.
         */
        public static long GetLogicalWriteOperations(IPageFileStatistics statistics)
        {
            return statistics.GetWriteOperations();
        }

        /**
         * Get the number of (physical) read operations (with caching).
         * 
         * @param statistics Statistics.
         * @return physical read operations.
         */
        public static long GetPhysicalReadOperations(IPageFileStatistics statistics)
        {
            IPageFileStatistics inner = statistics.GetInnerStatistics();
            if (inner != null)
            {
                return GetPhysicalReadOperations(inner);
            }
            return statistics.GetReadOperations();
        }

        /**
         * Get the number of (physical) write operations (with caching).
         * 
         * @param statistics Statistics.
         * @return physical write operations.
         */
        public static long GetPhysicalWriteOperations(IPageFileStatistics statistics)
        {
            IPageFileStatistics inner = statistics.GetInnerStatistics();
            if (inner != null)
            {
                return GetPhysicalWriteOperations(inner);
            }
            return statistics.GetWriteOperations();
        }
    }
}
