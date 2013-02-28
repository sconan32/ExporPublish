using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Indexes.Preprocessed.Preference
{

    public abstract class AbstractPreferenceVectorIndex :
        AbstractPreprocessorIndex<BitArray>, IPreferenceVectorIndex
 
    {
        /**
         * Constructor.
         * 
         * @param relation Relation to use
         */
        public AbstractPreferenceVectorIndex(IRelation relation) :
            base(relation)
        {
        }

        /**
         * Preprocessing step.
         */
        abstract protected void Preprocess();


        public BitArray GetPreferenceVector(IDbIdRef objid)
        {
            if (storage == null)
            {
                Preprocess();
            }
            return storage[(objid)];
        }

        /**
         * Factory class
         * 
         * @author Erich Schubert
         * 
         * @apiviz.stereotype factory
         * @apiviz.uses AbstractPreferenceVectorIndex oneway - - 芦create禄
         */
        public abstract class Factory : IPreferenceVectorIndexFactory, IParameterizable
        {

            public abstract IIndex Instantiate(IRelation relation);


            public ITypeInformation GetInputTypeRestriction()
            {
                return TypeUtil.NUMBER_VECTOR_FIELD;
            }
        }
    }
}
