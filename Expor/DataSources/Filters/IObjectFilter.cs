using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.Utilities;

namespace Socona.Expor.DataSources.Filters
{

    public interface IObjectFilter : IInspectionUtilFrequentlyScanned
    {
        /**
         * Filter a set of object packages.
         * 
         * @param objects Object to filter
         * @return Filtered objects
         */
        MultipleObjectsBundle Filter(MultipleObjectsBundle objects);
    }

}
