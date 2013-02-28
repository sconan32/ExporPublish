using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results.TextIO;

namespace Socona.Expor.Results.TextIO.Writers
{

    public class TextWriterTextWriteable : ITextWriter
    {
        /**
         * Use the objects own text serialization.
         */

        public void Write(TextWriterStream sout, String label, object obj)
        {
           ((ITextWriteable) obj).WriteToText(sout, label);
        }
    }

}
