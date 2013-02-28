using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Results
{

    public interface IIterableResult<O> : IResult, IEnumerable<O>
    {
        /**
         * Retrieve an iterator for the result.
         * 
         * @return iterator
         */

    }
}
