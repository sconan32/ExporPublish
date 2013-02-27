using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Databases.Ids
{
    public interface IDbId : IDbIdRef, IComparable<IDbIdRef>, IArrayDbIds {
  /**
   * Compare the <em>current</em> value of two referenced DBIDs.
   * 
   * @param other Other DBID reference (or DBID)
   * @return {@code true} when the references <em>currently</em> refer to the same.
   */

  public override bool IsSameDbId(IDbIdRef other);
  
  /**
   * Compare two objects by the value of the referenced DBID.
   * 
   * @param other Other DBID or object
   * @return -1, 0 or +1
   */
  
  public override int CompareDbId(IDbIdRef other);

  /**
   * In contrast to {@link DBIDRef}, the DBID interface is supposed to have a
   * stable hash code. However, it is generally preferred to use optimized
   * storage classes instead of Java collections!
   * 
   * @return hash code
   */
  
  public override int GetHashCode();

  /**
   * In contrast to {@link DBIDRef}, the DBID interface is supposed to have a
   * stable equals for other DBIDs.
   * 
   * Yet, {@link #sameDBID} is more type safe and explicit.
   * 
   * @return true when the object is the same DBID.
   */

  public override bool Equals(Object obj);

  /**
   * Part of the DBIDRef API, this <em>must</em> return {@code this} for an
   * actual DBID.
   * 
   * @return {@code this}
   * @deprecated When the object is known to be a DBID, the usage of this method
   *             is pointless, therefore it is marked as deprecated to cause a
   *             warning.
   */

  public override IDbId DbId{get;}

  /**
   * Compare two DBIDs for ordering.
   * 
   * Consider using {@link #compareDBID}, which is more explicit.
   * 
   * @param other Other DBID object
   * @return Comparison result
   */
 
  public override int CompareTo(IDbIdRef other);
}
}
