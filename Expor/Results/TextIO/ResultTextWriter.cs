using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.DataSources.Bundles;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Results;
using Socona.Expor.Results.TextIO;
using Socona.Expor.Results.TextIO.Naming;
using Socona.Expor.Results.TextIO.Writers;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Expor.Utilities.Pairs;
using Wintellect.PowerCollections;

namespace Socona.Expor.Results.TextIO
{
    public class ResultTextWriter
    {
        /**
         * Extension for txt-files.
         */
        public static readonly String FILE_EXTENSION = ".txt";

        /**
         * Hash map for supported classes in writer.
         */
        public static HandlerList<ITextWriter> writers = new HandlerList<ITextWriter>();

        /**
         * Add some default handlers
         */
        static ResultTextWriter()
        {
            TextWriterObjectInline trivialwriter = new TextWriterObjectInline();
            writers.InsertHandler(typeof(Object), new TextWriterObjectComment());
            writers.InsertHandler(typeof(IPair<,>), new TextWriterPair());
            writers.InsertHandler(typeof(IHierarchicalResult), new TextWriterHierarchicalResult());

            writers.InsertHandler(typeof(DoubleDoublePair), new TextWriterDoubleDoublePair());

            writers.InsertHandler(typeof(Triple<,,>), new TextWriterTriple());
            writers.InsertHandler(typeof(IDataVector), trivialwriter);
            // these object can be serialized inline with toString()
            writers.InsertHandler(typeof(String), trivialwriter);
            writers.InsertHandler(typeof(double), trivialwriter);
            writers.InsertHandler(typeof(int), trivialwriter);
            writers.InsertHandler(typeof(String[]), new TextWriterObjectArray());
            writers.InsertHandler(typeof(Double[]), new TextWriterObjectArray());
            writers.InsertHandler(typeof(int[]), new TextWriterObjectArray());
            writers.InsertHandler(typeof(BitArray), trivialwriter);
            writers.InsertHandler(typeof(Vector), new TextWriterVector());
            writers.InsertHandler(typeof(IDistanceValue), trivialwriter);
            writers.InsertHandler(typeof(SimpleClassLabel), trivialwriter);
            writers.InsertHandler(typeof(HierarchicalClassLabel), trivialwriter);
            writers.InsertHandler(typeof(LabelList), trivialwriter);
            writers.InsertHandler(typeof(IDbId), trivialwriter);
            // Objects that have an own writeToText method.
            writers.InsertHandler(typeof(ITextWriteable), new TextWriterTextWriteable());
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
        protected String GetFilename(Object result, String filenamepre)
        {
            if (filenamepre == null || filenamepre.Length == 0)
            {
                filenamepre = "result";
            }
            int i = 0;
            while (true)
            {
                String filename;
                if (i > 0)
                {
                    filename = filenamepre + "-" + i;
                }
                else
                {
                    filename = filenamepre;
                }
                Object existing = null;
                filenames.TryGetValue(filename, out existing);
                if (existing == null || existing == result)
                {
                    filenames[filename] = result;
                    return filename;
                }
                i++;
            }
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
        public void Output(IDatabase db, IResult r, IStreamFactory streamOpener)
        {
            IList<IRelation> ra = new List<IRelation>();
            IList<IOrderingResult> ro = new List<IOrderingResult>();
            IList<ClusterList> rc = new List<ClusterList>();
            IList<IIterableResult<object>> ri = new List<IIterableResult<object>>();
            IList<SettingsResult> rs = new List<SettingsResult>();
            IList<IResult> otherres = new List<IResult>();

            // collect other results
            {
                IList<IResult> results = ResultUtil.FilterResults<IResult>(r, typeof(IResult));
                foreach (IResult res in results)
                {
                    if (res is IDatabase)
                    {
                        continue;
                    }
                    if (res is IRelation)
                    {
                        ra.Add((IRelation)res);
                        continue;
                    }
                    if (res is IOrderingResult)
                    {
                        ro.Add((IOrderingResult)res);
                        continue;
                    }
                    if (res is ClusterList)
                    {
                        rc.Add((ClusterList)res);
                        continue;
                    }
                    if (res is IIterableResult<object>)
                    {
                        ri.Add((IIterableResult<object>)res);
                        continue;
                    }
                    if (res is SettingsResult)
                    {
                        rs.Add((SettingsResult)res);
                        continue;
                    }
                    otherres.Add(res);
                }
            }

            WriteSettingsResult(streamOpener, rs);
            foreach (IIterableResult<object> rii in ri)
            {
                WriteIterableResult(streamOpener, rii, rs);
            }
            foreach (ClusterList c in rc)
            {
                string hstr = GetHierarchyString(c as IHierarchicalResult) ?? "[ISOLATED]";
                INamingScheme naming = new SimpleEnumeratingScheme(c);

                foreach (Cluster clus in c.GetAllClusters())
                {

                    WriteClusterResult(db, streamOpener, clus, ra, naming, hstr);
                }
            }
            foreach (IOrderingResult ror in ro)
            {
                WriteOrderingResult(db, streamOpener, ror, ra, rs);
            }
            foreach (IResult otherr in otherres)
            {
                WriteOtherResult(streamOpener, otherr);
            }
        }

        private string GetHierarchyString(IHierarchicalResult r)
        {
            if (r == null)
                return null;

            StringBuilder sb = new StringBuilder();
            var anc = r;
            while (anc != null)
            {
                sb.Append(anc.LongName).Append("->");
                var list = anc.Hierarchy.Ancestors(anc);
                if (list == null)
                { break; }
                anc = list.LastOrDefault() as IHierarchicalResult;

            }
            return sb.ToString();

        }

        private void PrintObject(TextWriterStream sout, IDatabase db, IDbId objID, IList<IRelation> ra)
        {
            SingleObjectBundle bundle = db.GetBundle(objID);
            // Write database element itself.
            for (int i = 0; i < bundle.MetaLength(); i++)
            {
                Object obj = bundle.data(i);
                if (obj != null)
                {
                    ITextWriter owriter = sout.GetWriterFor(obj);
                    if (owriter == null)
                    {
                        throw new UnableToComplyException("No handler for database object itself: " + obj.GetType().Name);
                    }
                    String lbl = null;
                    // TODO: ugly compatibility hack...
                    if (TypeUtil.DBID.IsAssignableFromType(bundle.Meta(i)))
                    {
                        lbl = "ID";
                    }
                    owriter.Write(sout, lbl, obj);
                }
            }

            ICollection<IRelation> dbrels = db.GetRelations();
            // print the annotations
            if (ra != null)
            {
                foreach (IRelation a in ra)
                {
                    // Avoid duplicated output.
                    if (dbrels.Contains(a))
                    {
                        continue;
                    }
                    String label = a.ShortName;
                    Object value = a[objID];
                    if (value == null)
                    {
                        continue;
                    }
                    ITextWriter writer = sout.GetWriterFor(value);
                    if (writer == null)
                    {
                        // Ignore
                        continue;
                    }
                    writer.Write(sout, label, value);
                }
            }
            sout.Flush();
            sout.Flush();
        }

        private void WriteOtherResult(IStreamFactory streamOpener, IResult r)
        {


            Stream outStream = streamOpener.OpenStream(GetFilename(r, r.ShortName));
            TextWriterStream sout = new TextWriterStream(new StreamWriter(outStream), writers);
            string hstr = GetHierarchyString(r as IHierarchicalResult);
            sout.CommentPrintLine(hstr ?? "[ISOLATED]");
            ITextWriter owriter = sout.GetWriterFor(r);
            if (owriter == null)
            {
                throw new UnableToComplyException("No handler for result class: " + r.GetType().Name);
            }
            // Write settings preamble
            // PrintSettings(sout, rs);
            // Write data
            owriter.Write(sout, null, r);
            sout.Flush();
        }

        private void WriteClusterResult(IDatabase db, IStreamFactory streamOpener,
            Cluster clus, IList<IRelation> ra, INamingScheme naming, string hstr)
        {
            String filename = null;
            if (naming != null)
            {
                filename = FilenameFromLabel(naming.GetNameFor(clus));
            }
            else
            {
                filename = "cluster";
            }

            Stream outStream = streamOpener.OpenStream(GetFilename(clus, filename));
            TextWriterStream sout = new TextWriterStream(new StreamWriter(outStream), writers);

            sout.CommentPrintLine(hstr);
            // Write cluster information
            sout.CommentPrintLine("Cluster: " + naming.GetNameFor(clus));
            IModel model = clus.Model;
            if (model != ClusterModel.CLUSTER && model != null)
            {
                ITextWriter mwri = writers.GetHandler(model);
                mwri.Write(sout, null, model);
            }
            if (clus.GetParents().Count > 0)
            {
                StringBuilder buf = new StringBuilder();
                buf.Append("Parents:");
                foreach (Cluster parent in clus.GetParents())
                {
                    buf.Append(" ").Append(naming.GetNameFor(parent));
                }
                sout.CommentPrintLine(buf.ToString());
            }
            if (clus.GetChildren().Count > 0)
            {
                StringBuilder buf = new StringBuilder();
                buf.Append("Children:");
                foreach (Cluster child in clus.GetChildren())
                {
                    buf.Append(" ").Append(naming.GetNameFor(child));
                }
                sout.CommentPrintLine(buf.ToString());
            }
            sout.Flush();

            // print ids.
            IDbIds ids = clus.Ids;
            foreach (var id in ids)
            {
                PrintObject(sout, db, id.DbId, ra);
            }
            sout.Flush();

        }

        private void WriteIterableResult<T>(IStreamFactory streamOpener, IIterableResult<T> ri,
            IList<SettingsResult> sr)
        {

            Stream outStream = streamOpener.OpenStream(GetFilename(ri, ri.ShortName));
            TextWriterStream sout = new TextWriterStream(new StreamWriter(outStream), writers);
            // PrintSettings(sout, sr);
            string hstr = GetHierarchyString(ri as IHierarchicalResult);
            sout.CommentPrintLine(hstr ?? "[ISOLATED]");
            // hack to print collectionResult header information
            if (ri is CollectionResult<T>)
            {
                ICollection<String> hdr = ((CollectionResult<T>)ri).GetHeader();
                if (hdr != null)
                {
                    foreach (String header in hdr)
                    {
                        sout.CommentPrintLine(header);
                    }
                    sout.Flush();
                }
            }
            foreach (var res in ri)
            {

                //Iterator<?> i = ri.iterator();
                // while(i.hasNext()) {
                Object o = res;
                ITextWriter writer = sout.GetWriterFor(o);
                if (writer != null)
                {
                    writer.Write(sout, null, o);
                }
                sout.Flush();
            }
            sout.CommentPrintSeparator();
            sout.Flush();
        }

        private void WriteSettingsResult(IStreamFactory streamOpener, IList<SettingsResult> rs)
        {
            if (rs.Count < 1)
            {
                return;
            }
            SettingsResult r = rs[(0)];
            Stream outStream = streamOpener.OpenStream(GetFilename(r, r.ShortName));
            TextWriterStream sout = new TextWriterStream(new StreamWriter(outStream), writers);
            // Write settings preamble

            sout.CommentPrintLine("Settings:");

            if (rs != null)
            {
                foreach (SettingsResult settings in rs)
                {
                    string hstr = GetHierarchyString(settings as IHierarchicalResult);
                    sout.CommentPrintLine(hstr ?? "[ISOLATED]");
                    Object last = null;
                    foreach (var setting in settings.GetSettings())
                    {
                        if (setting.First != last && setting.First != null)
                        {
                            if (last != null)
                            {
                                sout.CommentPrintLine("");
                            }
                            String name;
                            try
                            {
                                if (setting.First is Type)
                                {
                                    var tfirst = setting.First as Type;
                                    if (tfirst.IsNested)
                                    {
                                        name = tfirst.DeclaringType.Name + "+" + tfirst.Name;
                                    }
                                    else
                                    {
                                        name = tfirst.Name;
                                    }
                                }
                                else
                                {
                                    name = setting.First.GetType().Name;
                                }
                                if (typeof(ClassParameter).IsInstanceOfType(setting.First))
                                {
                                    name = ((ClassParameter)setting.First).GetValue().Name;
                                }
                            }
                            catch (NullReferenceException)
                            {
                                name = "[null]";
                            }
                            sout.CommentPrintLine(name);
                            last = setting.First;
                        }
                        String vname = setting.Second.GetOptionDescription().Name;
                        String value = "[unset]";
                        try
                        {
                            if (setting.Second.IsDefined())
                            {
                                value = setting.Second.GetValueAsString();
                            }
                        }
                        catch (NullReferenceException)
                        {
                            value = "[null]";
                        }
                        sout.CommentPrintLine(SerializedParameterization.OPTION_PREFIX + vname + " " + value);
                    }
                }
            }
            sout.Flush();

        }
        private void WriteOrderingResult(IDatabase db, IStreamFactory streamOpener, IOrderingResult or,
            IList<IRelation> ra, IList<SettingsResult> sr)
        {

            Stream outStream = streamOpener.OpenStream(GetFilename(or, or.ShortName));
            TextWriterStream sout = new TextWriterStream(new StreamWriter(outStream), writers);
            //   PrintSettings(sout, sr);

            string hstr = GetHierarchyString(or as IHierarchicalResult);
            sout.CommentPrintLine(hstr ?? "[ISOLATED]");
            foreach (var id in or.GetDbIds())
            {
                //for (DBIDIter i = or.iter(or.GetDBIDs()).iter(); i.valid(); i.advance()) {
                PrintObject(sout, db, id.DbId, ra);
            }
            sout.CommentPrintSeparator();
            sout.Flush();
        }

        /**
         * Derive a file name from the cluster label.
         * 
         * @param label cluster label
         * @return cleaned label suitable for file names.
         */
        private string FilenameFromLabel(string label)
        {
            return label.ToLower().Replace("[^a-zA-Z0-9_.\\[\\]-]", "_");
        }
    }
}
