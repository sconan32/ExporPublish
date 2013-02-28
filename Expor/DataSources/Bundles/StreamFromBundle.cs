using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.DataSources.Bundles
{

    public class StreamFromBundle : IBundleStreamSource
    {
        /**
         * Bundle to access
         */
        MultipleObjectsBundle bundle;

        /**
         * Offset in bundle
         */
        int onum = -2;

        /**
         * Constructor.
         * 
         * @param bundle Existing object bundle
         */
        public StreamFromBundle(MultipleObjectsBundle bundle)
            : base()
        {
            this.bundle = bundle;
        }


        public BundleMeta GetMeta()
        {
            return bundle.Meta();
        }


        public Object Data(int rnum)
        {
            return bundle.Data(onum, rnum);
        }


        public StreamSourceEventType NextEvent()
        {
            onum += 1;
            if (onum < 0)
            {
                return StreamSourceEventType.META_CHANGED;
            }
            if (onum >= bundle.DataLength())
            {
                return StreamSourceEventType.END_OF_STREAM;
            }
            return StreamSourceEventType.NEXT_OBJECT;
        }
    }
}
