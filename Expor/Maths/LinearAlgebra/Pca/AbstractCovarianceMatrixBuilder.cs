using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Maths.LinearAlgebra.Pca
{

    public abstract class AbstractCovarianceMatrixBuilder<V> : IParameterizable, ICovarianceMatrixBuilder<V>
    {

        public virtual Matrix ProcessDatabase(IRelation database)
        {
            return ProcessIds(database.GetDbIds(), database);
        }


        public abstract Matrix ProcessIds(IDbIds ids, IRelation database);


        public virtual Matrix ProcessQueryResults(ICollection<IDistanceDbIdPair> results,
            IRelation database, int k)
        {
            IModifiableDbIds ids = DbIdUtil.NewArray(k);
            int have = 0;
            // for(Iterator<? extends DistanceResultPair<D>> it = results.iterator(); it.hasNext() && have < k; have++) {
            foreach (var it in results)
            {

                ids.Add(it.DbId);
                if (have++ >= k)
                {
                    break;
                }
            }
            return ProcessIds(ids, database);
        }


        public Matrix ProcessQueryResults(ICollection<IDistanceDbIdPair> results, IRelation database)
        {
            return ProcessQueryResults(results, database, results.Count);
        }

        // TODO: Allow KNNlist to avoid building the DbId array?
    }
}
