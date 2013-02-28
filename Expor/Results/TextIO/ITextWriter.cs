using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Results.TextIO
{
    public interface ITextWriter
    {
        /**
         * Write a given object to the output stream.
         * 
         * @param out Output stream
         * @param label Label to prefix
         * @param object object to output
         * @throws UnableToComplyException on errors
         * @throws IOException on IO errors
         */
        void Write(TextWriterStream sout, String label, Object o);

    }

}
