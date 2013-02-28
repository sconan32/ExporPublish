using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Results
{

    public class DbIdSelection
    {
        /**
         * Selected IDs
         */
        private IDbIds selectedIds = DbIdUtil.EMPTYDBIDS;

        /**
         * Constructor with new object IDs.
         * 
         * @param selectedIds selection IDs
         */
        public DbIdSelection(IDbIds selectedIds)
            : base()
        {

            this.selectedIds = selectedIds;
        }

        /**
         * Getter for the selected IDs
         * 
         * @return DbIds
         */
        public IDbIds GetSelectedIds()
        {
            return selectedIds;
        }
    }
}
