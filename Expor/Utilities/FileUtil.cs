using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities
{

    public sealed class FileUtil
    {
        /**
         * Returns the lower case extension of the selected file.
         * 
         * If no file is selected, <code>null</code> is returned.
         * 
         * @param file FileInfo object
         * @return Returns the extension of the selected file in lower case or
         *         <code>null</code>
         */
        public static String GetFilenameExtension(FileInfo file)
        {
            return GetFilenameExtension(file.Name);
        }


        /**
         * Returns the lower case extension of the selected file.
         * 
         * If no file is selected, <code>null</code> is returned.
         * 
         * @param name FileInfo name
         * @return Returns the extension of the selected file in lower case or
         *         <code>null</code>
         */
        public static String GetFilenameExtension(String name)
        {
            if (name == null)
            {
                return null;
            }
            return Path.GetExtension(name).ToLower();

        }

        /**
         * Try to open a file, first trying the file system,
         * then falling back to the classpath.
         * 
         * @param filename FileInfo name in system notation
         * @return Input stream
         * @throws FileNotFoundException When no file was found.
         */
        public static StreamReader OpenSystemFile(String filename)
        {
            try
            {
                return new StreamReader(filename);
            }
            catch (FileNotFoundException e)
            {
                // try with classloader
                String resname = filename.Replace(Path.DirectorySeparatorChar, '/');
                StreamReader result = new StreamReader(Properties.Resources.ResourceManager.GetStream(resname));
                if (result == null)
                {
                    throw e;
                }
                return result;
            }
        }

        /**
         * Try to open a stream as gzip, if it starts with the gzip magic.
         * 
         * TODO: move to utils package.
         * 
         * @param in original input stream
         * @return old input stream or a {@link GZIPInputStream} if appropriate.
         * @throws IOException on IO error
         */
        public static Stream TryGzipInput(Stream ins)
        {
            // try autodetecting gzip compression.
            if (!ins.CanSeek)
            {
                MemoryStream pb = (MemoryStream)ins;
                //PushbackInputStream pb = new PushbackInputStream(ins, 16);
                // ins = pb;
                // read a magic from the file header
                byte[] magic = { 0, 0 };
                pb.Read(magic, 0, 2);
                pb.Seek(0, SeekOrigin.Begin);
                if (magic[0] == 31 && magic[1] == 0x8B)
                {
                    ins = (GZipStream)ins;
                }
            }
            else
                if (ins.CanSeek)
                {
                    //ins.mark(16);
                    if (ins.ReadByte() == 31 && ins.ReadByte() == 0x8B)
                    {
                        ins.Seek(0, SeekOrigin.Begin);
                        ins = (GZipStream)(ins);
                    }
                    else
                    {
                        // just rewind the stream
                        ins.Seek(0, SeekOrigin.Begin);
                    }
                }
            return ins;
        }

        /**
         * Try to locate an file in the filesystem, given a partial name and a prefix.
         * 
         * @param name file name
         * @param basedir extra base directory to try
         * @return file, if the file could be found. {@code null} otherwise
         */
        public static FileInfo LocateFile(String name, String basedir)
        {
            // Try exact match first.
            FileInfo f = new FileInfo(name);
            if (f.Exists)
            {
                return f;
            }
            // Try with base directory
            if (basedir != null)
            {
                f = new FileInfo(basedir + name);
                // logger.warning("Trying: "+f.getAbsolutePath());
                if (f.Exists)
                {
                    return f;
                }
            }
            // try stripping whitespace
            {
                String name2 = name.Trim();
                if (!name.Equals(name2))
                {
                    // logger.warning("Trying without whitespace.");
                    f = LocateFile(name2, basedir);
                    if (f != null)
                    {
                        return f;
                    }
                }
            }
            // try substituting path separators
            {
                String name2 = name.Replace('/', Path.DirectorySeparatorChar);
                if (!(name == name2))
                {
                    // logger.warning("Trying with replaced separators.");
                    f = LocateFile(name2, basedir);
                    if (f != null)
                    {
                        return f;
                    }
                }
                name2 = name.Replace('\\', Path.DirectorySeparatorChar);
                if (!(name == name2))
                {
                    // logger.warning("Trying with replaced separators.");
                    f = LocateFile(name2, basedir);
                    if (f != null)
                    {
                        return f;
                    }
                }
            }
            // try stripping extra characters, such as quotes.
            if (name.Length > 2 && name[0] == '"' && name[name.Length - 1] == '"')
            {
                // logger.warning("Trying without quotes.");
                f = LocateFile(name.Substring(1, name.Length - 1), basedir);
                if (f != null)
                {
                    return f;
                }
            }
            return null;
        }
    }

}
