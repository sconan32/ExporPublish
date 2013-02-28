using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Graph
{
    public class AdjacencyList
    {
        List<InnerHeadNode> list;
        public AdjacencyList(IList<IGraphVertex> nodes)
        {
            list = new List<InnerHeadNode>();
            foreach (var node in nodes)
            {
                InnerHeadNode nd = new InnerHeadNode() { FromVertex = node };
                list.Add(nd);
            }
        }
        public void AddNode(IGraphVertex node)
        {
            InnerHeadNode nd = new InnerHeadNode() { FromVertex = node };
            list.Add(nd);
        }

        public void DeleteNode(IGraphVertex node)
        {
            InnerHeadNode hn = list.First(v => v.FromVertex == node);
            if (hn != null)
            {
                list.Remove(hn);
                foreach (var ahn in list)
                {
                    for (int i = ahn.NodesList.Count - 1; i >= 0; i--)
                    {
                        if (ahn.NodesList[i].ToVertex == node)
                        {
                            ahn.NodesList.RemoveAt(i);
                        }
                    }
                }
            }

        }
        public void AddEdge(IGraphEdge edge)
        {
            InnerNode ind = new InnerNode() { Edge = edge, ToVertex = edge.To };

            var nd = list.First(v => v.FromVertex == edge.From);
            nd.NodesList.Add(ind);

        }
        public bool IsEdgeExist(IGraphEdge edge)
        {
            var nd = list.First(v => v.FromVertex == edge.From);
            if (nd != null)
            {
                return nd.NodesList.Exists(t => t.ToVertex == edge.To);
            }
            return false;
        }
        private class InnerHeadNode
        {
            public IGraphVertex FromVertex;
            public List<InnerNode> NodesList = new List<InnerNode>(4);

        }
        private class InnerNode
        {

            public IGraphEdge Edge;
            public IGraphVertex ToVertex;


            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[-(").Append(Edge.ToString()).Append(")->").Append(ToVertex.ToString()).Append("]");
                return sb.ToString();
            }

        }
        public class EdgeEnumerator : IEnumerator<IGraphEdge>
        {

            InnerHeadNode hnode;
            IEnumerator<InnerNode> iter;

            public EdgeEnumerator(AdjacencyList list, IGraphVertex fromNode)
            {
                hnode = list.list.First(v => v.FromVertex == fromNode);
                if (hnode != null)
                {
                    iter = hnode.NodesList.GetEnumerator();
                }
                else
                { iter = null; }
            }



            public IGraphEdge Current
            {
                get { return iter.Current.Edge; }
            }

            public void Dispose()
            {
                iter.Dispose();
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                return iter.MoveNext();

            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
}
