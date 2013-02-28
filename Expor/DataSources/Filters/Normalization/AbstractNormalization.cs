using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Maths.LinearAlgebra;

namespace Socona.Expor.DataSources.Filters.Normalization
{

    /**
     * Abstract super class for all normalizations.
     * 
     * @author Elke Achtert
     * 
     * @param <O> Object type processed
     */
    public abstract class AbstractNormalization<O> : AbstractVectorConversionFilter<O, O>, INormalization<O>
    where O : INumberVector
    {
        /**
         * Initializes the option handler and the parameter map.
         */
        protected AbstractNormalization()
        {

        }


        protected override SimpleTypeInformation ConvertedType(SimpleTypeInformation tin)
        {
            InitializeOutputType(tin);
            return tin;
        }


        public virtual LinearEquationSystem Transform(LinearEquationSystem linearEquationSystem)
        {
            // FIXME: implement.
            throw new InvalidOperationException("Not yet implemented!");
        }


        public override String ToString()
        {
            return GetType().Name;
        }

        public abstract Bundles.MultipleObjectsBundle NormalizeObjects(Bundles.MultipleObjectsBundle objects);


        public abstract O Restore(O featureVector);

    }

}
