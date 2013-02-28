using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Constraints.Pairwise
{
    class ConsTreeDepthFirstEnumerator : IEnumerator<ConsTreeNode>
    {

        Stack<ConsTreeNode> nodestack;
        ConsTreeNode current;
        ConsTreeNode root;

        public ConsTreeDepthFirstEnumerator(ConsTreeNode root)
        {
            this.root = root;
            this.nodestack = new Stack<ConsTreeNode>();
            this.current = null;
            nodestack.Push(root);
        }

        public ConsTreeNode Current
        {
            get { return current; }
        }

        public void Dispose() { }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            if (nodestack.Count <= 0)
            {
                return false;
            }
            current = nodestack.Peek();
            nodestack.Pop();
            foreach (var ch in current.Children)
            {
                nodestack.Push(ch);
            }
            return true;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
