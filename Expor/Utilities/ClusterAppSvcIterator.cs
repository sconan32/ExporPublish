using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Exceptions;
using Socona.Log;

namespace Socona.Expor.Utilities
{
    class ClusterAppSvcEnumerator:IEnumerator<Type>
    {
         /**
   * Class logger.
   */
  private static readonly Logging logger = Logging.GetLogger(typeof(ClusterAppSvcEnumerator));

  /**
   * Prefix for the ELKI functionality discovery.
   */
  public static readonly String PREFIX = "/";

  /**
   * Comment character
   */
  public static readonly char COMMENT_CHAR = '#';

  /**
   * Parent class
   */
  private Type parent;

  /**
   * Classloader
   */
  //private ClassLoader cl;

  /**
   * Enumeration of configuration files
   */
  //private IEnumerable<Uri> configfiles;

  /**
   * Current iterator
   */
 // private IEnumerator<Type> curiter = null;

  /**
   * Next class to return
   */
  private Type nextclass;

  /**
   * Constructor.
   * 
   * @param parent Parent class
   * @param cl Classloader to use
   */
  public ClusterAppSvcEnumerator(Type parent) {
    this.parent = parent;
  
  }


  /**
   * Get services files for a given class.
   * 
   * @param parent Parent class
   */
  private void GetServiceFiles(Type parent) {
    try {
      String fullName = PREFIX + parent.Name;
      //configfiles = cl.getResources(fullName);
    }
    catch(IOException x) {
      throw new AbortException("Could not load service configuration files.", x);
    }
  }


  public  bool HasNext() {
    if(nextclass != null) {
      return true;
    }
    // Find next iterator
    //while((curiter == null) || !curiter.HasNext()) {
    //  if(!configfiles.hasMoreElements()) {
    //    return false;
    //  }
    //  curiter = parseFile(configfiles.nextElement());
    //}
    //nextclass = curiter.next();
    return true;
  }

  //private Iterator<Type> parseFile(URL nextElement) {
  //  ArrayList<Type> classes = new ArrayList<Type>();
  //  try {
  //    BufferedReader r = new BufferedReader(new InputStreamReader(nextElement.openStream(), "utf-8"));
  //    while(parseLine(r.readLine(), classes, nextElement)) {
  //      // Continue
  //    }
  //  }
  //  catch(IOException x) {
  //    throw new AbortException("Error reading configuration file", x);
  //  }
  //  return classes.iterator();
  //}

  private bool ParseLine(String line, IList<Type> classes, Uri nextElement)  {
    if(line == null) {
      return false;
    }
    // Ignore comments, trim whitespace
    {
      int begin = 0;
      int end = line.IndexOf(COMMENT_CHAR);
      if(end < 0) {
        end = line.Length;
      }
      while(begin < end && line[begin] == ' ') {
        begin++;
      }
      while(end - 1 > begin && line[end - 1] == ' ') {
        end--;
      }
      if(begin > 0 || end < line.Length) {
        line = line.Substring(begin, end);
      }
    }
    if(line.Length <= 0) {
      return true; // Empty/comment lines are okay, continue
    }
    // Try to load the class
    //try {
    //  Type cls = cl.loadClass(line);
    //  // Should not happen. Check anyway.
    //  if(cls == null) {
    //    assert (cls != null);
    //    return true;
    //  }
    //  if(parent.isAssignableFrom(cls)) {
    //    classes.add(cls);
    //  }
    //  else {
    //    logger.warning("Class " + line + " does not implement " + parent + " but listed in service file " + nextElement);
    //  }
    //}
    //catch(ClassNotFoundException e) {
    //  logger.warning("Class not found: " + line + "; listed in service file " + nextElement, e);
    //}
    return true;
  }

 
  public  Type Next() {
    Type ret = nextclass;
    nextclass = null;
    return ret;
  }



  public Type Current
  {
      get { throw new NotImplementedException(); }
  }

  public void Dispose()
  {
      throw new NotImplementedException();
  }

  object System.Collections.IEnumerator.Current
  {
      get { throw new NotImplementedException(); }
  }

  public bool MoveNext()
  {
      throw new NotImplementedException();
  }

  public void Reset()
  {
      throw new NotImplementedException();
  }
    }
}
