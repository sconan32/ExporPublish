using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Results
{

    //TODO:  Delete this Interface
    public interface IResultHandler : IParameterizable, IResultProcessor
    {
        // Empty - moved to ResultProcessor, this interface merely serves UI purposes.
    }
}
