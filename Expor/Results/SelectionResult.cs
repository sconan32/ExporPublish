using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Results
{

    public class SelectionResult : IResult
    {
        /**
         * The actual selection
         */
        DbIdSelection selection = null;

        /**
         * Constructor.
         */
        public SelectionResult()
        {

        }

        /**
         * @return the selection
         */
        public DbIdSelection GetSelection()
        {
            return selection;
        }

        /**
         * @param selection the selection to set
         */
        public void SetSelection(DbIdSelection selection)
        {
            this.selection = selection;
        }


        public String LongName
        {
            get { return "Selection"; }
        }


        public String ShortName
        {
            get { return "selection"; }
        }
    }
}
