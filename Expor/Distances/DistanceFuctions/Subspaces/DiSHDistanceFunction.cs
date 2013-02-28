using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Indexes.Preprocessed.Preference;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Extenstions;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Log;

namespace Socona.Expor.Distances.DistanceFuctions.Subspace
{

    public class DiSHDistanceFunction :
        AbstractPreferenceVectorBasedCorrelationDistanceFunction<INumberVector>
    {
        /**
         * Logger for debug.
         */
        static Logging logger = Logging.GetLogger(typeof(DiSHDistanceFunction));

        /**
         * Constructor.
         * 
         * @param indexFactory
         * @param epsilon
         */
        public DiSHDistanceFunction(DiSHPreferenceVectorIndex<INumberVector>.Factory indexFactory, double epsilon) :
            base(indexFactory, epsilon)
        {
        }


        public override IDistanceQuery Instantiate(IRelation database)
        {
            // We can't really avoid these Warnings, due to a limitation in Java
            // Generics (AFAICT)

            DiSHPreferenceVectorIndex<INumberVector> indexinst = 
                (DiSHPreferenceVectorIndex<INumberVector>)indexFactory.Instantiate((IRelation)database);
            return new Instance(database, indexinst, GetEpsilon(), this);
        }

        /**
         * Get the minpts value.
         * 
         * @return the minpts parameter
         */
        public int GetMinpts()
        {
            // TODO: Get rid of this cast?
            return ((DiSHPreferenceVectorIndex<INumberVector>.Factory)indexFactory).GetMinpts();
        }

        /**
         * The actual instance bound to a particular database.
         * 
         * @author Erich Schubert
         */
        public new class Instance : AbstractPreferenceVectorBasedCorrelationDistanceFunction<INumberVector>.Instance
        {
            /**
             * Constructor.
             * 
             * @param database IDatabase
             * @param index Preprocessed index
             * @param epsilon Epsilon
             * @param distanceFunction parent distance function
             */
            public Instance(IRelation database, DiSHPreferenceVectorIndex<INumberVector> index, double epsilon,
                DiSHDistanceFunction distanceFunction) :
                base(database, index, epsilon, distanceFunction)
            {
            }

            /**
             * Computes the correlation distance between the two specified vectors
             * according to the specified preference vectors.
             * 
             * @param v1 first vector
             * @param v2 second vector
             * @param pv1 the first preference vector
             * @param pv2 the second preference vector
             * @return the correlation distance between the two specified vectors
             */

            public override PreferenceVectorBasedCorrelationDistance
                CorrelationDistance(INumberVector v1, INumberVector v2, BitArray pv1, BitArray pv2)
            {
                BitArray commonPreferenceVector = (BitArray)pv1.Clone();
                commonPreferenceVector.And(pv2);
                int dim = v1.Count;

                // number of zero values in commonPreferenceVector
                Int32 subspaceDim = dim - commonPreferenceVector.Cardinality();

                // special case: v1 and v2 are in parallel subspaces
                if (commonPreferenceVector.Equals(pv1) || commonPreferenceVector.Equals(pv2))
                {
                    double d = WeightedDistance(v1, v2, commonPreferenceVector);
                    if (d > 2 * epsilon)
                    {
                        subspaceDim++;
                        if (logger.IsDebugging)
                        {
                            //Representation<String> rep = database.GetObjectLabelQuery();
                            StringBuilder msg = new StringBuilder();
                            msg.Append("d ").Append(d);
                            //msg.Append("\nv1 ").Append(rep.Get(v1.GetID()));
                            //msg.Append("\nv2 ").Append(rep.Get(v2.GetID()));
                            msg.Append("\nsubspaceDim ").Append(subspaceDim);
                            msg.Append("\ncommon pv ").Append(FormatUtil.Format(dim, commonPreferenceVector));
                            logger.Debug(msg.ToString());
                        }
                    }
                }

                // flip commonPreferenceVector for distance computation in common subspace
                BitArray inverseCommonPreferenceVector = (BitArray)commonPreferenceVector.Clone();
                inverseCommonPreferenceVector.Flip(0, dim);

                return new PreferenceVectorBasedCorrelationDistance(DatabaseUtil.Dimensionality(relation), subspaceDim,
                    WeightedDistance(v1, v2, inverseCommonPreferenceVector), commonPreferenceVector);
            }
        }

        /**
         * IParameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer :
            AbstractPreferenceVectorBasedCorrelationDistanceFunction<INumberVector>.Parameterizer
        {

            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                Type cls = ClassGenericsUtil.UglyCastIntoSubclass(
                    typeof(DiSHPreferenceVectorIndex<INumberVector>.Factory));
                factory = config.TryInstantiate<DiSHPreferenceVectorIndex<INumberVector>.Factory>(cls);
            }


            protected override object MakeInstance()
            {
                return new DiSHDistanceFunction((DiSHPreferenceVectorIndex<INumberVector>.Factory)factory, epsilon);
            }
        }

       
    }
}
