using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.KnnQueries;
using Socona.Expor.Databases.Queries.RangeQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Queries;
using Socona.Expor.Persistent;
using Socona.Expor.Utilities.Exceptions;
using Socona.Log;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Deliclu
{
    
public class DeLiCluTreeIndex<O > : DeLiCluTree , IKNNIndex, IRangeIndex
where O:INumberVector{
  /**
   * The relation we index
   */
  private IRelation relation;

  /**
   * Constructor.
   * 
   * @param relation Relation to index
   * @param pagefile Page file
   */
  public DeLiCluTreeIndex(IRelation relation, IPageFile<DeLiCluNode> pagefile) :
    base(pagefile){
    this.relation = relation;
    this.Initialize();
  }

  /**
   * The appropriate logger for this index.
   */
  private static  Logging logger = Logging.GetLogger(typeof(DeLiCluTreeIndex<O>));

  /**
   * Creates a new leaf entry representing the specified data object.
   * 
   * @param id Object id
   */
  protected DeLiCluLeafEntry CreateNewLeafEntry(IDbId id) {
      return new DeLiCluLeafEntry(id, (INumberVector)relation[(id)]);
  }

  /**
   * Marks the specified object as handled and returns the path of node ids from
   * the root to the objects's parent.
   * 
   * @param id the objects id to be marked as handled
   * @param obj the object to be marked as handled
   * @return the path of node ids from the root to the objects's parent
   */
  public  IList<TreeIndexPathComponent<IDeLiCluEntry>> SetHandled(IDbId id, O obj) {
      lock(this){
    if(logger.IsDebugging) {
      logger.Debug("setHandled " + id + ", " + obj + "\n");
    }
    }

    // find the leaf node containing o
    IndexTreePath<IDeLiCluEntry> pathToObject = FindPathToObject(GetRootPath(), obj, id);

    if(pathToObject == null) {
      throw new AbortException("Object not found in setHandled.");
    }

    // set o handled
    IDeLiCluEntry entry = pathToObject.GetLastPathComponent().GetEntry();
    entry.SetHasHandled(true);
    entry.SetHasUnhandled(false);

    for(IndexTreePath<IDeLiCluEntry> path = pathToObject; path.GetParentPath() != null; path = path.GetParentPath()) {
      IDeLiCluEntry parentEntry = path.GetParentPath().GetLastPathComponent().GetEntry();
      DeLiCluNode node = GetNode(parentEntry);
      bool hasHandled = false;
      bool hasUnhandled = false;
      for(int i = 0; i < node.GetNumEntries(); i++) {
         IDeLiCluEntry nodeEntry = node.GetEntry(i);
        hasHandled = hasHandled || nodeEntry.HasHandled();
        hasUnhandled = hasUnhandled || nodeEntry.HasUnhandled();
      }
      parentEntry.SetHasUnhandled(hasUnhandled);
      parentEntry.SetHasHandled(hasHandled);
    }

    return pathToObject.GetPath();
  }

  /**
   * Inserts the specified real vector object into this index.
   * 
   * @param id the object id that was inserted
   */
  
  public  void Insert(IDbId id) {
    InsertLeaf(CreateNewLeafEntry(id));
  }

  /**
   * Inserts the specified objects into this index. If a bulk load mode is
   * implemented, the objects are inserted in one bulk.
   * 
   * @param ids the objects to be inserted
   */
  
  public  void InsertAll(IDbIds ids) {
    if(ids.IsEmpty() || (ids.Count == 1)) {
      return;
    }

    // Make an example leaf
    if(CanBulkLoad()) {
      IList<IDeLiCluEntry> leafs = new List<IDeLiCluEntry>(ids.Count);
     // for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
        foreach(var iter in ids){
        leafs.Add(CreateNewLeafEntry(iter.DbId));
      }
      BulkLoad(leafs);
    }
    else {
      //for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
        foreach(var iter in ids){
        Insert(iter.DbId);
      }
    }

    DoExtraIntegrityChecks();
  }

  /**
   * Deletes the specified object from this index.
   * 
   * @return true if this index did contain the object with the specified id,
   *         false otherwise
   */
  
  public  bool Delete(IDbId id) {
    // find the leaf node containing o
    O obj =(O) relation[id];
    IndexTreePath<IDeLiCluEntry> deletionPath = FindPathToObject(GetRootPath(), obj, id);
    if(deletionPath == null) {
      return false;
    }
    DeletePath(deletionPath);
    return true;
  }

  
  public void DeleteAll(IDbIds ids) {
    //for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
      foreach(var iter in ids){
      Delete(iter.DbId);
    }
  }

  
  public   IRangeQuery GetRangeQuery(IDistanceQuery distanceQuery, params Object[] hints) {
    // Query on the relation we index
    if(distanceQuery.Relation != relation) {
      return null;
    }
    // Can we support this distance function - spatial distances only!
    if(!(distanceQuery is ISpatialDistanceQuery)) {
      return null;
    }
    ISpatialDistanceQuery dq = (ISpatialDistanceQuery) distanceQuery;
    return RStarTreeUtil.GetRangeQuery(this, dq, hints);
  }

  
  public  IKNNQuery GetKNNQuery(IDistanceQuery distanceQuery,params Object[] hints) {
    // Query on the relation we index
    if(distanceQuery.Relation != relation) {
      return null;
    }
    // Can we support this distance function - spatial distances only!
    if(!(distanceQuery is ISpatialDistanceQuery)) {
      return null;
    }
    ISpatialDistanceQuery dq = (ISpatialDistanceQuery) distanceQuery;
    return RStarTreeUtil.GetKNNQuery(this, dq, hints);
  }

  
  public String LongName {
      get { return "DeLiClu-Tree"; }
  }

  
  public String ShortName {
      get { return "deliclutree"; }
  }

  
  protected override Logging GetLogger() {
    return logger;
  }
}
}
