using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.DataSources
{
    public interface IDatabaseConnection : IParameterizable
    {
        /**
         * Returns the initial data for a database.
         * 
         * @return a database object bundle
         */
        // TODO: streaming load?
        MultipleObjectsBundle LoadData();
    }
}
