using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.DataSources.Filters;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.DataSources
{

    public abstract class AbstractDatabaseConnection : IDatabaseConnection
    {
        /**
         * A sign to separate components of a label.
         */
        public static readonly String LABEL_CONCATENATION = " ";

        /**
         * Filters to apply to the input data.
         * <p>
         * Key: {@code -dbc.filter}
         * </p>
         */
        public static readonly OptionDescription FILTERS_ID = OptionDescription.GetOrCreate("dbc.filter", "The filters to apply to the input data.");

        /**
         * Parameter to specify the parser to provide a database.
         * <p>
         * Key: {@code -dbc.parser}
         * </p>
         */
        public static readonly OptionDescription PARSER_ID = OptionDescription.GetOrCreate("dbc.parser", "Parser to provide the database.");

        /**
         * The filters to invoke
         */
        protected IList<IObjectFilter> filters;

        /**
         * Constructor.
         * 
         * @param filters Filters to apply, can be null
         */
        protected AbstractDatabaseConnection(IList<IObjectFilter> filters)
        {
            this.filters = filters;
        }

        /**
         * Transforms the specified list of objects and their labels into a list of
         * objects and their associations.
         * 
         * @param bundle the objects to process
         * @return processed objects
         */
        protected MultipleObjectsBundle InvokeFilters(MultipleObjectsBundle bundle)
        {
            IBundleStreamSource prevs = null;
            MultipleObjectsBundle prevb = bundle;
            if (filters != null)
            {
                foreach (IObjectFilter filter in filters)
                {
                    if (filter is IStreamFilter)
                    {
                        IStreamFilter sfilter = (IStreamFilter)filter;
                        if (prevs != null)
                        {
                            sfilter.Init(prevs);
                        }
                        else
                        {
                            sfilter.Init(new StreamFromBundle(prevb));
                        }
                        prevs = sfilter;
                        prevb = null;
                    }
                    else
                    {
                        if (prevs != null)
                        {
                            prevb = filter.Filter(MultipleObjectsBundle.FromStream(prevs));
                            prevs = null;
                        }
                        else
                        {
                            prevb = filter.Filter(prevb);
                            prevs = null;
                        }
                    }
                }
            }
            if (prevb != null)
            {
                return prevb;
            }
            else
            {
                return MultipleObjectsBundle.FromStream(prevs);
            }
        }

        /**
         * Transforms the specified list of objects and their labels into a list of
         * objects and their associations.
         * 
         * @param bundle the objects to process
         * @return processed objects
         */
        protected IBundleStreamSource InvokeFilters(IBundleStreamSource bundle)
        {
            IBundleStreamSource prevs = bundle;
            MultipleObjectsBundle prevb = null;
            if (filters != null)
            {
                foreach (IObjectFilter filter in filters)
                {
                    if (filter is IStreamFilter)
                    {
                        IStreamFilter sfilter = (IStreamFilter)filter;
                        if (prevs != null)
                        {
                            sfilter.Init(prevs);
                        }
                        else
                        {
                            sfilter.Init(new StreamFromBundle(prevb));
                        }
                        prevs = sfilter;
                        prevb = null;
                    }
                    else
                    {
                        if (prevs != null)
                        {
                            prevb = filter.Filter(MultipleObjectsBundle.FromStream(prevs));
                            prevs = null;
                        }
                        else
                        {
                            prevb = filter.Filter(prevb);
                            prevs = null;
                        }
                    }
                }
            }
            if (prevs != null)
            {
                return prevs;
            }
            else
            {
                return new StreamFromBundle(prevb);
            }
        }
        public abstract MultipleObjectsBundle LoadData();
        
        /**
         * Get the logger for this database connection.
         * 
         * @return Logger
         */
        protected abstract Logging GetLogger();

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public abstract class Parameterizer : AbstractParameterizer
        {
            protected IList<IObjectFilter> filters;
            protected IParser parser = null;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
            }

            protected void ConfigFilters(IParameterization config)
            {
                ObjectListParameter<IObjectFilter> filterParam =
                    new ObjectListParameter<IObjectFilter>(FILTERS_ID, typeof(IObjectFilter), true);
                if (config.Grab(filterParam))
                {
                    filters = filterParam.InstantiateClasses(config);
                }
            }

            protected void ConfigParser(IParameterization config, Type parserRestrictionClass, Type parserDefaultValueClass)
            {
                ObjectParameter<IParser> parserParam = new ObjectParameter<IParser>(PARSER_ID, parserRestrictionClass, parserDefaultValueClass);
                if (config.Grab(parserParam))
                {
                    parser = parserParam.InstantiateClass(config);
                }
            }
        }

     
    }
}
