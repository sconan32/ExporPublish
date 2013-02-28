using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.Persistent;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Pairs;
using Socona.Log;

namespace Socona.Expor.DataSources.Parsers
{
    class PairwiseConstraintsParser : AbstractStreamingParser
    {

        private static Logging logger = Logging.GetLogger(typeof(PairwiseConstraintsParser));

        /**
      * Constant used for unknown dimensionality (e.g. empty files)
      */
        public static readonly int DIMENSIONALITY_UNKNOWN = -1;

        /**
         * Constant used for records of variable dimensionality (e.g. time series)
         */
        public static readonly int DIMENSIONALITY_VARIABLE = -2;


        Pair<int, int> factory=new Pair<int,int> (0,0);

        /**
       * Current line number
       */
        protected int lineNumber;

        /**
         * Dimensionality reported
         */
        protected int dimensionality;

        /**
         * Metadata
         */
        protected BundleMeta meta = null;

        StreamReader reader;
        /**
      * Column names
      */
        protected List<String> columnnames = null;

        /**
         * Bitset to indicate which columns are numeric
         */
        protected BitArray labelcolumns = null;

        /**
         * Current vector
         */
        protected Pair<int, int> curvec = null;

        /**
         * Current labels
         */
        protected LabelList curlbl = null;
        /**
        * Event to report next
        */

        StreamSourceEventType nextevent = default(StreamSourceEventType);


        public PairwiseConstraintsParser()
            : base(new Regex(DEFAULT_SEPARATOR), QUOTE_CHAR)
        {
        }

        public override void InitStream(System.IO.Stream ins)
        {
            reader = new StreamReader(ins);
            lineNumber = 1;
            dimensionality = DIMENSIONALITY_UNKNOWN;
            columnnames = null;
            labelcolumns = new BitArray(100);
        }

        public override Bundles.BundleMeta GetMeta()
        {
            return meta;
        }

        public override object Data(int rnum)
        {

            if (rnum == 0)
            {
                return curvec;
            }

            if (rnum == 1)
            {
                return curlbl;
            }
            throw new IndexOutOfRangeException();

        }

        public override Bundles.StreamSourceEventType NextEvent()
        {

            if (nextevent != StreamSourceEventType.NONE)
            {
                StreamSourceEventType ret = nextevent;
                nextevent = StreamSourceEventType.NONE;
                return ret;
            }
            try
            {
                for (String line; (line = reader.ReadLine()) != null; lineNumber++)
                {
                    if (!line.StartsWith(COMMENT) && line.Length > 0)
                    {
                        ParseLineInternal(line);
                        // Maybe a header column?
                        if (curvec == null)
                        {
                            continue;
                        }
                        if (dimensionality == DIMENSIONALITY_UNKNOWN)
                        {
                            dimensionality = 3;
                            BuildMeta();
                            nextevent = StreamSourceEventType.NEXT_OBJECT;
                            return StreamSourceEventType.META_CHANGED;
                        }
                        else if (dimensionality > 0)
                        {
                            if (dimensionality != 3)
                            {
                                dimensionality = DIMENSIONALITY_VARIABLE;
                                BuildMeta();
                                nextevent = StreamSourceEventType.NEXT_OBJECT;
                                return StreamSourceEventType.META_CHANGED;
                            }
                        }
                        else if (curlbl != null && meta != null && meta.Count == 1)
                        {
                            BuildMeta();
                            nextevent = StreamSourceEventType.NEXT_OBJECT;
                            return StreamSourceEventType.META_CHANGED;
                        }
                        return StreamSourceEventType.NEXT_OBJECT;
                    }
                }
                reader.Close();
                reader = null;
                return StreamSourceEventType.END_OF_STREAM;
            }
            catch (IOException)
            {
                throw new ArgumentException("Error while parsing line " + lineNumber + ".");
            }
        }
        /**
       * Internal method for parsing a single line. Used by both line based parsing
       * as well as block parsing. This saves the building of meta data for each
       * line.
       * 
       * @param line Line to process
       */
        protected void ParseLineInternal(String line)
        {
            List<String> entries = Tokenize(line);
            // Split into numerical attributes and labels
            List<int> attributes = new List<int>(entries.Count);
            LabelList lbls = new LabelList();

            for (int i = 0; i < 2; i++)
            {
                string str = entries[i];

                try
                {
                    int attr = int.Parse(str);
                    attributes.Add(attr);
                    continue;
                }
                catch (FormatException)
                {
                    Debug.Assert(1 == 1, "Input File Data Format Error");
                }


            }
            lbls.Add(entries[2]);
            curvec = CreateDBObject(attributes);
            curlbl = lbls;
        }
        protected override Log.Logging GetLogger()
        {
            return logger;
        }
        SimpleTypeInformation GetTypeInformation(int dimensionality)
        {

            Type cls = factory.GetType();
            if (dimensionality > 0)
            {

                return new SimpleTypeInformation(cls);
            }
            throw new AbortException("No vectors were read from the input file - cannot determine vector data type.");
        }
        /**
       * <p>
       * Creates a database object of type V.
       * </p>
       * 
       * @param attributes the attributes of the vector to create.
       * @return a RalVector of type V containing the given attribute values
       */
        protected Pair<int, int> CreateDBObject(IList<int> attributes)
        {
            return new Pair<int, int>(attributes[0], attributes[1]);
            //IDbId id1 = null;
            //IDbId id2 = null;
            //if (relation == null)
            //{
            //    throw new AbortException("Relation Must be Set to check constraints");
            //}
            //foreach (var id in relation.GetDbIds())
            //{
            //    if (id.IntegerID == attributes[0])
            //    {
            //        id1 = id;
            //    }
            //    else if
            //        (id.IntegerID == attributes[1])
            //    {
            //        id2 = id;
            //    }
            //    if (id1 != null && id2 != null)
            //    {
            //        return new Pair<IDbId, IDbId>(id1, id2);
            //    }
            //}
            //throw new ArgumentException("ID provided in constraints file cannot be find in data");
        }
        /**
      * Update the meta element.
      */
        protected void BuildMeta()
        {

            meta = new BundleMeta(2);
            meta.Add(GetTypeInformation(dimensionality));
            meta.Add(TypeUtil.LABELLIST);

        }
        public new class Parameterizer : AbstractParser.Parameterizer
        {
            protected override object MakeInstance()
            {
                return new PairwiseConstraintsParser();
            }
        }
    }
}
