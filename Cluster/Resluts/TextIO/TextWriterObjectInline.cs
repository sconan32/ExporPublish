using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Resluts.TextIO
{
    class TextWriterObjectInline : ITextWriter
    {
        /**
  * Serialize an object into the inline section.
  */

        public override void Write(TextWriterStream sout, String label, Object o)
        {
            String res = "";
            if (label != null)
            {
                res = res + label + "=";
            }
            if (o != null)
            {
                if (label != null)
                {
                    res = res + o.ToString().Replace(" ", "");
                }
                else
                {
                    res = res + o.ToString();
                }
            }
            sout.InlinePrintNoQuotes(res);
        }
    }
}
