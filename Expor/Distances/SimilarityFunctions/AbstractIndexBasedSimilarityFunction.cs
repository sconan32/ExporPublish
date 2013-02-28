using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Queries.SimilarityQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Indexes;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Distances.SimilarityFunctions
{

    public abstract class AbstractIndexBasedSimilarityFunction<O> : IIndexBasedSimilarityFunction<O>
    {
        /**
         * Parameter to specify the preprocessor to be used.
         * <p>
         * Key: {@code -similarityfunction.preprocessor}
         * </p>
         */
        public static OptionDescription INDEX_ID = OptionDescription.GetOrCreate("similarityfunction.preprocessor", "Preprocessor to use.");

        /**
         * Parameter to specify the preprocessor to be used.
         * <p>
         * Key: {@code -similarityfunction.preprocessor}
         * </p>
         */
        protected IIndexFactory indexFactory;

        /**
         * Constructor.
         * 
         * @param indexFactory
         */
        public AbstractIndexBasedSimilarityFunction(IIndexFactory indexFactory) :
            base()
        {
            this.indexFactory = indexFactory;
        }


        abstract public ISimilarityQuery Instantiate(IRelation database);


        public bool IsSymmetric()
        {
            return true;
        }


        public ITypeInformation GetInputTypeRestriction()
        {
            return indexFactory.GetInputTypeRestriction();
        }
        public abstract IDistanceValue GetDistanceFactory();
        /**
         * The actual instance bound to a particular database.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.uses Index
         * 
         * @param <O> Object type
         * @param <I> Index type
         * @param <IDistance> Distance result type
         */
        abstract public class Instance : AbstractDbIdSimilarityQuery<O>, IIndexBasedSimilarityFunctionInstance<O>
        {
            /**
             * Parent index
             */
            protected IIndex index;

            /**
             * Constructor.
             * 
             * @param database Database
             * @param index Index to use
             */
            public Instance(IRelation database, IIndex index) :
                base(database)
            {
                this.index = index;
            }


            public IIndex GetIndex()
            {
                return index;
            }
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public abstract class Parameterizer : AbstractParameterizer
        {
            /**
             * The index factory we use.
             */
            protected IIndexFactory factory = null;

            /**
             * Get the index factory parameter.
             * 
             * @param config Parameterization
             * @param restrictionClass Restriction class
             * @param defaultClass Default class
             */
            protected void ConfigIndexFactory(IParameterization config, Type restrictionClass, Type defaultClass)
            {
                ObjectParameter<IIndexFactory> param = new ObjectParameter<IIndexFactory>(INDEX_ID, restrictionClass, defaultClass);
                if (config.Grab(param))
                {
                    factory = param.InstantiateClass(config);
                }
            }
        }


      
    }
}
