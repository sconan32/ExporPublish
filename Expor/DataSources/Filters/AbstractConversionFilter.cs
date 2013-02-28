using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Types;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.Utilities.Exceptions;
using Socona.Log;

namespace Socona.Expor.DataSources.Filters
{

    /**
     * Abstract base class for simple conversion filters such as normalizations and projections.
     * 
     * @author Erich Schubert
     * 
     * @param <I> Input object type
     * @param <O> Input object type
     */
    public abstract class AbstractConversionFilter<I, O> : IObjectFilter
    {
        /**
         * A standard implementation of the filter process. First of all, all suitable
         * representations are found. Then (if {@link #prepareStart} returns true),
         * the data is processed read-only in a first pass.
         * 
         * In the main pass, each object is then filtered using
         * {@link #filterSingleObject}.
         * 
         * @param objects Objects to filter
         * @return Filtered bundle
         */

        public virtual MultipleObjectsBundle Filter(MultipleObjectsBundle objects)
        {
            if (objects.DataLength() == 0)
            {
                return objects;
            }
            MultipleObjectsBundle bundle = new MultipleObjectsBundle();

            for (int r = 0; r < objects.MetaLength(); r++)
            {

                SimpleTypeInformation type = (SimpleTypeInformation)objects.Meta(r);

                List<Object> column = (List<Object>)objects.GetColumn(r);
                if (!GetInputTypeRestriction().IsAssignableFromType(type))
                {
                    bundle.AppendColumn(type, column);
                    continue;
                }
                // Get the replacement type information

                SimpleTypeInformation castType = (SimpleTypeInformation)type;

                // When necessary, perform an initialization scan
                if (PrepareStart(castType))
                {
                    //FiniteProgress pprog = getLogger().isVerbose() ? new FiniteProgress("Preparing normalization.", objects.dataLength(), getLogger()) : null;
                    foreach (Object o in column)
                    {
                        //  @SuppressWarnings("unchecked")
                        I obj = (I)o;
                        PrepareProcessInstance(obj);
                        //    if (pprog != null) {
                        //    pprog.incrementProcessed(getLogger());
                        //  }
                    }
                    // if (pprog != null) {
                    //   pprog.ensureCompleted(getLogger());
                    // }
                    PrepareComplete();
                }


                List<Object> castColumn = (List<Object>)column;
                bundle.AppendColumn(ConvertedType(castType), castColumn);

                // Normalization scan
                // FiniteProgress nprog = getLogger().isVerbose() ? new FiniteProgress("Data normalization.", objects.dataLength(), getLogger()) : null;
                for (int i = 0; i < objects.DataLength(); i++)
                {
                    // @SuppressWarnings("unchecked")
                    I obj = (I)column[(i)];
                    O normalizedObj = FilterSingleObject(obj);
                    castColumn[i] = normalizedObj;
                    // if (nprog != null) {
                    //  nprog.incrementProcessed(getLogger());
                    // }
                }
                // if (nprog != null) {
                //   nprog.ensureCompleted(getLogger());
                // }
            }
            return bundle;
        }

        /**
         * Class logger.
         * 
         * @return Logger
         */
        abstract protected Logging GetLogger();

        /**
         * Normalize a single instance.
         * 
         * You can implement this as UnsupportedOperationException if you override
         * both public "normalize" functions!
         * 
         * @param obj Database object to normalize
         * @return Normalized database object
         */
        protected abstract O FilterSingleObject(I obj);

        /**
         * Get the input type restriction used for negotiating the data query.
         * 
         * @return Type restriction
         */
        protected abstract SimpleTypeInformation GetInputTypeRestriction();

        /**
         * Get the output type from the input type after conversion.
         * 
         * @param in input type restriction
         * @return output type restriction
         */
        protected abstract SimpleTypeInformation ConvertedType(SimpleTypeInformation tin);

        /**
         * Return "true" when the normalization needs initialization (two-pass filtering!).
         * 
         * @param in Input type information
         * @return true or false
         */
        protected virtual bool PrepareStart(SimpleTypeInformation tin)
        {
            return false;
        }

        /**
         * Process a single object during initialization.
         * 
         * @param obj Object to process
         */
        protected virtual void PrepareProcessInstance(I obj)
        {
            throw new AbortException("ProcessInstance not implemented, but prepareStart true?");
        }

        /**
         * Complete the initialization phase.
         */
        protected virtual void PrepareComplete()
        {
            // optional - default NOOP.
        }
    }
}
