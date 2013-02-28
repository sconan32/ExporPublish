using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases;
using Socona.Expor.Evaluation;
using Socona.Expor.Results;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.WorkFlows
{

    public class EvaluationStep : IWorkflowStep
    {
        /**
         * Evaluators to run
         */
        private IList<IEvaluator> evaluators = null;

        /**
         * The result we last processed.
         */
        private IHierarchicalResult result;

        /**
         * Constructor.
         * 
         * @param evaluators
         */
        public EvaluationStep(IList<IEvaluator> evaluators)
            : base()
        {

            this.evaluators = evaluators;
        }

        public void RunEvaluators(IHierarchicalResult r, IDatabase db)
        {
            // Run evaluation helpers
            if (evaluators != null)
            {
                foreach (var eva in evaluators)
                {
                    eva.ProcessNewResult(r, r);
                }
                  //new Evaluation(r, evaluators).Update(r);
            }
            this.result = r;
        }


        ///**
        // * Class to handle running the evaluators on a database instance.
        // * 
        // * @author Erich Schubert
        // * 
        // * @apiviz.exclude
        // */
        //public class Evaluation : IResultListener {
        //  /**
        //   * Database
        //   */
        //  private IHierarchicalResult baseResult;

        //  /**
        //   * Evaluators to run.
        //   */
        //  private IList<IEvaluator> evaluators;

        //  /**
        //   * Constructor.
        //   * 
        //   * @param baseResult base result
        //   * @param evaluators Evaluators
        //   */
        //  public Evaluation(IHierarchicalResult baseResult, IList<IEvaluator> evaluators) {
        //    this.baseResult = baseResult;
        //    this.evaluators = evaluators;

        //    baseResult.Hierarchy.AddResultListener(this);
        //  }

        //  /**
        //   * Update on a particular result.
        //   * 
        //   * @param r Result
        //   */
        //  public void update(IResult r) {
        //    foreach(IEvaluator evaluator in evaluators) {
        //      /*
        //       * if(normalizationUndo) { evaluator.setNormalization(normalization); }
        //       */
        //      evaluator.ProcessNewResult(baseResult, r);
        //    }
        //  }


        //  public void ResultAdded(IResult child, IResult parent) {
        //    // Re-run evaluators on result
        //    update(child);
        //  }


        //  public void resultChanged(IResult current) {
        //    // Ignore for now. TODO: re-evaluate?
        //  }


        //  public void resultRemoved(IResult child, IResult parent) {
        //    // TODO: Implement
        //  }
        //}

        public IHierarchicalResult GetResult()
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
             * Evaluators to run
             */
            private IList<IEvaluator> evaluators = null;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                IList<Type> def = new List<Type>(1);
                def.Add(typeof(AutomaticEvaluation));
                // evaluator parameter
                ObjectListParameter<IEvaluator> evaluatorP =
                    new ObjectListParameter<IEvaluator>(OptionDescription.EVALUATOR, typeof(IEvaluator));
                evaluatorP.SetDefaultValue(def);
                if (config.Grab(evaluatorP))
                {
                    evaluators = evaluatorP.InstantiateClasses(config);
                }
            }


            protected override object MakeInstance()
            {
                return new EvaluationStep(evaluators);
            }

          
        }
    }
}
