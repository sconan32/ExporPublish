using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Pairs
{
    /**
     * Pair interface.
     * 
     * Note: this currently is <em>empty by design</em>. You should always decide
     * explicitly whether to use boxing pairs {@link Pair} or primitive pairs such
     * as {@link IntIntPair}
     * 
     * @author Erich Schubert
     * 
     * @param FIRST first type
     * @param SECOND second type
     */
    public interface IPair< FIRST,  SECOND>
    {
        /**
         * Get the first object - note: this may cause autoboxing, use pair.first for native pairs!
         * 
         * @return First object
         */
        FIRST First { get; set; }

        /**
         * Get the second object - note: this may cause autoboxing, use pair.second for native pairs!
         * 
         * @return Second object
         */
        SECOND Second { get; set; }
    }
}
