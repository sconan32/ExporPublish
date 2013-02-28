using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Socona.Expor.Data;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Maths.Histograms;
using Socona.Expor.Results;
using Socona.Expor.Results.Outliers;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Expor.Utilities.Pairs;
using Socona.Expor.Utilities.Scaling;
using Socona.Log;

namespace Socona.Expor.Evaluation.Histograms
{

    public class ComputeOutlierHistogram : IEvaluator
    {
        /**
         * Logger for debugging.
         */
        protected static Logging logger = Logging.GetLogger(typeof(ComputeOutlierHistogram));

        /**
         * The object pattern to identify positive classes
         * <p>
         * Key: {@code -comphist.positive}
         * </p>
         */
        public static OptionDescription POSITIVE_CLASS_NAME_ID =
            OptionDescription.GetOrCreate("comphist.positive", "Class label for the 'positive' class.");

        /**
         * number of bins for the histogram
         * <p>
         * Default value: {@link EuclideanDistanceFunction}
         * </p>
         * <p>
         * Key: {@code -comphist.bins}
         * </p>
         */
        public static OptionDescription BINS_ID =
            OptionDescription.GetOrCreate("comphist.bins", "number of bins");

        /**
         * Parameter to specify a scaling function to use.
         * <p>
         * Key: {@code -comphist.scaling}
         * </p>
         */
        public static OptionDescription SCALING_ID =
            OptionDescription.GetOrCreate("comphist.scaling", "Class to use as scaling function.");

        /**
         * Flag to count frequencies of outliers and non-outliers separately
         * <p>
         * Key: {@code -histogram.splitfreq}
         * </p>
         */
        public static OptionDescription SPLITFREQ_ID =
            OptionDescription.GetOrCreate("histogram.splitfreq", "Use separate frequencies for outliers and non-outliers.");

        /**
         * Stores the "positive" class.
         */
        private Regex positiveClassName = null;

        /**
         * Number of bins
         */
        private int bins;

        /**
         * Scaling function to use
         */
        private IScalingFunction scaling;

        /**
         * Flag to make split frequencies
         */
        private bool splitfreq = false;

        /**
         * Constructor.
         * 
         * @param positive_class_name Class name
         * @param bins Bins
         * @param scaling Scaling
         * @param splitfreq Scale inlier and outlier frequencies independently
         */
        public ComputeOutlierHistogram(Regex positive_class_name, int bins,
            IScalingFunction scaling, bool splitfreq)
        {

            this.positiveClassName = positive_class_name;
            this.bins = bins;
            this.scaling = scaling;
            this.splitfreq = splitfreq;
        }

        /**
         * Evaluate a single outlier result as histogram.
         * 
         * @param database Database to process
         * @param or Outlier result
         * @return Result
         */
        public HistogramResult<DoubleVector> EvaluateOutlierResult(IDatabase database, OutlierResult or)
        {
            if (scaling is IOutlierScalingFunction)
            {
                IOutlierScalingFunction oscaling = (IOutlierScalingFunction)scaling;
                oscaling.Prepare(or);
            }

            IModifiableDbIds ids = DbIdUtil.NewHashSet(or.GetScores().GetDbIds());
            IDbIds outlierIds = DatabaseUtil.GetObjectsByLabelMatch(database, positiveClassName);
            // first value for outliers, second for each object
            AggregatingHistogram<DoubleDoublePair, DoubleDoublePair> hist;
            // If we have useful (finite) min/max, use these for binning.
            double min = scaling.GetMin();
            double max = scaling.GetMax();
            if (Double.IsInfinity(min) || Double.IsNaN(min) || Double.IsInfinity(max) || Double.IsNaN(max))
            {
                hist = FlexiHistogram<object, object>.DoubleSumDoubleSumHistogram(bins);
            }
            else
            {
                hist = AggregatingHistogram<object, object>.DoubleSumDoubleSumHistogram(bins, min, max);
            }
            // first fill histogram only with values of outliers
            DoubleDoublePair positive, negative;
            if (!splitfreq)
            {
                positive = new DoubleDoublePair(0.0, 1.0 / ids.Count);
                negative = new DoubleDoublePair(1.0 / ids.Count, 0.0);
            }
            else
            {
                positive = new DoubleDoublePair(0.0, 1.0 / outlierIds.Count);
                negative = new DoubleDoublePair(1.0 / (ids.Count - outlierIds.Count), 0.0);
            }
            ids.RemoveDbIds(outlierIds);
            // fill histogram with values of each object
            foreach (var id in ids)
            {
                //for(DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
                double result =(double) or.GetScores()[id];
                result = scaling.GetScaled(result);
                hist.Aggregate(result, negative);
            }
            // for(DbIdIter iter = outlierIds.iter(); iter.valid(); iter.advance()) {
            foreach (var id in outlierIds)
            {
                double result =(double) or.GetScores()[id];
                result = scaling.GetScaled(result);
                hist.Aggregate(result, positive);
            }

            // turn into Collection

            ICollection<DoubleVector> collHist = new List<DoubleVector>(hist.GetNumBins());
            foreach (DoubleObjPair<DoubleDoublePair> ppair in hist)
            {
                DoubleDoublePair data = ppair.GetSecond();
                DoubleVector row = new DoubleVector(new double[] { ppair.first, data.first, data.second });
                collHist.Add(row);
            }
            return new HistogramResult<DoubleVector>("Outlier Score Histogram", "outlier-histogram", collHist);
        }


        public void ProcessNewResult(IHierarchicalResult baseResult, IResult result)
        {
            IDatabase db = ResultUtil.FindDatabase(baseResult);
            IList<OutlierResult> ors = ResultUtil.FilterResults<OutlierResult>(result, typeof(OutlierResult));
            if (ors == null || ors.Count <= 0)
            {
                // logger.warning("No outlier results found for "+ComputeOutlierHistogram.class.GetSimpleName());
                return;
            }

            foreach (OutlierResult or in ors)
            {
                db.Hierarchy.Add(or, EvaluateOutlierResult(db, or));
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
             * Stores the "positive" class.
             */
            protected Regex positiveClassName = null;

            /**
             * Number of bins
             */
            protected int bins;

            /**
             * Scaling function to use
             */
            protected IScalingFunction scaling;

            /**
             * Flag to make split frequencies
             */
            protected bool splitfreq = false;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                PatternParameter positiveClassNameP = new PatternParameter(POSITIVE_CLASS_NAME_ID, true);
                if (config.Grab(positiveClassNameP))
                {
                    positiveClassName = positiveClassNameP.GetValue();
                }

                IntParameter binsP = new IntParameter(BINS_ID, new GreaterConstraint<int>(1), 50);
                if (config.Grab(binsP))
                {
                    bins = binsP.GetValue();
                }

                ObjectParameter<IScalingFunction> scalingP = new ObjectParameter<IScalingFunction>(SCALING_ID, typeof(IScalingFunction), typeof(IdentityScaling));
                if (config.Grab(scalingP))
                {
                    scaling = scalingP.InstantiateClass(config);
                }

                BoolParameter splitfreqF = new BoolParameter(SPLITFREQ_ID);
                if (config.Grab(splitfreqF))
                {
                    splitfreq = splitfreqF.GetValue();
                }

            }


            protected override object MakeInstance()
            {
                return new ComputeOutlierHistogram(positiveClassName, bins, scaling, splitfreq);
            }

        }
    }
}
