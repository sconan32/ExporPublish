using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Documentation;

namespace Socona.Expor.Evaluation.Clustering
{

    [Reference(Authors = "Meil膬, M.",
        Title = "Comparing clusterings by the variation of information",
        BookTitle = "Learning theory and kernel machines Volume 2777/2003",
        Url = "http://dx.doi.org/10.1007/978-3-540-45167-9_14")]
    public class Entropy
    {
        /**
         * Entropy in first
         */
        protected double entropyFirst = -1.0;

        /**
         * Entropy in second
         */
        protected double entropySecond = -1.0;

        /**
         * Joint entropy
         */
        protected double entropyJoint = -1.0;

        /**
         * Constructor.
         * 
         * @param table Contingency table
         */
        internal Entropy(ClusterContingencyTable table)
        {

            double norm = 1.0 / table.Contingency[table.Size1, table.Size2];
            {
                entropyFirst = 0.0;
                // iterate over first clustering
                for (int i1 = 0; i1 < table.Size1; i1++)
                {
                    if (table.Contingency[i1, table.Size2] > 0)
                    {
                        double probability = norm * table.Contingency[i1, table.Size2];
                        entropyFirst -= probability * Math.Log(probability);
                    }
                }
            }
            {
                entropySecond = 0.0;
                // iterate over first clustering
                for (int i2 = 0; i2 < table.Size2; i2++)
                {
                    if (table.Contingency[table.Size1, i2] > 0)
                    {
                        double probability = norm * table.Contingency[table.Size1, i2];
                        entropySecond -= probability * Math.Log(probability);
                    }
                }
            }
            {
                entropyJoint = 0.0;
                for (int i1 = 0; i1 < table.Size1; i1++)
                {
                    for (int i2 = 0; i2 < table.Size2; i2++)
                    {
                        if (table.Contingency[i1, i2] > 0)
                        {
                            double probability = norm * table.Contingency[i1, i2];
                            entropyJoint -= probability * Math.Log(probability);
                        }
                    }
                }
            }
        }

        /**
         * Get the entropy of the first clustering using Log_2. (not normalized, 0 =
         * equal)
         * 
         * @return Entropy of first clustering
         */
        public double EntropyFirst
        {
            get { return entropyFirst; }
        }

        /**
         * Get the entropy of the second clustering using Log_2. (not normalized, 0
         * = equal)
         * 
         * @return Entropy of second clustering
         */
        public double EntropySecond
        {
            get { return entropySecond; }
        }

        /**
         * Get the joint entropy of both clusterings (not normalized, 0 = equal)
         * 
         * @return Joint entropy of both clusterings
         */
        public double EntropyJoint
        {
            get { return entropyJoint; }
        }

        /**
         * Get the conditional entropy of the first clustering. (not normalized, 0 =
         * equal)
         * 
         * @return Conditional entropy of first clustering
         */
        public double EntropyConditionalFirst()
        {
            return (EntropyJoint - EntropySecond);
        }

        /**
         * Get the conditional entropy of the first clustering. (not normalized, 0 =
         * equal)
         * 
         * @return Conditional entropy of second clustering
         */
        public double EntropyConditionalSecond()
        {
            return (EntropyJoint - EntropyFirst);
        }

        /**
         * Get Powers entropy  (normalized, 0 = equal)
         * Powers = 1 - NMI_Sum
         * 
         * @return Powers
         */
        public double EntropyPowers()
        {
            return (2 * EntropyJoint / (EntropyFirst + EntropySecond) - 1);
        }

        /**
         * Get the mutual information (not normalized, 0 = equal)
         * 
         * @return Mutual information
         */
        public double EntropyMutualInformation()
        {
            return (EntropyFirst + EntropySecond - EntropyJoint);
        }

        /**
         * Get the joint-normalized mutual information (normalized, 0 = unequal)
         * 
         * @return Joint Normalized Mutual information
         */
        public double EntropyNMIJoint()
        {
            if (EntropyJoint == 0)
            {
                return 0;
            }
            return (EntropyMutualInformation() / EntropyJoint);
        }

        /**
         * Get the min-normalized mutual information (normalized, 0 = unequal)
         * 
         * @return Min Normalized Mutual information
         */
        public double EntropyNMIMin()
        {
            return (EntropyMutualInformation() / Math.Min(EntropyFirst, EntropySecond));
        }

        /**
         * Get the max-normalized mutual information (normalized, 0 = unequal)
         * 
         * @return Max Normalized Mutual information
         */
        public double EntropyNMIMax()
        {
            return (EntropyMutualInformation() / Math.Max(EntropyFirst, EntropySecond));
        }

        /**
         * Get the sum-normalized mutual information (normalized, 0 = unequal)
         * 
         * @return Sum Normalized Mutual information
         */
        public double EntropyNMISum()
        {
            return (2 * EntropyMutualInformation() / (EntropyFirst + EntropySecond));
        }

        /**
         * Get the sqrt-normalized mutual information (normalized, 0 = unequal)
         * 
         * @return Sqrt Normalized Mutual information
         */
        public double EntropyNMISqrt()
        {
            if (EntropyFirst * EntropySecond <= 0)
            {
                return EntropyMutualInformation();
            }
            return (EntropyMutualInformation() / Math.Sqrt(EntropyFirst * EntropySecond));
        }

        /**
         * Get the variation of information (not normalized, 0 = equal)
         * 
         * @return Variation of information
         */
        public double VariationOfInformation()
        {
            return (2 * EntropyJoint - (EntropyFirst + EntropySecond));
        }

        /**
         * Get the normalized variation of information (normalized, 0 = equal)
         * NVI = 1 - NMI_Joint
         * 
         * <p>
         * Vinh, N.X. and Epps, J. and Bailey, J.<br />
         * Information theoretic measures for clusterings comparison: is a
         * correction for chance necessary?<br />
         * In: Proc. ICML '09 Proceedings of the 26th Annual International
         * Conference on Machine Learning
         * </p>
         * 
         * @return Normalized Variation of information
         */
        [Reference(Authors = "Vinh, N.X. and Epps, J. and Bailey, J.",
            Title = "Information theoretic measures for clusterings comparison: is a correction for chance necessary?",
            BookTitle = "Proc. ICML '09 Proceedings of the 26th Annual International Conference on Machine Learning",
            Url = "http://dx.doi.org/10.1145/1553374.1553511")]
        public double NormalizedVariationOfInformation()
        {
            return (1.0 - (EntropyMutualInformation() / EntropyJoint));
        }

    }
}
