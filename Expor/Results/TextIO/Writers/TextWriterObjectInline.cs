using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results.TextIO;

namespace Socona.Expor.Results.TextIO.Writers
{

    public class TextWriterObjectInline : ITextWriter
    {
        /**
         * Serialize an object into the inline section.
         */

        public void Write(TextWriterStream sout, String label, Object obj)
        {
            String res = "";
            if (label != null)
            {
                res = res + label + "=";
            }
            if (obj != null)
            {
                if (label != null)
                {
                    res = res + obj.ToString().Replace(" ", "");
                }
                else
                {
                    res = res + obj.ToString();
                }
            }
            sout.InlinePrintNoQuotes(res);
        }
    }

}
