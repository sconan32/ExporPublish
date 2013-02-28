using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results.TextIO;

namespace Socona.Expor.Results.TextIO.Writers
{

    public class TextWriterVector : ITextWriter
    {
        /**
         * Serialize an object into the inline section.
         */

        public void Write(TextWriterStream sout, String label, object v)
        {
            String res = "";
            if (label != null)
            {
                res = res + label + "=";
            }
            if (v != null)
            {
                res = res + v.GetType().InvokeMember(
                    "ToStringNoWhitespace", System.Reflection.BindingFlags.Default, null, v, null).
                    ToString();
            }
            sout.InlinePrintNoQuotes(res);
        }
    }

}
