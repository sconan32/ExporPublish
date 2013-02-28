using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids.Distance
{
    public abstract class AbstractReadOnlyDistanceDbIdList<TPair> : AbstractDistanceDbIdList<TPair>
        where TPair : IDistanceDbIdPair
    {
        public override void Add(TPair pair)
        {
            throw new InvalidOperationException("Read Only List doesnot support adding items");
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }
        public override TPair this[int index]
        {
            get { throw new NotImplementedException("You Must Implement this method in your derived class."); }

            set { throw new InvalidOperationException("Read Only List doesnot support Modifying items"); }

        }
        public override void Clear()
        {
            throw new InvalidOperationException("Read Only List doesnot support adding items");
        }

        public override bool Remove(TPair item)
        {
            throw new InvalidOperationException("Read Only List doesnot support adding items");
        }
    }
}
