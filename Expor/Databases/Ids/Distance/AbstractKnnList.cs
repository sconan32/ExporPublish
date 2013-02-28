using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Distance
{
    public abstract class AbstractKnnList<TPair> : AbstractDistanceDbIdList<TPair>, IKNNList
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

        public override IEnumerator<IDistanceDbIdPair> GetEnumerator()
        {
            int minimun = Math.Min(K, this.Count);
            for (int i = 0; i < minimun; i++)
            {
                yield return this[i];
            }

        }
        public override IDbIds ToDbIds()
        {
            IModifiableDbIds ids = DbIdUtil.NewArray();
           foreach(var id in this)
           {
               ids.Add(id.DbId);
           }
           return ids;
        }
    }
}
