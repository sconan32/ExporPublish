using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Models;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Results.TextIO;
using Socona.Expor.Utilities.DataStructures.Hierarchy;

namespace Socona.Expor.Data
{

    // TODO: return unmodifiable collections?
    // TODO: disallow clusters without a IDbIds?
    // TODO: Add IModel interface and delegations consequently since we have the
    // IDbId group and hierarchy delegators?
    public class Cluster : IHierarchical<Cluster>, ITextWriteable
      
    {
        /**
         * Object that the hierarchy management is delegated to.
         */
        private IHierarchy<Cluster> hierarchy = null;

        /**
         * Cluster name.
         */
        protected String name = null;

        /**
         * Cluster data.
         */
        private IDbIds ids = null;

        /**
         * Cluster model.
         */
        private IModel model = null;

        /**
         * Noise?
         */
        private bool noise = false;

        /**
         * Full constructor
         * 
         * @param name Cluster name. May be null.
         * @param ids Object Group
         * @param noise Noise flag
         * @param model IModel. May be null.
         * @param hierarchy Hierarchy object. May be null.
         */
        public Cluster(String name, IDbIds ids, bool noise, IModel model, IHierarchy<Cluster> hierarchy)
            : base()
        {

            // TODO: any way to check that this is a C? (see asC() method)
            this.name = name;
            this.ids = ids;
            this.noise = noise;
            this.model = model;
            this.hierarchy = hierarchy;
        }

        /**
         * Constructor with hierarchy information. A new FullHierarchy object will be
         * created to store the hierarchy information.
         * 
         * @param name Cluster name. May be null.
         * @param ids Object Group
         * @param noise Noise flag
         * @param model IModel. May be null.
         * @param children Children. Will NOT be copied.
         * @param parents Parents. Will NOT be copied.
         */
        public Cluster(String name, IDbIds ids, bool noise, IModel model, IList<Cluster> children, IList<Cluster> parents) :
            this(name, ids, noise, model, null)
        {
            this.SetHierarchy(new HierarchyReferenceLists<Cluster>(this, children, parents));
        }

        /**
         * Constructor without hierarchy information.
         * 
         * @param name Cluster name. May be null.
         * @param ids Object group
         * @param noise Noise flag
         * @param model IModel
         */
        public Cluster(String name, IDbIds ids, bool noise, IModel model) :
            this(name, ids, noise, model, null)
        {
        }

        /**
         * Constructor without hierarchy information.
         * 
         * @param name Cluster name. May be null.
         * @param ids Object group
         * @param model IModel
         */
        public Cluster(String name, IDbIds ids, IModel model) :
            this(name, ids, false, model, null)
        {
        }

        /**
         * Constructor without hierarchy information and name
         * 
         * @param ids Object group
         * @param noise Noise flag
         * @param model IModel
         */
        public Cluster(IDbIds ids, bool noise, IModel model) :
            this(null, ids, noise, model, null)
        {
        }

        /**
         * Constructor without hierarchy information and name
         * 
         * @param ids Object group
         * @param model IModel
         */
        public Cluster(IDbIds ids, IModel model) :
            this(null, ids, false, model, null)
        {
        }

        /**
         * Constructor without hierarchy information and model
         * 
         * @param name Cluster name. May be null.
         * @param ids Object group
         * @param noise Noise flag
         */
        public Cluster(String name, IDbIds ids, bool noise) :
            this(name, ids, noise, null, null)
        {
        }

        /**
         * Constructor without hierarchy information and model
         * 
         * @param name Cluster name. May be null.
         * @param ids Object group
         */
        public Cluster(String name, IDbIds ids) :
            this(name, ids, false,null, null)
        {
        }

        /**
         * Constructor without hierarchy information and name and model
         * 
         * @param ids Cluster name. May be null.
         * @param noise Noise flag
         */
        public Cluster(IDbIds ids, bool noise) :
            this(null, ids, noise, null, null)
        {
        }

        /**
         * Constructor without hierarchy information and name and model
         * 
         * @param ids Object group
         */
        public Cluster(IDbIds ids) :
            this(null, ids, false, null, null)
        {
        }

        /**
         * Constructor with hierarchy but noise flag defaulting to false.
         * 
         * @param name Cluster name. May be null.
         * @param ids Object group
         * @param model IModel. May be null.
         * @param hierarchy Hierarchy object. May be null.
         */
        public Cluster(String name, IDbIds ids, IModel model, IHierarchy<Cluster> hierarchy) :
            this(name, ids, false, model, hierarchy)
        {
        }

        /**
         * Constructor with hierarchy information, but no noise flag. A new
         * FullHierarchy object will be created to store the hierarchy information.
         * 
         * @param name Cluster name. May be null.
         * @param ids Object Group
         * @param model IModel. May be null.
         * @param children Children. Will NOT be copied.
         * @param parents Parents. Will NOT be copied.
         */
        public Cluster(String name, IDbIds ids, IModel model, List<Cluster> children, List<Cluster> parents) :
            this(name, ids, false, model, null)
        {
            this.SetHierarchy(new HierarchyReferenceLists<Cluster>(this, children, parents));
        }

        /**
         * Test hierarchy
         */

        public bool IsHierarchical()
        {
            if (hierarchy == null)
            {
                return false;
            }
            return true;
        }

        /**
         * Delegate to hierarchy object
         */

        public int NumChildren()
        {
            if (hierarchy == null)
            {
                return 0;
            }
            return hierarchy.NumChildren(this);
        }

        /**
         * Delegate to hierarchy object
         */

        public IList<Cluster> GetChildren()
        {
            if (hierarchy == null)
            {
                return new List<Cluster>(0);
            }
            return hierarchy.GetChildren(this);
        }

        /**
         * Delegate to hierarchy object
         */
        //
        //public Iterator<Cluster> iterDescendants() {
        //  if(hierarchy == null) {
        //    return EmptyIterator.STATIC();
        //  }
        //  return hierarchy.iterDescendants(this);
        //}

        /**
         * Collect descendants
         * 
         * @return Set of descendants
         */
        public ISet<Cluster> GetDescendants()
        {
            HashSet<Cluster> set = new HashSet<Cluster>();
            // Add all
            if (hierarchy != null)
            {
                foreach (var d in hierarchy.Descendants(this))
                {
                    set.Add(d);
                }
            }
            return set;
        }

        /**
         * Delegate to hierarchy object
         */

        public int NumParents()
        {
            if (hierarchy == null)
            {
                return 0;
            }
            return hierarchy.NumParents(this);
        }

        /**
         * Delegate to hierarchy object
         */

        public IList<Cluster> GetParents()
        {
            if (hierarchy == null)
            {
                return new List<Cluster>(0);
            }
            return hierarchy.GetParents(this);
        }

        /**
         * Delegate to hierarchy object
         */
        //
        //public Iterator<Cluster> iterAncestors() {
        //  if(hierarchy == null) {
        //    return EmptyIterator.STATIC();
        //  }
        //  return hierarchy.iterAncestors(this);
        //}

        /**
         * Delegate to database object group.
         * 
         * @return Cluster size retrieved from object group.
         */
        public int Size()
        {
            return ids.Count;
        }
        public int Count { get { return ids.Count; } }
        /**
         * Get hierarchy object
         * 
         * @return hierarchy object
         */
        public IHierarchy<Cluster> GetHierarchy()
        {
            return hierarchy;
        }

        /**
         * Set hierarchy object
         * 
         * @param hierarchy new hierarchy object
         */
        public void SetHierarchy(IHierarchy<Cluster> hierarchy)
        {
            this.hierarchy = hierarchy;
        }

        /**
         * Return either the assigned name or the suggested label
         * 
         * @return a name for the cluster
         */
        public String GetNameAutomatic()
        {
            if (name != null)
            {
                return name;
            }
            if (IsNoise())
            {
                return "Noise";
            }
            else
            {
                return "Cluster";
            }
        }

        /**
         * Get Cluster name. May be null.
         * 
         * @return cluster name, or null
         */
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        //public String GetName() {
        //  return name;
        //}

        ///**
        // * Set Cluster name
        // * 
        // * @param name new cluster name
        // */
        //public void setName(String name) {
        //  this.name = name;
        //}

        /**
         * Access group object
         * 
         * @return database object group
         */
        public IDbIds Ids
        {
            get { return ids; }
            set { ids = value; }
        }
        //public IDbIds GetIDs() {
        //  return ids;
        //}

        ///**
        // * Access group object
        // * 
        // * @param g set database object group
        // */
        //public void setIDs(IDbIds g) {
        //  ids = g;
        //}

        /**
         * Access model object
         * 
         * @return Cluster model
         */
        public IModel Model
        {
            get { return model; }
            set { model = value; }
        }


        /**
         * Write to a textual representation. Writing the actual group data will be
         * handled by the caller, this is only meant to write the meta information.
         * 
         * @param out output writer stream
         * @param label Label to prefix
         */

        public void WriteToText(TextWriterStream sout, String label)
        {
            String name = GetNameAutomatic();
            sout.CommentPrintLine(TextWriterStream.SER_MARKER + " " + typeof(Cluster).Name);
            if (name != null)
            {
                sout.CommentPrintLine("Name: " + name);
            }
            sout.CommentPrintLine("Noise flag: " + IsNoise());
            sout.CommentPrintLine("Size: " + ids.Count);
            // print hierarchy information.
            if (IsHierarchical())
            {
                sout.CommentPrint("Parents: ");
                for (int i = 0; i < NumParents(); i++)
                {
                    if (i > 0)
                    {
                        sout.CommentPrint(", ");
                    }
                    sout.CommentPrint(GetParents()[i].GetNameAutomatic());
                }
                sout.CommentPrintLine();
                sout.CommentPrint("Children: ");
                for (int i = 0; i < NumChildren(); i++)
                {
                    if (i > 0)
                    {
                        sout.CommentPrint(", ");
                    }
                    sout.CommentPrint(GetChildren()[i].GetNameAutomatic());
                }
                sout.CommentPrintLine();
            }
            // also print model, if any and printable
            if (Model != null)
            {
                sout.CommentPrintLine("IModel class: " + Model.GetType().Name);
                if (Model is ITextWriteable)
                {
                    ((ITextWriteable)Model).WriteToText(sout, label);
                }
            }
        }

        /**
         * Getter for noise flag.
         * 
         * @return noise flag
         */
        public bool IsNoise()
        {
            return noise;
        }

        /**
         * Setter for noise flag.
         * 
         * @param noise new noise flag value
         */
        public void SetNoise(bool noise)
        {
            this.noise = noise;
        }

        /**
         * A partial comparator for Clusters, based on their name. Useful for sorting
         * clusters. Do NOT use in e.g. a TreeSet since it is
         * <em>inconsistent with Equals</em>.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public class PartialComparator : IComparer<Cluster>
        {

            public int Compare(Cluster o1, Cluster o2)
            {
                if (o1 == o2)
                {
                    return 0;
                }
                // sort by label if possible
                if (o1 != null && o1.name != null && o2 != null && o2.name != null)
                {
                    int lblresult = o1.name.CompareTo(o2.Name);
                    if (lblresult != 0)
                    {
                        return lblresult;
                    }
                }
                int hashresult = o1.GetHashCode() - o2.GetHashCode();
                if (hashresult != 0)
                {
                    return hashresult;
                }
                return 0;
            }
        }

        /** {@inheritDoc} */

        public override String ToString()
        {
            String mstr = (model == null) ? "null" : model.ToString();
            String nstr = noise ? ",noise" : "";
            return "Cluster(size=" + Size() + ",model=" + mstr + nstr + ")";
        }



    }
}
