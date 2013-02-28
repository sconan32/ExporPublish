using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Bulk;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Insert;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Overflow;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Split;
using Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Utilities;
using Socona.Expor.Persistent;
using Socona.Expor.Utilities.Exceptions;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants
{
    
public abstract class AbstractRStarTree<N , E>: SpatialIndexTree<N, E>
where N : AbstractRStarTreeNode<N, E>
    where E :ISpatialEntry
{
  /**
   * Development flag: This will enable some extra integrity checks on the tree.
   */
  protected  static bool extraIntegrityChecks = false;

  /**
   * The height of this R*-Tree.
   */
  protected int height;

  /**
   * For counting the number of distance computations.
   */
  public int distanceCalcs = 0;

  /**
   * The last inserted entry
   */
  E lastInsertedEntry = default(E);

  /**
   * The strategy for bulk load.
   */
  protected IBulkSplit bulkSplitter;

  /**
   * The split strategy
   */
  protected ISplitStrategy nodeSplitter = TopologicalSplitter.STATIC;

  /**
   * The insertion strategy to use
   */
  protected IInsertionStrategy insertionStrategy = LeastOverlapInsertionStrategy.STATIC;

  /**
   * Overflow treatment
   */
  protected IOverflowTreatment overflowTreatment = LimitedReinsertOverflowTreatment.RSTAR_OVERFLOW;

  /**
   * Relative minimum fill
   */
  protected double relativeMinFill = 0.4;

  /**
   * Constructor
   * 
   * @param pagefile Page file
   */
  public AbstractRStarTree(IPageFile<N> pagefile) :
    base(pagefile){
  }

  /**
   * Set the bulk loading strategy
   * 
   * @param bulkSplitter Bulk loading strategy
   */
  public void SetBulkStrategy(IBulkSplit bulkSplitter) {
    this.bulkSplitter = bulkSplitter;
  }

  /**
   * Set the node splitting strategy.
   * 
   * @param nodeSplitter the split strategy to set
   */
  public void SetNodeSplitStrategy(ISplitStrategy nodeSplitter) {
    if(nodeSplitter != null) {
      this.nodeSplitter = nodeSplitter;
    }
    else {
      GetLogger().Warning("Ignoring setNodeSplitStrategy(null)");
    }
  }

  /**
   * Set insertion strategy
   * 
   * @param insertionStrategy the insertion strategy to set
   */
  public void SetInsertionStrategy(IInsertionStrategy insertionStrategy) {
    if(insertionStrategy != null) {
      this.insertionStrategy = insertionStrategy;
    }
    else {
      GetLogger().Warning("Ignoring setInsertionStrategy(null)");
    }
  }

  /**
   * Set the overflow treatment strategy.
   * 
   * @param overflowTreatment overflow treatment strategy
   */
  public void SetOverflowTreatment(IOverflowTreatment overflowTreatment) {
    this.overflowTreatment = overflowTreatment;
  }

  /**
   * Set the relative minimum fill. (Only supported before the tree was used!)
   * 
   * @param relative Relative minimum fill
   */
  public void SetMinimumFill(double relative) {
    this.relativeMinFill = relative;
  }

  /**
   * Returns the path to the leaf entry in the specified subtree that represents
   * the data object with the specified mbr and id.
   * 
   * @param subtree the subtree to be tested
   * @param mbr the mbr to look for
   * @param id the id to look for
   * @return the path to the leaf entry of the specified subtree that represents
   *         the data object with the specified mbr and id
   */
  protected IndexTreePath<E> FindPathToObject(IndexTreePath<E> subtree, ISpatialComparable mbr, IDbIdRef id) {
    N node = GetNode(subtree.GetLastPathComponent().GetEntry());
    if(node.IsLeaf()) {
      for(int i = 0; i < node.GetNumEntries(); i++) {
        if(((ILeafEntry) node.GetEntry(i)).GetDbId().IsSameDbId(id)) {
          return subtree.PathByAddingChild(new TreeIndexPathComponent<E>(node.GetEntry(i), i));
        }
      }
    }
    // directory node
    else {
      for(int i = 0; i < node.GetNumEntries(); i++) {
        if(SpatialUtil.Intersects(node.GetEntry(i), mbr)) {
          IndexTreePath<E> childSubtree = subtree.PathByAddingChild(new TreeIndexPathComponent<E>(node.GetEntry(i), i));
          IndexTreePath<E> path = FindPathToObject(childSubtree, mbr, id);
          if(path != null) {
            return path;
          }
        }
      }
    }
    return null;
  }

  
  public override void InsertLeaf(E leaf) {
    if(!initialized) {
      Initialize(leaf);
    }
    overflowTreatment.Reinitialize();

    PreInsert(leaf);
    InsertLeafEntry(leaf);

    DoExtraIntegrityChecks();
  }

  /**
   * Inserts the specified leaf entry into this R*-Tree.
   * 
   * @param entry the leaf entry to be inserted
   */
  protected void InsertLeafEntry(E entry) {
    lastInsertedEntry = entry;
    // choose subtree for insertion
    IndexTreePath<E> subtree = ChoosePath(GetRootPath(), entry, 1);

    if(GetLogger().IsDebugging) {
      GetLogger().Debug("insertion-subtree " + subtree);
    }

    N parent = GetNode(subtree.GetLastPathComponent().GetEntry());
    parent.AddLeafEntry(entry);
    WriteNode(parent);

    // adjust the tree from subtree to root
    AdjustTree(subtree);
  }

  /**
   * Inserts the specified directory entry at the specified level into this
   * R*-Tree.
   * 
   * @param entry the directory entry to be inserted
   * @param level the level at which the directory entry is to be inserted
   */
  protected void InsertDirectoryEntry(E entry, int level) {
    lastInsertedEntry = entry;
    // choose node for insertion of o
    IndexTreePath<E> subtree = ChoosePath(GetRootPath(), entry, level);
    if(GetLogger().IsDebugging) {
      GetLogger().Debug("subtree " + subtree);
    }

    N parent = GetNode(subtree.GetLastPathComponent().GetEntry());
    parent.AddDirectoryEntry(entry);
    WriteNode(parent);

    // adjust the tree from subtree to root
    AdjustTree(subtree);
  }

  /**
   * Delete a leaf at a given path - deletions for non-leaves are not supported!
   * 
   * @param deletionPath Path to delete
   */
  protected void DeletePath(IndexTreePath<E> deletionPath) {
    N leaf = GetNode(deletionPath.GetParentPath().GetLastPathComponent().GetEntry());
    int index = deletionPath.GetLastPathComponent().GetIndex();

    // delete o
    E entry = leaf.GetEntry(index);
    leaf.DeleteEntry(index);
    WriteNode(leaf);

    // condense the tree
    Stack<N> stack = new Stack<N>();
    CondenseTree(deletionPath.GetParentPath(), stack);

    // reinsert underflow nodes
    while(stack.Count>0) {
      N node = stack.Pop();
      if(node.IsLeaf()) {
        for(int i = 0; i < node.GetNumEntries(); i++) {
          overflowTreatment.Reinitialize(); // Intended?
          this.InsertLeafEntry(node.GetEntry(i));
        }
      }
      else {
        for(int i = 0; i < node.GetNumEntries(); i++) {
          stack.Push(GetNode(node.GetEntry(i)));
        }
      }
      DeleteNode(node);
    }
    PostDelete(entry);

    DoExtraIntegrityChecks();
  }

  /**
   * Initializes this R*-Tree from an existing persistent file.
   */
  
  public override void InitializeFromFile(TreeIndexHeader header, IPageFile<N> file) {
    base.InitializeFromFile(header, file);
    // compute height
    this.height = ComputeHeight();

    if(GetLogger().IsDebugging) {
      StringBuilder msg = new StringBuilder();
      msg.Append(GetType());
      msg.Append("\n height = ").Append(height);
      GetLogger().Debug(msg.ToString());
    }
  }

  
  protected override void InitializeCapacities(E exampleLeaf) {
    /* Simulate the creation of a leaf page to Get the page capacity */
    try {
      int cap = 0;
      //ByteArrayOutputStream baos = new ByteArrayOutputStream();
      //ObjectOutputStream oos = new ObjectOutputStream(baos);
      MemoryStream oos = new MemoryStream();
      BinaryFormatter bf = new BinaryFormatter();
      SpatialPointLeafEntry sl = new SpatialPointLeafEntry(
          DbIdUtil.ImportInt32(0), new double[exampleLeaf.Count]);
      while(oos.Length<= GetPageSize()) {
          bf.Serialize(oos, sl);
       
        cap++;
      }
      // the last one caused the page to overflow.
      leafCapacity = cap - 1;
    }
    catch(IOException e) {
      throw new AbortException("Error determining page sizes.", e);
    }

    /* Simulate the creation of a directory page to Get the capacity */
    try {
      int cap = 0;
      //ByteArrayOutputStream baos = new ByteArrayOutputStream();
      //ObjectOutputStream oos = new ObjectOutputStream(baos);
      MemoryStream oos = new MemoryStream();
      BinaryFormatter bf = new BinaryFormatter();
      ModifiableHyperBoundingBox hb = new ModifiableHyperBoundingBox(
          new double[exampleLeaf.Count], new double[exampleLeaf.Count]);
      SpatialDirectoryEntry sl = new SpatialDirectoryEntry(0, hb);
      while(oos.Length<= GetPageSize()) {
          bf.Serialize(oos,sl);
       
        cap++;
      }
      dirCapacity = cap - 1;
    }
    catch(IOException e) {
      throw new AbortException("Error determining page sizes.", e);
    }

    if(dirCapacity <= 1) {
      throw new ArgumentException("Node size of " + GetPageSize() + " Bytes is chosen too small!");
    }

    if(dirCapacity < 10) {
      GetLogger().Warning("Page size is choosen very small! Maximum number of entries " + "in a directory node = " + (dirCapacity - 1));
    }

    // minimum entries per directory node
    dirMinimum = (int) Math.Round((dirCapacity - 1) * relativeMinFill);
    if(dirMinimum < 2) {
      dirMinimum = 2;
    }

    if(leafCapacity <= 1) {
      throw new ArgumentException("Node size of " + GetPageSize() + " Bytes is chosen too small!");
    }

    if(leafCapacity < 10) {
      GetLogger().Warning("Page size is choosen very small! Maximum number of entries " + "in a leaf node = " + (leafCapacity - 1));
    }

    // minimum entries per leaf node
    leafMinimum = (int) Math.Round((leafCapacity - 1) * relativeMinFill);
    if(leafMinimum < 2) {
      leafMinimum = 2;
    }

    if(GetLogger().IsVerbose) {
      GetLogger().Verbose("Directory Capacity:  " + (dirCapacity - 1) + "\nDirectory minimum: " + dirMinimum + "\nLeaf Capacity:     " + (leafCapacity - 1) + "\nLeaf Minimum:      " + leafMinimum);
    }
  }

  /**
   * Test whether a bulk insert is still possible.
   * 
   * @return Success code
   */
  public bool CanBulkLoad() {
    return (bulkSplitter != null && !initialized);
  }

  /**
   * Creates and returns the leaf nodes for bulk load.
   * 
   * @param objects the objects to be inserted
   * @return the array of leaf nodes containing the objects
   */
  protected List<E> CreateBulkLeafNodes(IList<E> objects) {
    int minEntries = leafMinimum;
    int maxEntries = leafCapacity - 1;

    List<E> result = new List<E>();
    IList<IList<E>> partitions = bulkSplitter.Partition(objects, minEntries, maxEntries);

    foreach(List<E> partition in partitions) {
      // create leaf node
      N leafNode = CreateNewLeafNode();

      // insert data
      foreach(E o in partition) {
        leafNode.AddLeafEntry(o);
      }
      // write to file
      WriteNode(leafNode);

      result.Add(CreateNewDirectoryEntry(leafNode));

      if(GetLogger().IsDebugging) {
        GetLogger().Debug("Created leaf page "+leafNode.GetPageID());
      }
    }

    if(GetLogger().IsDebugging) {
      GetLogger().Debug("numDataPages = " + result.Count);
    }
    return result;
  }

  /**
   * Performs a bulk load on this RTree with the specified data. Is called by
   * the constructor.
   */
  abstract protected void BulkLoad(IList<E> entrys);

  /**
   * Returns the height of this R*-Tree.
   * 
   * @return the height of this R*-Tree
   */
  public  int GetHeight() {
    return height;
  }

  /**
   * Sets the height of this R*-Tree.
   * 
   * @param height the height to be set
   */
  protected void SetHeight(int height) {
    this.height = height;
  }

  /**
   * Computes the height of this RTree. Is called by the constructor.
   * 
   * @return the height of this RTree
   */
  abstract protected int ComputeHeight();

  /**
   * Returns true if in the specified node an overflow occurred, false
   * otherwise.
   * 
   * @param node the node to be tested for overflow
   * @return true if in the specified node an overflow occurred, false otherwise
   */
  abstract protected bool HasOverflow(N node);

  /**
   * Returns true if in the specified node an underflow occurred, false
   * otherwise.
   * 
   * @param node the node to be tested for underflow
   * @return true if in the specified node an underflow occurred, false
   *         otherwise
   */
  abstract protected bool HasUnderflow(N node);

  /**
   * Creates a new directory entry representing the specified node.
   * 
   * @param node the node to be represented by the new entry
   * @return the newly created directory entry
   */
  abstract protected E CreateNewDirectoryEntry(N node);

  /**
   * Creates a new root node that points to the two specified child nodes and
   * return the path to the new root.
   * 
   * @param oldRoot the old root of this RTree
   * @param newNode the new split node
   * @return the path to the new root node that points to the two specified
   *         child nodes
   */
  protected IndexTreePath<E> CreateNewRoot( N oldRoot,  N newNode) {
    N root = CreateNewDirectoryNode();
    WriteNode(root);

    // switch the ids
    oldRoot.SetPageID(root.GetPageID());
    if(!oldRoot.IsLeaf()) {
      for(int i = 0; i < oldRoot.GetNumEntries(); i++) {
        N node = GetNode(oldRoot.GetEntry(i));
        WriteNode(node);
      }
    }

    root.SetPageID(GetRootID());
    E oldRootEntry = CreateNewDirectoryEntry(oldRoot);
    E newNodeEntry = CreateNewDirectoryEntry(newNode);
    root.AddDirectoryEntry(oldRootEntry);
    root.AddDirectoryEntry(newNodeEntry);

    WriteNode(root);
    WriteNode(oldRoot);
    WriteNode(newNode);
    if(GetLogger().IsDebugging) {
      String msg = "Create new Root: ID=" + root.GetPageID();
      msg += "\nchild1 " + oldRoot + " " + new HyperBoundingBox(oldRootEntry);
      msg += "\nchild2 " + newNode + " " + new HyperBoundingBox(newNodeEntry);
      msg += "\n";
      GetLogger().Debug(msg);
    }

    return new IndexTreePath<E>(new TreeIndexPathComponent<E>(GetRootEntry(), 0));
  }

  /**
   * Test on whether or not any child of <code>node</code> contains
   * <code>mbr</code>. If there are several containing children, the child with
   * the minimum volume is chosen in order to Get compact pages.
   * 
   * @param node subtree
   * @param mbr MBR to test for
   * @return the child of <code>node</code> containing <code>mbr</code> with the
   *         minimum volume or <code>null</code> if none exists
   */
  protected TreeIndexPathComponent<E> ContainedTest(N node, ISpatialComparable mbr) {
    E containingEntry = default(E);
    int index = -1;
    double cEVol = Double.NaN;
    E ei;
    for(int i = 0; i < node.GetNumEntries(); i++) {
      ei = node.GetEntry(i);
      // skip test on pairwise overlaps
      if(SpatialUtil.Contains(ei, mbr)) {
        if(containingEntry == null) {
          containingEntry = ei;
          index = i;
        }
        else {
          double tempVol = SpatialUtil.Volume(ei);
          if(Double.IsNaN(cEVol)) { // calculate volume of currently best
            cEVol = SpatialUtil.Volume(containingEntry);
          }
          // take containing node with lowest volume
          if(tempVol < cEVol) {
            cEVol = tempVol;
            containingEntry = ei;
            index = i;
          }
        }
      }
    }
    return (containingEntry == null ? null : new TreeIndexPathComponent<E>(containingEntry, index));
  }

  /**
   * Chooses the best path of the specified subtree for insertion of the given
   * mbr at the specified level.
   * 
   * @param subtree the subtree to be tested for insertion
   * @param mbr the mbr to be inserted
   * @param level the level at which the mbr should be inserted (level 1
   *        indicates leaf-level)
   * @return the path of the appropriate subtree to insert the given mbr
   */
  protected IndexTreePath<E> ChoosePath(IndexTreePath<E> subtree, ISpatialComparable mbr, int level) {
    if(GetLogger().IsDebugging) {
      GetLogger().Debug("node " + subtree + ", level " + level);
    }

    N node = GetNode(subtree.GetLastPathComponent().GetEntry());
    if(node == null) {
      throw new ApplicationException("Page file did not return node for node id: " + GetPageID(subtree.GetLastPathComponent().GetEntry()));
    }
    if(node.IsLeaf()) {
      return subtree;
    }
    // first test on containment
    TreeIndexPathComponent<E> containingEntry = ContainedTest(node, mbr);
    if(containingEntry != null) {
      IndexTreePath<E> newSubtree = subtree.PathByAddingChild(containingEntry);
      if(height - subtree.GetPathCount() == level) {
        return newSubtree;
      }
      else {
        return ChoosePath(newSubtree, mbr, level);
      }
    }

    N childNode = GetNode(node.GetEntry(0));
    int num = insertionStrategy.Choose(node as IEnumerable<ISpatialEntry>, NodeArrayAdapter.STATIC, mbr, height, subtree.GetPathCount());
    TreeIndexPathComponent<E> comp = new TreeIndexPathComponent<E>(node.GetEntry(num), num);
    // children are leafs
    if(childNode.IsLeaf()) {
      if(height - subtree.GetPathCount() == level) {
        return subtree.PathByAddingChild(comp);
      }
      else {
        throw new ArgumentException("childNode is leaf, but currentLevel != level: " + (height - subtree.GetPathCount()) + " != " + level);
      }
    }
    // children are directory nodes
    else {
      IndexTreePath<E> newSubtree = subtree.PathByAddingChild(comp);
      // desired level is reached
      if(height - subtree.GetPathCount() == level) {
        return newSubtree;
      }
      else {
        return ChoosePath(newSubtree, mbr, level);
      }
    }
  }

  /**
   * Treatment of overflow in the specified node: if the node is not the root
   * node and this is the first call of overflowTreatment in the given level
   * during insertion the specified node will be reinserted, otherwise the node
   * will be split.
   * 
   * @param node the node where an overflow occurred
   * @param path the path to the specified node
   * @return the newly created split node in case of split, null in case of
   *         reinsertion
   */
  private N OverflowTreatment(N node, IndexTreePath<E> path) {
    if(overflowTreatment.HandleOverflow(this, node, path)) {
      return null;
    }
    return Split(node);
  }

  /**
   * Splits the specified node and returns the newly created split node.
   * 
   * @param node the node to be split
   * @return the newly created split node
   */
  private N Split(N node) {
    // choose the split dimension and the split point
    int minimum = node.IsLeaf() ? leafMinimum : dirMinimum;
    BitArray split = nodeSplitter.Split<ISpatialEntry>(node as IEnumerable<ISpatialEntry>, NodeArrayAdapter.STATIC, minimum);

    // New node
     N newNode;
    if(node.IsLeaf()) {
      newNode = CreateNewLeafNode();
    }
    else {
      newNode = CreateNewDirectoryNode();
    }
    // do the split
    node.SplitByMask(newNode, split);

    // write changes to file
    WriteNode(node);
    WriteNode(newNode);

    return newNode;
  }

  /**
   * Reinserts the specified node at the specified level.
   * 
   * @param node the node to be reinserted
   * @param path the path to the node
   * @param offs the nodes indexes to reinsert
   */
  public void ReInsert(N node, IndexTreePath<E> path, int[] offs) {
     int level = height - (path.GetPathCount() - 1);

    BitArray remove = new BitArray(1000);
    List<E> reInsertEntries = new List<E>(offs.Length);
    for(int i = 0; i < offs.Length; i++) {
      reInsertEntries.Add(node.GetEntry(offs[i]));
      remove.Set(offs[i],true);
    }
    // Remove the entries we reinsert
    node.RemoveMask(remove);
    WriteNode(node);

    // and adapt the mbrs
    IndexTreePath<E> childPath = path;
    N child = node;
    while(childPath.GetParentPath() != null) {
      N parent = GetNode(childPath.GetParentPath().GetLastPathComponent().GetEntry());
      int indexOfChild = childPath.GetLastPathComponent().GetIndex();
      if(child.AdjustEntry(parent.GetEntry(indexOfChild))) {
        WriteNode(parent);
        childPath = childPath.GetParentPath();
        child = parent;
      }
      else {
        break;
        // TODO: stop writing when MBR didn't change!
      }
    }

    // reinsert the first entries
    foreach(E entry in reInsertEntries) {
      if(node.IsLeaf()) {
        if(GetLogger().IsDebugging) {
          GetLogger().Debug("reinsert " + entry);
        }
        InsertLeafEntry(entry);
      }
      else {
        if(GetLogger().IsDebugging) {
          GetLogger().Debug("reinsert " + entry + " at " + level);
        }
        InsertDirectoryEntry(entry, level);
      }
    }
  }

  /**
   * Adjusts the tree after insertion of some nodes.
   * 
   * @param subtree the subtree to be adjusted
   */
  protected void AdjustTree(IndexTreePath<E> subtree) {
    if(GetLogger().IsDebugging) {
      GetLogger().Debug("Adjust tree " + subtree);
    }

    // Get the root of the subtree
    N node = GetNode(subtree.GetLastPathComponent().GetEntry());

    // overflow in node
    if(HasOverflow(node)) {
      // treatment of overflow: reinsertion or split
      N split = OverflowTreatment(node, subtree);

      // node was split
      if(split != null) {
        // if root was split: create a new root that points the two
        // split nodes
        if(IsRoot(node)) {
          IndexTreePath<E> newRootPath = CreateNewRoot(node, split);
          height++;
          AdjustTree(newRootPath);
        }
        // node is not root
        else {
          // Get the parent and Add the new split node
          N parent = GetNode(subtree.GetParentPath().GetLastPathComponent().GetEntry());
          if(GetLogger().IsDebugging) {
            GetLogger().Debug("parent " + parent);
          }
          parent.AddDirectoryEntry(CreateNewDirectoryEntry(split));

          // adjust the entry representing the (old) node, that has
          // been split

          // This does not work in the persistent version
          // node.adjustEntry(subtree.GetLastPathComponent().GetEntry());
          node.AdjustEntry(parent.GetEntry(subtree.GetLastPathComponent().GetIndex()));

          // write changes in parent to file
          WriteNode(parent);
          AdjustTree(subtree.GetParentPath());
        }
      }
    }
    // no overflow, only adjust parameters of the entry representing the
    // node
    else {
      if(!IsRoot(node)) {
        N parent = GetNode(subtree.GetParentPath().GetLastPathComponent().GetEntry());
        E entry = parent.GetEntry(subtree.GetLastPathComponent().GetIndex());
        bool changed = node.AdjustEntryIncremental(entry, lastInsertedEntry);
        if(changed) {
          // node.adjustEntry(parent.GetEntry(index));
          // write changes in parent to file
          WriteNode(parent);
          AdjustTree(subtree.GetParentPath());
        }
      }
      // root level is reached
      else {
        node.AdjustEntry(GetRootEntry());
      }
    }
  }

  /**
   * Condenses the tree after deletion of some nodes.
   * 
   * @param subtree the subtree to be condensed
   * @param stack the stack holding the nodes to be reinserted after the tree
   *        has been condensed
   */
  private void CondenseTree(IndexTreePath<E> subtree, Stack<N> stack) {
    N node = GetNode(subtree.GetLastPathComponent().GetEntry());
    // node is not root
    if(!IsRoot(node)) {
      N parent = GetNode(subtree.GetParentPath().GetLastPathComponent().GetEntry());
      int index = subtree.GetLastPathComponent().GetIndex();
      if(HasUnderflow(node)) {
        if(parent.DeleteEntry(index)) {
          stack.Push(node);
        }
        else {
          node.AdjustEntry(parent.GetEntry(index));
        }
      }
      else {
        node.AdjustEntry(parent.GetEntry(index));
      }
      WriteNode(parent);
      // Get subtree to parent
      CondenseTree(subtree.GetParentPath(), stack);
    }

    // node is root
    else {
      if(HasUnderflow(node) & node.GetNumEntries() == 1 && !node.IsLeaf()) {
        N child = GetNode(node.GetEntry(0));
        N newRoot;
        if(child.IsLeaf()) {
          newRoot = CreateNewLeafNode();
          newRoot.SetPageID(GetRootID());
          for(int i = 0; i < child.GetNumEntries(); i++) {
            newRoot.AddLeafEntry(child.GetEntry(i));
          }
        }
        else {
          newRoot = CreateNewDirectoryNode();
          newRoot.SetPageID(GetRootID());
          for(int i = 0; i < child.GetNumEntries(); i++) {
            newRoot.AddDirectoryEntry(child.GetEntry(i));
          }
        }
        WriteNode(newRoot);
        height--;
      }
    }
  }

  
  public override  IList<E> GetLeaves() {
    IList<E> result = new List<E>();

    if(height == 1) {
      result.Add(GetRootEntry());
      return result;
    }

    GetLeafNodes(GetRoot(), result, height);
    return result;
  }

  /**
   * Determines the entries pointing to the leaf nodes of the specified subtree
   * 
   * @param node the subtree
   * @param result the result to store the ids in
   * @param currentLevel the level of the node in the R-Tree
   */
  private void GetLeafNodes(N node, IList<E> result, int currentLevel) {
    // Level 1 are the leaf nodes, Level 2 is the one atop!
    if(currentLevel == 2) {
      for(int i = 0; i < node.GetNumEntries(); i++) {
        result.Add(node.GetEntry(i));
      }
    }
    else {
      for(int i = 0; i < node.GetNumEntries(); i++) {
        N child = GetNode(node.GetEntry(i));
        GetLeafNodes(child, result, (currentLevel - 1));
      }
    }
  }

  /**
   * Perform Additional integrity checks.
   */
  public void DoExtraIntegrityChecks() {
    if(extraIntegrityChecks) {
      GetRoot().integrityCheck(this);
    }
  }

  /**
   * Returns a string representation of this R*-Tree.
   * 
   * @return a string representation of this R*-Tree
   */
  
  public override String ToString() {
    StringBuilder result = new StringBuilder();
    int dirNodes = 0;
    int leafNodes = 0;
    int objects = 0;
    int levels = 0;

    if(initialized) {
      N node = GetRoot();
      int dim = GetRootEntry().Count;

      while(!node.IsLeaf()) {
        if(node.GetNumEntries() > 0) {
          E entry = node.GetEntry(0);
          node = GetNode(entry);
          levels++;
        }
      }

      BreadthFirstEnumeration<N, E> enumeration = new BreadthFirstEnumeration<N, E>(this, GetRootPath());
      while(enumeration.MoveNext()) {
        IndexTreePath<E> indexPath = enumeration.Current;
        E entry = indexPath.GetLastPathComponent().GetEntry();
        if(entry.IsLeafEntry()) {
          objects++;
        }
        else {
          node = GetNode(entry);
          if(node.IsLeaf()) {
            leafNodes++;
          }
          else {
            dirNodes++;
          }
        }
      }
      result.Append(GetType().Name).Append(" has ").Append((levels + 1)).Append(" levels.\n");
      result.Append(dirNodes).Append(" Directory Knoten (max = ").Append(dirCapacity - 1).Append(", min = ").Append(dirMinimum).Append(")\n");
      result.Append(leafNodes).Append(" Daten Knoten (max = ").Append(leafCapacity - 1).Append(", min = ").Append(leafMinimum).Append(")\n");
      result.Append(objects).Append(" ").Append(dim).Append("-dim. Punkte im Baum \n");
      PageFileUtil.AppendPageFileStatistics(result, GetPageFileStatistics());
    }
    else {
      result.Append(GetType().Name).Append(" is empty!\n");
    }

    return result.ToString();
  }
}
}
