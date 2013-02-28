using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids
{

    /**
     * Some object referencing a {@link DBID}. Could be a {@link DBID}, a
     * {@link DBIDIter}, for example.
     * 
     * Important note: <em>do not assume this reference to be stable</em>. Iterators
     * are a good example how the DBIDRef may change.
     * 
     * @author Erich Schubert
     */
    public interface IDbIdRef
    {
        /**
         * Get the referenced {@link DBID}.
         * 
         * Efficiency note: this may require materialization of a DBID object.
         * 
         * @return referenced DBID
         */
        IDbId DbId { get; }

        /**
         * Return the integer value of the object ID, if possible.
         * 
         * @return integer id
         */
        int Int32Id { get; }
        /**
         * Compare the <em>current</em> value of two referenced DBIDs.
         * 
         * @param other Other DBID reference (or DBID)
         * @return {@code true} when the references <em>currently</em> refer to the same.
         */
        bool IsSameDbId(IDbIdRef other);

        /**
         * Compare two objects by the value of the referenced DBID.
         * 
         * @param other Other DBID or object
         * @return -1, 0 or +1
         */
        int CompareDbId(IDbIdRef other);
        /**
         * Get the internal index.
         * 
         * <b>NOT FOR PUBLIC USE - ELKI Optimization engine only</b>
         * 
         * @return Internal index
         */
        int InternalGetIndex();
    }
}
