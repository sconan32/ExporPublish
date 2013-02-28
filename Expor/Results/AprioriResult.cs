using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Results
{

    public class AprioriResult : BasicResult
    {
        /**
         * The frequent itemsets.
         */
        private List<BitArray> solution;

        /**
         * The supports of all itemsets.
         */
        private IDictionary<BitArray, int?> supports;

        /**
         * Constructor.
         * 
         * @param name The long name (for pretty printing)
         * @param shortname the short name (for filenames etc.)
         * @param solution Frequent itemsets
         * @param supports Supports for the itemsets
         */
        public AprioriResult(String name, String shortname, List<BitArray> solution, IDictionary<BitArray, int?> supports) :
            base(name, shortname)
        {
            this.solution = solution;
            this.supports = supports;
        }

        /**
         * Returns the frequent item sets.
         *
         * @return the frequent item sets.
         */
        public List<BitArray> GetSolution()
        {
            return solution;
        }

        /**
         * Returns the frequencies of the frequent item sets.
         *
         * @return the frequencies of the frequent item sets
         */
        public IDictionary<BitArray, int?> GetSupports()
        {
            return supports;
        }

        // TODO: text writer for AprioriResult!
    }

}
