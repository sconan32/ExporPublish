using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Utilities.DataStructures.Graph;

namespace Socona.Expor.Constraints.Pairwise
{
    public class ConsGraph : AdjacencyListGraph
    {



        IList<MustLink> mls;
        IList<CannotLink> cls;
        /// <summary>
        /// return a copy of ConstraintTreeRootNode List
        /// </summary>
        public IList<ConsTreeRootNode> SetNodesList
        {
            get
            {
                return (from ConsGraphVertex node in nodes select node.Entry).
                    ToList<ConsTreeRootNode>();
            }
        }
        public ConsGraph(IList<MustLink> mls, IList<CannotLink> cls) :
            base()
        {
            this.mls = mls;
            this.cls = cls;
            BuildGraph();
        }

        protected void BuildGraph()
        {
            if (initialized)
            {
                return;
            }
            foreach (var cl in cls)
            {
                var root1 = FindRoot(cl.First);
                var root2 = FindRoot(cl.Second);
                if (root1 == null)
                {
                    root1 = CreateNode(cl.First);
                    AddNode(root1);
                }
                if (root2 == null)
                {
                    root2 = CreateNode(cl.Second);
                    AddNode(root2);
                }
                AddEdge(new CannotLinkGraphEdge(cl, root1, root2, 1));
                AddEdge(new CannotLinkGraphEdge(cl, root2, root1, 1));
            }
            foreach (var ml in mls)
            {
                var root1 = FindRoot(ml.First);
                var root2 = FindRoot(ml.Second);
                if (root1 == null)
                {
                    root1 = CreateNode(ml.First);
                    AddNode(root1);
                }
                if (root2 == null)
                {
                    root2 = CreateNode(ml.Second);
                    AddNode(root2);
                }

                if (root1 != root2)
                {
                    if (root1.Entry.Children.Count < root2.Entry.Children.Count)
                    {
                        var c = root1;
                        root1 = root2;
                        root2 = c;
                    }
                    MergeNode(root1, root2);

                }
                root1.Entry.AddMustLink(ml, root1.Entry.DbId);
            }
            initialized = true;
        }

        /// <summary>
        /// Merge root2 to root1
        /// </summary>
        /// <param name="root1"></param>
        /// <param name="root2"></param>
        public void MergeNode(ConsGraphVertex root1, ConsGraphVertex root2)
        {

            var tree1 = root1.Entry;
            var tree2 = root2.Entry;

            int count1 = tree1.Children.Count();
            int count2 = tree2.Children.Count();

            ConsTreeNode newtreenode = tree2.Clone() as ConsTreeNode;

            newtreenode.Base = tree1;
            tree1.Children.Add(newtreenode);

            IEnumerator<IGraphEdge> eit = EnumEdgesFor(root2);
            while (eit.MoveNext())
            {
                var nedge = eit.Current as CannotLinkGraphEdge;

                var newedge = new CannotLinkGraphEdge(nedge.CannotLinks, root1, nedge.To, nedge.Weight);
                if (!IsEdgeExist(newedge))
                {
                    AddEdge(newedge);
                }
                else
                {
                    var theedge = (GetEdge(root1, nedge.To) as CannotLinkGraphEdge);
                    theedge.Weight += 1;
                    theedge.CannotLinks.AddRange(nedge.CannotLinks);
                }
                var newedge2 = new CannotLinkGraphEdge(nedge.CannotLinks, nedge.To, root1, nedge.Weight);
                if (!IsEdgeExist(newedge2))
                {
                    AddEdge(newedge2);
                }
                else
                {
                    var theedge = (GetEdge(nedge.To, root1) as CannotLinkGraphEdge);
                    theedge.Weight += 1;
                    theedge.CannotLinks.AddRange(nedge.CannotLinks);
                }
            }
            DeleteNode(root2);
        }
        protected ConsGraphVertex CreateNode(IDbId id)
        {
            ConsTreeRootNode rootn = new ConsTreeRootNode(id);
            ConsGraphVertex res = new ConsGraphVertex() { Entry = rootn };
            return res;

        }

        protected ConsGraphVertex FindRoot(IDbId dbid)
        {
            foreach (var node in Nodes)
            {
                var nn = (node as ConsGraphVertex);
                if (nn.Entry.IdSet.Contains(dbid))
                {
                    return nn;
                }

            }
            return null;
        }

    }
}
