using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra.Generic;
using Socona.Clustering.Datasets;
namespace Socona.Clustering.Utilities
{
    public class DatasetGenerator:DataAdapter
    {
        protected Matrix data;
        protected int seed;
        List<int> records;
        Matrix mu;
        List<Matrix> sigma;
        MathNet.Numerics.Distributions.Normal generator;

        public DatasetGenerator(Matrix mu,
            List<Matrix> sigma,
            List<int> records,
            int seed)
        {
            this.mu = mu;
            this.records = records;
            this.sigma = sigma;
            this.seed = seed;
            this.generator = new MathNet.Numerics.Distributions.Normal();
            int n = 0;
            for (int i = 0; i < records.Count; i++)
            {
                n += records[i];
            }
            data = new DenseMatrix(n, mu.ColumnCount);
        }

        public override void Fill(ref Datasets.Dataset ds)
        {
            for (int i = 0; i < records.Count; i++)
            {
                Generate(i);
            }
            Schema schema = new Schema();
            DAttriInfo labelInfo = new DAttriInfo("Label");
            DAttriInfo idInfo = new DAttriInfo("Identifier");
            schema.LabelInfo = labelInfo;
            schema.IdInfo = idInfo;

            

            for (int s = 0; s < data.ColumnCount; s++)
            {
              
                CAttrInfo cai = new CAttrInfo("Attribute"+(s+1));
                schema.Add(cai);
            }
            ds = new Dataset(schema);
            int count = 0;
            for (int i = 0; i < records.Count; i++)
            {
                string label = (i + 1).ToString();
                for (int j = 0; j < records[i]; j++)
                {
                    string id = count.ToString();
                    Record r = new Record(schema);
                    schema.SetId(r, id);
                    schema.SetLabel(r, label);

                    for (int s = 0; s < data.ColumnCount; s++)
                    {
                        r[s] = data[count, s];
                    }
                    ds.Add(r);
                    count++;
                }
            }
        }
        protected void Generate(int ind)
        {
            Matrix t = new DenseMatrix(mu.ColumnCount, mu.ColumnCount);
            int k = Chol(sigma[ind], t);
            Debug.Assert(k == 0, "Cannot Decompose Sigma");

            int start = 0;
            for (int i = 0; i < ind; i++)
            {
                start = records[i];
            }
            Vector v = new DenseVector(mu.ColumnCount);
            Vector<double> w = new DenseVector(mu.ColumnCount);

            for (int i = 0; i < records[ind]; i++)
            {
                for (int j = 0; j < v.Count; j++)
                {
                    v[j] = generator.Sample();
                }
            
                w = t.Multiply(v);
                for (int j = 0; j < v.Count; j++)
                {
                    data[start+i,j]=w[j]+mu[ind,j];
                }
        
            }
        }

        public int Chol(Matrix a, Matrix l)
        {
            Debug.Assert(a.RowCount == a.ColumnCount, "Matrix A is not Square");
            Debug.Assert(l.RowCount == l.ColumnCount, "Matrix L is not Square");
            Debug.Assert(a.RowCount == l.RowCount, "Matrix A and Matrix L have Different Dimensions");

            int n = a.RowCount;
            for (int k = 0; k < n; k++)
            {
                double ql_kk = a[k, k] - l.Row(k).DotProduct(l.Row(k));
                if (ql_kk <= 0)
                {
                    return 1 + k;
                }
                double l_kk = Math.Sqrt(ql_kk);
                l[k, k] = l_kk;

                Vector<double> clk = l.Column(k);
                Vector<double> clksub = a.Column(k).SubVector(k + 1, n-k-1) -
                  l.SubMatrix(k + 1, n-k-1, 0, k).
                  Multiply(l.Column(k)).
                  Divide(l_kk);
                for (int i = k + 1; i < n; i++)
                {
                    clk[i] = clksub[i];
                }
            }
          return 0;
        }
        
            
    }
}
