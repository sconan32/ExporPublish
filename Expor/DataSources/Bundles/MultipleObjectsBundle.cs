using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Types;
using Socona.Expor.Utilities.Exceptions;
using Socona.Log;

namespace Socona.Expor.DataSources.Bundles
{
    /// <summary>
    /// 这个类中存储的是数据库中列名和列的内容
    /// </summary>
    public class MultipleObjectsBundle : IObjectBundle
    {
        /**
         * Storing the meta data.
         */
        private BundleMeta meta;

        /**
         * Storing the real contents.
         */
        private IList<IList<object>> columns;

        /**
         * Constructor.
         */
        public MultipleObjectsBundle()
        {
            this.meta = new BundleMeta();
            this.columns = new List<IList<object>>();
        }

        /**
         * Constructor.
         * 
         * @param meta Meta data contained.
         * @param columns Content in columns
         */
        [Obsolete]
        public MultipleObjectsBundle(BundleMeta meta, IList<IList<object>> columns)
            : base()
        {
            this.meta = meta;
            this.columns = columns;
            if (this.columns.Count != this.meta.Count)
            {
                throw new AbortException("Meta size and columns do not agree!");
            }
            int len = -1;
            foreach (IList<object> col in columns)
            {
                if (len < 0)
                {
                    len = col.Count;
                }
                else
                {
                    if (col.Count != len)
                    {
                        throw new AbortException("Column lengths do not agree.");
                    }
                }
            }
        }


        public BundleMeta Meta()
        {
            return meta;
        }


        public SimpleTypeInformation Meta(int i)
        {
            return (SimpleTypeInformation)meta[i];
        }


        public int MetaLength()
        {
            return meta.Count;
        }


        public Object Data(int onum, int rnum)
        {
            if (rnum < 0 || rnum >= meta.Count)
            {
                throw new IndexOutOfRangeException();
            }
            return columns[rnum][onum];
        }


        public int DataLength()
        {
            try
            {
                return columns[0].Count;
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0;
            }
        }

        /**
         * Append a new record to the data set. Pay attention to having the right
         * number of values!
         * 
         * @param data Data to append
         */
        public void AppendSimple(params Object[] data)
        {
            if (data.Length != meta.Count)
            {
                throw new AbortException("Invalid number of attributes in 'append'.");
            }
            for (int i = 0; i < data.Length; i++)
            {

                IList<Object> col = (IList<Object>)columns[i];
                col.Add(data[i]);
            }
        }

        /**
         * Helper to add a single column to the bundle.
         * 
         * @param type Type information
         * @param data Data to add
         * @return This object, for chaining.
         */
        public MultipleObjectsBundle AppendColumn(SimpleTypeInformation type, IList<object> data)
        {
            meta.Add(type);
            columns.Add(data);
            return this;
        }

        /**
         * Get the raw objects columns. Use with caution!
         * 
         * @param i column number
         * @return the ith column
         */
        public IList<object> GetColumn(int i)
        {
            return columns[i];
        }

        /**
         * Helper to add a single column to the bundle.
         * 
         * @param <V> Object type
         * @param type Type information
         * @param data Data to add
         */
        public static MultipleObjectsBundle MakeSimple<V>(SimpleTypeInformation type, IList<V> data)
        {
            MultipleObjectsBundle bundle = new MultipleObjectsBundle();
            SimpleTypeInformation stype = type as SimpleTypeInformation;
            IList<object> sdata = data as IList<object>;
            bundle.AppendColumn(stype, sdata);
            return bundle;
        }

        /**
         * Helper to add a single column to the bundle.
         * 
         * @param <V1> First Object type
         * @param <V2> Second Object type
         * @param type1 Type information
         * @param data1 Data column to add
         * @param type2 Second Type information
         * @param data2 Second data column to add
         */
        public static MultipleObjectsBundle MakeSimple<V1, V2>(SimpleTypeInformation type1, IList<V1> data1, SimpleTypeInformation type2, IList<V2> data2)
        {
            MultipleObjectsBundle bundle = new MultipleObjectsBundle();
            SimpleTypeInformation st1 = type1 as SimpleTypeInformation;

            SimpleTypeInformation st2 = type2 as SimpleTypeInformation;

            IList<object> sd1 = data1 as IList<object>;
            IList<object> sd2 = data2 as IList<object>;
            bundle.AppendColumn(st1, sd1);
            bundle.AppendColumn(st2, sd2);
            return bundle;
        }

        /**
         * Helper to add a single column to the bundle.
         * 
         * @param <V1> First Object type
         * @param <V2> Second Object type
         * @param <V3> Third Object type
         * @param type1 First type information
         * @param data1 First data column to add
         * @param type2 Second type information
         * @param data2 Second data column to add
         * @param type3 Third type information
         * @param data3 Third data column to add
         */
        public static MultipleObjectsBundle MakeSimple<V1, V2, V3>(SimpleTypeInformation type1, List<V1> data1, SimpleTypeInformation type2, List<V2> data2, SimpleTypeInformation type3, List<V3> data3)
        {
            MultipleObjectsBundle bundle = new MultipleObjectsBundle();
            SimpleTypeInformation st1 = type1 as SimpleTypeInformation;

            SimpleTypeInformation st2 = type2 as SimpleTypeInformation;
            SimpleTypeInformation st3 = type3 as SimpleTypeInformation;

            IList<object> sd1 = data1 as IList<object>;
            IList<object> sd2 = data2 as IList<object>;
            IList<object> sd3 = data3 as IList<object>;
            bundle.AppendColumn(st1, sd1);
            bundle.AppendColumn(st2, sd2);
            bundle.AppendColumn(st3, sd3);
            return bundle;
        }

        /**
         * Convert an object stream to a bundle
         * 
         * @param source Object stream
         * @return Static bundle
         */
        public static MultipleObjectsBundle FromStream(IBundleStreamSource source)
        {
            MultipleObjectsBundle bundle = new MultipleObjectsBundle();
            bool stop = false;
            while (!stop)
            {
                StreamSourceEventType ev = source.NextEvent();
                switch (ev)
                {
                    case StreamSourceEventType.END_OF_STREAM:
                        stop = true;
                        break;
                    case StreamSourceEventType.META_CHANGED:
                        BundleMeta smeta = source.GetMeta();
                        // rebuild bundle meta
                        bundle.meta = new BundleMeta();
                        for (int i = 0; i < bundle.columns.Count; i++)
                        {
                            bundle.meta.Add(smeta[i]);
                        }
                        for (int i = bundle.MetaLength(); i < smeta.Count; i++)
                        {
                            List<Object> data = new List<Object>(bundle.DataLength() + 1);
                            bundle.AppendColumn(smeta[i], data);
                        }
                        continue;
                    case StreamSourceEventType.NEXT_OBJECT:
                        for (int i = 0; i < bundle.MetaLength(); i++)
                        {

                            IList<Object> col = bundle.columns[i];
                            col.Add(source.Data(i));
                        }
                        continue;
                    default:
                        Logging.GetLogger(typeof(MultipleObjectsBundle)).Warning("Unknown event: " + ev);
                        stop = true;//当遇到其它信号时停止继续解析
                        continue;
                }
            }
            return bundle;
        }

        /**
         * Get an object row.
         * 
         * @param row Row number
         * @return Array of values
         */
        public Object[] GetRow(int row)
        {
            Object[] ret = new Object[columns.Count];
            for (int c = 0; c < columns.Count; c++)
            {
                ret[c] = Data(row, c);
            }
            return ret;
        }
    }
}
