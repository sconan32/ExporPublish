using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Socona.Expor.Data
{

    public class HierarchicalClassLabel : ClassLabel
    {
        /**
         * The default separator pattern, a point ('.').
         */
        public static Regex DEFAULT_SEPARATOR = new Regex("\\.");

        /**
         * The default separator, a point ('.').
         */
        public static String DEFAULT_SEPARATOR_STRING = ".";

        /**
         * Holds the Pattern to separate different levels parsing input.
         */
        private Regex separatorPattern;

        /**
         * A String to separate different levels in a String representation of this
         * HierarchicalClassLabel.
         */
        private String separatorString;

        /**
         * Holds the names on the different levels.
         */
        private IComparable[] levelwiseNames;

        /**
         * Constructs a hierarchical class label from the given name, using the given
         * Pattern to match separators of different levels in the given name, and
         * setting the given separator-String to separate different levels in String
         * representations of this HierarchicalClassLabel.
         * 
         * @param name a String describing a hierarchical class label
         * @param regex a Pattern to match separators of different levels in the given
         *        name
         * @param separator a separator String to separate different levels in the
         *        String-representation of this HierarchicalClassLabel
         */
        public HierarchicalClassLabel(String name, Regex regex, String separator)
            : base()
        {

            this.separatorPattern = regex;
            this.separatorString = separator;
            String[] levelwiseStrings = separatorPattern.Split(name);
            this.levelwiseNames = new IComparable[levelwiseStrings.Length];
            for (int i = 0; i < levelwiseStrings.Length; i++)
            {
                try
                {
                    levelwiseNames[i] = (levelwiseStrings[i]);
                }
                catch (FormatException )
                {
                    levelwiseNames[i] = levelwiseStrings[i];
                }
            }
        }

        /**
         * Constructs a hierarchical class label from the given name. Different levels
         * are supposed to be separated by points ('.'), as defined by
         * {@link #DEFAULT_SEPARATOR DEFAULT_SEPARATOR}. Also, in a
         * String-representation of this HierarchicalClassLabel, different levels get
         * separated by '.'.
         * 
         * @param label a String describing a hierarchical class label
         */
        public HierarchicalClassLabel(String label) :
            this(label, DEFAULT_SEPARATOR, DEFAULT_SEPARATOR_STRING)
        {
        }

        /**
         * Compares two HierarchicalClassLabels. Names at higher levels are compared
         * first. Names at a lower level are compared only if their parent-names are
         * equal. Names at a level are tried to be compared as integer values. If this
         * does not succeed, both names are compared as Strings.
         * 
         */

        public override int CompareTo(ClassLabel o)
        {
            HierarchicalClassLabel h = (HierarchicalClassLabel)o;
            for (int i = 0; i < this.levelwiseNames.Length && i < h.levelwiseNames.Length; i++)
            {
                int comp = 0;
                try
                {
                    IComparable first = this.levelwiseNames[i];
                    IComparable second = h.levelwiseNames[i];
                    comp = first.CompareTo(second);
                }
                catch (ApplicationException )
                {
                    String h1 = (String)(this.levelwiseNames[i] is Int32 ?
                        this.levelwiseNames[i].ToString() : this.levelwiseNames[i]);
                    String h2 = (String)(h.levelwiseNames[i] is Int32 ?
                        h.levelwiseNames[i].ToString() : h.levelwiseNames[i]);
                    comp = h1.CompareTo(h2);
                }
                if (comp != 0)
                {
                    return comp;
                }
            }
            return (this.levelwiseNames.Length).CompareTo((h.levelwiseNames.Length));
        }

        /**
         * The length of the hierarchy of names.
         * 
         * @return length of the hierarchy of names
         */
        public int Depth()
        {
            return levelwiseNames.Length - 1;
        }

        /**
         * Returns the name at the given level as a String.
         * 
         * @param level the level to return the name at
         * @return the name at the given level as a String
         */
        public String GetNameAt(int level)
        {
            return this.levelwiseNames[level] is Int32 ? this.levelwiseNames[level].ToString() : (String)this.levelwiseNames[level];

        }

        /**
         * Returns a String representation of this HierarchicalClassLabel using
         * {@link #separatorString separatorString} to separate levels.
         * 
         * @see #toString(int)
         */

        public override String ToString()
        {
            return ToString(levelwiseNames.Length);
        }

        /**
         * Provides a String representation of this ClassLabel comprising only the
         * first <code>level</code> levels.
         * 
         * @param level the lowest level to include in the String representation.
         * @return a String representation of this ClassLabel comprising only the
         *         first <code>level</code> levels
         */
        public String ToString(int level)
        {
            if (level > levelwiseNames.Length)
            {
                throw new ArgumentException("Specified level exceeds depth of hierarchy.");
            }

            StringBuilder name = new StringBuilder();
            for (int i = 0; i < level; i++)
            {
                name.Append(this.GetNameAt(i));
                if (i < level - 1)
                {
                    name.Append(this.separatorString);
                }
            }
            return name.ToString();
        }

        /**
         * Factory class
         * 
         * @author Erich Schubert
         * 
         * @apiviz.has HierarchicalClassLabel - - 芦creates禄
         * @apiviz.stereotype factory
         */
        public class Factory : ClassLabel.Factory<HierarchicalClassLabel>
        {

            public override HierarchicalClassLabel MakeFromString(String lbl)
            {
                HierarchicalClassLabel l = existing[(lbl)];
                if (l == null)
                {
                    l = new HierarchicalClassLabel(lbl);
                    existing[lbl] = l;
                }
                return l;
            }
        }
    }
}
