using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Relations;

namespace Socona.Expor.Maths.LinearAlgebra.Pca
{

    public interface ICovarianceMatrixBuilder<V>
    {
        /**
         * Compute Covariance Matrix for a complete database
         * 
         * @param database the database used
         * @return Covariance Matrix
         */
        Matrix ProcessDatabase(IRelation database);

        /**
         * Compute Covariance Matrix for a collection of database IDs
         * 
         * @param ids a collection of ids
         * @param database the database used
         * @return Covariance Matrix
         */
        Matrix ProcessIds(IDbIds ids, IRelation database);

        /**
         * Compute Covariance Matrix for a QueryResult Collection
         * 
         * By default it will just collect the ids and run processIds
         * 
         * @param results a collection of QueryResults
         * @param database the database used
         * @param k the number of entries to process
         * @return Covariance Matrix
         */
        Matrix ProcessQueryResults(ICollection<IDistanceDbIdPair> results, IRelation database, int k);

        /**
         * Compute Covariance Matrix for a QueryResult Collection
         * 
         * By default it will just collect the ids and run processIds
         * 
         * @param results a collection of QueryResults
         * @param database the database used
         * @return Covariance Matrix
         */
        Matrix ProcessQueryResults(ICollection<IDistanceDbIdPair> results, IRelation database);
    }
}
