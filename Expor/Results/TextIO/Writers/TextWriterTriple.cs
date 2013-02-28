using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results.TextIO;
using Socona.Expor.Utilities.Exceptions;

namespace Socona.Expor.Results.TextIO.Writers
{

    public class TextWriterTriple : ITextWriter
    {
        /**
         * Serialize a triple, component-wise
         */

        public void Write(TextWriterStream sout, String label, object obj)
        {
            if (obj != null)
            {
                Object first = obj.GetType().InvokeMember(
                    "First", System.Reflection.BindingFlags.Default, null, obj, null);
                if (first != null)
                {
                    ITextWriter tw = (ITextWriter)sout.GetWriterFor(first);
                    if (tw == null)
                    {
                        throw new UnableToComplyException("No handler for database object itself: " + first.GetType().Name);
                    }
                    tw.Write(sout, label, first);
                }
                Object second = obj.GetType().InvokeMember(
                      "Second", System.Reflection.BindingFlags.Default, null, obj, null);
                if (second != null)
                {
                    ITextWriter tw = (ITextWriter)sout.GetWriterFor(second);
                    if (tw == null)
                    {
                        throw new UnableToComplyException("No handler for database object itself: " + second.GetType().Name);
                    }
                    tw.Write(sout, label, second);
                }
                Object third = obj.GetType().InvokeMember(
                      "Third", System.Reflection.BindingFlags.Default, null, obj, null);
                if (third != null)
                {
                    ITextWriter tw = (ITextWriter)sout.GetWriterFor(third);
                    if (tw == null)
                    {
                        throw new UnableToComplyException("No handler for database object itself: " + third.GetType().Name);
                    }
                    tw.Write(sout, label, third);
                }
            }
        }
    }

}
