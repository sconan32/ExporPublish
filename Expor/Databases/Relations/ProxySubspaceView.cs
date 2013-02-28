using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.DataStore;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.Relations
{
    class ProxySubspaceView : ProxyView
    {
        int[] dimmask;
      //  bool isBuffered;
        SortedDictionary<IDbIdRef, IDataVector> bufferedStore;
       // IDbIds bufferId;
        VectorFieldTypeInformation<INumberVector> type;
        public ProxySubspaceView(IDatabase database, IDbIds idview, IRelation inner, int[] dimmask)
            : base(database, idview, inner)
        {
            this.dimmask = dimmask;
            //this.isBuffered = true;
            int dim = 0;
            for (int mi = 0; mi < dimmask.Length; mi++)
            {
                if (dimmask[mi] >= 0) dim++;
            }
            type = new VectorFieldTypeInformation<INumberVector>(
             typeof(DoubleVector), dim, new DoubleVector(new double[dim]));
            bufferedStore = new SortedDictionary<IDbIdRef, IDataVector>();
        }

        public override object this[IDbIdRef id]
        {
            get
            {
                Debug.Assert(idview.Contains(id), "Accessing object not included in view.");

                if (bufferedStore.ContainsKey(id))
                {
                    return bufferedStore[id];
                }
                else
                {
                    INumberVector origin = inner[id] as INumberVector;

                    double[] target = new double[type.Dimensionality()];
                    for (int i = 0; i < dimmask.Length; i++)
                    {
                        if (dimmask[i] >= 0)
                        {
                            target[dimmask[i]] = (origin)[i];
                        }
                    }

                    INumberVector rres = type.GetFactory().NewNumberVector(target);
                    bufferedStore.Add(id, rres);
                    return rres;
                }
            }
            set
            {
                throw new InvalidOperationException("You can't alter database through DataView");
            }
        }
        public override IDataVector DataAt(IDbIdRef id)
        {
            return this[id] as IDataVector;
        }
        public override INumberVector VectorAt(IDbIdRef id)
        {
            return this[id] as INumberVector;
        }
        public override SimpleTypeInformation GetDataTypeInformation()
        {
            return type;
        }
    }
}
