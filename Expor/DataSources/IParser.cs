using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.DataSources
{
  
public interface IParser :IParameterizable, IInspectionUtilFrequentlyScanned {
  /**
   * Returns a list of the objects parsed from the specified input stream.
   * 
   * @param in the stream to parse objects from
   * @return a list containing those objects parsed from the input stream
   */
  MultipleObjectsBundle Parse(Stream ins);
}

}
