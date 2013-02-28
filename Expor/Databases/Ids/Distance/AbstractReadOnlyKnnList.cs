using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Distance
{
    public abstract class AbstractReadOnlyKnnList<TPair> : AbstractReadOnlyDistanceDbIdList<TPair>, IKNNList
        where TPair : IDistanceDbIdPair
    {

        public abstract int K
        {
            get;
        }

        public abstract Distances.DistanceValues.IDistanceValue KNNDistance
        {
            get;
        }


    }
}
