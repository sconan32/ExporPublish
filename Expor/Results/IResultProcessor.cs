using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities;

namespace Socona.Expor.Results
{

    public interface IResultProcessor : IInspectionUtilFrequentlyScanned
    {
        /**
         * Process a result.
         * 
         * @param baseResult The base of the result tree.
         * @param newResult Newly added result subtree.
         */
        void ProcessNewResult(IHierarchicalResult baseResult, IResult newResult);
    }

}
