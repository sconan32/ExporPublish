using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Data.Types
{

    /**
     * Class that combines multiple type restrictions into one using the "or"
     * operator.
     * 
     * @author Erich Schubert
     */
    public class AlternativeTypeInformation : ITypeInformation
    {
        /**
         * The wrapped type restrictions
         */
        private readonly ITypeInformation[] restrictions;

        /**
         * Constructor.
         * 
         * @param restrictions
         */
        public AlternativeTypeInformation(params ITypeInformation[] restrictions)
            : base()
        {

            this.restrictions = restrictions;
        }


        public  bool IsAssignableFromType(ITypeInformation type)
        {
            for (int i = 0; i < restrictions.Length; i++)
            {
                if (restrictions[i].IsAssignableFromType(type))
                {
                    return true;
                }
            }
            return false;
        }


        public  bool IsAssignableFrom(Object other)
        {
            for (int i = 0; i < restrictions.Length; i++)
            {
                if (restrictions[i].IsAssignableFrom(other))
                {
                    return true;
                }
            }
            return false;
        }


        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < restrictions.Length; i++)
            {
                if (i > 0)
                {
                    buf.Append(" OR ");
                }
                buf.Append(restrictions[i].ToString());
            }
            return buf.ToString();
        }
    }
}
