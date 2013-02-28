using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Data.Models
{

    public class OPTICSModel : BaseModel
    {
        /**
         * Start index
         */
        private int startIndex;

        /**
         * End index
         */
        private int endIndex;

        /**
         * @param startIndex
         * @param endIndex
         */
        public OPTICSModel(int startIndex, int endIndex) :
            base()
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }

        /**
         * Starting index of OPTICS cluster
         * 
         * @return index of cluster start
         */
        public int GetStartIndex()
        {
            return startIndex;
        }

        /**
         * End index of OPTICS cluster
         * 
         * @return index of cluster end
         */
        public int GetEndIndex()
        {
            return endIndex;
        }


        public override String ToString()
        {
            return "OPTICSModel";
        }
    }
}
