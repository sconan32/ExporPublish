using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Socona.Expor.Indexes.Tree
{

    public interface IEntry : ISerializable
    {

        /// <summary>
        ///  Returns true if this entry is an entry in a leaf node (i.e. this entry 
        ///  represents a data object), false otherwise.
        /// </summary>
        /// <returns>true if this entry is an entry in a leaf node, false otherwise</returns>
         bool IsLeafEntry();
    }
}
