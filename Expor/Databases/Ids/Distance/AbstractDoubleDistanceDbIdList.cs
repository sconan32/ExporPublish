using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Distance
{
    public abstract class AbstractDoubleDistanceDbIdList<TDPair> : AbstractDistanceDbIdList<TDPair>, IDoubleDistanceDbIdList
        where TDPair : IDoubleDistanceDbIdPair
    {

    }
}
