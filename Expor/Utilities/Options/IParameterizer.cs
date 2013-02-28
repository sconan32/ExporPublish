using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameterizations;

namespace Socona.Expor.Utilities.Options
{

    public interface IParameterizer
    {
        /**
         * Configure the class.
         * 
         * Note: the status is collected by the parameterization object, so that
         * multiple errors may arise and be reported in one run.
         * 
         * @param config Parameterization
         */
        void Configure(IParameterization config);
    }
}
