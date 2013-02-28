using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;

namespace Socona.Expor.Results
{
  
public class SamplingResult:IResult {
  /**
   * The actual selection
   */
  IDbIds sample = null;

  /**
   * Constructor.
   * 
   * @param rel Relation
   */
  public SamplingResult(IRelation rel) {
   
    sample = rel.GetDbIds();
  }

  /**
   * @return the current sample
   */
  public IDbIds GetSample() {
    return sample;
  }

  /**
   * Note: trigger a resultchanged event!
   * 
   * @param sample the new sample
   */
  public void SetSample(IDbIds sample) {
    this.sample = sample;
  }


  public String LongName {
   get{ return "Sample";}
  }

  
  public String ShortName {
      get { return "sample"; }
  }
}
}
