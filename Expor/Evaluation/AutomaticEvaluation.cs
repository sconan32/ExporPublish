using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Socona.Expor.Algorithms.Clustering.Trivial;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Databases;
using Socona.Expor.Evaluation.Clustering;
using Socona.Expor.Evaluation.Histograms;
using Socona.Expor.Evaluation.Outliers;
using Socona.Expor.Results;
using Socona.Expor.Results.Outliers;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Scaling;
using Socona.Log;

namespace Socona.Expor.Evaluation
{

    public class AutomaticEvaluation : IEvaluator
    {
        /**
         * Class logger
         */
        private static Logging logger = Logging.GetLogger(typeof(AutomaticEvaluation));


        public void ProcessNewResult(IHierarchicalResult baseResult, IResult newResult)
        {
            AutoEvaluateClusterings(baseResult, newResult);
            AutoEvaluateOutliers(baseResult, newResult);
        }

        protected void AutoEvaluateOutliers(IHierarchicalResult baseResult, IResult newResult)
        {
            ICollection<OutlierResult> outliers = ResultUtil.FilterResults<OutlierResult>(newResult, typeof(OutlierResult));
            if (logger.IsDebugging)
            {
                logger.Debug("Number of new outlier results: " + outliers.Count);
            }
            if (outliers.Count > 0)
            {
                ResultUtil.EnsureClusteringResult<IModel>(ResultUtil.FindDatabase(baseResult), baseResult);
                ICollection<ClusterList> clusterings =
                    ResultUtil.FilterResults<ClusterList>(baseResult, typeof(ClusterList));
                if (clusterings.Count == 0)
                {
                    logger.Warning("Could not find a clustering result, even after running 'ensureClusteringResult'?!?");
                    return;
                }
                ClusterList basec = clusterings.GetEnumerator().Current;
                // Find minority class label
                int min = int.MaxValue;
                int total = 0;
                String label = null;
                if (basec.GetAllClusters().Count > 1)
                {
                    foreach (var c in basec.GetAllClusters())
                    {
                        int csize = c.Ids.Count;
                        total += csize;
                        if (csize < min)
                        {
                            min = csize;
                            label = c.Name;
                        }
                    }
                }
                if (label == null)
                {
                    logger.Warning("Could not evaluate outlier results, as I could not find a minority label.");
                    return;
                }
                if (min == 1)
                {
                    logger.Warning("The minority class label had a single object. Try using 'ClassLabelFilter' to identify the class label column.");
                }
                if (min > 0.05 * total)
                {
                    logger.Warning("The minority class I discovered (labeled '" + label + "') has " + (min * 100.0 / total) + "% of objects. Outlier classes should be more rare!");
                }
                logger.Verbose("Evaluating using minority class: " + label);
                Regex pat = new Regex("^" + Regex.Unescape(label) + "$");
                // Compute ROC curve
                new OutlierROCCurve(pat).ProcessNewResult(baseResult, newResult);
                // Compute Precision at k
                new OutlierPrecisionAtKCurve(pat, min * 2).ProcessNewResult(baseResult, newResult);
                // Compute ROC curve
                new OutlierPrecisionRecallCurve(pat).ProcessNewResult(baseResult, newResult);
                // Compute outlier histogram
                new ComputeOutlierHistogram(pat, 50, new LinearScaling(), false).ProcessNewResult(baseResult, newResult);

            }
        }

        protected void AutoEvaluateClusterings(IHierarchicalResult baseResult, IResult newResult)
        {
            ICollection<ClusterList> clusterings =
                ResultUtil.FilterResults<ClusterList>(newResult, typeof(ClusterList));
            if (logger.IsDebugging)
            {
                logger.Warning("Number of new clustering results: " + clusterings.Count);
            }
            for (int i = 0; i < clusterings.Count; i++)
            {
                var test = clusterings.ElementAt(i);

                switch (test.ShortName)
                {
                    case "allinone-clustering":
                    case "allinnoise-clustering":
                    case "bylabel-clustering":
                    case "bymodel-clustering":
                        clusterings.Remove(test);
                        break;
                    default:
                        break;
                }


            }
            if (clusterings.Count > 0)
            {
                new EvaluateClustering(new ByLabelOrAllInOneClustering(), false, true).ProcessNewResult(baseResult, newResult);
            }
        }

        /**
         * Parameterization class
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public class Parameterizer : AbstractParameterizer
        {

            protected override object MakeInstance()
            {
                return new AutomaticEvaluation();
            }

           
        }
    }

}
