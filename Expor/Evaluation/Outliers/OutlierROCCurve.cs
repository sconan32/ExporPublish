using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Maths.Geometry;
using Socona.Expor.Results.TextIO;
using Socona.Expor.Results;
using Socona.Expor.Results.Outliers;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.Evaluation.Outliers
{

    public class OutlierROCCurve : IEvaluator
    {
        /**
         * The label we use for marking ROCAUC values.
         */
        public static String ROCAUC_LABEL = "ROCAUC";

        /**
         * The logger.
         */
        private static Logging logger = Logging.GetLogger(typeof(OutlierROCCurve));

        /**
         * The pattern to identify positive classes.
         * 
         * <p>
         * Key: {@code -rocauc.positive}
         * </p>
         */
        public static OptionDescription POSITIVE_CLASS_NAME_ID = 
            OptionDescription.GetOrCreate("rocauc.positive", "Class label for the 'positive' class.");

        /**
         * Stores the "positive" class.
         */
        private Regex positiveClassName;

        /**
         * Constructor.
         * 
         * @param positive_class_name Positive class name pattern
         */
        public OutlierROCCurve(Regex positive_class_name) :
            base()
        {
            this.positiveClassName = positive_class_name;
        }

        private ROCResult ComputeROCResult(int size, ISetDbIds positiveids, IDbIds order)
        {
            if (order.Count != size)
            {
                throw new InvalidOperationException(
                    "Iterable result doesn't match database size - incomplete ordering?");
            }
            XYCurve roccurve = ROC.MaterializeROC(
                size, positiveids, new ROC.SimpleAdapter(order.GetEnumerator()));
            double rocauc = XYCurve.AreaUnderCurve(roccurve);
            if (logger.IsVerbose)
            {
                logger.Verbose(ROCAUC_LABEL + ": " + rocauc);
            }

            ROCResult rocresult = new ROCResult(roccurve, rocauc);

            return rocresult;
        }

        private ROCResult ComputeROCResult(int size, ISetDbIds positiveids, OutlierResult or)
        {
            XYCurve roccurve = ROC.MaterializeROC(size, positiveids, new ROC.OutlierScoreAdapter(or));
            double rocauc = XYCurve.AreaUnderCurve(roccurve);
            if (logger.IsVerbose)
            {
                logger.Verbose(ROCAUC_LABEL + ": " + rocauc);
            }

            ROCResult rocresult = new ROCResult(roccurve, rocauc);

            return rocresult;
        }


        public void ProcessNewResult(IHierarchicalResult baseResult, IResult result)
        {
            IDatabase db = ResultUtil.FindDatabase(baseResult);
            // Prepare
            ISetDbIds positiveids = DbIdUtil.EnsureSet(
                DatabaseUtil.GetObjectsByLabelMatch(db, positiveClassName));

            if (positiveids.Count == 0)
            {
                logger.Warning("Computing a ROC curve failed - no objects matched.");
                return;
            }

            bool nonefound = true;
            IList<OutlierResult> oresults = ResultUtil.GetOutlierResults(result);
            IList<IOrderingResult> orderings = ResultUtil.GetOrderingResults(result);
            // Outlier results are the main use case.
            foreach (OutlierResult o in oresults)
            {
                db.Hierarchy.Add(o, ComputeROCResult(o.GetScores().Count, positiveids, o));
                // Process them only once.
                orderings.Remove(o.GetOrdering());
                nonefound = false;
            }

            // FIXME: find appropriate place to add the derived result
            // otherwise apply an ordering to the database IDs.
            foreach (IOrderingResult or in orderings)
            {
                IDbIds sorted = or.Iter(or.GetDbIds());
                db.Hierarchy.Add(or, ComputeROCResult(or.GetDbIds().Count, positiveids, sorted));
                nonefound = false;
            }

            if (nonefound)
            {
                return;
                // logger.warning("No results found to process with ROC curve analyzer. Got "+iterables.size()+" iterables, "+orderings.size()+" orderings.");
            }
        }

        /**
         * Result object for ROC curves.
         * 
         * @author Erich Schubert
         */
        public class ROCResult : XYCurve
        {
            /**
             * AUC value
             */
            private double auc;

            /**
             * Constructor.
             * 
             * @param col roc curve
             * @param rocauc ROC AUC value
             */
            public ROCResult(XYCurve col, double rocauc) :
                base(col)
            {
                this.auc = rocauc;
            }

            /**
             * @return the area under curve
             */
            public double getAUC()
            {
                return auc;
            }


            public String getLongName()
            {
                return "ROC Curve";
            }


            public String getShortName()
            {
                return "roc-curve";
            }


            public void writeToText(TextWriterStream sout, String label)
            {
                sout.CommentPrintLine(ROCAUC_LABEL + ": " + auc);
                sout.Flush();
                base.WriteToText(sout, label);
            }
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
             * Regex for positive class.
             */
            protected Regex positiveClassName = null;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                PatternParameter positiveClassNameP = new PatternParameter(POSITIVE_CLASS_NAME_ID);
                if (config.Grab(positiveClassNameP))
                {
                    positiveClassName = positiveClassNameP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new OutlierROCCurve(positiveClassName);
            }

          
        }
    }
}
