using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;

namespace Socona.Expor.Results.TextIO.Naming
{

    public class SimpleEnumeratingScheme : INamingScheme
    {
        /**
         * Clustering this scheme is applied to.
         */
        private ClusterList clustering;

        /**
         * count how often each name occurred so far.
         */
        private IDictionary<String, Int32> namecount = new Dictionary<String, Int32>();

        /**
         * Assigned cluster names.
         */
        private IDictionary<Cluster, String> names = new Dictionary<Cluster, String>();

        /**
         * This is the postfix added to the first cluster, which will be removed when
         * there is only one cluster of this name.
         */
        private static String nullpostfix = " " + 0;

        /**
         * Constructor.
         * 
         * @param clustering Clustering result to name.
         */
        public SimpleEnumeratingScheme(ClusterList clustering)
        {

            this.clustering = clustering;
            UpdateNames();
        }

        /**
         * Assign names to each cluster (which doesn't have a name yet)
         */
        private void UpdateNames()
        {
            foreach (Cluster cluster in clustering.GetAllClusters())
            {
                string result = null;
                names.TryGetValue(cluster, out result);
                if (result == null)
                {
                    String sugname = cluster.GetNameAutomatic();
                    Int32 count = 0;
                    namecount.TryGetValue(sugname,out count);

                    names[cluster] = sugname + " " + count.ToString();
                    count++;
                    namecount[sugname] = count;
                }
            }
        }

        /**
         * Retrieve the cluster name. When a name has not yet been assigned, call
         * {@link #updateNames}
         */

        public  String GetNameFor(Cluster cluster)
        {
            String nam = names[(cluster)];
            if (nam == null)
            {
                UpdateNames();
                nam = names[cluster];
            }
            if (nam.EndsWith(nullpostfix))
            {
                if (namecount[nam.Substring(0, nam.Length - nullpostfix.Length)] == 1)
                {
                    nam = nam.Substring(0, nam.Length - nullpostfix.Length);
                }
            }
            return nam;
        }
    }
}
