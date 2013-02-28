using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results.TextIO;

namespace Socona.Expor.Results.TextIO.Writers
{

    public class TextWriterObjectComment : ITextWriter
    {
        /**
         * Put an object into the comment section
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
                res = res + obj.ToString();
            }
            sout.CommentPrintLine(res);
        }
    }

}