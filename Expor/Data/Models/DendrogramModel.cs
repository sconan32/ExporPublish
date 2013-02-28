using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Results.TextIO;

namespace Socona.Expor.Data.Models
{
   
public class DendrogramModel : BaseModel {

  private IDistanceValue distance;

  public DendrogramModel(IDistanceValue distance) :
    base(){
    this.distance = distance;
  }

  /**
   * @return the distance
   */
  public IDistanceValue GetDistance() {
    return distance;
  }

  /**
   * Implementation of {@link TextWriteable} interface.
   */
  
  public override  void WriteToText(TextWriterStream sout, String label) {
    base.WriteToText(sout, label);
    sout.CommentPrintLine("Distance to children: " + (distance != null ? distance.ToString() : "null"));
  }

  
  public override String ToString() {
    return "Distance to children: " + (distance != null ? distance.ToString() : "null");
  }
}

}
