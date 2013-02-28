using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Distances.DistanceFuctions.Subspace
{

    public interface IDimensionSelectingSubspaceDistanceFunction : IDistanceFunction
    {
        /**
         * get or set a bit set representing the selected dimensions.
         * 
         * @return a bit set representing the selected dimensions
         */
        BitArray SelectedDimensions { get; set; }
    }

}
