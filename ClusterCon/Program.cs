using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Utilities;
using Socona.Clustering.Datasets;
using Socona.Clustering.Distances;
using Socona.Clustering.Algorithms;
using Socona.Clustering.Clusters;
using Socona.Clustering.Patterns;
using System.IO;
namespace ClusterCon
{
    class Program
    {
        static void Main(string[] args)
        {
           //FSC();
           // Kmeans();
            //AggHrch();
            //CopKmeans();
            LAC();
        }
        static void Kmeans()
        {
            string dataFile = "wine.data.txt";
            uint numClust = 3;
            uint maxIter = 100;
            uint numRun = 50;
            uint seed = 0;

            //try
            {
                DatasetReader reader = new DatasetReader(dataFile);
                Dataset ds = null;
                reader.Fill(ref ds);
                Console.WriteLine("Data File: " + System.IO.Path.GetFileName(dataFile));
                Console.WriteLine(ds.ToDescriptionString());

                EuclideanDistance ed = new EuclideanDistance();
                Results res = null;
                double avgIter = 0;
                double avgError = 0;
                double dMin = double.MaxValue;
                double error;
                DateTime startTime = DateTime.Now;
                for (int i = 0; i < numRun; i++)
                {
                    Kmean ca = new Kmean();
                    Arguments arg = ca.Arguments;

                    arg.Dataset = ds;
                    arg.Distance = ed;
                    arg.Insert("numclust", numClust);
                    arg.Insert("maxiter", maxIter);
                    arg.Insert("seed", seed);
                    if (numRun == 1)
                    {
                        arg.Values["seed"] = seed;
                    }
                    else
                    {
                        arg.Values["seed"] = i;
                    }
                    ca.Clusterize();

                    Results tmp = ca.Results;
                    avgIter += (int)tmp.Get("numiter");
                    error = (double)tmp.Get("error");
                    avgError += error;
                    if (error < dMin)
                    {
                        dMin = error;
                        res = tmp;
                    }
                }
                avgIter /= numRun;
                avgError /= numRun;
                DateTime endTime = DateTime.Now;
                PClustering clu = res.Get("pc") as PClustering;
                Console.WriteLine(clu.ToCompleteString());

                Console.WriteLine("===");
                Console.WriteLine("Total Run Time: " + (endTime - startTime).ToString());
                Console.WriteLine("Number of Runs: " + numRun);
                Console.WriteLine("Average Number of Iterations: " + avgIter);
                Console.WriteLine("Average Error: " + avgError);
                Console.WriteLine("Best Error: " + dMin);





            }
            //catch(Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
        }
        static void FSC()
        {
            string dataFile = "wine.data.txt";
            int numClust = 3;
            int maxIter = 100;
            int numRun = 30;
            int seed = 0;
            double alpha = 2.1 ;
            double epsilon = 1e-10; ;
            double threshold=1e-12;

            //try
            {
                DatasetReader reader = new DatasetReader(dataFile);
                Dataset ds = null;
                reader.Fill(ref ds);
                Console.WriteLine("Data File: " + System.IO.Path.GetFileName(dataFile));
                Console.WriteLine(ds.ToDescriptionString());

                EuclideanDistance ed = new EuclideanDistance();
                Results res = null;
                double avgIter = 0;
                double avgError = 0;
                double dMin = double.MaxValue;
                double error;
                DateTime startTime = DateTime.Now;
                for (int i = 0; i < numRun; i++)
                {
                     FSC ca = new FSC();
                    Arguments arg = ca.Arguments;

                    arg.Dataset = ds;
                    //arg.Distance = ed;
                    arg.Insert("alpha", alpha);
                    arg.Insert("epsilon", epsilon);
                    arg.Insert("threshold", threshold);
                    arg.Insert("numclust", numClust);
                    arg.Insert("maxiter", maxIter);
                    arg.Insert("seed", seed);
                    if (numRun == 1)
                    {
                        arg.Values["seed"] = seed;
                    }
                    else
                    {
                        arg.Values["seed"] = i;
                    }
                    ca.Clusterize();

                    Results tmp = ca.Results;
                    avgIter += (int)tmp.Get("numiter");
                    error = (double)tmp.Get("obj");
                    avgError += error;
                    if (error < dMin)
                    {
                        dMin = error;
                        res = tmp;
                    }
                }
                avgIter /= numRun;
                avgError /= numRun;
                DateTime endTime = DateTime.Now;
                PClustering clu = res.Get("pc") as PClustering;
                Console.WriteLine(clu.ToCompleteString());
                int numIter = (int)res.Get("numiter");
                error = (double)res.Get("obj");
                SubspaceCluster sc;
                Console.WriteLine("Attribute Weights:");
                for (int k = 0; k < clu.Count; k++)
                {
                    sc = clu[k] as SubspaceCluster;
                    Console.Write("Cluster " + k);
                    for (int j = 0; j < sc.Weights.Count; j++)
                    {
                        Console.Write(", " + sc.Weights[j]);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("===");
                Console.WriteLine("Total Run Time: " + (endTime - startTime).ToString());
                Console.WriteLine("Number of Runs: " + numRun);
                Console.WriteLine("Average Number of Iterations: " + avgIter);
                Console.WriteLine("Average Error: " + avgError);
                Console.WriteLine("Best Error: " + dMin);





            }
            //catch(Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
        }
        static void AggHrch()
        {
            string dataFile = "iris.data.txt";


            //string method="complete";
            //string method = "centroid";
            //string method = "gaverage";
            //string method = "wgaverage";
            //string method = "single";
            //string method = "median";
            string method = "ward";
            int maxClust=3;
            int p=0;
            DatasetReader reader = new DatasetReader(dataFile);
            Dataset ds =null;
            reader.Fill(ref ds);
            Console.WriteLine("Data File: " + System.IO.Path.GetFileName(dataFile));
            Console.WriteLine(ds.ToDescriptionString());

            EuclideanDistance ed = new EuclideanDistance();
            

            AbstractAlgorithm ca=null;
            switch (method)
            {
                case "single":
                    ca = new Socona.Clustering.Algorithms.Single();
                    break;
                case "complete":

                    ca = new Complete();
                    break;
                case "gaverage":
                    ca = new Average();
                    break;
                case "wgaverage":
                    ca = new Weighted();
                    break;
                case "centroid":
                    ca = new Centroid();
                    break;
                case "median":
                    ca = new Median();
                    break;
                case "ward":
                    ca = new Ward();
                    break;
                default:
                    throw new ArgumentException("Method" + method + "is not Available");
            }
            Arguments arg = ca.Arguments;
            arg.Dataset = ds;
            arg.Distance = ed;
            DateTime startTime = DateTime.Now;
            ca.Clusterize();
            DateTime endTime = DateTime.Now;
            Console.WriteLine("===");
            Console.WriteLine("Total Run Time: " + (endTime - startTime).ToString());

            string prefix = System.IO.Path.GetFileNameWithoutExtension(dataFile);
            prefix += "-" + method + "-";
            Results res  = ca.Results;
            HClustering hc = res.Get("hc") as HClustering;
            hc.Save(prefix + "dendrogram.eps", p);

            JoinValueVisitor jv = new JoinValueVisitor();
            hc.Root.Accept(jv);

            string jvFile = prefix + "-joinValues.csv";
            FileStream jvf = File.OpenWrite(jvFile);
            StreamWriter sr = new StreamWriter(jvf);
            sr.Write(jv.ToString());
            sr.Close();
            jvf.Close();

            PClustering pc = hc.GetPC(maxClust);
            Console.WriteLine(pc.ToCompleteString());

            pc.Save(prefix + "-pc-summary.txt");

            

        }
        static void CopKmeans()
        {
            string dataFile = "iris.data.txt";
            uint numClust = 3;
            uint maxIter = 100;
            uint numRun = 50;
            uint seed = 0;

            //try
            {
                Console.WriteLine("COP K-means");
                DatasetReader reader = new ConstraintedDatasetReader(dataFile);
                Dataset ds = null;
                reader.Fill(ref ds);

                Console.WriteLine("Data File: " + System.IO.Path.GetFileName(dataFile));
                Console.WriteLine(ds.ToDescriptionString());

                EuclideanDistance ed = new EuclideanDistance();
                Results res = null;
                double avgIter = 0;
                double avgError = 0;
                double dMin = double.MaxValue;
                double error;
                DateTime startTime = DateTime.Now;
                for (int i = 0; i < numRun; i++)
                {
                    CopKmeans ca = new CopKmeans();
                    
                    Arguments arg = ca.Arguments;

                    arg.Dataset = ds;
                    arg.Distance = ed;
                    arg.Insert("numclust", numClust);
                    arg.Insert("maxiter", maxIter);
                    arg.Insert("seed", seed);
                    if (numRun == 1)
                    {
                        arg.Values["seed"] = seed;
                    }
                    else
                    {
                        arg.Values["seed"] = i;
                    }
                    ca.Clusterize();

                    Results tmp = ca.Results;
                    avgIter += (int)tmp.Get("numiter");
                    error = (double)tmp.Get("error");
                    avgError += error;
                    if (error < dMin)
                    {
                        dMin = error;
                        res = tmp;
                    }
                }
                avgIter /= numRun;
                avgError /= numRun;
                DateTime endTime = DateTime.Now;
                PClustering clu = res.Get("pc") as PClustering;
                Console.WriteLine(clu.ToCompleteString());

                Console.WriteLine("===");
                Console.WriteLine("Total Run Time: " + (endTime - startTime).ToString());
                Console.WriteLine("Number of Runs: " + numRun);
                Console.WriteLine("Average Number of Iterations: " + avgIter);
                Console.WriteLine("Average Error: " + avgError);
                Console.WriteLine("Best Error: " + dMin);





            }
            //catch(Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
        }
        static void LAC()
        {
            string dataFile = "iris.data.txt";
            int numClust = 3;
            int maxIter = 100;
            int numRun = 50;
            int seed = 0;
            double h = 2;
            double epsilon = 1e-10; ;
            double threshold = 1e-12;
            //try
            {
                Console.WriteLine("LAC");
                DatasetReader reader = new DatasetReader(dataFile);
                Dataset ds = null;
                reader.Fill(ref ds);
                Console.WriteLine("Data File: " + System.IO.Path.GetFileName(dataFile));
                Console.WriteLine(ds.ToDescriptionString());

                EuclideanDistance ed = new EuclideanDistance();
                Results res = null;
                double avgIter = 0;
                double avgError = 0;
                double dMin = double.MaxValue;
                double error;
                DateTime startTime = DateTime.Now;
                for (int i = 0; i < numRun; i++)
                {
                    LAC ca = new Socona.Clustering.Algorithms.LAC();
                    Arguments arg = ca.Arguments;

                    arg.Dataset = ds;
                    arg.Distance = ed;
                    arg.Insert("numclust", numClust);
                    arg.Insert("maxiter", maxIter);
                    arg.Insert("seed", seed);
                    arg.Insert("h", h);
                    arg.Insert("epsilon", epsilon);
                    arg.Insert("threshold", threshold);
                    if (numRun == 1)
                    {
                        arg.Values["seed"] = seed;
                    }
                    else
                    {
                        arg.Values["seed"] = i;
                    }
                    ca.Clusterize();

                    Results tmp = ca.Results;
                    avgIter += (int)tmp.Get("numiter");
                    error = (double)tmp.Get("error");
                    avgError += error;
                    if (error < dMin)
                    {
                        dMin = error;
                        res = tmp;
                    }
                }
                avgIter /= numRun;
                avgError /= numRun;
                DateTime endTime = DateTime.Now;
                PClustering clu = res.Get("pc") as PClustering;
                Console.WriteLine(clu.ToCompleteString());

                Console.WriteLine("===");
                Console.WriteLine("Total Run Time: " + (endTime - startTime).ToString());
                Console.WriteLine("Number of Runs: " + numRun);
                Console.WriteLine("Average Number of Iterations: " + avgIter);
                Console.WriteLine("Average Error: " + avgError);
                Console.WriteLine("Best Error: " + dMin);
                string prefix = System.IO.Path.GetFileNameWithoutExtension(dataFile);
                prefix += "-" + "LAC" + "-";
                clu.Save(prefix + "-pc-summary.txt");




            }
            //catch(Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
        }
    }
}
