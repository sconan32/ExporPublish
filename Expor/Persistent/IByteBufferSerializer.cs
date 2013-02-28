using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.DataStructures;

namespace Socona.Expor.Persistent
{

    /**
     * Class to convert from and to byte arrays (in index structures)
     * 
     * @author Erich Schubert
     * 
     * @apiviz.uses ByteBuffer
     *
     * @param <T> Object type processed
     */
    public interface IByteBufferSerializer
    {
        /**
         * Deserialize an object from a byte buffer (e.g. disk)
         * 
         * @param buffer Data array to process
         * @return Deserialized object
         */
        object FromByteBuffer(Type type, ByteBuffer buffer);

        /**
         * Serialize the object to a byte array (e.g. disk)
         * 
         * @param buffer Buffer to serialize to
         * @param object Object to serialize
         */
        void ToByteBuffer(ByteBuffer buffer, object o, Type t);

        /**
         * Get the size of the object in bytes.
         * 
         * @param object Object to serialize
         * @return maximum size in serialized form
         */
        int GetByteSize(Object o, Type type);
    }
}
