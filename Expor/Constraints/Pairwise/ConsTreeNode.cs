using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Constraints.Pairwise
{
    public class ConsTreeNode : ICloneable, IEnumerable<ConsTreeNode>
    {
        IDbId dbid;

        protected ConsTreeNode basenode;
        protected List<ConsTreeNode> children;
        protected List<MustLink> mustlinks;

        public List<MustLink> Mustlinks
        { get { return mustlinks; } }
        public ConsTreeNode Base
        {
            get { return basenode; }
            set { basenode = value; basenode.needRefreshIdSet = true; }
        }
        public List<ConsTreeNode> Children { get { return children; } }
        public IDbId DbId { get { return dbid; } }

        protected IModifiableDbIds idset;
        protected bool needRefreshIdSet = true;

        public ConsTreeNode(IDbId id)
            : this(id, null)
        { }

        public ConsTreeNode(IDbId id, ConsTreeNode basenode)
        {
            this.dbid = id;
            this.basenode = basenode;
            children = new List<ConsTreeNode>();
            mustlinks = new List<MustLink>();
        }
        public IModifiableDbIds IdSet
        {
            get
            {
                if (needRefreshIdSet)
                {
                    BuildIdSet();
                }
                return idset;
            }
        }
        public List<MustLink> MustLinkSet
        {
            get { return BuildMustLinkSet(); }
        }

        protected List<MustLink> BuildMustLinkSet()
        {
            List<MustLink> mlset = new List<MustLink>();
            ConsTreeWidthFirstMustlinkEnumerator ci = new ConsTreeWidthFirstMustlinkEnumerator(this);
            while (ci.MoveNext())
            {
                mlset.Add(ci.Current);
            }
            if (basenode != null)
            {
                mlset.Add(basenode.Mustlinks.First(
                    v => v.First == this.dbid || v.Second == this.dbid));
            }
            return mlset;
        }
        protected void BuildIdSet()
        {
            idset = DbIdUtil.NewArray(this.Count());
            foreach (var node in this)
            {
                idset.Add(node.DbId);

            }
            needRefreshIdSet = false;
        }

        public ConsTreeNode DeepFisrtSearch(IDbId id)
        {
            ConsTreeNode res = null;
            if (this.DbId.Equals(id))
            {
                return this;
            }
            if (this.children.Count > 0)
            {
                foreach (var ch in children)
                {
                    res = ch.DeepFisrtSearch(id);
                    if (res != null)
                    {
                        return res;
                    }
                }
            }
            return null;


        }
        public IEnumerator<ConsTreeNode> GetEnumerator()
        {
            return new ConsTreeDepthFirstEnumerator(this);

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public object Clone()
        {
            return new ConsTreeNode(this.dbid) { basenode = this.basenode, children = this.children };
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{Id=").Append(dbid.ToString());
            sb.Append(", ChildrenCount=").Append(Children.Count).Append("}");
            return sb.ToString();
        }
    }
}
