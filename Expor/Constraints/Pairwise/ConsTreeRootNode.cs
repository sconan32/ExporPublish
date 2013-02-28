using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Utilities.Exceptions;

namespace Socona.Expor.Constraints.Pairwise
{
    public class ConsTreeRootNode : ConsTreeNode
    {
        public void AddTo(IDbId id1, IDbId id2)
        {
            var node1 = DeepFisrtSearch(id1);
            if (node1 == null)
            {
                throw new AbortException(string.Format("Node Including {0} not Existing", id1.ToString()));
            }
            node1.Children.Add(new ConsTreeNode(id2));
            needRefreshIdSet = true;
        }
        public void AddMustLink(MustLink ml,IDbId toId)
        {
            var node1 = DeepFisrtSearch(toId);
            node1.Mustlinks.Add(ml);
        }
        public ConsTreeRootNode(IDbId id)
            : base(id, null)
        {

        }

        public override string ToString()
        {
            return ">" + base.ToString();
        }


    }
}
