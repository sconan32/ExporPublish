using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Log;

namespace Socona.Expor.DataSources.Filters
{
    /**
   * Utilities for implementing filters.
   * 
   * @author Erich Schubert
   */
    public sealed class FilterUtil
    {
        /**
         * Fake constructor: do not instantiate.
         */
        private FilterUtil()
        {
            // Do not instantiate.
        }

        /**
         * Try to guess the appropriate factory.
         * 
         * @param in Input type
         * @param <V> Vector type
         * @return Factory
         */

        public static INumberVector GuessFactory<V>(SimpleTypeInformation tin) where V : IDataVector
        {
            INumberVector factory = null;
            if (tin is VectorFieldTypeInformation<V>)
            {
                factory = (INumberVector)((VectorFieldTypeInformation<V>)tin).GetFactory();
            }
            if (factory == null)
            {
                // FIXME: hack. Add factories to simple type information, too?
                try
                {
                    FieldInfo f = tin.GetRestrictionClass().GetField("FACTORY");
                    factory = (INumberVector)f.GetValue(null);
                }
                catch (Exception e)
                {
                    Logging.GetLogger(typeof(FilterUtil)).Warning("Cannot determine factory for type " + tin.GetRestrictionClass(), e);
                }
            }
            return factory;
        }
    }
}
