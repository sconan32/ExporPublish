using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Utilities.DataStructures.Hierarchy
{
    /**
 * Centralized hierarchy implementation, using a HashMap of Lists.
 * 
 * @author Erich Schubert
 * 
 * @param <O> Object type (arbitrary!)
 */
    public class HierarchyDictList<O> : IModifiableHierarchy<O>
    {
        /**
         * The data storage for parents
         */
        readonly private Dictionary<O, IList<O>> pmap;

        /**
         * The data storage for children
         */
        readonly private Dictionary<O, IList<O>> cmap;

        /**
         * Constructor
         */
        public HierarchyDictList()
        {

            this.pmap = new Dictionary<O, IList<O>>();
            this.cmap = new Dictionary<O, IList<O>>();
        }

        public override void Add(O parent, O child)
        {
            // Add child to parent.
            {
                IList<O> pchi = this.cmap[parent];
                if (pchi == null)
                {
                    pchi = new List<O>();
                    this.cmap[parent] = pchi;
                }
                if (!pchi.Contains(child))
                {
                    pchi.Add(child);
                }
                else
                {
                    //LoggingUtil.warning("Result added twice: "+parent+" -> "+child);
                }
            }
            // Add child to parent
            {
                IList<O> cpar = this.pmap[child];
                if (cpar == null)
                {
                    cpar = new List<O>();
                    this.pmap[child] = cpar;
                }
                if (!cpar.Contains(parent))
                {
                    cpar.Add(parent);
                }
                else
                {
                    //LoggingUtil.warning("Result added twice: "+parent+" <- "+child);
                }
            }
        }


        public override void Remove(O parent, O child)
        {
            // Remove child from parent.
            {
                IList<O> pchi = this.cmap[parent];
                if (pchi != null)
                {
                    while (pchi.Remove(child))
                    {
                        // repeat - remove all instances
                    }
                    if (pchi.Count == 0)
                    {
                        this.cmap.Remove(parent);
                    }
                }
            }
            // Remove parent from child
            {
                IList<O> cpar = this.pmap[child];
                if (cpar != null)
                {
                    while (cpar.Remove(parent))
                    {
                        // repeat - remove all instances
                    }
                    if (cpar.Count == 0)
                    {
                        this.pmap.Remove(child);
                    }
                }
            }
        }

        /**
         * Put an object along with parent and child lists.
         * 
         * @param obj Object
         * @param parents Parent list
         * @param children Child list
         */
        public void Put(O obj, List<O> parents, List<O> children)
        {
            this.pmap.Add(obj, parents);
            this.cmap.Add(obj, children);
        }


        public override int NumChildren(O obj)
        {
            IList<O> children = this.cmap[obj];
            if (children == null)
            {
                return 0;
            }
            return children.Count;
        }


        public override IList<O> GetChildren(O obj)
        {
            IList<O> children = this.cmap[obj];
            if (children == null)
            {
                return null;
            }
            return children;
        }


        public override IEnumerable<O> Descendants(O obj)
        {
            return GetChildren(obj);
        }


        public override int NumParents(O obj)
        {
            IList<O> parents = this.pmap[obj];
            if (parents == null)
            {
                return 0;
            }
            return parents.Count;
        }


        public override IList<O> GetParents(O obj)
        {
            IList<O> parents = this.pmap[obj];
            if (parents == null)
            {
                return null;
            }
            return parents;
        }


        public override IEnumerable<O> Ancestors(O obj)
        {
            return GetParents(obj);
        }
    }
}
