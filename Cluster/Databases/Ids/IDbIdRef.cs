using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Databases.Ids
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
public interface IDbIdRef {
  /**
   * Get the referenced {@link DBID}.
   * 
   * Efficiency note: this may require materialization of a DBID object.
   * 
   * @return referenced DBID
   */
    public IDbId DbId { get; }

  /**
   * Return the integer value of the object ID, if possible.
   * 
   * @return integer id
   */
  public int IntegerID { get; }

  /**
   * WARNING: Hash codes of this interface <b>might not be stable</b> (e.g. for
   * iterators).
   * 
   * @return current hash code (<b>may change!</b>)
   * 
   * @deprecated Do not use this hash code. Some implementations will not offer
   *             stable hash codes!
   */
 
  public override int GetHashCode();

  /**
   * WARNING: calling equality on a reference may be an indicator of incorrect
   * usage, as it is not clear whether the programmer meant the references to be
   * the same or the DBIDs.
   * 
   * @param obj Object to compare with
   * @return True when they are the same object
   */
 
  public override bool Equals(Object obj);
  
  /**
   * Compare the <em>current</em> value of two referenced DBIDs.
   * 
   * @param other Other DBID reference (or DBID)
   * @return {@code true} when the references <em>currently</em> refer to the same.
   */
  public bool IsSameDbId(IDbIdRef other);
  
  /**
   * Compare two objects by the value of the referenced DBID.
   * 
   * @param other Other DBID or object
   * @return -1, 0 or +1
   */
  public int CompareDbId(IDbIdRef other);
}
}
