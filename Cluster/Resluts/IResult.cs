using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Resluts
{
    /**
 * Interface for arbitrary result objects.
 * 
 * @author Erich Schubert
 * 
 * @apiviz.landmark
 * @apiviz.excludeSubtypes
 */
    public interface IResult
    {
        /**
         * A "pretty" name for the result, for use in titles, captions and menus.
         * 
         * @return result name
         */
        // TODO: turn this into an optional annotation? But: no inheritance?
        public String LongName { get; }

        /**
         * A short name for the result, useful for file names.
         * 
         * @return result name
         */
        // TODO: turn this into an optional annotation? But: no inheritance?
        public String ShortName { get; }
    }
}
