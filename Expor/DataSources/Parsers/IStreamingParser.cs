using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Socona.Expor.DataSources.Bundles;

namespace Socona.Expor.DataSources.Parsers
{
public interface IStreamingParser :IParser, IBundleStreamSource {
  /**
   * Init the streaming parser for the given input stream.
   * 
   * @param in the stream to parse objects from
   */
  void InitStream(Stream ins);
}
}
