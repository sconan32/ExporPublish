using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Results;
using Socona.Expor.Utilities.Documentation;
using Socona.Log;

namespace Socona.Expor.Algorithms
{

    [Title("Dummy Algorithm")]
    [Description("The algorithm executes an Euclidean 10NN query on all data points, and can be used in unit testing")]
    public class DummyAlgorithm : AbstractAlgorithm
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(DummyAlgorithm));

        /**
         * Constructor, adhering to
         * {@link de.lmu.ifi.dbs.elki.utilities.optionhandling.Parameterizable}
         */
        public DummyAlgorithm() :
            base()
        {
        }

        /**
         * Run the algorithm.
         * 
         * @param database Database
         * @param relation Relation
         * @return Null result
         */
        public IResult Run(IDatabase database, IRelation relation)
        {
            // Get a distance and knn query for the Euclidean distance
            // Hardcoded, only use this if you only allow the eucliden distance
            IDistanceQuery distQuery = database.GetDistanceQuery(
                relation, EuclideanDistanceFunction.STATIC);
            IKNNQuery knnQuery = database.GetKNNQuery(distQuery, 10);

            //for(DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance()) {
            foreach (var iditer in relation.GetDbIds())
            {
                // Get the actual object from the database (but discard the result)
                object o = relation[(iditer)];
                // run a 10NN query for each point (but discard the result)
                knnQuery.GetKNNForDbId(iditer, 10);
            }
            return null;
        }


        public override ITypeInformation[] GetInputTypeRestriction()
        {
            return TypeUtil.Array(EuclideanDistanceFunction.STATIC.GetInputTypeRestriction());
        }


        protected override Logging GetLogger()
        {
            return logger;
        }
    }
}
