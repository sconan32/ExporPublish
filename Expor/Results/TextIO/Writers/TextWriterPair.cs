using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results.TextIO;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Results.TextIO.Writers
{

    public class TextWriterPair : ITextWriter
    {
        /**
         * Serialize a pair, component-wise
         */

        public void Write(TextWriterStream sout, String label, object obj)
        {
            if (obj != null)
            {
                if (obj.GetType().GetGenericTypeDefinition() == typeof(IPair<,>))
                {


                    Object first = obj.GetType().
                        InvokeMember("GetFirst", System.Reflection.BindingFlags.Default, null, obj, null);

                    if (first != null)
                    {
                        ITextWriter tw = (ITextWriter)sout.GetWriterFor(first);
                        if (tw == null)
                        {
                            throw new UnableToComplyException("No handler for database object itself: " + first.GetType().Name);
                        }
                        tw.Write(sout, label, first);
                    }
                    Object second = obj.GetType().
                              InvokeMember("GetSecond", System.Reflection.BindingFlags.Default, null, obj, null);
                    if (second != null)
                    {
                        ITextWriter tw = (ITextWriter)sout.GetWriterFor(second);
                        if (tw == null)
                        {
                            throw new UnableToComplyException("No handler for database object itself: " + second.GetType().Name);
                        }
                        tw.Write(sout, label, second);
                    }
                }
                else
                {
                    ITextWriter tw = sout.GetWriterFor(obj);
                    tw.Write(sout, label, obj);
                }
            }

        }
    }

}
