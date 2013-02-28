using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Data
{
   
public interface ISparseFeatureVector<V > :IDataVector {
  /**
   * Bit set of non-null features
   * 
   * @return Bit set
   */
   BitArray GetNotNullMask();
}

}
