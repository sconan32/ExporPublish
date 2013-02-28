using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.DataSources.Bundles;

namespace Socona.Expor.DataSources.Filters
{

    public interface IStreamFilter : IObjectFilter, IBundleStreamSource
    {
        /**
         * Connect to the previous stream.
         * 
         * @param source Stream source
         */
         void Init(IBundleStreamSource source);
    }
}
