using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using Socona.Log;

namespace Socona.Expor.Results.TextIO
{

    public class MultipleFilesOutput : IStreamFactory
    {
        /**
         * File name extension.
         */
        private static String EXTENSION = ".txt";

        /**
         * GZip extra file extension
         */
        private static String GZIP_EXTENSION = ".gz";

        /**
         * Default stream to write to when no name is supplied.
         */
        private Stream defaultStream = null;

        /**
         * Base file name.
         */
        private FileInfo basename;

        /**
         * HashMap of open print streams.
         */
        private Dictionary<String, Stream> map = new Dictionary<String, Stream>();

        /**
         * Control gzip compression of output.
         */
        private bool usegzip = false;

        /**
         * Logger for debugging.
         */
        private static Logging logger = Logging.GetLogger(typeof(MultipleFilesOutput));

        /**
         * Constructor
         * 
         * @param base Base file name (folder name)
         */
        public MultipleFilesOutput(FileInfo _base) :
            this(_base, false)
        {
        }

        /**
         * Constructor
         * 
         * @param base Base file name (folder name)
         * @param gzip Use gzip compression.
         */
        public MultipleFilesOutput(FileInfo _base, bool gzip)
        {
            this.basename = _base;
            this.usegzip = gzip;
        }

        /**
         * Retrieve/open the default output stream.
         * 
         * @return default output stream
         * @throws IOException
         */
        private Stream GetDefaultStream()
        {
            if (defaultStream != null)
            {
                return defaultStream;
            }
            defaultStream = NewStream("default");
            return defaultStream;
        }

        /**
         * Open a new stream of the given name
         * 
         * @param name file name (which will be appended to the base name)
         * @return stream object for the given name
         * @throws IOException
         */
        private Stream NewStream(String name)
        {
            if (logger.IsDebugging)
            {
                logger.Debug("Requested stream: " + name);
            }
            Stream res = null;
            map.TryGetValue(name,out res);
            if (res != null)
            {
                return res;
            }
            // Ensure the directory exists:
            if (!basename.Exists)
            {
                new DirectoryInfo(basename.FullName).Create();
            }
            //TODO:  Avoid "\\"
            String fn = basename.FullName + "\\" + name + EXTENSION;
            if (usegzip)
            {
                fn = fn + GZIP_EXTENSION;
            }
            FileInfo n = new FileInfo(fn);
            res = new FileStream(n.FullName, FileMode.OpenOrCreate);
            if (usegzip)
            {
                // wrap into gzip stream.
                res = new GZipOutputStream(res);
            }
            // res = new Stream(os);
            if (logger.IsDebugging)
            {
                logger.Debug("Opened new output stream:" + fn);
            }
            // cache.
            map[name] = res;
            return res;
        }

        /**
         * Retrieve the output stream for the given file name.
         */

        public Stream OpenStream(String filename)
        {
            if (filename == null)
            {
                return GetDefaultStream();
            }
            return NewStream(filename);
        }

        /**
         * Get GZIP compression flag.
         * 
         * @return if GZIP compression is enabled
         */
        protected bool IsGzipCompression()
        {
            return usegzip;
        }

        /**
         * Set GZIP compression flag.
         * 
         * @param usegzip use GZIP compression
         */
        protected void setGzipCompression(bool usegzip)
        {
            this.usegzip = usegzip;
        }

        /**
         * Close (and forget) all output streams.
         */

        public void CloseAllStreams()
        {
            lock (this)
            {
                foreach (Stream s in map.Values)
                {
                    s.Close();
                }
                map.Clear();
            }
        }
    }

}
