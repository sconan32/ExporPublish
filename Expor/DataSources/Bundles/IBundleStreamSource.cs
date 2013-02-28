using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.DataSources.Bundles
{

    public interface IBundleStreamSource
    {
        /**
         * Events
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */


        /**
         * Get the current meta data.
         * 
         * @return Metadata
         */
         BundleMeta GetMeta();

        /**
         * Access a particular object and representation.
         * 
         * @param rnum Representation number
         * @return Contained data
         */
         Object Data(int rnum);

        /**
         * Get the next event
         * 
         * @return Event type
         */
         StreamSourceEventType NextEvent();
    }
    public enum StreamSourceEventType
    {
        NONE,
        // Metadata has changed
        META_CHANGED,
        // Next object available
        NEXT_OBJECT,
        // Stream ended
        END_OF_STREAM,
    };
}
