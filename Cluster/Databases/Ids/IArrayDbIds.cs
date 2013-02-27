using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Databases.Ids
{
   /**
 * Interface for array based DBIDs.
 * 
 * @author Erich Schubert
 */
public interface IArrayDbIds : IDbIds {
  /**
   * Get the i'th entry (starting at 0)
   * 
   * @param i Index
   * @return DBID of i'th entry.
   */
  public IDbId get(int i);



  /**
   * Size of the DBID "collection".
   * 
   * @return size
   */

  public override int Count { get; }

  /**
   * Search for the position of the given key, assuming that the data set is
   * sorted.
   * 
   * For keys not found, <code>-(1+insertion position)</code> is returned, as
   * for Java {@link java.util.Collections#binarySearch}
   * 
   * @param key Key to search for
   * @return Offset of key
   */
  public int BinarySearch(IDbIdRef key);
}

}
