using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Socona.Clustering.Datasets;
using System.Diagnostics;

namespace Socona.Clustering.Utilities
{
    public class ConstraintedDatasetReader:DatasetReader
    {

        public ConstraintedDatasetReader(string fileName)
            : base(fileName)
        {
           
        }
        public override void Fill(ref Datasets.Dataset ds)
        {
            CreateSchema();
            FillData();
           

            if (idColumn < 0)
            {
                for (int i = 0; i < dataset.Count; i++)
                {
                    schema.SetId(dataset[i], i.ToString());
                }
            }
            int ind = Path.GetFileNameWithoutExtension(fileName).LastIndexOf('.');
            Debug.Assert(ind >= 0, "Invalid File Name!");

            string mlFile = fileName.Substring(0, ind + 1) + "ml.txt";
            string clFile = fileName.Substring(0, ind + 1) + "cl.txt";
            ReadConstraints(mlFile, (dataset as ConstraintedDataset).MustLinks);
            ReadConstraints(clFile, (dataset as ConstraintedDataset).CannotLinks);
            ds = dataset;
        }
        protected void ReadConstraints(string filename,Constraints.ConstraintSet set)
        {

            FileStream file = File.OpenRead(filename);
            StreamReader reader = new StreamReader(file);
            string line;
            int ind;
            bool tag = false;
            while (reader.Peek() >= 0)
            {
                line = reader.ReadLine();
                ind = line.IndexOf("#!");
                if (ind >= 0)
                {
                    tag = true;
                    break;
                }
            }
            Debug.Assert(tag, "Invalid Constraints File (No \"#!\") " + filename);

            // List<string> temp = new List<string>();
            
            int nLine = 0;
          

            while (reader.Peek() >= 0)
            {
                line = reader.ReadLine().Trim();
                //跳过空行和注释行
                if (string.IsNullOrWhiteSpace(line)||line.StartsWith("#"))
                {
                    continue;
                }

                string[] temp = line.Split(',', ' ', '\t');

                Debug.Assert(temp.Length == 2, "Invalid Schema Line :" + line);

                int id1 = schema.IdInfo.GetIdFromValue(temp[0]);
                int id2 = schema.IdInfo.GetIdFromValue(temp[1]);
                if (id1 >= 0 && id2 >= 0)
                {
                    set.Add(new Constraints.PairConstraint(dataset[id1], dataset[id2]));
                }
                else
                {
                    throw new Exception("Cannot Find the Id(s)"+line);
                }
                ++nLine;
            }

            numColumn = nLine;
            reader.Close();
            file.Close();
        }
        protected override void FillData()
        {
            FileStream file = File.OpenRead(fileName);
            StreamReader reader = new StreamReader(file);

            string line = null;
            dataset = new ConstraintedDataset(schema);


            while (reader.Peek() >= 0)
            {
                line = reader.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }
                string[] temp = line.Split(',', ' ', '\t');
                Record rec = CreateRecord(temp);
                dataset.Add(rec);
            }
        }
    }
}
