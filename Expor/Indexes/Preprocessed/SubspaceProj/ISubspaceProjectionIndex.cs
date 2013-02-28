using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Indexes.Preprocessed.SubspaceProj
{

    public interface ISubspaceProjectionIndex : ILocalProjectionIndex
    {
        /**
         * Get the precomputed local subspace for a particular object ID.
         * 
         * @param objid Object ID
         * @return Matrix
         */

        // public P getLocalProjection(DBIDRef objid);

        /**
         * Factory interface
         * 
         * @author Erich Schubert
         * 
         * @apiviz.landmark
         * @apiviz.stereotype factory
         * @apiviz.uses SubspaceProjectionIndex oneway - - 芦create禄
         * 
         * @param <NV> Vector type
         * @param <I> Index type produced
         */

    }
    public interface ISubspaceProjectionIndexFactory : ILocalProjectionIndexFactory
    {
        /**
         * Instantiate the index for a given database.
         * 
         * @param relation Relation
         * 
         * @return Index
         */

        //  public I instantiate(Relation<NV> relation);
    }
}
