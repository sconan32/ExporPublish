using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;

namespace Socona.Expor.DataSources.Filters
{
    /**
  * Abstract class for filters that produce number vectors.
  * 
  * @author Erich Schubert
  *
  * @param <I> Input vector type
  * @param <O> Output vector type
  */
    public abstract class AbstractVectorConversionFilter<I, O> : AbstractConversionFilter<I, O>
        where O : IDataVector
    {
        /**
         * Number vector factory.
         */
        protected INumberVector factory;

        /**
         * Initialize factory from a data type.
         * 
         * @param type Output data type information.
         */
        protected virtual void InitializeOutputType(SimpleTypeInformation type)
        {
            factory = FilterUtil.GuessFactory<O>(type);
        }
    }

}
