using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results.TextIO;

namespace Socona.Expor.Results.TextIO.Writers
{

    public class TextWriterDoubleDoublePair : ITextWriter
    {
        /**
         * Serialize a pair, component-wise
         */

        public void Write(TextWriterStream sout, String label, object obj)
        {
            if (obj != null)
            {

                String res;
                string fstr = obj.GetType().
                    InvokeMember("GetFirst", System.Reflection.BindingFlags.Default, null, obj, null).
                    ToString();
                string sstr = obj.GetType().
                  InvokeMember("GetSecond", System.Reflection.BindingFlags.Default, null, obj, null).
                  ToString();

                if (label != null)
                {
                    res = label + "=" + fstr + "," + sstr;
                }
                else
                {
                    res = fstr + " " + sstr;
                }
                sout.InlinePrintNoQuotes(res);
            }
        }
    }

}
