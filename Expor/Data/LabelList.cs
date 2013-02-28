using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities;

namespace Socona.Expor.Data
{

    /**
     * A list of string labels
     * 
     * @author Erich Schubert
     * 
     * @apiviz.composedOf String
     */
    public class LabelList : List<String>
    {
        /**
         * Serial number
         */
       // private static readonly long serialVersionUID = 1L;

        /**
         * Constructor.
         */
        public LabelList()
            : base()
        {
        }

        /**
         * Constructor.
         * 
         * @param c existing collection
         */
        public LabelList(IEnumerable<String> c)
            : base(c)
        {

        }

        /**
         * Constructor.
         * 
         * @param initialCapacity initial size
         */
        public LabelList(int initialCapacity)
            : base(initialCapacity)
        {
        }


        public override String ToString()
        {
            return FormatUtil.Format(this, " ");
        }
    }
}
