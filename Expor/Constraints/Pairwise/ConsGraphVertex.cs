using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Utilities.DataStructures.Graph;

namespace Socona.Expor.Constraints.Pairwise
{
    public class ConsGraphVertex : GraphVertex<ConsTreeRootNode>
    {

        int infoId=-1;

        public int InfoId
        {
            get { return infoId; }
            set { infoId = value; }
        }
        object info;

        public object Info
        {
            get { return info; }
            set { info = value; }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Entry=");
            sb.Append(this.Entry.ToString());
            sb.Append(", IsVisited=");
            sb.Append(this.IsVisited);
            return sb.ToString();
        }
        // protected  ConsTreeRootNode entry;


        // public ConsTreeRootNode Entry { get { return entry; } }
        // public IDbId DbId { get { return entry.DbId; } }
    }
}
