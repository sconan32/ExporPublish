using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Constraints.Pairwise
{
    public class CannotLink : Pair<IDbId, IDbId>, IPairwiseConstraint
    {
        IDbId id;
        public IDbId CId { get { return id; } }
        public CannotLink(IDbId id1, IDbId id2)
            : this(null, id1, id2)
        {
        }
        public CannotLink(IDbId cid, IDbId id1, IDbId id2)
            : base(id1, id2)
        {
            this.id = cid;
        }
        public override bool Equals(object obj)
        {
            if (obj is CannotLink)
            {
                var another = obj as CannotLink;
                //  return (obj as CannotLink).id == this.id;
                return this.first.Equals(another.first) && this.second.Equals(another.second)||
                    this.second.Equals(another.first)&&this.first.Equals(another.second);

            }
            return false;
        }
        public override int GetHashCode()
        {
            return this.first.GetHashCode() ^ this.second.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("CANNOT<{0},{1}>", first, second);
        }
    }
}
