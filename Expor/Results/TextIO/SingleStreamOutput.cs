using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;

namespace Socona.Expor.Results.TextIO
{
   
public class SingleStreamOutput :IStreamFactory {
  /**
   * Output stream
   */
  private Stream stream;
  
  /**
   * Constructor using stdout.
   * @throws IOException on IO error
   */
  public SingleStreamOutput()  :
    this(System.Console.OpenStandardOutput(),false){
  }
  
  /**
   * Constructor using stdout
   * 
   * @param gzip Use gzip compression
   * @throws IOException on IO error
   */
  public SingleStreamOutput(bool gzip)  :
    this(System.Console.OpenStandardOutput(), gzip){
  }
  
  /**
   * Constructor with given file name.
   * 
   * @param out filename
   * @throws IOException on IO error
   */
  public SingleStreamOutput(FileInfo sout) :
    this(new FileStream(sout.FullName,  FileMode.OpenOrCreate, FileAccess.Write)){
  }

  /**
   * Constructor with given file name.
   * 
   * @param out filename
   * @param gzip Use gzip compression
   * @throws IOException on IO error
   */
  public SingleStreamOutput(FileInfo sout, bool gzip) :
    this(new FileStream(sout.FullName,FileMode.OpenOrCreate,FileAccess.Write), gzip){
  }

  /**
   * Constructor with given FileDescriptor
   * 
   * @param out file descriptor
   * @throws IOException on IO error
   */
  //public SingleStreamOutput(Stream sout)  {
  //  this(sout);
  //}
  
  /**
   * Constructor with given FileDescriptor
   * 
   * @param out file descriptor
   * @param gzip Use gzip compression
   * @throws IOException on IO error
   */
 // public SingleStreamOutput(FileDescriptor out, boolean gzip) throws IOException {
    //this(new FileOutputStream(out), gzip);
 // }
  
  /**
   * Constructor with given FileOutputStream.
   * 
   * @param out File output stream
   * @throws IOException on IO error
   */
  public SingleStreamOutput(FileStream sout):
    this(sout, false){
  }

  /**
   * Constructor with given FileOutputStream.
   * 
   * @param out File output stream
   * @param gzip Use gzip compression
   * @throws IOException on IO error
   */
  public SingleStreamOutput(Stream sout, bool gzip)  {
    Stream os = sout;
    if (gzip) {
      // wrap into gzip stream.
      os = new GZipOutputStream(os);
    }
    this.stream =(os);
  }

  /**
   * Return the objects shared print stream.
   * 
   * @param filename ignored filename for SingleStreamOutput, as the name suggests
   */
  
  public Stream OpenStream(String filename) {
    return stream;
  }

  /**
   * Close output stream.
   */
  
  public void CloseAllStreams() {
    stream.Close();
  }
}
}
