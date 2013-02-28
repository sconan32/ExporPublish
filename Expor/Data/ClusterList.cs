using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Socona.Expor.Data.Models;
using Socona.Expor.Results;
using Socona.Expor.Utilities.DataStructures.Hierarchy;

namespace Socona.Expor.Data
{

    public class ClusterList : BasicResult
    {
        /**
         * Keep a list of top level clusters.
         */
        private List<Cluster> toplevelclusters;
        /**
        * Cluster hierarchy.
        */
        private IModifiableHierarchy<Cluster> hierarchy;

        /**
         * Constructor with a list of top level clusters
         * 
         * @param name The long name (for pretty printing)
         * @param shortname the short name (for filenames etc.)
         * @param toplevelclusters Top level clusters
         */
        public ClusterList(String name, String shortname, List<Cluster> toplevelclusters)
            : base(name, shortname)
        {
            this.toplevelclusters = toplevelclusters;
              this.hierarchy = new HashMapHierarchy<Cluster>();
        }

        /**
         * Constructor for an empty clustering
         * 
         * @param name The long name (for pretty printing)
         * @param shortname the short name (for filenames etc.)
         */
        public ClusterList(String name, String shortname) :
            this(name, shortname, new List<Cluster>())
        {
        }

        /**
         * Add a cluster to the clustering.
         * 
         * @param n new cluster
         */
        public void AddCluster(Cluster n)
        {
            toplevelclusters.Add(n);
        }

        /**
         * Return top level clusters
         * 
         * @return top level clusters
         */
        public List<Cluster> GetToplevelClusters()
        {
            return toplevelclusters;
        }
        /**
        * Add a cluster to the clustering.
        * 
        * @param clus new cluster
        */
        public void AddToplevelCluster(Cluster clus)
        {
            toplevelclusters.Add(clus);
            hierarchy.Add(clus);
        }

        /**
         * Collect all clusters (recursively) into a List.
         * 
         * @return List of all clusters.
         */
        public List<Cluster> GetAllClusters()
        {
            ISet<Cluster> clu = new HashSet<Cluster>();
            foreach (Cluster rc in toplevelclusters)
            {
                if (!clu.Contains(rc))
                {
                    clu.Add(rc);
                    var id = rc.GetDescendants();
                    foreach (var desc in id)
                    {
                        clu.Add(desc);
                    }

                }
            }
            // Note: we canNOT use TreeSet above, because this comparator is only
            // partial!
            List<Cluster> res = new List<Cluster>(clu);
            res.Sort(new Cluster.PartialComparator());
            return res;
        }
    }

}
