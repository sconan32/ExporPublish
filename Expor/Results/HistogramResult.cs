using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Results
{

    /**
     * Histogram result.
     * 
     * @author Erich Schubert
     *
     * @param <O> Object class (e.g. {@link de.lmu.ifi.dbs.elki.data.DoubleVector})
     */
    public class HistogramResult<O> : CollectionResult<O>
    {
        /**
         * Constructor
         * 
         * @param name The long name (for pretty printing)
         * @param shortname the short name (for filenames etc.)
         * @param col Collection
         */
        public HistogramResult(String name, String shortname, ICollection<O> col) :
            base(name, shortname, col)
        {
        }

        /**
         * Constructor
         * 
         * @param name The long name (for pretty printing)
         * @param shortname the short name (for filenames etc.)
         * @param col Collection
         * @param header Header information
         */
        public HistogramResult(String name, String shortname, ICollection<O> col, ICollection<String> header) :
            base(name, shortname, col, header)
        {
        }
    }
}
