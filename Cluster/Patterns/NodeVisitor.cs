using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Patterns
{
    public interface INodeVisitor
    {
        void Visit(LeafNode node);
        void Visit(InternalNode node);
    }
}
