using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries
{

    /**
     * Collection of objects and their distances.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.composedOf DistanceResultPair
     *
     * @param <D> Distance type
     */
    public interface IDistanceDbIdResult : IList<IDistanceResultPair>
    {
        // Empty. TODO: add "sorted" property?
    }

}
