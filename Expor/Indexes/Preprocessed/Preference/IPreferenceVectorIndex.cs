using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;

namespace Socona.Expor.Indexes.Preprocessed.Preference
{

    public interface IPreferenceVectorIndex: IIndex
  
    {
        /**
         * Get the precomputed preference vector for a particular object ID.
         * 
         * @param id Object ID
         * @return Matrix
         */
        BitArray GetPreferenceVector(IDbIdRef id);

        /**
         * Factory interface
         * 
         * @author Erich Schubert
         * 
         * @apiviz.stereotype factory
         * @apiviz.uses PreferenceVectorIndex oneway - - 芦create禄
         * 
         * @param <V> vector type
         * @param <I> index type
         */

    }
    public interface IPreferenceVectorIndexFactory : IIndexFactory
        
    {
        /**
         * Instantiate the index for a given database.
         * 
         * @param relation Relation to use
         * 
         * @return Index
         */

       // IPreferenceVectorIndex<V> Instantiate(IRelation relation);
    }
}
