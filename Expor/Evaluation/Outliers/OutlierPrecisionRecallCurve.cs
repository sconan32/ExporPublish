using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
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

    public class OutlierPrecisionRecallCurve : IEvaluator
    {
        /**
         * The logger.
         */
        private static Logging logger = Logging.GetLogger(typeof(OutlierPrecisionRecallCurve));

        /**
         * The pattern to identify positive classes.
         * 
         * <p>
         * Key: {@code -precision.positive}
         * </p>
         */
        public static OptionDescription POSITIVE_CLASS_NAME_ID = OptionDescription.GetOrCreate
            ("precision.positive", "Class label for the 'positive' class.");

        /**
         * Stores the "positive" class.
         */
        private Regex positiveClassName;

        /**
         * Constructor.
         * 
         * @param positiveClassName Regex to recognize outliers
         */
        public OutlierPrecisionRecallCurve(Regex positiveClassName)
        {

            this.positiveClassName = positiveClassName;
        }


        public void ProcessNewResult(IHierarchicalResult baseResult, IResult result)
        {
            IDatabase db = ResultUtil.FindDatabase(baseResult);
            // Prepare
            ISetDbIds positiveids = DbIdUtil.EnsureSet(DatabaseUtil.GetObjectsByLabelMatch(db, positiveClassName));

            if (positiveids.Count == 0)
            {
                logger.Warning("Computing a ROC curve failed - no objects matched.");
                return;
            }

            IList<OutlierResult> oresults = ResultUtil.GetOutlierResults(result);
            IList<IOrderingResult> orderings = ResultUtil.GetOrderingResults(result);
            // Outlier results are the main use case.
            foreach (OutlierResult o in oresults)
            {
                IDbIds sorted = o.GetOrdering().Iter(o.GetOrdering().GetDbIds());
                XYCurve curve = ComputePrecisionResult(o.GetScores().Count, positiveids, sorted, o.GetScores());
                db.Hierarchy.Add(o, curve);
                // Process them only once.
                orderings.Remove(o.GetOrdering());
            }

            // FIXME: find appropriate place to add the derived result
            // otherwise apply an ordering to the database IDs.
            foreach (IOrderingResult or in orderings)
            {
                IDbIds sorted = or.Iter(or.GetDbIds());
                XYCurve curve = ComputePrecisionResult(or.GetDbIds().Count, positiveids, sorted, null);
                db.Hierarchy.Add(or, curve);
            }
        }

        private XYCurve ComputePrecisionResult(int size, ISetDbIds ids, IEnumerable<IDbId> iter, IRelation scores)
        {
            int postot = ids.Count;
            int poscnt = 0, total = 0;
            XYCurve curve = new PRCurve(postot + 2);

            double prevscore = Double.NaN;
            foreach(var id in iter)
            
            //for (; iter.valid(); iter.advance())
            {
                // Previous precision rate - y axis
                double curprec = ((double)poscnt) / total;
                // Previous recall rate - x axis
                double curreca = ((double)poscnt) / postot;

                // Analyze next point
                IDbId cur = id.DbId;
                // positive or negative match?
                if (ids.Contains(cur))
                {
                    poscnt += 1;
                }
                total += 1;
                // First iteration ends here
                if (total == 1)
                {
                    continue;
                }
                // defer calculation for ties
                if (scores != null)
                {
                    double curscore =(double) scores[cur];
                    if (prevscore.CompareTo(curscore) == 0)
                    {
                        continue;
                    }
                    prevscore = curscore;
                }
                // Add a new point (for the previous entry - because of tie handling!)
                curve.AddAndSimplify(curreca, curprec);
            }
            // End curve - always at all positives found.
            curve.AddAndSimplify(1.0, postot / total);
            return curve;
        }

        /**
         * P/R Curve
         * 
         * @author Erich Schubert
         */
        public class PRCurve : XYCurve
        {
            /**
             * AUC value for PR curve
             */
            public static String PRAUC_LABEL = "PR-AUC";

            /**
             * Area under curve
             */
            double auc = Double.NaN;

            /**
             * Constructor.
             * 
             * @param size Size estimation
             */
            public PRCurve(int size) :
                base("Recall", "Precision", size)
            {
            }


            public override String LongName
            {
                get { return "Precision-Recall-Curve"; }
            }


            public override String ShortName
            {
                get { return "pr-curve"; }
            }

            /**
             * Get AUC value
             * 
             * @return AUC value
             */
            public double GetAUC()
            {
                if (Double.IsNaN(auc))
                {
                    auc = AreaUnderCurve(this);
                }
                return auc;
            }


            public override void WriteToText(TextWriterStream sout, String label)
            {
                sout.CommentPrintLine(PRAUC_LABEL + ": " + GetAUC());
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
                return new OutlierPrecisionRecallCurve(positiveClassName);
            }

          
        }
    }
}
