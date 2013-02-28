using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Documentation;

namespace Socona.Expor.Evaluation.Clustering
{
    [Reference(Authors = "Bagga, A. and Baldwin, B.",
        Title = "Entity-based cross-document coreferencing using the Vector Space Model",
        BookTitle = "Proc. COLING '98 Proceedings of the 17th international conference on Computational linguistics",
        Url = "http://dx.doi.org/10.3115/980451.980859")]
    public class BCubed
    {
        /**
         * Result cache
         */
        protected double bCubedPrecision = -1.0, bCubedRecall = -1.0;

        /**
         * Constructor.
         * 
         * @param table Contingency table
         */
        internal BCubed(ClusterContingencyTable table)
        {

            bCubedPrecision = 0.0;
            bCubedRecall = 0.0;

            for (int i1 = 0; i1 < table.Size1; i1++)
            {
                for (int i2 = 0; i2 < table.Size2; i2++)
                {
                    // precision of one item
                    double precision = 1.0 * table.Contingency[i1, i2] / table.Contingency[i1, table.Size2];
                    // precision for all items in cluster
                    bCubedPrecision += (precision * table.Contingency[i1, i2]);

                    // recall of one item
                    double recall = 1.0 * table.Contingency[i1, i2] / table.Contingency[table.Size1, i2];
                    // recall for all items in cluster
                    bCubedRecall += (recall * table.Contingency[i1, i2]);
                }
            }
            bCubedPrecision = bCubedPrecision / table.Contingency[table.Size1, table.Size2];
            bCubedRecall = bCubedRecall / table.Contingency[table.Size1, table.Size2];
        }

        /**
         * Get the BCubed Precision (first clustering) (normalized, 0 = unequal)
         * 
         * @return BCubed Precision
         */
        public double Precision
        {
            get { return bCubedPrecision; }
        }

        /**
         * Get the BCubed Recall (first clustering) (normalized, 0 = unequal)
         * 
         * @return BCubed Recall
         */
        public double Recall
        {
            get { return bCubedRecall; }
        }

        /**
         * Get the BCubed F1-Measure
         * 
         * @return BCubed F1-Measure
         */
        public double F1Measure()
        {
            return Socona.Expor.Evaluation.Clustering.ClusterContingencyTable.Util.
                F1Measure(Precision, Recall);
        }
    }
}
