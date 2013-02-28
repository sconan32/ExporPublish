using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Socona.Expor.DataSources.Bundles;

namespace Socona.Expor.DataSources.Parsers
{

    public abstract class AbstractStreamingParser : AbstractParser, IStreamingParser
    {
        /**
         * Constructor.
         * 
         * @param colSep Column separator pattern
         * @param quoteChar Quote character
         */
        public AbstractStreamingParser(Regex colSep, char quoteChar) :
            base(colSep, quoteChar)
        {
        }

        public  MultipleObjectsBundle Parse(Stream ins)
        {
            this.InitStream(ins);
            return MultipleObjectsBundle.FromStream(this);
        }

        public abstract void InitStream(Stream ins);

        public abstract BundleMeta GetMeta();


        public abstract object Data(int rnum);

        public abstract StreamSourceEventType NextEvent();




       
    }
}
