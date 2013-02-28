using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.Persistent;
using Socona.Expor.Utilities.DataStructures.ArrayLike;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Log;
using Socona.Expor.Utilities.Extenstions;

namespace Socona.Expor.DataSources.Parsers
{

    public class NumberVectorLabelParser : AbstractStreamingParser
    {
        /**
         * Logging class.
         */
        private static readonly Logging logger = Logging.GetLogger(typeof(NumberVectorLabelParser));

        /**
         * A comma separated list of the indices of labels (may be numeric), counting
         * whitespace separated entries in a line starting with 0. The corresponding
         * entries will be treated as a label.
         * <p>
         * Key: {@code -parser.labelIndices}
         * </p>
         */
        public static readonly OptionDescription LABEL_INDICES_ID = OptionDescription.GetOrCreate("parser.labelIndices",
            "A comma separated list of the indices of labels (may be numeric), " +
            "counting whitespace separated entries in a line starting with 0. The corresponding entries will be treated as a label.");

        /**
         * Parameter to specify the type of vectors to produce.
         * <p>
         * Key: {@code -parser.vector-type}<br />
         * Default: DoubleVector
         * </p>
         */
        public static readonly OptionDescription VECTOR_TYPE_ID = OptionDescription.GetOrCreate("parser.vector-type",
            "The type of vectors to create for numerical attributes.");

        /**
         * Constant used for unknown dimensionality (e.g. empty files)
         */
        public static readonly int DIMENSIONALITY_UNKNOWN = -1;

        /**
         * Constant used for records of variable dimensionality (e.g. time series)
         */
        public static readonly int DIMENSIONALITY_VARIABLE = -2;

        /**
         * Keeps the indices of the attributes to be treated as a string label.
         */
        protected BitArray labelIndices;

        /**
         * Vector factory class
         */
        protected INumberVector factory;

        /**
         * Buffer reader
         */
        private StreamReader reader;

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
        protected INumberVector curvec = default(INumberVector);

        /**
         * Current labels
         */
        protected LabelList curlbl = null;

        /**
         * Event to report next
         */

        StreamSourceEventType nextevent = default(StreamSourceEventType);

        /**
         * Constructor with defaults
         * 
         * @param factory Vector factory
         */
        public NumberVectorLabelParser(INumberVector factory) :
            this(new Regex(DEFAULT_SEPARATOR), QUOTE_CHAR, null, factory)
        {
            this.labelIndices = new BitArray(20000);
        }


        /**
         * Constructor
         * 
         * @param colSep
         * @param quoteChar
         * @param labelIndices
         * @param factory Vector factory
         */
        public NumberVectorLabelParser(Regex colSep, char quoteChar, BitArray labelIndices, INumberVector factory) :
            base(colSep, quoteChar)
        {
            this.labelIndices = labelIndices;
            this.factory = factory;
        }


        public override void InitStream(Stream ins)
        {
            reader = new StreamReader(ins);
            lineNumber = 1;
            dimensionality = DIMENSIONALITY_UNKNOWN;
            columnnames = null;
            labelcolumns = new BitArray(20000);
            nextevent = StreamSourceEventType.NONE;
        }


        public override BundleMeta GetMeta()
        {
            return meta;
        }


        public override StreamSourceEventType NextEvent()
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
                            dimensionality = curvec.Count;
                            BuildMeta();
                            nextevent = StreamSourceEventType.NEXT_OBJECT;
                            return StreamSourceEventType.META_CHANGED;
                        }
                        else if (dimensionality > 0)
                        {
                            if (dimensionality != curvec.Count)
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
         * Update the meta element.
         */
        protected void BuildMeta()
        {
            if (labelcolumns.Count > 0)
            {
                meta = new BundleMeta(2);
                meta.Add(GetTypeInformation(dimensionality));
                meta.Add(TypeUtil.LABELLIST);
            }
            else
            {
                meta = new BundleMeta(1);
                meta.Add(GetTypeInformation(dimensionality));
            }
        }


        public override Object Data(int rnum)
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
            List<double> attributes = new List<double>(entries.Count);
            LabelList labels = null;

            for (int i = 0; i < entries.Count; i++)
            {
                string str = entries[i];
                if (!labelIndices[i])
                {
                    try
                    {
                        double attr = double.Parse(str);
                        attributes.Add(attr);
                        continue;
                    }
                    catch (FormatException)
                    {
                        labelcolumns[i] = true;
                    }
                }

                if (labels == null)
                {
                    labels = new LabelList(1);
                }
                labels.Add(str);
            }
            // Maybe a label row?
            if (lineNumber == 1 && attributes.Count == 0)
            {
                columnnames = labels;
                labelcolumns = new BitArray(100, false);
                curvec = default(INumberVector);
                curlbl = null;
                return;
            }
            // Pass outside via class variables
            curvec = CreateDBObject(attributes);
            curlbl = labels;
        }

        /**
         * <p>
         * Creates a database object of type V.
         * </p>
         * 
         * @param attributes the attributes of the vector to create.
         * @return a RalVector of type V containing the given attribute values
         */
        protected INumberVector CreateDBObject(IList<double> attributes)
        {
            return (INumberVector)factory.NewNumberVector(attributes.ToArray());
        }

        /**
         * Get a prototype object for the given dimensionality.
         * 
         * @param dimensionality Dimensionality
         * @return Prototype object
         */
        SimpleTypeInformation GetTypeInformation(int dimensionality)
        {

            Type cls = factory.GetType();
            if (dimensionality > 0)
            {
                String[] colnames = null;
                if (columnnames != null)
                {
                    if (columnnames.Count - labelcolumns.Cardinality() == dimensionality)
                    {
                        colnames = new String[dimensionality];
                        for (int i = 0, j = 0; i < columnnames.Count; i++)
                        {
                            if (labelcolumns[i] == false)
                            {
                                colnames[j] = columnnames[i];
                                j++;
                            }
                        }
                    }
                }
                INumberVector f = factory.NewNumberVector(new double[dimensionality]);
                if (f is IByteBufferSerializer)
                {
                    // TODO: Remove, once we have serializers for all types

                    IByteBufferSerializer ser = (IByteBufferSerializer)f;
                    return new VectorFieldTypeInformation<INumberVector>(cls, ser, dimensionality, colnames, f);
                }
                return new VectorFieldTypeInformation<INumberVector>(cls, dimensionality, colnames, f);
            }
            // Variable dimensionality - return non-vector field type
            if (dimensionality == DIMENSIONALITY_VARIABLE)
            {
                INumberVector f = factory.NewNumberVector(new double[0]);
                if (f is IByteBufferSerializer)
                {
                    // TODO: Remove, once we have serializers for all types

                    IByteBufferSerializer ser = (IByteBufferSerializer)f;
                    return new SimpleTypeInformation(cls, ser);
                }
                return new SimpleTypeInformation(cls);
            }
            throw new AbortException("No vectors were read from the input file - cannot determine vector data type.");
        }


        protected override Logging GetLogger()
        {
            return logger;
        }

        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public new class Parameterizer : AbstractParser.Parameterizer
        {
            /**
             * Keeps the indices of the attributes to be treated as a string label.
             */
            protected BitArray labelIndices = null;

            /**
             * Factory
             */
            protected INumberVector factory;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                GetLabelIndices(config);
                GetFactory(config);
            }

            protected void GetFactory(IParameterization config)
            {
                ObjectParameter<INumberVector> factoryP = new ObjectParameter<INumberVector>(VECTOR_TYPE_ID, typeof(INumberVector), typeof(DoubleVector));
                if (config.Grab(factoryP))
                {
                    factory = factoryP.InstantiateClass(config);
                }
            }

            protected void GetLabelIndices(IParameterization config)
            {
                IntListParameter labelIndicesP = new IntListParameter(LABEL_INDICES_ID, true);

                labelIndices = new BitArray(1000);
                if (config.Grab(labelIndicesP))
                {
                    IList<Int32> labelcols = labelIndicesP.GetValue();
                    foreach (Int32 idx in labelcols)
                    {
                        labelIndices.Set(idx, true);
                    }
                }
            }


            protected override object MakeInstance()
            {
                return new NumberVectorLabelParser(colSep, quoteChar, labelIndices, factory);
            }




        }




    }
}
