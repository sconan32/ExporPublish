using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Indexes;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Distances.DistanceFuctions
{

    public abstract class AbstractIndexBasedDistanceFunction<O> :
        AbstractDatabaseDistanceFunction<O>, IIndexBasedDistanceFunction
    {
        public static OptionDescription INDEX_ID = OptionDescription.GetOrCreate(
            "distancefunction.index", "Distance index to use.");

        /**
         * Parameter to specify the preprocessor to be used.
         * <p>
         * Key: {@code -distancefunction.preprocessor}
         * </p>
         */
        protected IIndexFactory indexFactory;

        /**
         * Constructor.
         * 
         * @param indexFactory Index factory
         */
        public AbstractIndexBasedDistanceFunction(IIndexFactory indexFactory) :
            base()
        {
            this.indexFactory = indexFactory;
        }


        public override bool IsMetric
        {
            get { return false; }
        }


        public override bool IsSymmetric
        {
            get { return true; }
        }


        public override ITypeInformation GetInputTypeRestriction()
        {
            return indexFactory.GetInputTypeRestriction();
        }

        /**
         * The actual instance bound to a particular database.
         * 
         * @author Erich Schubert
         * 
         * @param <O> Object type
         * @param <I> Index type
         * @param <D> Distance type
         * @param <F> Distance function type
         */
        abstract new public class Instance :
            AbstractDatabaseDistanceQuery<O>, IIndexBasedDistanceFunctionInstance
        {
            /**
             * Index we use
             */
            protected IIndex index;

            /**
             * Our parent distance function
             */
            protected IDistanceFunction parent;

            /**
             * Constructor.
             * 
             * @param database Database
             * @param index Index to use
             * @param parent Parent distance function
             */
            public Instance(IRelation database, IIndex index, IDistanceFunction parent) :
                base(database)
            {
                this.index = index;
                this.parent = parent;
            }


            public IIndex GetIndex()
            {
                return index;
            }


            public override IDistanceFunction DistanceFunction
            {
                get { return parent; }
            }
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         * 
         * @param <F> Factory type
         */
        public abstract class Parameterizer : AbstractParameterizer
        {
            /**
             * The index factory we use.
             */
            protected IIndexFactory factory;

            /**
             * Index factory parameter
             * 
             * @param config Parameterization
             * @param restriction Restriction class
             * @param defaultClass Default value
             */
            public void ConfigIndexFactory(IParameterization config, Type restriction, Type defaultClass)
            {
                ObjectParameter<IIndexFactory> param = new ObjectParameter<IIndexFactory>(INDEX_ID, restriction, defaultClass);
                if (config.Grab(param))
                {
                    factory = param.InstantiateClass(config);
                }
            }
        }
    }
}
