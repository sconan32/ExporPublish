using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Data.Types
{
    public class NoSupportedDataTypeException : Exception
    {
        /**
         * Serial version.
         */
        //private static readonly long serialVersionUID = 1L;

        /**
         * Available types
         */
        private ICollection<ITypeInformation> types = null;

        /**
         * Constructor.
         * 
         * @param type Requested type
         * @param types Available types.
         */
        public NoSupportedDataTypeException(ITypeInformation type, ICollection<ITypeInformation> types)
            : base("No data type found satisfying: " + type.ToString())
        {

            this.types = types;
        }

        /**
         * Constructor with string message. If possible, use the type parameter
         * instead!
         * 
         * @param string Error message
         */
        public NoSupportedDataTypeException(String str)
            : base(str)
        {
        }


        public override String Message
        {
            get
            {
                StringBuilder buf = new StringBuilder(base.Message);
                if (types != null)
                {
                    buf.Append("\nAvailable types:");
                    foreach (ITypeInformation type in types)
                    {
                        buf.Append(" ");
                        buf.Append(type.ToString());
                    }
                }
                return buf.ToString();
            }
        }
    }
}
