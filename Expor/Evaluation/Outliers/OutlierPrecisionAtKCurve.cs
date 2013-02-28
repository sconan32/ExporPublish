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

    public class OutlierPrecisionAtKCurve : IEvaluator
    {
        /**
         * The logger.
         */
        private static Logging logger = Logging.GetLogger(typeof(OutlierPrecisionAtKCurve));

        /**
         * The pattern to identify positive classes.
         * 
         * <p>
         * Key: {@code -precision.positive}
         * </p>
         */
        public static OptionDescription POSITIVE_CLASS_NAME_ID =
            OptionDescription.GetOrCreate("precision.positive", "Class label for the 'positive' class.");

        /**
         * Maximum value for k
         * 
         * <p>
         * Key: {@code -precision.k}
         * </p>
         */
        public static OptionDescription MAX_K_ID =
            OptionDescription.GetOrCreate("precision.maxk", "Maximum value of 'k' to compute the curve up to.");

        /**
         * Stores the "positive" class.
         */
        private Regex positiveClassName;

        /**
         * Maximum value for k
         */
        int maxk = Int32.MaxValue;

        /**
         * Constructor.
         * 
         * @param positiveClassName Regex to recognize outliers
         * @param maxk Maximum value for k
         */
        public OutlierPrecisionAtKCurve(Regex positiveClassName, int maxk)
        {

            this.positiveClassName = positiveClassName;
            this.maxk = maxk;
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
                db.Hierarchy.Add(o, ComputePrecisionResult(o.GetScores().Count, positiveids, sorted));
                // Process them only once.
                orderings.Remove(o.GetOrdering());
            }

            // FIXME: find appropriate place to add the derived result
            // otherwise apply an ordering to the database IDs.
            foreach (IOrderingResult or in orderings)
            {
                IDbIds sorted = or.Iter(or.GetDbIds());
                db.Hierarchy.Add(or, ComputePrecisionResult(or.GetDbIds().Count, positiveids, sorted));
            }
        }

        private XYCurve ComputePrecisionResult(int size, ISetDbIds positiveids, IDbIds order)
        {
            if (order.Count != size)
            {
                throw new Exception("Iterable result doesn't match database size - incomplete ordering?");
            }
            int lastk = Math.Min(size, maxk);
            XYCurve curve = new PrecisionAtKCurve("k", "Precision", lastk);

            int pos = 0;
            int i = 0;
            for (int k = 1; k <= lastk; k++, i++)
            {
                if (positiveids.Contains(order.ElementAt(i).DbId))
                {
                    pos++;
                }
                curve.AddAndSimplify(k, pos / (double)k);
            }
            if (logger.IsVerbose)
            {
                logger.Verbose("Precision @ " + lastk + " " + ((pos * 1.0) / lastk));
            }
            return curve;
        }

        /**
         * Precision at K curve.
         * 
         * @author Erich Schubert
         */
        public class PrecisionAtKCurve : XYCurve
        {
            /**
             * Constructor.
             * 
             * @param size Size estimation
             */
            public PrecisionAtKCurve(String labelx, String labely, int size) :
                base("k", "Precision", size)
            {
            }


            public override String LongName
            {
                get { return "Precision @ k Curve"; }
            }


            public override String ShortName
            {
                get { return "precision-at-k"; }
            }


            public override void WriteToText(TextWriterStream sout, String label)
            {
                int last = Count - 1;
                sout.CommentPrintLine("Precision @ " + ((int)GetX(last)) + ": " + GetY(last));
                sout.CommentPrintSeparator();
                sout.Flush();
                sout.CommentPrint(labelx);
                sout.CommentPrint(" ");
                sout.CommentPrint(labely);
                sout.Flush();
                for (int pos = 0; pos < data.Count; pos += 2)
                {
                    sout.InlinePrint((int)data[pos]);
                    sout.InlinePrint(data[pos + 1]);
                    sout.Flush();
                }
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

            protected int maxk = Int32.MaxValue;

            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                PatternParameter positiveClassNameP = new PatternParameter(POSITIVE_CLASS_NAME_ID);
                if (config.Grab(positiveClassNameP))
                {
                    positiveClassName = positiveClassNameP.GetValue();
                }
                IntParameter maxkP = new IntParameter(MAX_K_ID, true);
                if (config.Grab(maxkP))
                {
                    maxk = maxkP.GetValue();
                }
            }


            protected override object MakeInstance()
            {
                return new OutlierPrecisionAtKCurve(positiveClassName, maxk);
            }

         
        }

    }
}
