using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Documentation;

namespace Socona.Expor.Evaluation.Clustering
{

    [Reference(Authors = "Meil膬, M",
        Title = "Comparing clusterings",
        BookTitle = "University of Washington, Seattle, Technical Report 418, 2002",
        Url = "http://www.stat.washington.edu/mmp/www.stat.washington.edu/mmp/Papers/compare-colt.pdf")]

    public class SetMatchingPurity
    {
        /**
         * Result cache
         */
        protected double smPurity = -1.0, smInversePurity = -1.0, smFFirst = -1.0, smFSecond = -1.0;

        /**
         * Constructor.
         * 
         * @param table Contingency table
         */
        internal SetMatchingPurity(ClusterContingencyTable table)
        {

            int numobj = table.Contingency[table.Size1, table.Size2];
            {
                smPurity = 0.0;
                smFFirst = 0.0;
                // iterate first clustering
                for (int i1 = 0; i1 < table.Size1; i1++)
                {
                    double precisionMax = 0.0;
                    double fMax = 0.0;
                    for (int i2 = 0; i2 < table.Size2; i2++)
                    {
                        precisionMax = Math.Max(precisionMax, (1.0 * table.Contingency[i1, i2]));
                        fMax = Math.Max(fMax, (2.0 * table.Contingency[i1, i2]) / (table.Contingency[i1, table.Size2] + table.Contingency[table.Size1, i2]));
                        // / numobj));
                    }
                    smPurity += (precisionMax / numobj);
                    smFFirst += (table.Contingency[i1, table.Size2] /(double) table.Contingency[table.Size1, table.Size2]) * fMax;
                    // * Contingency[i1,Size2]/numobj;
                }
            }
            {
                smInversePurity = 0.0;
                smFSecond = 0.0;
                // iterate second clustering
                for (int i2 = 0; i2 < table.Size2; i2++)
                {
                    double recallMax = 0.0;
                    double fMax = 0.0;
                    for (int i1 = 0; i1 < table.Size1; i1++)
                    {
                        recallMax = Math.Max(recallMax, (1.0 * table.Contingency[i1, i2]));
                        fMax = Math.Max(fMax, (2.0 * table.Contingency[i1, i2]) / (table.Contingency[i1, table.Size2] + table.Contingency[table.Size1, i2]));
                        // / numobj));
                    }
                    smInversePurity += (recallMax / numobj);
                    smFSecond += (table.Contingency[table.Size1, i2] / table.Contingency[table.Size1, table.Size2]) * fMax;
                    // * Contingency[i1,Size2]/numobj;
                }
            }
        }

        /**
         * Get the set matchings purity (first:second clustering) (normalized, 1 =
         * equal)
         * 
         * @return purity
         */
        [Reference(Authors = "Zhao, Y. and Karypis, G.",
            Title = "Criterion functions for document clustering: Experiments and analysis",
            BookTitle = "University of Minnesota, Department of Computer Science, Technical Report 01-40, 2001",
            Url = "http://www-users.cs.umn.edu/~karypis/publications/Papers/PDF/vscluster.pdf")]
        public double Purity
        {
            get { return smPurity; }
        }

        /**
         * Get the set matchings inverse purity (second:first clustering)
         * (normalized, 1 = equal)
         * 
         * @return Inverse purity
         */
        public double InversePurity
        {
            get { return smInversePurity; }
        }

        /**
         * Get the set matching F1-Measure
         * 
         * <p>
         * Steinbach, M. and Karypis, G. and Kumar, V. and others<br />
         * A comparison of document clustering techniques<br />
         * KDD workshop on text mining, 2000
         * </p>
         * 
         * @return Set Matching F1-Measure
         */
        [Reference(Authors = "Steinbach, M. and Karypis, G. and Kumar, V. and others",
            Title = "A comparison of document clustering techniques",
            BookTitle = "KDD workshop on text mining, 2000",
            Url = "http://www-users.itlabs.umn.edu/~karypis/publications/Papers/PDF/doccluster.pdf")]
        public double F1Measure()
        {
            return Socona.Expor.Evaluation.Clustering.ClusterContingencyTable.Util.
                F1Measure(Purity, InversePurity);
        }

        /**
         * Get the Van Rijsbergen鈥檚 F measure (asymmetric) for first clustering
         * 
         * <p>
         * E. Amigo虂, J. Gonzalo, J. Artiles, and F. Verdejo <br />
         * A comparison of extrinsic clustering evaluation metrics based on formal constraints<br />
         * Inf. Retrieval, vol. 12, no. 5, pp. 461鈥�86, 2009
         * </p>
         * 
         * @return Set Matching F-Measure of first clustering
         */
        public double FMeasureFirst
        {
            get { return smFFirst; }
        }

        /**
         * Get the Van Rijsbergen鈥檚 F measure (asymmetric) for second clustering
         * 
         * <p>
         * E. Amigo虂, J. Gonzalo, J. Artiles, and F. Verdejo <br />
         * A comparison of extrinsic clustering evaluation metrics based on formal constraints<br />
         * Inf. Retrieval, vol. 12, no. 5, pp. 461鈥�86, 2009
         * </p>
         * 
         * @return Set Matching F-Measure of second clustering
         */
        public double FMeasureSecond
        {
            get { return smFSecond; }
        }
    }
}
