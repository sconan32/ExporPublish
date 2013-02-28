using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Socona.Expor.Data
{

    /**
     * External ID objects.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.composedOf String
     */
    public class ExternalID
    {
        /**
         * Object name
         */
        private readonly String name;

        /**
         * Constructor.
         * 
         * @param name
         */
        public ExternalID(String name)
        {

            Debug.Assert(name != null);
            this.name = name;
        }


        public override String ToString()
        {
            return name;
        }


        public override int GetHashCode()
        {
            return name.GetHashCode();
        }


        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }
            ExternalID other = (ExternalID)obj;
            return name.Equals(other.name);
        }
    }
}
