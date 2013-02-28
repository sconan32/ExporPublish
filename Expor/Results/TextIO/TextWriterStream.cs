using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities;
using System.IO;

namespace Socona.Expor.Results.TextIO
{
    /**
  * Representation of an output stream to a text file.
  * 
  * @author Erich Schubert
  *
  * @apiviz.uses de.lmu.ifi.dbs.elki.result.textwriter.StreamFactory oneway - - wraps
  */
    public class TextWriterStream
    {
        /**
         * Actual stream to write to.
         */
        private StreamWriter outStream;

        /**
         * Buffer for inline data to output.
         */
        private StringBuilder inline;

        /**
         * Buffer for comment data to output.
         */
        private StringBuilder comment;

        /**
         * Handlers for various object types.
         */
        private HandlerList<ITextWriter> writers;

        /**
         * String to separate different entries while printing.
         */
        public static readonly String SEPARATOR = " ";

        /**
         * String to separate different entries while printing.
         */
        public static readonly String QUOTE = "# ";

        /**
         * Comment separator line.
         * Since this will be printed without {@link #QUOTE} infront, it should be quoted string itself. 
         */
        public static readonly String COMMENTSEP = "###############################################################";

        /**
         * System newline character(s)
         */
        private readonly static String NEWLINE = Environment.NewLine;

        /**
         * Marker used in text serialization (and re-parsing)
         */
        public readonly static String SER_MARKER = "Serialization class:";

        /**
         * Force incomments flag
         */
        // TODO: solve this more gracefully
        private bool forceincomments = false;

        /**
         * Constructor.
         * 
         * @param out Actual stream to write to
         * @param writers Handlers for various data types
         */
        public TextWriterStream(StreamWriter sout, HandlerList<ITextWriter> writers)
        {
            this.outStream = sout;
            this.writers = writers;
            inline = new StringBuilder();
            comment = new StringBuilder();
        }

        /**
         * Print an object into the comments section
         * 
         * @param line object to print into commments
         */
        public void CommentPrint(Object line)
        {
            comment.Append(line);
        }

        /**
         * Print an object into the comments section with trailing newline.
         * 
         * @param line object to print into comments
         */
        public void CommentPrintLine(Object line)
        {
            comment.Append(line);
            comment.Append(NEWLINE);

        }

        /**
         * Print a newline into the comments section.
         */
        public void CommentPrintLine()
        {
            comment.Append(NEWLINE);

        }

        /**
         * Print a separator line in the comments section.
         */
        public void CommentPrintSeparator()
        {
            comment.Append(NEWLINE + COMMENTSEP + NEWLINE);

        }

        /**
         * Print data into the inline part of the file.
         * Data is sanitized: newlines are replaced with spaces, and text
         * containing separators is put in quotes. Quotes and escape characters
         * are escaped.
         * 
         * @param o object to print
         */
        public void InlinePrint(Object o)
        {
            if (forceincomments)
            {
                CommentPrint(o);
                return;
            }
            if (inline.Length > 0)
            {
                inline.Append(SEPARATOR);
            }
            // remove newlines
            String str = o.ToString().Replace(NEWLINE, " ");
            // escaping
            str = str.Replace("\\", "\\\\").Replace("\"", "\\\"");
            // when needed, add quotes.
            if (str.Contains(SEPARATOR))
            {
                str = "\"" + str + "\"";
            }
            inline.Append(str);
        }

        /**
         * Print data into the inline part of the file WITHOUT checking for
         * separators (and thus quoting).
         * 
         * @param o object to print.
         */
        public void InlinePrintNoQuotes(Object o)
        {
            if (forceincomments)
            {
                CommentPrint(o);
                return;
            }
            if (inline.Length > 0)
            {
                inline.Append(SEPARATOR);
            }
            // remove newlines
            String str = o.ToString().Replace(NEWLINE, " ");
            // escaping
            str = str.Replace("\\", "\\\\").Replace("\"", "\\\"");
            inline.Append(str);
        }

        /**
         * Flush output:
         * write inline data, then write comment section. Reset streams.
         */
        public void Flush()
        {
            if (inline.Length > 0)
            {
                outStream.WriteLine(inline);
                outStream.Flush();
            }
            inline.Clear();
            if (comment.Length > 0)
            {
                QuotePrintLine(outStream, comment.ToString());
            }
            comment.Clear();
        }

        /**
         * Quoted println. All lines written will be prefixed with {@link #QUOTE}
         * 
         * @param outStream output stream to write to
         * @param data data to print
         */
        private void QuotePrintLine(StreamWriter outStream, String data)
        {
            String[] lines = data.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (String line in lines)
            {
                if (line.Equals(COMMENTSEP))
                {
                    outStream.WriteLine(COMMENTSEP);
                }
                else
                {
                    outStream.WriteLine(QUOTE + line);
                }
                outStream.Flush();
            }
        }

        /**
         * Retrieve an appropriate writer from the handler list.
         * 
         * @param o query object
         * @return appropriate write, if available
         */
        public ITextWriter GetWriterFor(Object o)
        {
            return writers.GetHandler(o);
        }

        /**
         * Restore a vector undoing any normalization that was applied.
         * (This class does not support normalization, it is only provided
         * by derived classes, which will then have to use generics.)
         * 
         * @param <O> Object class
         * @param v vector to restore
         * @return restored value.
         */
        public Object NormalizationRestore(Object v)
        {
            return v;
        }

        /**
         * Test force-in-comments flag.
         * 
         * @return flag value
         */
        /**
      * Set the force-in-comments flag.
      * 
      * @param forceincomments the new flag value
      */
        protected bool IsForceincomments
        {
            get { return forceincomments; }
            set { forceincomments = value; }
        }



    }

}
