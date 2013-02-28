using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Algorithms.Clustering;
using Socona.Expor.Algorithms.Clustering.Trivial;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Databases;
using Socona.Expor.Results.TextIO;
using Socona.Expor.Results;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.Evaluation.Clustering
{
    public class EvaluateClustering : IEvaluator
    {
        /**
         * Logger for Debug output.
         */
        protected static Logging logger = Logging.GetLogger(typeof(EvaluateClustering));

        /**
         * Parameter to obtain the reference clustering. Defaults to a flat label
         * clustering.
         */
        public static OptionDescription REFERENCE_ID = OptionDescription.GetOrCreate(
            "paircounting.reference" , "Reference clustering to compare with. Defaults to a by-label clustering.");

        /**
         * Parameter flag for special noise handling.
         */
        public static OptionDescription NOISE_ID = OptionDescription.GetOrCreate(
            "paircounting.noisespecial" ,"Use special handling for noise clusters.");

        /**
         * Parameter flag to disable self-pairing
         */
        public static OptionDescription SELFPAIR_ID = OptionDescription.GetOrCreate(
            "paircounting.selfpair" , "Enable self-pairing for cluster comparison.");

        /**
         * Reference algorithm.
         */
        private IClusteringAlgorithm referencealg;

        /**
         * Apply special handling to noise "clusters".
         */
        private bool noiseSpecialHandling;

        /**
         * Use self-pairing in pair-counting measures
         */
        private bool selfPairing;

        /**
         * Constructor.
         * 
         * @param referencealg Reference clustering
         * @param noiseSpecialHandling Noise handling flag
         * @param selfPairing Self-pairing flag
         */
        public EvaluateClustering(IClusteringAlgorithm referencealg, bool noiseSpecialHandling, bool selfPairing)
        {

            this.referencealg = referencealg;
            this.noiseSpecialHandling = noiseSpecialHandling;
            this.selfPairing = selfPairing;
        }


        public void ProcessNewResult(IHierarchicalResult baseResult, IResult result)
        {
            IDatabase db = ResultUtil.FindDatabase(baseResult);
            var crs = ResultUtil.GetClusteringResults(result);
            if (crs == null || crs.Count < 1)
            {
                return;
            }
            // Compute the reference clustering
            ClusterList refc = null;
            // Try to find an existing reference clustering (globally)
            if (refc == null)
            {
                ICollection<ClusterList> cs = ResultUtil.FilterResults<ClusterList>
                    (baseResult, typeof(ClusterList));
                foreach (ClusterList test in cs)
                {
                    if (IsReferenceResult(test))
                    {
                        refc = test;
                        break;
                    }
                }
            }
            // Try to find an existing reference clustering (locally)
            if (refc == null)
            {
                ICollection<ClusterList> cs = ResultUtil.FilterResults<ClusterList>
                    (result, typeof(ClusterList));
                foreach (ClusterList test in cs)
                {
                    if (IsReferenceResult(test))
                    {
                        refc = test;
                        break;
                    }
                }
            }
            if (refc == null)
            {
                logger.Debug("Generating a new reference clustering.");
                IResult refres = referencealg.Run(db);
                IList<ClusterList> refcrs = ResultUtil.GetClusteringResults(refres);
                if (refcrs.Count == 0)
                {
                    logger.Warning("Reference algorithm did not return a clustering result!");
                    return;
                }
                if (refcrs.Count > 1)
                {
                    logger.Warning("Reference algorithm returned more than one result!");
                }
                refc = refcrs[(0)];
            }
            else
            {
                logger.Debug("Using existing clustering: " + refc.LongName + " " + refc.ShortName);
            }
            foreach (ClusterList c in crs)
            {
                if (c == refc)
                {
                    continue;
                }
                ClusterContingencyTable contmat = new ClusterContingencyTable(selfPairing, noiseSpecialHandling);
                contmat.Process(refc, c);

                db.Hierarchy.Add(c, new ScoreResult(contmat));
            }
        }

        private bool IsReferenceResult(ClusterList t)
        {
            // FIXME: don't hard-code strings


            switch (t.ShortName)
            {
                case "bylabel-clustering":
                case "bymodel-clustering":
                case "allinone-clustering":
                case "allinnoise-clustering":

                    return true;
                //  break;
                default:
                    break;
            }
            return false;
        }

    }

    /**
     * Result object for outlier score judgements.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.composedOf ClusterContingencyTable
     */
    public class ScoreResult : BasicResult, ITextWriteable
    {
        /**
         * Cluster contingency table
         */
        protected ClusterContingencyTable contmat;

        /**
         * Constructor.
         * 
         * @param contmat score result
         */
        public ScoreResult(ClusterContingencyTable contmat) :
            base("Cluster-Evalation" ,"cluster-evaluation")
        {
            this.contmat = contmat;
        }

        /**
         * Get the contingency table
         * 
         * @return the contingency table
         */
        public ClusterContingencyTable GetContingencyTable()
        {
            return contmat;
        }


        public void WriteToText(TextWriterStream sout, String label)
        {
            sout.CommentPrint("Pair-F1 :=\t");
            sout.CommentPrint(contmat.GetPaircount().F1Measure());
            sout.CommentPrintLine();
            sout.CommentPrint("Pair-Precision :=\t");
            sout.CommentPrint(contmat.GetPaircount().Precision());
             sout.CommentPrintLine();
            sout.CommentPrint("Pair-Recall :=\t");
            sout.CommentPrint(contmat.GetPaircount().Recall());
             sout.CommentPrintLine();
            sout.CommentPrint("Pair-Rand :=\t");

            sout.CommentPrint(contmat.GetPaircount().RandIndex());
             sout.CommentPrintLine();
            sout.CommentPrint("Pair-AdjustedRand :=\t");
            sout.CommentPrint(contmat.GetPaircount().AdjustedRandIndex());
             sout.CommentPrintLine();
            sout.CommentPrint("Pair-FowlkesMallows :=\t");
            sout.CommentPrint(contmat.GetPaircount().FowlkesMallows());
             sout.CommentPrintLine();
            sout.CommentPrint("Pair-Jaccard :=\t");
            sout.CommentPrint(contmat.GetPaircount().Jaccard());
             sout.CommentPrintLine();
            sout.CommentPrint("Pair-Mirkin :=\t");
            sout.CommentPrint(contmat.GetPaircount().Mirkin());
             sout.CommentPrintLine();
            sout.CommentPrint("Entropy-VI :=\t");

            sout.CommentPrint(contmat.GetEntropy().VariationOfInformation());
             sout.CommentPrintLine();
            sout.CommentPrint("Entropy-NormalizedVI :=\t");
            sout.CommentPrint(contmat.GetEntropy().NormalizedVariationOfInformation());
             sout.CommentPrintLine();
            //sout.CommentPrint("Entropy-F1 := ");
       
            sout.CommentPrint("Edit-F1 :=\t");
            sout.CommentPrint(contmat.GetEdit().F1Measure());
             sout.CommentPrintLine();
            sout.CommentPrint("SM-InvPurity :=\t");

            sout.CommentPrint(contmat.GetSetMatching().InversePurity);
             sout.CommentPrintLine();
            sout.CommentPrint("SM-Purity :=\t");
            sout.CommentPrint(contmat.GetSetMatching().Purity);
             sout.CommentPrintLine();
            sout.CommentPrint("SM-F1 :=\t");
            sout.CommentPrint(contmat.GetSetMatching().F1Measure());
             sout.CommentPrintLine();
            sout.CommentPrint("BCubed-Precision :=\t");
            sout.CommentPrint(contmat.GetBCubed().Precision);
             sout.CommentPrintLine();
            sout.CommentPrint("BCubed-Recall :=\t");
            sout.CommentPrint(contmat.GetBCubed().Recall);
             sout.CommentPrintLine();
            sout.CommentPrint("BCubed-F1 :=\t");
            sout.CommentPrint(contmat.GetBCubed().F1Measure());
             sout.CommentPrintLine();
            sout.Flush();
           
         
         
 
          
         
        
          



     
      
        
            sout.Flush();
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
        protected IClusteringAlgorithm referencealg = null;

        protected bool noiseSpecialHandling = false;

        protected bool selfPairing = false;


        protected override void MakeOptions(IParameterization config)
        {
            base.MakeOptions(config);
            ObjectParameter<IClusteringAlgorithm> referencealgP =
                new ObjectParameter<IClusteringAlgorithm>(
                    EvaluateClustering.REFERENCE_ID, typeof(IClusteringAlgorithm),
                    typeof(ByLabelOrAllInOneClustering));
            if (config.Grab(referencealgP))
            {
                referencealg = referencealgP.InstantiateClass(config);
            }

            BoolParameter noiseSpecialHandlingF = new BoolParameter(EvaluateClustering.NOISE_ID);
            if (config.Grab(noiseSpecialHandlingF))
            {
                noiseSpecialHandling = noiseSpecialHandlingF.GetValue();
            }

            BoolParameter selfPairingF = new BoolParameter(EvaluateClustering.SELFPAIR_ID);
            if (config.Grab(selfPairingF))
            {
                selfPairing = selfPairingF.GetValue();
            }
        }


        protected override Object MakeInstance()
        {
            return new EvaluateClustering(referencealg, noiseSpecialHandling, !selfPairing);
        }

      
    }
}
