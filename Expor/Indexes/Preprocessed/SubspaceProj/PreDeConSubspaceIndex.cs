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
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.Indexes.Preprocessed.SubspaceProj
{

    [Title("PreDeCon Preprocessor")]
    [Description("Computes the projected dimension of objects of a certain database according to the PreDeCon algorithm.\n" + "The variance analysis is based on epsilon range queries.")]
    public class PreDeConSubspaceIndex<V> : AbstractSubspaceProjectionIndex<V, SubspaceProjectionResult>
    where V : INumberVector
    {
        /*
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(PreDeConSubspaceIndex<V>));

        /**
         * The threshold for small eigenvalues.
         */
        protected double delta;

        /**
         * The kappa value for generating the variance vector.
         */
        private int kappa = 50;

        /**
         * Constructor.
         * 
         * @param relation Relation
         * @param epsilon Epsilon value
         * @param rangeQueryDistanceFunction range query distance
         * @param minpts Minpts parameter
         * @param delta Delta value
         */
        public PreDeConSubspaceIndex(IRelation relation, IDistanceValue epsilon,
            IDistanceFunction rangeQueryDistanceFunction, int minpts, double delta) :
            base(relation, epsilon, rangeQueryDistanceFunction, minpts)
        {
            this.delta = delta;
        }


        protected override SubspaceProjectionResult computeProjection(IDbIdRef id,
            IDistanceDbIdList neighbors, IRelation database)
        {
            StringBuilder msg = null;

            int referenceSetSize = neighbors.Count;
            V obj = (V)database[(id)];

            if (GetLogger().IsDebugging)
            {
                msg = new StringBuilder();
                msg.Append("referenceSetSize = " + referenceSetSize);
                msg.Append("\ndelta = " + delta);
            }

            if (referenceSetSize == 0)
            {
                throw new ApplicationException("Reference Set Size = 0. This should never happen!");
            }

            // prepare similarity matrix
            int dim = obj.Count;
            Matrix simMatrix = new Matrix(dim, dim, 0);
            for (int i = 0; i < dim; i++)
            {
                simMatrix[i, i] = 1;
            }

            // prepare projected dimensionality
            int projDim = 0;

            // start variance analysis
            double[] sum = new double[dim];
            foreach (IDistanceResultPair neighbor in neighbors)
            {
                V o = (V)database[(neighbor.DbId)];
                for (int d = 0; d < dim; d++)
                {
                    sum[d] += Math.Pow(obj[(d + 1)] - o[(d + 1)], 2.0);
                }
            }

            for (int d = 0; d < dim; d++)
            {
                if (Math.Sqrt(sum[d]) / referenceSetSize <= delta)
                {
                    if (msg != null)
                    {
                        msg.Append("\nsum[" + d + "]= " + sum[d]);
                        msg.Append("\n  Math.Sqrt(sum[d]) / referenceSetSize)= " + Math.Sqrt(sum[d]) / referenceSetSize);
                    }
                    // projDim++;
                    simMatrix[d, d] = kappa;
                }
                else
                {
                    // bug in paper?
                    projDim++;
                }
            }

            if (projDim == 0)
            {
                if (msg != null)
                {
                    // msg.Append("\nprojDim == 0!");
                }
                projDim = dim;
            }

            if (msg != null)
            {
                msg.Append("\nprojDim " /*+ database.getObjectLabelQuery().get(id)*/ + ": " + projDim);
                msg.Append("\nsimMatrix " /*+ database.getObjectLabelQuery().get(id)*/ + ": " +
                    FormatUtil.Format(simMatrix, FormatUtil.NF4));
                GetLogger().Debug(msg.ToString());
            }

            return new SubspaceProjectionResult(projDim, simMatrix);
        }


        public override String LongName
        {
            get { return "PreDeCon Subspaces"; }
        }


        public override String ShortName
        {
            get { return "PreDeCon-subsp"; }
        }


        protected override Logging GetLogger()
        {
            return logger;
        }

        /**
         * Factory
         * 
         * @author Erich Schubert
         * 
         * @apiviz.stereotype factory
         * @apiviz.uses PreDeConSubspaceIndex oneway - - 芦creates禄
         * 
         * @param <V> Vector type
         * @param <D> Distance type
         */
        public new class Factory : AbstractSubspaceProjectionIndex<V, SubspaceProjectionResult>.Factory
        {
            /**
             * The default value for delta.
             */
            public static double DEFAULT_DELTA = 0.01;

            /**
             * Parameter for Delta
             */
            public static OptionDescription DELTA_ID = OptionDescription.GetOrCreate("predecon.delta",
                "a double between 0 and 1 specifying the threshold for small Eigenvalues (default is delta = " +
                DEFAULT_DELTA + ").");

            /**
             * The threshold for small eigenvalues.
             */
            protected double delta;

            /**
             * Constructor.
             * 
             * @param epsilon
             * @param rangeQueryDistanceFunction
             * @param minpts
             * @param delta
             */
            public Factory(IDistanceValue epsilon, IDistanceFunction rangeQueryDistanceFunction, int minpts, double delta) :
                base(epsilon, rangeQueryDistanceFunction, minpts)
            {
                this.delta = delta;
            }


            public override IIndex Instantiate(IRelation relation)
            {
                return new PreDeConSubspaceIndex<V>(relation, epsilon, rangeQueryDistanceFunction, minpts, delta);
            }

            /**
             * Parameterization class.
             * 
             * @author Erich Schubert
             * 
             * @apiviz.exclude
             */
            public new class Parameterizer :
                AbstractSubspaceProjectionIndex<V, SubspaceProjectionResult>.Factory.Parameterizer
            {
                /**
                 * The threshold for small eigenvalues.
                 */
                protected double delta;


                protected override void MakeOptions(IParameterization config)
                {
                    base.MakeOptions(config);
                    DoubleParameter deltaP = new DoubleParameter(DELTA_ID,
                        new IntervalConstraint<double>(0.0, IntervalConstraint<double>.IntervalBoundary.OPEN,
                            1.0, IntervalConstraint<double>.IntervalBoundary.OPEN),
                            DEFAULT_DELTA);
                    if (config.Grab(deltaP))
                    {
                        delta = deltaP.GetValue();
                    }
                }


                protected override object MakeInstance()
                {
                    return new Factory(epsilon, rangeQueryDistanceFunction, minpts, delta);
                }
            }
        }
    }
}
