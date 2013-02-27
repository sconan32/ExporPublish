using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Datasets;
using System.Diagnostics;
using System.IO;

namespace Socona.Clustering.Utilities
{
    public class DatasetReader:DataAdapter
    {
        protected int labelColumn;
        protected int idColumn;
        protected int numColumn;

        protected string fileName;
        protected Schema schema;
        protected Dataset dataset;
        public DatasetReader(string fileName)
        {
            this.fileName = fileName;
            labelColumn = -1;
            idColumn = -1;

            numColumn = 0;

        }
        public override void Fill(ref Dataset ds)
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
            ds = dataset;
        }
        protected virtual void FillData()
        {
            FileStream file = File.OpenRead(fileName);
            StreamReader reader = new StreamReader(file);

            string line = null;
            dataset = new Dataset(schema);

           
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
        protected void CreateSchema()
        {
            int ind = Path.GetFileNameWithoutExtension( fileName).LastIndexOf('.');
            Debug.Assert(ind >= 0, "Invalid File Name!");

            string schemaFile = fileName.Substring(0, ind + 1) + "names.txt";

            FileStream file = File.OpenRead(schemaFile);
            StreamReader reader=new StreamReader (file);
            string line;
            bool tag = false;
            while (reader.Peek() >= 0)
            {
                line = reader.ReadLine();
                ind = line.IndexOf("///:");
                if (ind >= 0)
                {
                    tag = true;
                    break;
                }
            }
            Debug.Assert(tag, "Invalid Names File (No ///:) " + schemaFile);

           // List<string> temp = new List<string>();
            schema = new Schema();
            schema.IdInfo = new DAttriInfo("Identifier");

            int nLine = 0;
            bool bClass = false;
            bool bId = false;

            while (reader.Peek() >= 0)
            {
                line = reader.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }
                string[] temp = line.Split(',', ' ', '\t');
                
                Debug.Assert(temp.Length == 2, "Invalid Schema Line :" + line);

                if (temp[1] == "Class")
                {
                    if (!bClass)
                    {
                        schema.LabelInfo = new DAttriInfo("Membership");
                        bClass = true;
                        labelColumn = nLine;
                    }
                    else
                    {
                        throw new Exception("Schema Cannot Have Two CLASS Columns");
                    }
                }
                else if (temp[1] == "Continuous")
                {
                    schema.Add(new CAttrInfo(temp[0]));
                }
                else if (temp[1] == "Discrete")
                {
                    schema.Add(new DAttriInfo(temp[0]));
                }
                else if (temp[1] == "RecordID")
                {
                    if (!bId)
                    {
                        bId = true;
                        idColumn = nLine;
                    }
                    else
                    {
                        throw new Exception("Schema Cannot Have Two ID columns");
                    }
                }
                else
                {
                    throw new Exception("Invalid Type " + temp[1] + " Note That Type Name is Case Sensitive");
                }
                ++nLine;
            }

            numColumn = nLine;
            reader.Close();
            file.Close();
        }
        protected Record CreateRecord(string[] val)
        {
            Record rec = new Record(schema);
            Debug.Assert(numColumn == val.Length, "Size Does Not Match.");

            string label=null, id=null;
            int j = 0;
            int s;
            for (int i = 0; i < val.Length; i++)
            {
                if (i == labelColumn)
                {
                    label = val[i];
                    continue;
                }
                else if (i==idColumn)
                {
                    id=val[i];
                    continue;
                }
                switch (schema[j].Type)
                {
                    case AttrType.Continuous:
                        double value = 0;
                        double.TryParse(val[i], out value);
                        //(schema[j] as CAttrInfo).SetContinousValue(rec[j], value);
                        rec[j] = value;
                        break;
                    case AttrType.Discrete:
                        s = (schema[j] as DAttriInfo).AddValue(val[i], true);
                        //(schema[j] as DAttriInfo).SetDiscreteValue(rec[j], s);
                        rec[j] = s;
                        break;
                }
                ++j;
            }
            if (labelColumn >= 0)
            {
                schema.SetLabel(rec, label);
            }
            if (idColumn >= 0)
            {
                schema.SetId(rec, id);
            }
            return rec;

        
        }

    }
}
