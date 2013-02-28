using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Socona.Expor.Constraints.Pairwise
{
    public class ConsTreeWidthFirstMustlinkEnumerator : IEnumerator<MustLink>
    {
        Queue<ConsTreeNode> nodequeue;
        MustLink current;
      //  ConsTreeNode pnode;
        ConsTreeNode curnode;
        ConsTreeNode root;
        List<MustLink> tmls;
        int tidx = 0;
        public ConsTreeWidthFirstMustlinkEnumerator(ConsTreeNode root)
        {
            this.root = root;
            this.nodequeue = new Queue<ConsTreeNode>();
            this.current = null;
           // this.pnode = null;
            nodequeue.Enqueue(root);
            this.curnode = root;
            //this.MoveNext();
        }

        public MustLink Current
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

            while (tmls == null || tmls.Count == 0 && nodequeue.Count > 0)
            {
                curnode = nodequeue.Peek();
                nodequeue.Dequeue();
                tmls = curnode.Mustlinks;
            }
            if (tmls == null || tmls.Count == 0 && nodequeue.Count == 0)
            { return false; }
            else
            {
                current = tmls[tidx++];
                var node = curnode.Children.FirstOrDefault(v => v.DbId == current.First || v.DbId == current.Second);
                Debug.Assert(node != null);
                nodequeue.Enqueue(node);
                if (tidx >= tmls.Count)
                {
                    tmls = null;
                    tidx = 0;
                }
                return true;
            }
        }

        public void Reset()
        { }
    }
}

