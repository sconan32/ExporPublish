using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results.TextIO;

namespace Socona.Expor.Results.TextIO
{
     public interface ITextWriteable
    {
        void WriteToText(TextWriterStream sout, String label);
    }
}
