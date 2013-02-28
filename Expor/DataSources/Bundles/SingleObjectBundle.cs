using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Types;

namespace Socona.Expor.DataSources.Bundles
{
    
public class SingleObjectBundle :IObjectBundle {
  /**
   * Store the meta data.
   */
  private BundleMeta meta;

  /**
   * Storing the real contents.
   */
  private List<Object> contents;

  /**
   * Constructor.
   */
  public SingleObjectBundle() :
    this(new BundleMeta(), new List<Object>(5)){
  }

  /**
   * Constructor.
   * 
   * @param meta Metadata
   * @param contents Object values
   */
  public SingleObjectBundle(BundleMeta meta, List<Object> contents) :base(){
   
    this.meta = meta;
    this.contents = contents;
    Debug.Assert (meta.Count== contents.Count);
  }


  public  BundleMeta Meta() {
    return meta;
  }


  public  SimpleTypeInformation Meta(int i) {
    return meta[i] as SimpleTypeInformation;
  }

 
  public  int MetaLength() {
    return meta.Count;
  }

  /**
   * Get the value of the ith component.
   * 
   * @param rnum representation number
   * @return value
   */
  public Object data(int rnum) {
    return contents[rnum];
  }

 
  public  int DataLength() {
    return 1;
  }

 
  public  Object data(int onum, int rnum) {
    if(onum != 0) {
      throw new IndexOutOfRangeException();
    }
    return contents[rnum];
  }

  /**
   * Append a single representation to the object.
   * 
   * @param meta Meta for the representation
   * @param data Data to append
   */
  public void Append(SimpleTypeInformation meta, Object data) {
    this.meta.Add(meta as SimpleTypeInformation);
    this.contents.Add(data);
  }

 

  public object Data(int onum, int rnum)
  {
      throw new NotImplementedException();
  }
}
}
