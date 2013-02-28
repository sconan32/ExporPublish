using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Log.Progress
{

    public interface IProgress
    {
        /**
         * Serialize a description into a String buffer.
         * 
         * @param buf Buffer to serialize to
         * @return Buffer the data was serialized to.
         */
        StringBuilder AppendToBuffer(StringBuilder buf);

        /**
         * Test whether a progress is complete (and thus doesn't need to be shown anymore)
         * 
         * @return Whether the progress was completed.
         */
        bool IsComplete { get; }


    }
}
