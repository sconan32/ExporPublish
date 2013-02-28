using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries.SimilarityQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Indexes.Preprocessed.Snn;
using Socona.Expor.Utilities.Options.Parameterizations;

namespace Socona.Expor.Distances.SimilarityFunctions
{

    public class SharedNearestNeighborSimilarityFunction<O> : AbstractIndexBasedSimilarityFunction<O>
        where O : ISpatialComparable
    {
        /**
         * Constructor.
         * 
         * @param indexFactory Index factory.
         */
        public SharedNearestNeighborSimilarityFunction(
            ISharedNearestNeighborIndexFactory<O> indexFactory) :
            base(indexFactory)
        {
        }


        public override IDistanceValue GetDistanceFactory()
        {
            return Int32DistanceValue.FACTORY;
        }

        /**
         * Compute the intersection size
         * 
         * @param neighbors1 SORTED neighbors of first
         * @param neighbors2 SORTED neighbors of second
         * @return Intersection size
         */
        static protected int CountSharedNeighbors(IDbIds neighbors1, IDbIds neighbors2)
        {
            int intersection = 0;
            //DbIdIter iter1 = neighbors1.iter();
            // DbIdIter iter2 = neighbors2.iter();

            int i1 = 0;
            int i2 = 0;

            // while(iter1.valid() && iter2.valid()) {
            while (i1 < neighbors1.Count && i2 < neighbors2.Count)
            {
                var iter1 = neighbors1.ElementAt(i1);
                var iter2 = neighbors2.ElementAt(i1);
                int comp = iter1.CompareDbId(iter2);
                if (comp == 0)
                {
                    intersection++;
                    i1++;
                    i2++;
                }
                else if (comp < 0)
                {
                    i1++;
                }
                else // iter2 < iter1
                {
                    i2++;
                }
            }
            return intersection;
        }



        public override ISimilarityQuery Instantiate(IRelation database)
        {
            ISharedNearestNeighborIndex<O> indexi = (ISharedNearestNeighborIndex<O>)indexFactory.Instantiate((IRelation)database);
            return new Instance((IRelation)database, indexi);
        }

        /**
         * Instance for a particular database.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.uses SharedNearestNeighborIndex
         * 
         * @param <O> Object type
         */
        public new class Instance : AbstractIndexBasedSimilarityFunction<O>.Instance
        {
            /**
             * Constructor.
             *
             * @param database Database
             * @param preprocessor Index
             */
            public Instance(IRelation database, ISharedNearestNeighborIndex<O> preprocessor) :
                base(database, preprocessor)
            {
            }


            public override IDistanceValue Similarity(IDbIdRef id1, IDbIdRef id2)
            {
                var ind = index as ISharedNearestNeighborIndex<O>;
                IDbIds neighbors1 = ind.GetNearestNeighborSet(id1);
                IDbIds neighbors2 = ind.GetNearestNeighborSet(id2);
                return new Int32DistanceValue(CountSharedNeighbors(neighbors1, neighbors2));
            }


            public override IDistanceValue DistanceFactory
            {
                get { return Int32DistanceValue.FACTORY; }
            }
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         * 
         * @param <O> Object type
         */
        public new class Parameterizer :
            AbstractIndexBasedSimilarityFunction<O>.Parameterizer
        {

            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                ConfigIndexFactory(config, typeof(ISharedNearestNeighborIndexFactory<O>),
                    typeof(SharedNearestNeighborPreprocessor<O>.Factory));
            }


            protected override object MakeInstance()
            {
                return new SharedNearestNeighborSimilarityFunction<O>((ISharedNearestNeighborIndexFactory<O>)factory);
            }
        }
    }
}
