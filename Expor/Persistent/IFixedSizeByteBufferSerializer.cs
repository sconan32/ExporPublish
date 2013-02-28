using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Persistent
{

    /**
     * Serializers with a fixed length serialization.
     * 
     * @author Erich Schubert
     * 
     * @param <T> Type
     */
    public interface IFixedSizeByteBufferSerializer<T> : IByteBufferSerializer
    {
        /**
         * Get the fixed size needed by this serializer.
         * 
         * @return Size
         */
         int GetFixedByteSize();
    }
}
