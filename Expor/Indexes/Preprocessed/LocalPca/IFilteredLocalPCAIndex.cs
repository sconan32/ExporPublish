using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Maths.LinearAlgebra.Pca;

namespace Socona.Expor.Indexes.Preprocessed.LocalPca
{

    public interface IFilteredLocalPCAIndex : ILocalProjectionIndex
    {
        /**
         * Get the precomputed local PCA for a particular object ID.
         * 
         * @param objid Object ID
         * @return Matrix
         */

         //PCAFilteredResult GetLocalProjection(IDbIdRef objid);


    }
    /**
   * Factory interface
   * 
   * @author Erich Schubert
   * 
   * @apiviz.stereotype factory
   * @apiviz.uses FilteredLocalPCAIndex oneway - - 芦create禄
   * 
   * @param <NV> Vector type
   * @param <I> Index type produced
   */
    public  interface IFilteredLocalPCAIndexFactory : ILocalProjectionIndexFactory
    {
        /**
         * Instantiate the index for a given database.
         * 
         * @param relation Relation to use
         * 
         * @return Index
         */

       //  IFilteredLocalPCAIndex<NV> Instantiate(IRelation relation);
    }
}
