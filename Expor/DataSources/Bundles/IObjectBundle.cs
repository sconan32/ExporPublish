using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Types;

namespace Socona.Expor.DataSources.Bundles
{
    public interface IObjectBundle
    {
        /**
         * Access the meta data.
         * 
         * @return metadata
         */
       BundleMeta Meta();

        /**
         * Access the meta data.
         * 
         * @param i component
         * @return metadata of component i
         */
      SimpleTypeInformation Meta(int i);

        /**
         * Get the metadata length.
         * 
         * @return length of metadata
         */
     int MetaLength();

        /**
         * Get the number of objects contained.
         * 
         * @return Number of objects
         */
       int DataLength();

        /**
         * Access a particular object and representation.
         * 
         * @param onum Object number
         * @param rnum Representation number
         * @return Contained data
         */
   Object Data(int onum, int rnum);
    }
}
