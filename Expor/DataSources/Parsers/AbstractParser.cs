using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;

namespace Socona.Expor.DataSources.Parsers
{

    public abstract class AbstractParser
    {
        /**
         * A pattern defining whitespace.
         */
        public static readonly String DEFAULT_SEPARATOR = "(\\s+|\\s*[,;]\\s*)";

        /**
         * A quote pattern
         */
        public static readonly char QUOTE_CHAR = '\"';

        /**
         * A pattern catching most numbers that can be parsed using Double.parseDouble:
         * 
         * Some examples: <code>1</code> <code>1.</code> <code>1.2</code>
         * <code>.2</code> <code>-.2e-03</code>
         */
        public static readonly String NUMBER_PATTERN = "[+-]?(?:\\d+\\.?|\\d*\\.\\d+)?(?:[eE][-]?\\d+)?";

        /**
         * OptionDescription for the column separator parameter (defaults to whitespace as in
         * {@link #DEFAULT_SEPARATOR}.
         */
        public static readonly OptionDescription COLUMN_SEPARATOR_ID = OptionDescription.GetOrCreate("parser.colsep", 
            "Column separator pattern. The default assumes whitespace separated data.");

        /**
         * OptionDescription for the quote character parameter (defaults to a double quotation
         * mark as in {@link #QUOTE_CHAR}.
         */
        public static readonly OptionDescription QUOTE_ID = OptionDescription.GetOrCreate("parser.quote", 
            "Quotation character. The default is to use a double quote.");

        /**
         * Stores the column separator pattern
         */
        private Regex colSep = null;

        /**
         * Stores the quotation character
         */
        protected char quoteChar = QUOTE_CHAR;

        /**
         * The comment character.
         */
        public static readonly String COMMENT = "#";

        /**
         * A sign to separate attributes.
         */
        public static readonly String ATTRIBUTE_CONCATENATION = " ";

        /**
         * Constructor.
         * 
         * @param colSep Column separator
         * @param quoteChar Quote character
         */
        public AbstractParser(Regex colSep, char quoteChar) :
            base()
        {
            this.colSep = colSep;
            this.quoteChar = quoteChar;
        }

        /**
         * Tokenize a string. Works much like colSep.split() except it honors
         * quotation characters.
         * 
         * @param input Input string
         * @return Tokenized string
         */
        protected List<String> Tokenize(String input)
        {
            List<String> matchList = new List<String>();
            MatchCollection m = colSep.Matches(input);

            int index = 0;
            bool inquote = (input.Length > 0) && (input[0] == quoteChar);
            for (int i = 0; i < m.Count; i++)
            {
                // Quoted code path vs. regular code path
                if (inquote && m[i].Index > 0)
                {
                    // Closing quote found?
                    if (m[i].Index > index + 1 && input[m[i].Index - 1] == quoteChar)
                    {
                        // Strip quote characters
                        if (index + 1 < m[i].Index - 1)
                        {
                            matchList.Add(input.Substring(index + 1, m[i].Index - 1 - index));
                        }
                        // Seek past
                        index = m[i].Index + m[i].Length;
                        // new quote?
                        inquote = (index < input.Length) && (input[index] == quoteChar);
                    }
                }
                else
                {
                    // Add match before separator
                    if (index < m[i].Index)
                    {
                        matchList.Add(input.Substring(index, m[i].Index - index));
                    }
                    // Seek past separator
                    index = m[i].Index + m[i].Length;
                    // new quote?
                    inquote = (index < input.Length) && (input[index] == quoteChar);
                }
            }
            // Nothing found - return original string.
            if (index == 0)
            {
                matchList.Add(input);
                return matchList;
            }
            // Add tail after last separator.
            if (inquote)
            {
                if (input[input.Length - 1] == quoteChar)
                {
                    if (index + 1 < input.Length - 1)
                    {
                        matchList.Add(input.Substring(index + 1, input.Length - 1 - index));
                    }
                }
                else
                {
                    GetLogger().Warning("Invalid quoted line in input.");
                    if (index < input.Length)
                    {
                        matchList.Add(input.Substring(index, input.Length - index));
                    }
                }
            }
            else
            {
                if (index < input.Length)
                {
                    matchList.Add(input.Substring(index, input.Length - index));
                }
            }
            // Return
            return matchList;
        }

        /**
         * Get the logger for this class.
         * 
         * @return Logger.
         */
        protected abstract Logging GetLogger();

        /**
         * Returns a string representation of the object.
         * 
         * @return a string representation of the object.
         */

        public override String ToString()
        {
            return this.GetType().ToString();
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public abstract class Parameterizer : AbstractParameterizer
        {
            /**
             * Stores the column separator pattern
             */
            protected Regex colSep = null;

            /**
             * Stores the quotation character
             */
            protected char quoteChar = QUOTE_CHAR;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                PatternParameter colParam = new PatternParameter(COLUMN_SEPARATOR_ID, DEFAULT_SEPARATOR);
                if (config.Grab(colParam))
                {
                    colSep = colParam.GetValue();
                }
                StringParameter quoteParam = new StringParameter(QUOTE_ID, new StringLengthConstraint(1, 1), "" + QUOTE_CHAR);
                if (config.Grab(quoteParam))
                {
                    quoteChar = quoteParam.GetValue()[0];
                }
            }


            protected override abstract object MakeInstance();
        }
    }
}
