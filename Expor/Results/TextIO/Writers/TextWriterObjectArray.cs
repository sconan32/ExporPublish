using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results.TextIO;

namespace Socona.Expor.Results.TextIO.Writers
{

    public class TextWriterObjectArray : ITextWriter
    {
        /**
         * Serialize an object into the inline section.
         */

        public void Write(TextWriterStream sout, String label, object v)
        {
            StringBuilder buf = new StringBuilder();
            if (label != null)
            {
                buf.Append(label).
                    Append("=");
            }
            if (v != null)
            {
                foreach (var o in (IEnumerable) v)
                {
                    buf.Append(o.ToString());
                }
            }
            sout.InlinePrintNoQuotes(buf.ToString());
        }
    }

}
