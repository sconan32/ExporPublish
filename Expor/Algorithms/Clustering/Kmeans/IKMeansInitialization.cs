using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;

namespace Socona.Expor.Algorithms.Clustering.KMeans
{

    public interface IKMeansInitialization<V>
    {
       
         IList<V> ChooseInitialMeans(IRelation relation, int k, IPrimitiveDistanceFunction<V> distanceFunction);
    }
}
