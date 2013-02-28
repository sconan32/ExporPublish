using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Utilities.Options
{

    public sealed class OptionUtil
    {
        /**
         * Returns a string representation of the specified list of options containing
         * the names of the options.
         * 
         * @param <O> Option type
         * @param options the list of options
         * @return the names of the options
         */
        public static String OptionsNamesToString<O>(List<O> options)
            where O : IParameter
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append("[");
            for (int i = 0; i < options.Count; i++)
            {
                buffer.Append(options[i].GetName());
                if (i != options.Count - 1)
                {
                    buffer.Append(",");
                }
            }
            buffer.Append("]");
            return buffer.ToString();
        }

        /**
         * Returns a string representation of the specified list of options containing
         * the names of the options.
         * 
         * @param <O> Option type
         * @param options the list of options
         * @return the names of the options
         */
        public static String optionsNamesToString<O>(O[] options) where O : IParameter
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append("[");
            for (int i = 0; i < options.Length; i++)
            {
                buffer.Append(options[i].GetName());
                if (i != options.Length - 1)
                {
                    buffer.Append(",");
                }
            }
            buffer.Append("]");
            return buffer.ToString();
        }

        /**
         * Returns a string representation of the list of number parameters containing
         * the names and the values of the parameters.
         * 
         * @param <N> Parameter type
         * @param parameters the list of number parameters
         * @return the names and the values of the parameters
         */
        public static String parameterNamesAndValuesToString<O>(List<O> parameters) where O : IParameter
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append("[");
            for (int i = 0; i < parameters.Count; i++)
            {
                buffer.Append(parameters[i].GetName());
                buffer.Append(":");
                buffer.Append(parameters[i].GetValueAsString());
                if (i != parameters.Count - 1)
                {
                    buffer.Append(", ");
                }
            }
            buffer.Append("]");
            return buffer.ToString();
        }

        /**
         * Format a list of options (and associated owning objects) for console help
         * output.
         * 
         * @param buf Serialization buffer
         * @param width Screen width
         * @param indent Indentation string
         * @param options List of options
         */
        public static void FormatForConsole(StringBuilder buf, int width, String indent,
            ICollection<IPair<Object, IParameter>> options)
        {
            foreach (IPair<Object, IParameter> pair in options)
            {
                String currentOption = pair.Second.GetName();
                String syntax = pair.Second.GetSyntax();
                String longDescription = pair.Second.GetFullDescription();

                buf.Append(SerializedParameterization.OPTION_PREFIX);
                buf.Append(currentOption);
                buf.Append(" ");
                buf.Append(syntax);
                buf.Append(FormatUtil.NEWLINE);
                Println(buf, width, longDescription, indent);
            }
        }

        /**
         * Simple writing helper with no indentation.
         * 
         * @param buf Buffer to write to
         * @param width Width to use for linewraps
         * @param data Data to write.
         * @param indent Indentation
         */
        public static void Println(StringBuilder buf, int width, String data, String indent)
        {
            foreach (String line in FormatUtil.SplitAtLastBlank(data, width - indent.Length))
            {
                buf.Append(indent);
                buf.Append(line);
                if (!line.EndsWith(FormatUtil.NEWLINE))
                {
                    buf.Append(FormatUtil.NEWLINE);
                }
            }
        }

        /**
         * Format a description of a Parameterizable (including recursive options).
         * 
         * @param buf Buffer to Append to.
         * @param pcls Parameterizable class to describe
         * @param width Width
         * @param indent Text indent
         * @return Formatted description
         */
        public static StringBuilder DescribeParameterizable(StringBuilder buf, Type pcls, int width, String indent)
        {
            try
            {
                Println(buf, width, "Description for class " + pcls.Name, "");

                String title = DocumentationUtil.GetTitle(pcls);
                if (title != null && title.Length > 0)
                {
                    Println(buf, width, title, "");
                }

                String desc = DocumentationUtil.GetDescription(pcls);
                if (desc != null && desc.Length > 0)
                {
                    Println(buf, width, desc, "  ");
                }

                ReferenceAttribute ref1 = DocumentationUtil.GetReference(pcls);
                if (ref1 != null)
                {
                    if (ref1.Prefix.Length > 0)
                    {
                        Println(buf, width, ref1.Prefix, "");
                    }
                    Println(buf, width, ref1.Authors + ":", "");
                    Println(buf, width, ref1.Title, "  ");
                    Println(buf, width, "in: " + ref1.BookTitle, "");
                    if (ref1.Url.Length > 0)
                    {
                        Println(buf, width, "see also: " + ref1.Url, "");
                    }
                }

                SerializedParameterization config = new SerializedParameterization();
                TrackParameters track = new TrackParameters(config);

                Object p = ClassGenericsUtil.TryInstantiate<object>(typeof(Object), pcls, track);
                ICollection<IPair<Object, IParameter>> options = track.GetAllParameters();
                if (options.Count > 0)
                {
                    OptionUtil.FormatForConsole(buf, width, indent, options);
                }
                // TODO: report global constraints?
                return buf;
            }
            catch (Exception e)
            {
                Socona.Log.Logging.GetLogger(typeof(OptionUtil)).Error("Error instantiating class to describe.", e.InnerException);
                buf.Append("No description available: " + e);
                return buf;
            }
        }
    }
}
