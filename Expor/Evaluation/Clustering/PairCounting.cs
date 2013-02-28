using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Documentation;
using Socona.Log;

namespace Socona.Expor.Evaluation.Clustering
{

    public class PairCounting
    {
        /**
         * Pair counting confusion matrix (flat: inBoth, inFirst, inSecond, inNone)
         */
        protected long[] pairconfuse = null;

        /**
         * Constructor.
         */
        internal PairCounting(ClusterContingencyTable table)
        {

            // Aggregations
            long inBoth = 0, in1 = 0, in2 = 0, total = 0;
            // Process first clustering:
            {
                for (int i1 = 0; i1 < table.Size1; i1++)
                {
                    int size = table.Contingency[i1, table.Size2 + 1];
                    if (table.BreakNoiseClusters && table.Noise1[(i1)])
                    {
                        if (table.SelfPairing)
                        {
                            in1 += size;
                        } // else: 0
                    }
                    else
                    {
                        if (table.SelfPairing)
                        {
                            in1 += size * size;
                        }
                        else
                        {
                            in1 += size * (size - 1);
                        }
                    }
                }
            }
            // Process second clustering:
            {
                for (int i2 = 0; i2 < table.Size2; i2++)
                {
                    int size = table.Contingency[table.Size1 + 1, i2];
                    if (table.BreakNoiseClusters && table.Noise2[(i2)])
                    {
                        if (table.SelfPairing)
                        {
                            in2 += size;
                        } // else: 0
                    }
                    else
                    {
                        if (table.SelfPairing)
                        {
                            in2 += size * size;
                        }
                        else
                        {
                            in2 += size * (size - 1);
                        }
                    }
                }
            }
            // Process combinations
            for (int i1 = 0; i1 < table.Size1; i1++)
            {
                for (int i2 = 0; i2 < table.Size2; i2++)
                {
                    int size = table.Contingency[i1, i2];
                    if (table.BreakNoiseClusters && (table.Noise1[(i1)] || table.Noise2[(i2)]))
                    {
                        if (table.SelfPairing)
                        {
                            inBoth += size;
                        } // else: 0
                    }
                    else
                    {
                        if (table.SelfPairing)
                        {
                            inBoth += size * size;
                        }
                        else
                        {
                            inBoth += size * (size - 1);
                        }
                    }
                }
            }
            // The official sum
            int tsize = table.Contingency[table.Size1, table.Size2];
            if (table.Contingency[table.Size1, table.Size2 + 1] != tsize ||
                table.Contingency[table.Size1 + 1, table.Size2] != tsize)
            {
                Logging.GetLogger(this.GetType()).Warning(
                    "PairCounting F-Measure is not well defined for overlapping and incomplete clusterings.");
            }
            if (tsize >= Math.Sqrt(long.MaxValue))
            {
                Logging.GetLogger(this.GetType()).Warning(
                    "Your data set size probably is too big for this implementation, which uses only long precision.");
            }
            if (table.SelfPairing)
            {
                total = tsize * tsize;
            }
            else
            {
                total = tsize * (tsize - 1);
            }
            long inFirst = in1 - inBoth, inSecond = in2 - inBoth;
            long inNone = total - (inBoth + inFirst + inSecond);
            pairconfuse = new long[] { inBoth, inFirst, inSecond, inNone };
        }

        /**
         * Get the pair-counting F-Measure
         * 
         * @param beta Beta value.
         * @return F-Measure
         */
        public double FMeasure(double beta)
        {
            double beta2 = beta * beta;
            double fmeasure = ((1 + beta2) * pairconfuse[0]) / ((1 + beta2) * pairconfuse[0] + beta2 * pairconfuse[1] + pairconfuse[2]);
            return fmeasure;
        }

        /**
         * Get the pair-counting F1-Measure.
         * 
         * @return F1-Measure
         */
        public double F1Measure()
        {
            return FMeasure(1.0);
        }

        /**
         * Computes the pair-counting precision.
         * 
         * @return pair-counting precision
         */
        public double Precision()
        {
            return ((double)pairconfuse[0]) / (pairconfuse[0] + pairconfuse[2]);
        }

        /**
         * Computes the pair-counting recall.
         * 
         * @return pair-counting recall
         */
        public double Recall()
        {
            return ((double)pairconfuse[0]) / (pairconfuse[0] + pairconfuse[1]);
        }

        /**
         * Computes the pair-counting Fowlkes-mallows (flat only, non-hierarchical!)
         * 
         * <p>
         * Fowlkes, E.B. and Mallows, C.L.<br />
         * A method for comparing two hierarchical clusterings<br />
         * In: Journal of the American Statistical Association, Vol. 78 Issue 383
         * </p>
         * 
         * @return pair-counting Fowlkes-mallows
         */
        // TODO: implement for non-flat clusterings!
        [Reference(Authors = "Fowlkes, E.B. and Mallows, C.L.",
            Title = "A method for comparing two hierarchical clusterings",
            BookTitle = "Journal of the American Statistical Association, Vol. 78 Issue 383")]
        public double FowlkesMallows()
        {
            return Math.Sqrt(Precision() * Recall());
        }

        /**
         * Computes the Rand index (RI).
         * 
         * <p>
         * Rand, W. M.<br />
         * Objective Criteria for the Evaluation of Clustering Methods<br />
         * Journal of the American Statistical Association, Vol. 66 Issue 336
         * </p>
         * 
         * @return The Rand index (RI).
         */
        [Reference(Authors = "Rand, W. M.",
            Title = "Objective Criteria for the Evaluation of Clustering Methods",
            BookTitle = "Journal of the American Statistical Association, Vol. 66 Issue 336",
            Url = "http://www.jstor.org/stable/10.2307/2284239")]
        public double RandIndex()
        {
            double sum = pairconfuse[0] + pairconfuse[1] + pairconfuse[2] + pairconfuse[3];
            return (pairconfuse[0] + pairconfuse[3]) / sum;
        }

        /**
         * Computes the adjusted Rand index (ARI).
         * 
         * @return The adjusted Rand index (ARI).
         */
        public double AdjustedRandIndex()
        {
            double nom = pairconfuse[0] * pairconfuse[3] - pairconfuse[1] * pairconfuse[2];
            long d1 = (pairconfuse[0] + pairconfuse[1]) * (pairconfuse[1] + pairconfuse[3]);
            long d2 = (pairconfuse[0] + pairconfuse[2]) * (pairconfuse[2] + pairconfuse[3]);
            return 2 * nom / (d1 + d2);
        }

        /**
         * Computes the Jaccard index
         * 
         * @return The Jaccard index
         */
        public double Jaccard()
        {
            double sum = pairconfuse[0] + pairconfuse[1] + pairconfuse[2];
            return pairconfuse[0] / sum;
        }

        /**
         * Computes the Mirkin index
         * 
         * @return The Mirkin index
         */
        public long Mirkin()
        {
            return 2 * (pairconfuse[1] + pairconfuse[2]);
        }

    }
}
