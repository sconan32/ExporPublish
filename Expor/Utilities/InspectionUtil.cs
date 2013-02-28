using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Socona.Log;

namespace Socona.Expor.Utilities
{

    public class InspectionUtil
    {
        /**
         * Class logger
         */
        private static readonly Logging logger = Logging.GetLogger(typeof(InspectionUtil));

        /**
         * Default package ignores.
         */
        private static readonly String[] DEFAULT_IGNORES = {
      // Sun Java
  "java.", "com.sun.",
      // Batik classes
  "org.apache.",
      // W3C / SVG / XML classes
  "org.w3c.", "org.xml.", "javax.xml.",
      // JUnit
  "org.junit.", "junit.", "org.hamcrest.",
      // Eclipse
  "org.eclipse.",
      // ApiViz
  "org.jboss.apiviz.",
      // JabRef
  "spin.", "osxadapter.", "antlr.", "ca.odell.", "com.jgoodies.", "com.michaelbaranov.", "com.mysql.", "gnu.dtools.", "net.sf.ext.", "net.sf.jabref.", "org.antlr.", "org.gjt.", "org.java.plugin.", "org.jempbox.", "org.pdfbox.", "wsi.ra.",
      // GNU trove
  "gnu.trove.",
  //
  };

        /**
         * If we have a non-static classpath, we do more extensive scanning for user
         * extensions.
         */
        public static readonly bool NONSTATIC_CLASSPATH;

        // Check for non-jar entries in classpath.
        static InspectionUtil()
        {
            String[] classpath = Environment.CurrentDirectory.Split(Path.PathSeparator);
            bool hasnonstatic = false;
            foreach (String path in classpath)
            {
                if (!path.EndsWith(".dll"))
                {
                    hasnonstatic = true;
                }
            }
            NONSTATIC_CLASSPATH = hasnonstatic;
        }

        /**
         * Weak hash map for class lookups
         */
        private static ConditionalWeakTable<Type, IList<Type>> CLASS_CACHE = new ConditionalWeakTable<Type, IList<Type>>();

        /**
         * (Non-weak) cache for all "frequently scanned" classes.
         */
       // private static List<Type> MASTER_CACHE = null;

        /**
         * Cached version of "findAllImplementations". For Parameterizable classes
         * only!
         * 
         * @param c Class to scan for
         * @return Found implementations
         */
        public static IList<Type> CachedFindAllImplementations(Type c)
        {
            if (c == null)
            {
                return new List<Type>();
            }
            IList<Type> res = null;
            CLASS_CACHE.TryGetValue(c, out res);
            if (res == null)
            {
                res = FindAllImplementations(c, false);
                CLASS_CACHE.Add(c, res);
            }
            return res;
        }

        /**
         * Find all implementations of a given class in the classpath.
         * 
         * Note: returned classes may be abstract.
         * 
         * @param c Class restriction
         * @param everything include interfaces, abstract and private classes
         * @return List of found classes.
         */
        public static IList<Type> FindAllImplementations(Type c, bool everything)
        {
            IList<Type> list = new List<Type>();
            // Add all from service files (i.e. jars)
            //{
            //  Iterator<Type> iter = new ELKIServiceLoader(c);
            //  while(iter.hasNext()) {
            //    list.Add(iter.next());
            //  }
            //}
            //if(!InspectionUtil.NONSTATIC_CLASSPATH) {
            //  if(list.Count == 0) {
            //    logger.Warning("No implementations for " + c.Name + " were found using index files.");
            //  }
            //}
            //else {
            //  // Duplicate checking
            //  HashSet<Type> dupes = new HashSet<Type>(list);
            //  // Scan for additional ones in class path
            //  Iterator<Type> iter;
            //  // If possible, reuse an existing scan result
            //  if(typeof(IInspectionUtilFrequentlyScanned).IsAssignableFrom(c)) {
            //    iter = GetFrequentScan();
            //  }
            //  else {
            //    iter = SlowScan(c).iterator();
            //  }
            //  while(iter.hasNext()) {
            //    Type cls = iter.next();
            //    // skip abstract / private classes.
            //    if(!everything && (Modifier.isInterface(cls.getModifiers()) || Modifier.isAbstract(cls.getModifiers()) || Modifier.isPrivate(cls.getModifiers()))) {
            //      continue;
            //    }
            //    if(c.isAssignableFrom(cls) && !dupes.contains(cls)) {
            //      list.add(cls);
            //      dupes.add(cls);
            //    }
            //  }
            //}
            return list;
        }

        /**
         * Get (or create) the result of a scan for any "frequent scanned" class.
         * 
         * @return Scan result
         */
        //private static Iterator<Type> GetFrequentScan() {
        //  if(MASTER_CACHE == null) {
        //    MASTER_CACHE = SlowScan(typeof(IInspectionUtilFrequentlyScanned));
        //  }
        //  return MASTER_CACHE.iterator();
        //}

        /**
         * Perform a full (slow) scan for classes.
         * 
         * @param cond Class to include
         * @return List with the scan result
         */
        private static IList<Type> SlowScan(Type cond)
        {
            IList<Type> res = new List<Type>();
            //try {
            //  ClassLoader cl = ClassLoader.getSystemClassLoader();
            //  Enumeration<URL> cps = cl.getResources("");
            //  while(cps.hasMoreElements()) {
            //    URL u = cps.nextElement();
            //    // Scan file sources only.
            //    if(u.getProtocol() == "file") {
            //      Iterator<String> it = new DirClassIterator(new File(u.getFile()), DEFAULT_IGNORES);
            //      while(it.hasNext()) {
            //        String classname = it.next();
            //        try {
            //          Type cls = cl.loadClass(classname);
            //          // skip classes where we can't get a full name.
            //          if(cls.getCanonicalName() == null) {
            //            continue;
            //          }
            //          // Implements the right interface?
            //          if(cond != null && !cond.isAssignableFrom(cls)) {
            //            continue;
            //          }
            //          res.add(cls);
            //        }
            //        catch(ClassNotFoundException e) {
            //          continue;
            //        }
            //        catch(NoClassDefFoundError e) {
            //          continue;
            //        }
            //        catch(Exception e) {
            //          continue;
            //        }

            //      }
            //    }
            //  }
            //}
            //catch(IOException e) {
            //  logger.Error(e);
            //}
            //Collections.sort(res, new ClassSorter());
            return res;
        }

        /**
         * Class to iterate over a Jar file.
         * 
         * Note: this is currently unused, as we now require all jar files to include
         * an index in the form of service-style files in META-INF/elki/
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        //static class JarClassIterator : IEnumerator<String> {
        //  private Enumeration<JarEntry> jarentries;

        //  private String ne;

        //  private String[] ignorepackages;

        //  /**
        //   * Constructor from Jar file.
        //   * 
        //   * @param path Jar file entries to iterate over.
        //   */
        //  public JarClassIterator(String path, String[] ignorepackages) {
        //    this.ignorepackages = ignorepackages;
        //    try {
        //      JarFile jf = new JarFile(path);
        //      this.jarentries = jf.entries();
        //      this.ne = findNext();
        //    }
        //    catch(IOException e) {
        //      LoggingUtil.exception("Error opening jar file: " + path, e);
        //      this.jarentries = null;
        //      this.ne = null;
        //    }
        //  }

        //  @Override
        //  public bool hasNext() {
        //    // Do we have a next entry?
        //    return (ne != null);
        //  }

        //  /**
        //   * Find the next entry, since we need to skip some jar file entries.
        //   * 
        //   * @return next entry or null
        //   */
        //  private String findNext() {
        //    nextfile: while(jarentries.hasMoreElements()) {
        //      JarEntry je = jarentries.nextElement();
        //      String name = je.getName();
        //      if(name.endsWith(".class")) {
        //        String classname = name.substring(0, name.length() - ".class".length()).replace('/', '.');
        //        for(String pkg : ignorepackages) {
        //          if(classname.startsWith(pkg)) {
        //            continue nextfile;
        //          }
        //        }
        //        if(classname.endsWith(ClassParameter.FACTORY_POSTFIX) || !classname.contains("$")) {
        //          return classname.replace('/', '.');
        //        }
        //      }
        //    }
        //    return null;
        //  }

        //  @Override
        //  public String next() {
        //    // Return the previously stored entry.
        //    String ret = ne;
        //    ne = findNext();
        //    return ret;
        //  }

        //  @Override
        //  public void remove() {
        //    throw new UnsupportedOperationException();
        //  }
        //}

        /**
         * Class to iterate over a directory tree.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        //static class DirClassIterator :IEnumerator<String> {
        //  private static readonly String CLASS_EXT = ".class";

        //  private static readonly String FACTORY_FILE_EXT = ClassParameter.FACTORY_POSTFIX + CLASS_EXT;

        //  private static readonly int CLASS_EXT_LENGTH = CLASS_EXT.length();

        //  private String prefix;

        //  private ArrayList<String> files = new ArrayList<String>(100);

        //  private ArrayList<Pair<File, String>> folders = new ArrayList<Pair<File, String>>(100);

        //  private String[] ignorepackages;

        //  /**
        //   * Constructor from Directory
        //   * 
        //   * @param path Directory to iterate over
        //   */
        //  public DirClassIterator(File path, String[] ignorepackages) {
        //    this.ignorepackages = ignorepackages;
        //    this.prefix = path.getAbsolutePath();
        //    if(prefix[prefix.length() - 1] != File.separatorChar) {
        //      prefix = prefix + File.separatorChar;
        //    }

        //    this.folders.add(new Pair<File, String>(path, ""));
        //  }

        //  @Override
        //  public bool hasNext() {
        //    if(files.size() == 0) {
        //      findNext();
        //    }
        //    return (files.size() > 0);
        //  }

        //  /**
        //   * Find the next entry, since we need to skip some directories.
        //   */
        //  private void findNext() {
        //    while(folders.size() > 0) {
        //      Pair<File, String> pair = folders.remove(folders.size() - 1);
        //      // recurse into directories
        //      if(pair.first.isDirectory()) {
        //        nextfile: for(String localname : pair.first.list()) {
        //          // Ignore unix-hidden files/dirs
        //          if(localname.charAt(0) == '.') {
        //            continue;
        //          }
        //          // Classes
        //          if(localname.endsWith(CLASS_EXT)) {
        //            if(localname.indexOf('$') >= 0) {
        //              if(!localname.endsWith(FACTORY_FILE_EXT)) {
        //                continue;
        //              }
        //            }
        //            files.add(pair.second + localname.substring(0, localname.length() - CLASS_EXT_LENGTH));
        //            continue;
        //          }
        //          // Recurse into directories
        //          File newf = new File(pair.first, localname);
        //          if(newf.isDirectory()) {
        //            String newpref = pair.second + localname + '.';
        //            for(String ignore : ignorepackages) {
        //              if(ignore.equals(newpref)) {
        //                continue nextfile;
        //              }
        //            }
        //            folders.add(new Pair<File, String>(newf, newpref));
        //          }
        //        }
        //      }
        //    }
        //  }

        //  @Override
        //  public String next() {
        //    if(files.size() == 0) {
        //      findNext();
        //    }
        //    if(files.size() > 0) {
        //      return files.remove(files.size() - 1);
        //    }
        //    return null;
        //  }

        //  @Override
        //  public void remove() {
        //    throw new UnsupportedOperationException();
        //  }
        //}

        /**
         * Sort classes by their class name. Package first, then class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public class ClassSorter : IComparer<Type>
        {

            public int Compare(Type o1, Type o2)
            {
                int pkgcmp = o1.Assembly.FullName.CompareTo(o2.Assembly.FullName);
                if (pkgcmp != 0)
                {
                    return pkgcmp;
                }
                return o1.FullName.CompareTo(o2.FullName);
            }
        }
    }
}
