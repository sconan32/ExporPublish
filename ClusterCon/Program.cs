using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Algorithms.Clustering.KMeans;
using Socona.Expor.Distances.DistanceFuctions;
using System.IO;
using Socona.Expor.Data;
using Socona.Expor.Databases;
using Socona.Expor.DataSources;
using Socona.Expor.DataSources.Parsers;
using Socona.Expor.Data.Types;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor;
using Socona.Expor.Applications;
using System.Collections;


namespace ClusterCon
{
    class Program
    {
        static void Main(string[] args)
        {

                RunExpor(args);
          

        }
        public static void RunExpor(string[] args)
        {
            string[] param = null;
            if (args.Length > 0 && args[0].ToLower() == "-file")
            {
                FileStream fs = File.OpenRead(args[1]);
                StreamReader sr = new StreamReader(fs);
                string content = sr.ReadToEnd();
                param = content.Split(new string[] { ":=", ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int pi = 0; pi < param.Length; pi++)
                {
                    param[pi] = param[pi].Trim();

                    string t = param[pi];
                    if (t.Contains("\n"))
                    {
                        t = t.Replace("\n", String.Empty);
                    }
                    if (t.Contains("\t"))
                    {
                        t = t.Replace("\t", string.Empty);
                    }
                    if (t.Contains(" "))
                    {
                        t = t.Replace(" ", string.Empty);
                    }
                    param[pi] = t;
                }


            }
            else
            {

                param = new string[] 
                    {
                        "-db",              @"Socona.Expor.Databases.WithConstraintsStaticArrayDatabase",
                     //   "-db.index",        @"Socona.Expor.Indexes.Preprocessed.Knn.KNNJoinMaterializeKNNPreprocessor+Factory",
                        "-dbc.in",          @"E:\SkyDrive\Work\DataSets\artificial\5000x20.txt",
                        "-dbc.in.cons",     @"E:\SkyDrive\Work\DataSets\artificial\5000x20.cons.txt",
                        "-dbc.filter",      @"Socona.Expor.DataSources.Filters.Normalization.AttributeWiseMinMaxNormalization",
                        "-algorithm", 
                                            "Clustering.Subspace.PROCLUS"+                                        
                                          
                                            "",


                        "-clique.xsi",       "100",
                        "-clique.tau",      "0.20",

                         "-proclus.mi",     "10",
                         "-projectedclustering.k","5",
                         "-projectedclustering.k_i","50",
                         "-projectedclustering.l","5",



                         "-resulthandler", "ResultWriter",                         
                         "-out","result.txt"
                    };
            }
            ExporCliApp.RunCliApp(typeof(ExporCliApp), param);

        }
      
    }
}
