using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Maths;

namespace Socona.Expor.Evaluation.Clustering
{

    public class ClusterContingencyTable
    {
        /**
         * Noise cluster handling
         */
        protected bool breakNoiseClusters = false;
        internal bool BreakNoiseClusters
        {
            get { return breakNoiseClusters; }
        }
        /**
         * Self pairing
         */
        protected bool selfPairing = true;
        internal bool SelfPairing
        {
            get { return selfPairing; }
        }
        /**
         * Number of clusters in first
         */
        protected int size1 = -1;
        internal int Size1 { get { return size1; } }
        /**
         * Number of clusters in second
         */
        protected int size2 = -1;
        internal int Size2 { get { return size2; } }

        /**
         * Contingency matrix
         */
        protected int[,] contingency = null;
        internal int[,] Contingency
        {
            get { return contingency; }
        }
        /**
         * Noise flags
         */
        protected BitArray noise1 = null;
        internal BitArray Noise1
        {
            get { return noise1; }
        }
        /**
         * Noise flags
         */
        protected BitArray noise2 = null;
        internal BitArray Noise2
        {
            get { return noise2; }
        }

        /**
         * Pair counting measures
         */
        protected PairCounting paircount = null;

        /**
         * Entropy-based measures
         */
        protected Entropy entropy = null;

        /**
         * Set matching purity measures
         */
        protected SetMatchingPurity smp = null;

        /**
         * Edit-Distance measures
         */
        protected EditDistance edit = null;

        /**
         * BCubed measures
         */
        protected BCubed bcubed = null;

        /**
         * Constructor.
         * 
         * @param selfPairing Build self-pairs
         * @param breakNoiseClusters Break noise clusters into individual objects
         */
        public ClusterContingencyTable(bool selfPairing, bool breakNoiseClusters)
        {

            this.selfPairing = selfPairing;
            this.breakNoiseClusters = breakNoiseClusters;
        }

        /**
         * Process two clustering results.
         * 
         * @param result1 First clustering
         * @param result2 Second clustering
         */
        public void Process(ClusterList result1, ClusterList result2)
        {
            // Get the clusters
            IList<Cluster> cs1 = result1.GetAllClusters();
            IList<Cluster> cs2 = result2.GetAllClusters();

            // Initialize
            size1 = cs1.Count;
            size2 = cs2.Count;
            contingency = new int[size1 + 2, size2 + 2];
            noise1 = new BitArray(size1);
            noise2 = new BitArray(size2);

            // Fill main part of matrix

            {

                for (int i2 = 0; i2 < cs2.Count; i2++)
                {
                    var c2 = cs2[i2];
                    if (c2.IsNoise())
                    {
                        noise2.Set(i2, true);
                    }
                    contingency[size1 + 1, i2] = c2.Size();
                    contingency[size1 + 1, size2] += c2.Size();
                }

                for (int i1 = 0; i1 < cs1.Count; i1++)
                {
                    var c1 = cs1[i1];
                    if (c1.IsNoise())
                    {
                        noise1.Set(i1, true);
                    }
                    IDbIds ids = DbIdUtil.EnsureSet(c1.Ids);
                    contingency[i1, size2 + 1] = c1.Count;
                    contingency[size1, size2 + 1] += c1.Count;


                    for (int i2 = 0; i2 < cs2.Count; i2++)
                    {
                        var c2 = cs2[i2];
                        int count = 0;
                        foreach (IDbId id in c2.Ids)
                        {
                            if (ids.Contains(id.DbId))
                            {
                                count++;
                            }
                        }
                        contingency[i1, i2] = count;
                        contingency[i1, size2] += count;
                        contingency[size1, i2] += count;
                        contingency[size1, size2] += count;
                    }
                }
            }
        }


        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            if (contingency != null)
            {
                for (int i1 = 0; i1 < size1 + 2; i1++)
                {
                    if (i1 >= size1)
                    {
                        buf.Append("------\n");
                    }
                    for (int i2 = 0; i2 < size2 + 2; i2++)
                    {
                        if (i2 >= size2)
                        {
                            buf.Append("| ");
                        }
                        buf.Append(contingency[i1, i2]).Append(" ");
                    }
                    buf.Append("\n");
                }
            }
            // if(pairconfuse != null) {
            // buf.Append(FormatUtil.format(pairconfuse));
            // }
            return buf.ToString();
        }

        /**
         * Get (compute) the pair counting measures.
         * 
         * @return Pair counting measures
         */
        public PairCounting GetPaircount()
        {
            if (paircount == null)
            {
                paircount = new PairCounting(this);
            }
            return paircount;
        }

        /**
         * Get (compute) the entropy based measures
         * 
         * @return Entropy based measures
         */
        public Entropy GetEntropy()
        {
            if (entropy == null)
            {
                entropy = new Entropy(this);
            }
            return entropy;
        }

        /**
         * Get (compute) the edit-distance based measures
         * 
         * @return Edit-distance based measures
         */
        public EditDistance GetEdit()
        {
            if (edit == null)
            {
                edit = new EditDistance(this);
            }
            return edit;
        }

        /**
         * The BCubed based measures
         * 
         * @return BCubed measures
         */
        public BCubed GetBCubed()
        {
            if (bcubed == null)
            {
                bcubed = new BCubed(this);
            }
            return bcubed;
        }

        /**
         * The set-matching measures
         * 
         * @return Set-Matching measures
         */
        public SetMatchingPurity GetSetMatching()
        {
            if (smp == null)
            {
                smp = new SetMatchingPurity(this);
            }
            return smp;
        }

        /**
         * Compute the average Gini for each cluster (in both clusterings -
         * symmetric).
         * 
         * @return Mean and variance of Gini
         */
        public MeanVariance AverageSymmetricGini()
        {
            MeanVariance mv = new MeanVariance();
            for (int i1 = 0; i1 < size1; i1++)
            {
                double purity = 0.0;
                if (contingency[i1,size2] > 0)
                {
                    double cs = contingency[i1,size2]; // sum, as double.
                    for (int i2 = 0; i2 < size2; i2++)
                    {
                        double rel = contingency[i1,i2] / cs;
                        purity += rel * rel;
                    }
                    mv.Put(purity, cs);
                }
            }
            for (int i2 = 0; i2 < size2; i2++)
            {
                double purity = 0.0;
                if (contingency[size1,i2] > 0)
                {
                    double cs = contingency[size1,i2]; // sum, as double.
                    for (int i1 = 0; i1 < size1; i1++)
                    {
                        double rel = contingency[i1,i2] / cs;
                        purity += rel * rel;
                    }
                    mv.Put(purity, cs);
                }
            }
            return mv;
        }

        /**
         * Utility class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public static class Util
        {
            /**
             * F-Measure
             * 
             * @param precision Precision
             * @param recall Recall
             * @param beta Beta value
             * @return F-Measure
             */
            public static double FMeasure(double precision, double recall, double beta)
            {
                double beta2 = beta * beta;
                return (1 + beta2) * precision * recall / (beta2 * precision + recall);
            }

            /**
             * F1-Measure (F-Measure with beta = 1)
             * 
             * @param precision Precision
             * @param recall Recall
             * @return F-Measure
             */
            public static double F1Measure(double precision, double recall)
            {
                return 2 * precision * recall / (precision + recall);
            }
        }

    }
}
