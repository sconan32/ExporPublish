using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Data.Types
{

    /**
     * Class wrapping a particular data type.
     * 
     * @author Erich Schubert
     */
    public interface ITypeInformation
    {
        /**
         * Test whether this type is assignable from another type.
         * 
         * @param type Other type
         * @return true when the other type is accepted as subtype.
         */
         bool IsAssignableFromType(ITypeInformation type);

        /**
         * Test whether this type is assignable from a given object instance.
         * 
         * @param other Other object
         * @return true when the other type is an acceptable instance.
         */
         bool IsAssignableFrom(Object other);
    }
}
