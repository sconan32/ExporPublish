using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Resluts.TextIO
{
   public class TextWriterObjectComment:ITextWriter {
  /**
   * Put an object into the comment section
   */
  
  public override void Write(TextWriterStream sout, String label, Object o) {
    String res = "";
    if(label != null) {
      res = res + label + "=";
    }
    if(o != null) {
      res = res + o.ToString();
    }
    sout.CommentPrintLine(res);
  }
}

}
