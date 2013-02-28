using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Socona.Expor.Results.TextIO
{

    public interface IStreamFactory
    {
        /**
         * Retrieve a print stream for output using the given label.
         * Note that multiple labels MAY result in the same PrintStream, so you
         * should be printing to only one stream at a time to avoid mixing outputs.
         * 
         * @param label Output label.
         * @return stream object for the given label
         * @throws IOException on IO error
         */
        Stream OpenStream(String label);

        /**
         * Close (and forget) all streams the factory has opened.
         */
        void CloseAllStreams();
    }
}
