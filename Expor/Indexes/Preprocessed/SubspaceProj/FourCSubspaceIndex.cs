using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Maths.LinearAlgebra.Pca;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.Indexes.Preprocessed.SubspaceProj
{

    [Title("4C Preprocessor")]
    [Description("Computes the local dimensionality and locally weighted matrix of objects of a certain database according to the 4C algorithm.\n" +
        "The PCA is based on epsilon range queries.")]
    public class FourCSubspaceIndex<V> : AbstractSubspaceProjectionIndex<V, PCAFilteredResult>
    where V : INumberVector
    {
        /**
         * Our logger
         */
        private static Logging logger = Logging.GetLogger(typeof(FourCSubspaceIndex<V>));

        /**
         * The Filtered PCA Runner
         */
        private PCAFilteredRunner<V> pca;

        /**
         * Full constructor.
         * 
         * @param relation Relation
         * @param epsilon Epsilon value
         * @param rangeQueryDistanceFunction
         * @param minpts MinPts value
         * @param pca PCA runner
         */
        public FourCSubspaceIndex(IRelation relation, IDistanceValue epsilon,
            IDistanceFunction rangeQueryDistanceFunction, int minpts, PCAFilteredRunner<V> pca) :
            base(relation, epsilon, rangeQueryDistanceFunction, minpts)
        {
            this.pca = pca;
        }


        protected override PCAFilteredResult computeProjection(IDbIdRef id, IDistanceDbIdList neighbors, IRelation database)
        {
            IModifiableDbIds ids = DbIdUtil.NewArray(neighbors.Count);
            foreach (IDistanceResultPair neighbor in neighbors)
            {
                ids.Add(neighbor.DbId);
            }
            PCAFilteredResult pcares = (PCAFilteredResult)pca.ProcessIds(ids, database);

            if (logger.IsDebugging)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append(id).Append(" "); //.Append(database.GetObjectLabelQuery().Get(id));
                msg.Append("\ncorrDim ").Append(pcares.GetCorrelationDimension());
                logger.Debug(msg.ToString());
            }
            return pcares;
        }


        protected override Logging GetLogger()
        {
            return logger;
        }


        public override String LongName
        {
            get { return "4C local Subspaces"; }
        }


        public override String ShortName
        {
            get { return "4C-subspaces"; }
        }

        /**
         * Factory class for 4C preprocessors.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.stereotype factory
         * @apiviz.uses FourCSubspaceIndex oneway - - 芦creates禄
         * 
         * @param <V> Vector type
         * @param <IDistance> Distance type
         */
        public new  class Factory : AbstractSubspaceProjectionIndex<V, PCAFilteredResult>.Factory
        {
            /**
             * The default value for delta.
             */
            public static double DEFAULT_DELTA = LimitEigenPairFilter.DEFAULT_DELTA;

            /**
             * The Filtered PCA Runner
             */
            private PCAFilteredRunner<V> pca;

            /**
             * Constructor.
             * 
             * @param epsilon
             * @param rangeQueryDistanceFunction
             * @param minpts
             * @param pca
             */
            public Factory(IDistanceValue epsilon, IDistanceFunction rangeQueryDistanceFunction,
                int minpts, PCAFilteredRunner<V> pca) :
                base(epsilon, rangeQueryDistanceFunction, minpts)
            {
                this.pca = pca;
            }


            public override IIndex Instantiate(IRelation relation)
            {
                return new FourCSubspaceIndex<V>(relation, epsilon, rangeQueryDistanceFunction, minpts, pca);
            }

            /**
             * Parameterization class.
             * 
             * @author Erich Schubert
             * 
             * @apiviz.exclude
             */
            public new class Parameterizer :
                AbstractSubspaceProjectionIndex<V, PCAFilteredResult>.Factory.Parameterizer
            {
                /**
                 * The Filtered PCA Runner
                 */
                private PCAFilteredRunner<V> pca;


                protected override void MakeOptions(IParameterization config)
                {
                    base.MakeOptions(config);
                    // flag absolute
                    bool absolute = false;
                    BoolParameter absoluteF = new BoolParameter(LimitEigenPairFilter.EIGENPAIR_FILTER_ABSOLUTE);
                    if (config.Grab(absoluteF))
                    {
                        absolute = absoluteF.GetValue();
                    }

                    // Parameter delta
                    double delta = 0.0;
                    DoubleParameter deltaP = new DoubleParameter(LimitEigenPairFilter.EIGENPAIR_FILTER_DELTA,
                        new GreaterEqualConstraint<double>(0), DEFAULT_DELTA);
                    if (config.Grab(deltaP))
                    {
                        delta = deltaP.GetValue();
                    }
                    // Absolute flag doesn't have a sensible default value for delta.
                    if (absolute && deltaP.TookDefaultValue())
                    {
                        config.ReportError(new WrongParameterValueException("Illegal parameter settingin " +
                            "Flag " + absoluteF.GetName() + " is set, " +
                            "but no value for " + deltaP.GetName() + " is specified."));
                    }

                    // if (optionHandler.isSet(DELTA_P)) {
                    // delta = (Double) optionHandler.GetOptionValue(DELTA_P);
                    // try {
                    // if (!absolute && delta < 0 || delta > 1)
                    // throw new WrongParameterValueException(DELTA_P, "delta", DELTA_D);
                    // } catch (NumberFormatException e) {
                    // throw new WrongParameterValueException(DELTA_P, "delta", DELTA_D, e);
                    // }
                    // } else if (!absolute) {
                    // delta = LimitEigenPairFilter.DEFAULT_DELTA;
                    // } else {
                    // throw new WrongParameterValueException("Illegal parameter settingin "
                    // +
                    // "Flag " + ABSOLUTE_F + " is set, " + "but no value for " + DELTA_P +
                    // " is specified.");
                    // }

                    // Parameterize PCA
                    ListParameterization pcaParameters = new ListParameterization();
                    // eigen pair filter
                    pcaParameters.AddParameter(PCAFilteredRunner<V>.PCA_EIGENPAIR_FILTER, typeof(LimitEigenPairFilter).Name);
                    // Abs
                    if (absolute)
                    {
                        pcaParameters.AddFlag(LimitEigenPairFilter.EIGENPAIR_FILTER_ABSOLUTE);
                    }
                    // delta
                    pcaParameters.AddParameter(LimitEigenPairFilter.EIGENPAIR_FILTER_DELTA, delta);
                    // big value
                    pcaParameters.AddParameter(PCAFilteredRunner<V>.BIG_ID, 50);
                    // small value
                    pcaParameters.AddParameter(PCAFilteredRunner<V>.SMALL_ID, 1);
                    Type cls = ClassGenericsUtil.UglyCastIntoSubclass(typeof(PCAFilteredRunner<V>));
                    pca = pcaParameters.TryInstantiate<PCAFilteredRunner<V>>(cls);
                    foreach (ParameterException e in pcaParameters.GetErrors())
                    {
                        Logging.GetLogger(this.GetType()).Warning("Error in internal parameterizationin " + e.Message);
                    }

                    List<IParameterConstraint> deltaCons = new List<IParameterConstraint>();
                    // TODOin this constraint is already set in the parameter itself, since
                    // it
                    // also applies to the relative case, right? -- erich
                    // deltaCons.Add(new GreaterEqualConstraint(0));
                    deltaCons.Add(new LessEqualConstraint<double>(1));

                    IGlobalParameterConstraint gpc = new ParameterFlagGlobalConstraint<Double>(deltaP, deltaCons, absoluteF, false);
                    config.CheckConstraint(gpc);
                }


                protected override object MakeInstance()
                {
                    return new Factory(epsilon, rangeQueryDistanceFunction, minpts, pca);
                }
            }
        }
    }
}
