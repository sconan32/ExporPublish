using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Utilities.DataStructures.ArrayLike;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Insert
{
   
/**
 * Use two different insertion strategies for directory and leaf nodes.
 * 
 * Using two different strategies was likely first suggested in:
 * <p>
 * N. Beckmann, H.-P. Kriegel, R. Schneider, B. Seeger:<br />
 * The R*-tree: an efficient and robust access method for points and rectangles<br />
 * in: Proceedings of the 1990 ACM SIGMOD International Conference on Management
 * of Data, Atlantic City, NJ, May 23-25, 1990
 * </p>
 * 
 * @author Erich Schubert
 */
[Reference(Authors = "N. Beckmann, H.-P. Kriegel, R. Schneider, B. Seeger", 
    Title = "The R*-tree: an efficient and robust access method for points and rectangles", 
    BookTitle = "Proceedings of the 1990 ACM SIGMOD International Conference on Management of Data, Atlantic City, NJ, May 23-25, 1990", 
    Url = "http://dx.doi.org/10.1145/93597.98741")]
public class CombinedInsertionStrategy :IInsertionStrategy {
  /**
   * Strategy when inserting into directory nodes
   */
  IInsertionStrategy dirStrategy;

  /**
   * Strategy when inserting into leaf nodes.
   */
  IInsertionStrategy leafStrategy;

  /**
   * Constructor.
   * 
   * @param dirStrategy Strategy for directory nodes
   * @param leafStrategy Strategy for leaf nodes
   */
  public CombinedInsertionStrategy(IInsertionStrategy dirStrategy, IInsertionStrategy leafStrategy) {

    this.dirStrategy = dirStrategy;
    this.leafStrategy = leafStrategy;
  }


  public int Choose(IEnumerable<ISpatialEntry> options, IArrayAdapter getter, ISpatialComparable obj, int height, int depth) {
    if(depth + 1 >= height) {
      return leafStrategy.Choose(options, getter, obj, height, depth);
    }
    else {
      return dirStrategy.Choose(options, getter, obj, height, depth);
    }
  }

  /**
   * Parameterization class.
   * 
   * @author Erich Schubert
   * 
   * @apiviz.exclude
   */
  public  class Parameterizer : AbstractParameterizer {
    /**
     * Insertion strategy for directory nodes.
     */
    public static  OptionDescription DIR_STRATEGY_ID = 
        OptionDescription.GetOrCreate("rtree.insert-directory", "Insertion strategy for directory nodes.");

    /**
     * Insertion strategy for leaf nodes.
     */
    public static  OptionDescription LEAF_STRATEGY_ID = 
        OptionDescription.GetOrCreate("rtree.insert-leaf", "Insertion strategy for leaf nodes.");

    /**
     * Strategy when inserting into directory nodes
     */
    IInsertionStrategy dirStrategy;

    /**
     * Strategy when inserting into leaf nodes.
     */
    IInsertionStrategy leafStrategy;

    
    protected override void MakeOptions(IParameterization config) {
      base.MakeOptions(config);
      ClassParameter dirP = 
          new ClassParameter(DIR_STRATEGY_ID, 
              typeof(IInsertionStrategy),typeof( LeastEnlargementWithAreaInsertionStrategy));
      if(config.Grab(dirP)) {
        dirStrategy = dirP.InstantiateClass<IInsertionStrategy>(config);
      }

      ClassParameter leafP = 
          new ClassParameter(LEAF_STRATEGY_ID,
              typeof(IInsertionStrategy),typeof(LeastOverlapInsertionStrategy));
      if(config.Grab(leafP)) {
        leafStrategy = leafP.InstantiateClass<IInsertionStrategy>(config);
      }
    }

    
    protected override object MakeInstance() {
      return new CombinedInsertionStrategy(dirStrategy, leafStrategy);
    }
  }
}
}
