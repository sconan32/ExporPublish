using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Options.Parameters
{

    public class FileParameter : Parameter<FileInfo>
    {
        /**
         * Available file types: {@link #INPUT_FILE} denotes an input file,
         * {@link #OUTPUT_FILE} denotes an output file.
         * 
         * @apiviz.exclude
         */
        public enum FileType
        {
            /**
             * Input files (i.e. read only)
             */
            INPUT_FILE,
            /**
             * Output files
             */
            OUTPUT_FILE
        }

        /**
         * The file type of this file parameter. Specifies if the file is an input of
         * output file.
         */
        private FileType fileType;

        /**
         * Constructs a file parameter with the given optionID, and file type.
         * 
         * @param optionID optionID the unique id of the option
         * @param fileType the file type of this file parameter
         */
        public FileParameter(OptionDescription optionID, FileType fileType) :
            base(optionID)
        {
            this.fileType = fileType;
        }

        /**
         * Constructs a file parameter with the given optionID, file type, and
         * optional flag.
         * 
         * @param optionID optionID the unique id of the option
         * @param fileType the file type of this file parameter
         * @param optional specifies if this parameter is an optional parameter
         */
        public FileParameter(OptionDescription optionID, FileType fileType, bool optional) :
            this(optionID, fileType)
        {
            SetOptional(optional);
        }

        /** {@inheritDoc} */

        public override String GetValueAsString()
        {
            try
            {
                return GetValue().Directory.FullName;
            }
            catch (IOException e)
            {
                throw new ApplicationException("", e);
            }
        }

        /** {@inheritDoc} */

        protected override FileInfo ParseValue(Object obj)
        {
            if (obj == null)
            {
                throw new UnspecifiedParameterException("Parameter \"" + GetName() + "\": No filename given!");
            }
            if (obj is FileInfo)
            {
                return (FileInfo)obj;
            }
            if (obj is String)
            {
                return new FileInfo((String)obj);
            }
            throw new UnspecifiedParameterException("Parameter \"" + GetName() + "\": Unsupported value given!");
        }

        /** {@inheritDoc} */

        protected override bool Validate(FileInfo obj)
        {
            if (!base.Validate(obj))
            {
                return false;
            }
            if (fileType == (FileType.INPUT_FILE))
            {
                try
                {
                    if (!obj.Exists)
                    {
                        throw new WrongParameterValueException("Given file " + obj.DirectoryName + " for parameter \"" + GetName() + "\" does not exist!\n");
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    throw new WrongParameterValueException("Given file \"" + obj.DirectoryName + "\" cannot be read, access denied!\n" + e.Message);
                }
            }
            return true;
        }

        /**
         * Returns a string representation of the parameter's type.
         * 
         * @return &quot;&lt;file_&gt;&quot;
         */

        public override String GetSyntax()
        {
            return "<file>";
        }

      
    }

}
