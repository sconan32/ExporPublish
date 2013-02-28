using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities;

namespace Socona.Expor.Utilities.Options
{


    public interface IParameterizable : IInspectionUtilFrequentlyScanned
    {
        // Empty marker interface - the \@Description / \@Title / \@Reference and
        // constructor requirements cannot be specified in Java!
    }
}
