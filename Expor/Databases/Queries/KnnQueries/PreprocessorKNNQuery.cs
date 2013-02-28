using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Indexes.Preprocessed.Knn;
using Socona.Expor.Utilities.DataStructures.Heap;
using Socona.Expor.Utilities.Exceptions;
using Socona.Log;

namespace Socona.Expor.Databases.Queries.KnnQueries
{

    /**
     * Instance for a particular database, invoking the preprocessor.
     * 
     * @author Erich Schubert
     */
    public class PreprocessorKNNQuery<O> : AbstractDataBasedQuery, IKNNQuery
        where O : INumberVector
    {
        /**
     * Logging class.
     */
        private static Logging LOG = Logging.GetLogger(typeof(PreprocessorKNNQuery<O>));
        /**
         * The last preprocessor result
         */
        private AbstractMaterializeKNNPreprocessor<O> preprocessor;

        /**
         * Warn only once.
         */
        private bool warned = false;

        /**
         * Constructor.
         * 
         * @param database Database to query
         * @param preprocessor Preprocessor instance to use
         */
        public PreprocessorKNNQuery(IRelation database, AbstractMaterializeKNNPreprocessor<O> preprocessor) :
            base(database)
        {
            this.preprocessor = preprocessor;
        }

        /**
         * Constructor.
         * 
         * @param database Database to query
         * @param preprocessor Preprocessor to use
         */
        public PreprocessorKNNQuery(IRelation database, AbstractMaterializeKNNPreprocessor<O>.Factory preprocessor) :
            this(database, (AbstractMaterializeKNNPreprocessor<O>)preprocessor.Instantiate(database))
        {
        }


        public IKNNList GetKNNForDbId(IDbIdRef id, int k)
        {
            if (!warned && k > preprocessor.GetK())
            {
                LOG.Warning("Requested more neighbors than preprocessed!");
            }
            if (!warned && k < preprocessor.GetK())
            {
                IKNNList dr = preprocessor.Get(id);
                int subk = k;
                IDistanceValue kdist = dr[(subk - 1)].Distance;
                while (subk < dr.Count)
                {
                    IDistanceValue ndist = dr[(subk)].Distance;
                    if (kdist.Equals(ndist))
                    {
                        // Tie - increase subk.
                        subk++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (subk < dr.Count)
                {
                    return DbIdUtil.SubList(dr, subk);
                }
                else
                {
                    return dr;
                }
            }
            return preprocessor.Get(id);
        }


        public IList<IKNNList> GetKNNForBulkDbIds(IArrayDbIds ids, int k)
        {
            if (!warned && k > preprocessor.GetK())
            {
                LOG.Warning("Requested more neighbors than preprocessed!");
            }
            List<IKNNList> result = new List<IKNNList>(ids.Count);
            if (k < preprocessor.GetK())
            {
                //  for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {      
                foreach (var iter in ids)
                {
                    IKNNList dr = preprocessor.Get(iter);
                    int subk = k;
                    IDistanceValue kdist = dr[(subk - 1)].Distance;
                    while (subk < dr.Count)
                    {
                        IDistanceValue ndist = dr[(subk)].Distance;
                        if (kdist.Equals(ndist))
                        {
                            // Tie - increase subk.
                            subk++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (subk < dr.Count)
                    {
                        result.Add(DbIdUtil.SubList(dr, subk));
                    }
                    else
                    {
                        result.Add(dr);
                    }
                }
            }
            else
            {
                // for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {      
                foreach (var iter in ids)
                {
                    result.Add(preprocessor.Get(iter));
                }
            }
            return result;
        }


        public IKNNList GetKNNForObject(O obj, int k)
        {
            throw new AbortException("Preprocessor KNN query only supports ID queries.");
        }

        /**
         * Get the preprocessor instance.
         * 
         * @return preprocessor instance
         */
        public AbstractMaterializeKNNPreprocessor<O> GetPreprocessor()
        {
            return preprocessor;
        }






        public void GetKNNForBulkHeaps(IDictionary<IDbId, IKNNHeap> heaps)
        {
            throw new NotImplementedException();
        }

        IKNNList IKNNQuery.GetKNNForDbId(IDbIdRef id, int k)
        {
            throw new NotImplementedException();
        }

        IList<IKNNList> IKNNQuery.GetKNNForBulkDbIds(IArrayDbIds ids, int k)
        {
            throw new NotImplementedException();
        }

        void IKNNQuery.GetKNNForBulkHeaps(IDictionary<IDbId, IKNNHeap> heaps)
        {
            throw new NotImplementedException();
        }

        IKNNList IKNNQuery.GetKNNForObject(IDataVector obj, int k)
        {
            throw new NotImplementedException();
        }

        IRelation IDatabaseQuery.Relation
        {
            get { throw new NotImplementedException(); }
        }
    }
}
