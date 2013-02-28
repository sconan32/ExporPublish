using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Results
{
    public class BasicResult : AbstractHierarchicalResult
    {
        /**
         * Result name, for presentation
         */
        private String name;

        /**
         * Result name, for output
         */
        private String shortname;

        /**
         * Result constructor.
         * 
         * @param name The long name (for pretty printing)
         * @param shortname the short name (for filenames etc.)
         */
        public BasicResult(String name, String shortname)
            : base()
        {
            this.name = name;
            this.shortname = shortname;
        }

        public override string LongName
        {
            get { return name; }
        }

        public override string ShortName
        {
            get { return shortname; }
        }
        public override string ToString()
        {
            return LongName;
        }
    }
}
