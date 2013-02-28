using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Algorithms;
using Socona.Expor.Databases;
using Socona.Expor.Indexes;
using Socona.Expor.Persistent;
using Socona.Expor.Results;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.WorkFlows
{

    public class AlgorithmStep : IWorkflowStep
    {
        /**
         * Logger
         */
        private static Logging logger = Logging.GetLogger(typeof(AlgorithmStep));

        /**
         * Holds the algorithm to run.
         */
        private IList<IAlgorithm> algorithms;

        /**
         * The algorithm output
         */
        private BasicResult result = null;

        /**
         * Constructor.
         * 
         * @param algorithms
         */
        public AlgorithmStep(IList<IAlgorithm> algorithms) :
            base()
        {
            this.algorithms = algorithms;
        }

        /**
         * Run algorithms.
         * 
         * @param database Database
         * @return Algorithm result
         */
        public IHierarchicalResult RunAlgorithms(IDatabase database)
        {
            result = new BasicResult("Algorithm Step", "main");
            result.AddChildResult(database);
            if (logger.IsVerbose && database.GetIndexes().Count > 0)
            {
                StringBuilder buf = new StringBuilder();
                buf.Append("Index statistics before running algorithms:");
                buf.Append(FormatUtil.NEWLINE);
                foreach (IIndex idx in database.GetIndexes())
                {
                    IPageFileStatistics stat = idx.GetPageFileStatistics();
                    PageFileUtil.AppendPageFileStatistics(buf, stat);
                }
                logger.Debug(buf.ToString());
            }
            foreach (IAlgorithm algorithm in algorithms)
            {
                Stopwatch swatch = Stopwatch.StartNew();
                //long start =DateTime.Now. System.CurrentTimeMillis();
                IResult res = algorithm.Run(database);
                //long end = System.currentTimeMillis();
                swatch.Stop();
                if (logger.IsVerbose)
                {
                    long elapsedTime = swatch.ElapsedMilliseconds;//end - start;

                    StringBuilder buf = new StringBuilder();
                    buf.Append(algorithm.GetType().Name).Append(" runtime  : ");
                    buf.Append(elapsedTime).Append(" milliseconds.").Append(FormatUtil.NEWLINE);
                    foreach (IIndex idx in database.GetIndexes())
                    {
                        IPageFileStatistics stat = idx.GetPageFileStatistics();
                        PageFileUtil.AppendPageFileStatistics(buf, stat);
                    }
                    logger.Debug(buf.ToString());
                }
                if (res != null)
                {
                    result.AddChildResult(res);
                }
            }
            return result;
        }

        /**
         * Get the algorithm result.
         * 
         * @return Algorithm result.
         */
        public IHierarchicalResult getResult()
        {
            return result;
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public class Parameterizer : AbstractParameterizer
        {
            /**
             * Enable logging of performance data
             */
            protected bool time = false;

            /**
             * Holds the algorithm to run.
             */
            protected IList<IAlgorithm> algorithms;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                // Time parameter
                BoolParameter timeF = new BoolParameter(OptionDescription.TIME_FLAG);
                if (config.Grab(timeF))
                {
                    time = timeF.GetValue();
                }
                // parameter algorithm
                ObjectListParameter<IAlgorithm> ALGORITHM_PARAM =
                    new ObjectListParameter<IAlgorithm>(OptionDescription.ALGORITHM, typeof(IAlgorithm));
                if (config.Grab(ALGORITHM_PARAM))
                {
                    algorithms = ALGORITHM_PARAM.InstantiateClasses(config);
                }
            }


            protected override object MakeInstance()
            {
                // LoggingConfiguration.SetTime(time);
                return new AlgorithmStep(algorithms);
            }

           
        }
    }
}
