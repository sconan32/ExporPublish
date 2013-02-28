using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases;
using Socona.Expor.Results;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Expor.Utilities.Pairs;
using Socona.Expor.WorkFlows;

namespace Socona.Expor
{

    public class ExporTask : IParameterizable
    {
        /**
         * The settings used, for settings reporting.
         */
        private ICollection<IPair<Object, IParameter>> settings;

        /**
         * The data input step
         */
        private InputStep inputStep;

        /**
         * The algorithm (data mining) step.
         */
        private AlgorithmStep algorithmStep;

        /**
         * The evaluation step.
         */
        private EvaluationStep evaluationStep;

        /**
         * The output/visualization step
         */
        private OutputStep outputStep;

        /**
         * The result object.
         */
        private IHierarchicalResult result;

        /**
         * Constructor.
         * 
         * @param inputStep
         * @param algorithmStep
         * @param evaluationStep
         * @param outputStep
         * @param settings
         */
        public ExporTask(InputStep inputStep, AlgorithmStep algorithmStep, EvaluationStep evaluationStep, OutputStep outputStep,
            ICollection<IPair<Object, IParameter>> settings)
            : base()
        {

            this.inputStep = inputStep;
            this.algorithmStep = algorithmStep;
             this.evaluationStep = evaluationStep;
            this.outputStep = outputStep;
            this.settings = settings;
        }

        /**
         * Method to run the specified algorithm using the specified database
         * connection.
         */
        public void Run()
        {
            // Input step
            IDatabase db = inputStep.GetDatabase();

            // Algorithms - Data Mining Step
            result = algorithmStep.RunAlgorithms(db);

            // TODO: this could be nicer
            result.Hierarchy.Add(result, new SettingsResult(settings));

            // Evaluation
            evaluationStep.RunEvaluators(result, db);

            // Output / Visualization
             outputStep.RunResultHandlers(result);
        }

        /**
         * Get the algorithms result.
         * 
         * @return the result
         */
        public IResult GetResult()
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
            InputStep inputStep = null;

            AlgorithmStep algorithmStep = null;

            EvaluationStep evaluationStep = null;

            ICollection<IPair<Object, IParameter>> settings = null;

            OutputStep outputStep = null;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                // Track the key parameters for reporting the settings.
                TrackParameters track = new TrackParameters(config);

                inputStep = track.TryInstantiate<InputStep>(typeof(InputStep));
                algorithmStep = track.TryInstantiate<AlgorithmStep>(typeof(AlgorithmStep));
                evaluationStep = track.TryInstantiate<EvaluationStep>(typeof(EvaluationStep));
                outputStep = track.TryInstantiate<OutputStep>(typeof(OutputStep));
                // We don't include output parameters
                settings = track.GetAllParameters();
                // configure output with the original parameterization
              
            }

            protected override object MakeInstance()
            {
                return new ExporTask(inputStep, algorithmStep, evaluationStep, outputStep, settings);
            }
        }  
    }
}
