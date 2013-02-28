using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Documentation;

namespace Socona.Expor.Evaluation.Clustering
{

    [Reference(Authors = "Pantel, P. and Lin, D.",
        Title = "Document clustering with committees",
        BookTitle = "Proc. 25th ACM SIGIR conference on Research and development in information retrieval",
        Url = "http://dx.doi.org/10.1145/564376.564412")]

    public class EditDistance
    {
        /**
         * Edit operations for first clustering to second clustering.
         */
        int editFirst = -1;

        /**
         * Edit operations for second clustering to first clustering.
         */
        int editSecond = -1;

        /**
         * Baseline for edit operations
         */
        int editOperationsBaseline;

        internal EditDistance(ClusterContingencyTable table)
        {

            editOperationsBaseline = table.Contingency[table.Size1, table.Size2];
            {
                editFirst = 0;

                // iterate over first clustering
                for (int i1 = 0; i1 < table.Size1; i1++)
                {
                    // Get largest cell
                    int largestLabelSet = 0;
                    for (int i2 = 0; i2 < table.Size2; i2++)
                    {
                        largestLabelSet = Math.Max(largestLabelSet, table.Contingency[i1, i2]);
                    }

                    // merge: found (largest) cluster to second clusterings cluster
                    editFirst++;
                    // move: wrong objects from this cluster to correct cluster (of second
                    // clustering)
                    editFirst += table.Contingency[i1, table.Size2] - largestLabelSet;
                }
            }
            {
                editSecond = 0;

                // iterate over second clustering
                for (int i2 = 0; i2 < table.Size2; i2++)
                {
                    // Get largest cell
                    int largestLabelSet = 0;
                    for (int i1 = 0; i1 < table.Size1; i1++)
                    {
                        largestLabelSet = Math.Max(largestLabelSet, table.Contingency[i1, i2]);
                    }

                    // merge: found (largest) cluster to second clusterings cluster
                    editSecond++;
                    // move: wrong objects from this cluster to correct cluster (of second
                    // clustering)
                    editSecond += table.Contingency[table.Size1, i2] - largestLabelSet;
                }
            }
        }

        /**
         * Get the baseline editing Operations ( = total Objects)
         * 
         * @return worst case amount of operations
         */
        public int EditOperationsBaseline
        {
            get { return editOperationsBaseline; }
        }

        /**
         * Get the editing operations required to transform first clustering to
         * second clustering
         * 
         * @return Editing operations used to transform first into second clustering
         */
        public int EditOperationsFirst
        {
            get { return editFirst; }
        }

        /**
         * Get the editing operations required to transform second clustering to
         * first clustering
         * 
         * @return Editing operations used to transform second into first clustering
         */
        public int EditOperationsSecond
        {
            get { return editSecond; }
        }

        /**
         * Get the editing distance to transform second clustering to first
         * clustering (normalized, 0 = unequal)
         * 
         * @return Editing distance first into second clustering
         */
        public double EditDistanceFirst()
        {
            return 1.0 - (1.0 * EditOperationsFirst / EditOperationsBaseline);
        }

        /**
         * Get the editing distance to transform second clustering to first
         * clustering (normalized, 0 = unequal)
         * 
         * @return Editing distance second into first clustering
         */
        public double EditDistanceSecond()
        {
            return 1.0 - (1.0 * EditOperationsSecond / EditOperationsBaseline);
        }

        /**
         * Get the edit distance F1-Measure
         * 
         * @return Edit Distance F1-Measure
         */
        public double F1Measure()
        {
            return Socona.Expor.Evaluation.Clustering.ClusterContingencyTable.Util.
                F1Measure(EditDistanceFirst(), EditDistanceSecond());
        }
    }
}
