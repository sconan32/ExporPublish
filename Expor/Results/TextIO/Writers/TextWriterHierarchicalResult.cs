using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Results.TextIO.Writers
{
    class TextWriterHierarchicalResult : ITextWriter
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
                var hv = v as IHierarchicalResult;
                hv.Hierarchy.GetChildren(hv).Select(t => buf.Append("\t").Append(t).Append(Environment.NewLine));

            }
            sout.InlinePrintNoQuotes(buf.ToString());
        }
    }
}
