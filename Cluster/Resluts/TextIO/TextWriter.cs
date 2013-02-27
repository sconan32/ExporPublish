using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Utilities;

namespace Socona.Clustering.Resluts.TextIO
{
    public class TextWriter {
  /**
   * Extension for txt-files.
   */
  public static const String FILE_EXTENSION = ".txt";

  /**
   * Hash map for supported classes in writer.
   */
  public  static HandlerList<ITextWriter> writers = new HandlerList<ITextWriter>();

  /**
   * Add some default handlers
   */
  static TextWriter(){
    TextWriterObjectInline trivialwriter = new TextWriterObjectInline();
    writers.InsertHandler(typeof(Object), new TextWriterObjectComment());
    writers.InsertHandler(typeof(KeyValuePair<object,object>), new TextWriterPair());
    /*
    
    writers.InsertHandler(typeof(DoubleDoublePair), new TextWriterDoubleDoublePair());
    writers.insertHandler(typeof(Triple), new TextWriterTriple());
    writers.insertHandler(typeof(FeatureVector), trivialwriter);
    // these object can be serialized inline with toString()
    writers.insertHandler(typeof(String), trivialwriter);
    writers.insertHandler(typeof(double), trivialwriter);
    writers.insertHandler(typeof(int), trivialwriter);
    writers.insertHandler(typeof(String[]), new TextWriterObjectArray<String>());
    writers.insertHandler(typeof(Double[]), new TextWriterObjectArray<Double>());
    writers.insertHandler(typeof(int[]), new TextWriterObjectArray<Integer>());
    writers.insertHandler(typeof(BitSet), trivialwriter);
    writers.insertHandler(typeof(Array), new TextWriterVector());
    writers.insertHandler(typeof(Distance), trivialwriter);
    writers.insertHandler(typeof(SimpleClassLabel), trivialwriter);
    writers.insertHandler(typeof(HierarchicalClassLabel), trivialwriter);
    writers.insertHandler(typeof(LabelList), trivialwriter);
    writers.insertHandler(typeof(DBID), trivialwriter);
    // Objects that have an own writeToText method.
    writers.insertHandler(TextWriteable.class, new TextWriterTextWriteable());
     * */
  }

  /**
   * For producing unique filenames.
   */
  protected Dictionary<String, Object> filenames = new Dictionary<String, Object>();

  /**
   * Try to find a unique file name.
   * 
   * @param result Result we print
   * @param filenamepre File name prefix to use
   * @return unique filename
   */
  protected String GetFilename(Object result, String filenamepre) {
    if(filenamepre == null || filenamepre.Length == 0) {
      filenamepre = "result";
    }
    int i = 0;
    while(true) {
      String filename;
      if(i > 0) {
        filename = filenamepre + "-" + i;
      }
      else {
        filename = filenamepre;
      }
      Object existing = filenames[filename];
      if(existing == null || existing == result) {
        filenames[filename]= result;
        return filename;
      }
      i++;
    }
  }

  /**
   * Writes a header providing information concerning the underlying database
   * and the specified parameter-settings.
   * 
   * @param out the print stream where to write
   * @param sr the settings to be written into the header
   */
  protected void PrintSettings(TextWriterStream sout, List<SettingsResult> sr) {
    out.commentPrintSeparator();
    out.commentPrintLn("Settings:");

    if(sr != null) {
      for(SettingsResult settings : sr) {
        Object last = null;
        for(Pair<Object, Parameter<?, ?>> setting : settings.getSettings()) {
          if(setting.first != last && setting.first != null) {
            if(last != null) {
              out.commentPrintLn("");
            }
            String name;
            try {
              if(setting.first instanceof Class) {
                name = ((Class<?>) setting.first).getName();
              }
              else {
                name = setting.first.getClass().getName();
              }
              if(ClassParameter.class.isInstance(setting.first)) {
                name = ((ClassParameter<?>) setting.first).getValue().getName();
              }
            }
            catch(NullPointerException e) {
              name = "[null]";
            }
            out.commentPrintLn(name);
            last = setting.first;
          }
          String name = setting.second.getOptionID().getName();
          String value = "[unset]";
          try {
            if(setting.second.isDefined()) {
              value = setting.second.getValueAsString();
            }
          }
          catch(NullPointerException e) {
            value = "[null]";
          }
          out.commentPrintLn(SerializedParameterization.OPTION_PREFIX + name + " " + value);
        }
      }
    }

    out.commentPrintSeparator();
    out.flush();
  }

  /**
   * Stream output.
   * 
   * @param db Database object
   * @param r Result class
   * @param streamOpener output stream manager
   * @throws UnableToComplyException when no usable results were found
   * @throws IOException on IO error
   */
  public void output(Database db, Result r, StreamFactory streamOpener) throws UnableToComplyException, IOException {
    List<Relation<?>> ra = new LinkedList<Relation<?>>();
    List<OrderingResult> ro = new LinkedList<OrderingResult>();
    List<Clustering<?>> rc = new LinkedList<Clustering<?>>();
    List<IterableResult<?>> ri = new LinkedList<IterableResult<?>>();
    List<SettingsResult> rs = new LinkedList<SettingsResult>();
    List<Result> otherres = new LinkedList<Result>();

    // collect other results
    {
      List<Result> results = ResultUtil.filterResults(r, Result.class);
      for(Result res : results) {
        if(res instanceof Database) {
          continue;
        }
        if(res instanceof Relation) {
          ra.add((Relation<?>) res);
          continue;
        }
        if(res instanceof OrderingResult) {
          ro.add((OrderingResult) res);
          continue;
        }
        if(res instanceof Clustering) {
          rc.add((Clustering<?>) res);
          continue;
        }
        if(res instanceof IterableResult) {
          ri.add((IterableResult<?>) res);
          continue;
        }
        if(res instanceof SettingsResult) {
          rs.add((SettingsResult) res);
          continue;
        }
        otherres.add(res);
      }
    }

    for(IterableResult<?> rii : ri) {
      writeIterableResult(streamOpener, rii, rs);
    }
    for(Clustering<?> c : rc) {
      NamingScheme naming = new SimpleEnumeratingScheme(c);
      for(Cluster<?> clus : c.getAllClusters()) {
        writeClusterResult(db, streamOpener, clus, ra, naming, rs);
      }
    }
    for(OrderingResult ror : ro) {
      writeOrderingResult(db, streamOpener, ror, ra, rs);
    }
    for(Result otherr : otherres) {
      writeOtherResult(streamOpener, otherr, rs);
    }
  }

  private void printObject(TextWriterStream out, Database db, final DBID objID, List<Relation<?>> ra) throws UnableToComplyException, IOException {
    SingleObjectBundle bundle = db.getBundle(objID);
    // Write database element itself.
    for(int i = 0; i < bundle.metaLength(); i++) {
      Object obj = bundle.data(i);
      if(obj != null) {
        TextWriterWriterInterface<?> owriter = out.getWriterFor(obj);
        if(owriter == null) {
          throw new UnableToComplyException("No handler for database object itself: " + obj.getClass().getSimpleName());
        }
        String lbl = null;
        // TODO: ugly compatibility hack...
        if(TypeUtil.DBID.isAssignableFromType(bundle.meta(i))) {
          lbl = "ID";
        }
        owriter.writeObject(out, lbl, obj);
      }
    }

    Collection<Relation<?>> dbrels = db.getRelations();
    // print the annotations
    if(ra != null) {
      for(Relation<?> a : ra) {
        // Avoid duplicated output.
        if(dbrels.contains(a)) {
          continue;
        }
        String label = a.getShortName();
        Object value = a.get(objID);
        if(value == null) {
          continue;
        }
        TextWriterWriterInterface<?> writer = out.getWriterFor(value);
        if(writer == null) {
          // Ignore
          continue;
        }
        writer.writeObject(out, label, value);
      }
    }
    out.flush();
    out.flush();
  }

  private void writeOtherResult(StreamFactory streamOpener, Result r, List<SettingsResult> rs) throws UnableToComplyException, IOException {
    PrintStream outStream = streamOpener.openStream(getFilename(r, r.getShortName()));
    TextWriterStream out = new TextWriterStream(outStream, writers);
    TextWriterWriterInterface<?> owriter = out.getWriterFor(r);
    if(owriter == null) {
      throw new UnableToComplyException("No handler for result class: " + r.getClass().getSimpleName());
    }
    // Write settings preamble
    printSettings(out, rs);
    // Write data
    owriter.writeObject(out, null, r);
    out.flush();
  }

  private void writeClusterResult(Database db, StreamFactory streamOpener, Cluster<?> clus, List<Relation<?>> ra, NamingScheme naming, List<SettingsResult> sr) throws FileNotFoundException, UnableToComplyException, IOException {
    String filename = null;
    if(naming != null) {
      filename = filenameFromLabel(naming.getNameFor(clus));
    }
    else {
      filename = "cluster";
    }

    PrintStream outStream = streamOpener.openStream(getFilename(clus, filename));
    TextWriterStream out = new TextWriterStream(outStream, writers);
    printSettings(out, sr);

    // Write cluster information
    out.commentPrintLn("Cluster: " + naming.getNameFor(clus));
    Model model = clus.getModel();
    if(model != ClusterModel.CLUSTER && model != null) {
      TextWriterWriterInterface<?> mwri = writers.getHandler(model);
      mwri.writeObject(out, null, model);
    }
    if(clus.getParents().size() > 0) {
      StringBuffer buf = new StringBuffer();
      buf.append("Parents:");
      for(Cluster<?> parent : clus.getParents()) {
        buf.append(" ").append(naming.getNameFor(parent));
      }
      out.commentPrintLn(buf.toString());
    }
    if(clus.getChildren().size() > 0) {
      StringBuffer buf = new StringBuffer();
      buf.append("Children:");
      for(Cluster<?> child : clus.getChildren()) {
        buf.append(" ").append(naming.getNameFor(child));
      }
      out.commentPrintLn(buf.toString());
    }
    out.flush();

    // print ids.
    DBIDs ids = clus.getIDs();
    for (DBIDIter iter = ids.iter(); iter.valid(); iter.advance()) {
      printObject(out, db, iter.getDBID(), ra);
    }
    out.commentPrintSeparator();
    out.flush();
  }

  private void writeIterableResult(StreamFactory streamOpener, IterableResult<?> ri, List<SettingsResult> sr) throws UnableToComplyException, IOException {
    PrintStream outStream = streamOpener.openStream(getFilename(ri, ri.getShortName()));
    TextWriterStream out = new TextWriterStream(outStream, writers);
    printSettings(out, sr);

    // hack to print collectionResult header information
    if(ri instanceof CollectionResult<?>) {
      final Collection<String> hdr = ((CollectionResult<?>) ri).getHeader();
      if(hdr != null) {
        for(String header : hdr) {
          out.commentPrintLn(header);
        }
        out.flush();
      }
    }
    Iterator<?> i = ri.iterator();
    while(i.hasNext()) {
      Object o = i.next();
      TextWriterWriterInterface<?> writer = out.getWriterFor(o);
      if(writer != null) {
        writer.writeObject(out, null, o);
      }
      out.flush();
    }
    out.commentPrintSeparator();
    out.flush();
  }

  private void writeOrderingResult(Database db, StreamFactory streamOpener, OrderingResult or, List<Relation<?>> ra, List<SettingsResult> sr) throws IOException, UnableToComplyException {
    PrintStream outStream = streamOpener.openStream(getFilename(or, or.getShortName()));
    TextWriterStream out = new TextWriterStream(outStream, writers);
    printSettings(out, sr);

    for (DBIDIter i = or.iter(or.getDBIDs()).iter(); i.valid(); i.advance()) {
      printObject(out, db, i.getDBID(), ra);
    }
    out.commentPrintSeparator();
    out.flush();
  }

  /**
   * Derive a file name from the cluster label.
   * 
   * @param label cluster label
   * @return cleaned label suitable for file names.
   */
  private String filenameFromLabel(String label) {
    return label.toLowerCase().replaceAll("[^a-zA-Z0-9_.\\[\\]-]", "_");
  }
}
}
