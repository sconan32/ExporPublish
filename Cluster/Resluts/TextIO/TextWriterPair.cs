using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Utilities.Exceptions;

namespace Socona.Clustering.Resluts.TextIO
{
   /**
 * Write a pair
 * 
 * @author Erich Schubert
 *
 */
public class TextWriterPair:ITextWriter {
  /**
   * Serialize a pair, component-wise
   */
  
  public override void Write(TextWriterStream sout, String label, KeyValuePair<object,object> o) 
{
    
      Object first = o.Key;
      if (first != null) {
        ITextWriter tw = (ITextWriter) sout.GetWriterFor(first);
        if (tw == null) {
          throw new UnableToComplyException("No handler for database object itself: " + first.GetType().Name);
        }
        tw.Write(sout, label, first);
      }
      Object second = o.Value;
      if (second != null) {
        ITextWriter tw = (ITextWriter) sout.GetWriterFor(second);
        if (tw == null) {
          throw new UnableToComplyException("No handler for database object itself: " + second.GetType().Name);
        }
        tw.Write(sout, label, second);
      }
    
  }
}

}
