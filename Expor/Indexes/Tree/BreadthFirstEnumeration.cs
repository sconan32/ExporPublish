using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Indexes.Tree
{

    public class BreadthFirstEnumeration<N, E> : IEnumerator<IndexTreePath<E>>
        where N : INode<E>
        where E : IEntry
    {

        /**
         * The queue for the enumeration.
         */
        private Queue<IndexTreePath<E>> queue;

        /**
         * The index storing the nodes.
         */
        private IndexTree<N, E> index;
        private IndexTreePath<E> rootPath;
        private IndexTreePath<E> current;
        /**
       * Creates a new breadth first enumeration with the specified node as root
       * node.
       * 
       * @param index the index tree storing the nodes
       * @param rootPath the root entry of the enumeration
       */
        public BreadthFirstEnumeration(IndexTree<N, E> index, IndexTreePath<E> rootPath) :
            base()
        {
            this.queue = new Queue<IndexTreePath<E>>();
            this.index = index;
            this.rootPath = rootPath;

            //Enumeration<IndexTreePath<E>> root_enum = new Enumeration<IndexTreePath<E>>()
            queue.Enqueue(rootPath);
            current = null;
        }


        public IndexTreePath<E> Current
        {
            get { return current; }
        }

        public void Dispose() { }

        object System.Collections.IEnumerator.Current
        {
            get { return current; }
        }

        public bool MoveNext()
        {

            if (queue.Count <= 0)
            {
                return false;
            }
            current = queue.Peek();


            IEnumerator<IndexTreePath<E>> children = null;
            if (current.GetLastPathComponent().GetEntry().IsLeafEntry())
            {
                children = null;
            }
            else
            {
                N node = index.GetNode(current.GetLastPathComponent().GetEntry());
                children = node.Children(current).GetEnumerator();
            }
            if (children != null)
            {
                while (children.MoveNext())
                {
                    queue.Enqueue(children.Current);
                }
            }

            queue.Dequeue();

            return true;
        }

        public void Reset()
        {
            queue.Clear();
            queue.Enqueue(rootPath);
            this.current = null;
        }

    }
}
