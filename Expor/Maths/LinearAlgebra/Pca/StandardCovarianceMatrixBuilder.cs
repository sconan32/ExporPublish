using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;

namespace Socona.Expor.Maths.LinearAlgebra.Pca
{

    public class StandardCovarianceMatrixBuilder : AbstractCovarianceMatrixBuilder<IDataVector>
    {
        /**
         * Compute Covariance Matrix for a complete database
         * 
         * @param database the database used
         * @return Covariance Matrix
         */

        public override Matrix ProcessDatabase(IRelation database)
        {
            return CovarianceMatrix.Make(database).DestroyToNaiveMatrix();
        }

        /**
         * Compute Covariance Matrix for a collection of database IDs
         * 
         * @param ids a collection of ids
         * @param database the database used
         * @return Covariance Matrix
         */

        public override Matrix ProcessIds(IDbIds ids, IRelation database)
        {
            return CovarianceMatrix.Make(database, ids).DestroyToNaiveMatrix();
        }
    }
}
