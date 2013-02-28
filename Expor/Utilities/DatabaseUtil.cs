using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Utilities
{

    public sealed class DatabaseUtil : IEnumerable
    {

        /// <summary>
        /// Get the dimensionality of a database
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="relation"></param>
        /// <returns></returns>
        public static VectorFieldTypeInformation<V> AssumeVectorField<V>(IRelation relation)
        where V : INumberVector
        {
            try
            {
                return ((VectorFieldTypeInformation<V>)relation.GetDataTypeInformation());
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Expected a vector field, got type information: " +
                    relation.GetDataTypeInformation().ToString());
            }
        }

        /**
         * Get the dimensionality of a database
         * 
         * @param relation relation
         * @return Database dimensionality
         */
        public static int Dimensionality(IRelation relation)
        {
            try
            {
                return ((VectorFieldTypeInformation<INumberVector>)relation.GetDataTypeInformation()).Dimensionality();
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /**
         * Returns the centroid as a NumberVector object of the specified database.
         * The objects must be instance of <code>NumberVector</code>.
         * 
         * @param <V> Vector type
         * @param relation the IRelation storing the objects
         * @return the centroid of the specified objects stored in the given database
         * @throws IllegalArgumentException if the database is empty
         */
        public static V Centroid<V>(IRelation relation) where V : INumberVector
        {
            return Maths.LinearAlgebra.Centroid.Make<V>(relation).ToVector<V>(relation);
        }

        /**
         * Returns the centroid as a NumberVector object of the specified objects
         * stored in the given database. The objects belonging to the specified ids
         * must be instance of <code>NumberVector</code>.
         * 
         * @param <V> Vector type
         * @param relation the relation
         * @param ids the ids of the objects
         * @return the centroid of the specified objects stored in the given database
         * @throws IllegalArgumentException if the id list is empty
         */



        public static V Centroid<V>(IRelation relation, IDbIds ids) where V : INumberVector
        {
            return Maths.LinearAlgebra.Centroid.Make<V>(relation, ids).ToVector<V>(relation);
        }

        /**
         * Returns the centroid w.r.t. the dimensions specified by the given BitArray as
         * a NumberVector object of the specified objects stored in the given
         * database. The objects belonging to the specified IDs must be instance of
         * <code>NumberVector</code>.
         * 
         * @param <V> Vector type
         * @param relation the database storing the objects
         * @param ids the identifiable objects
         * @param dimensions the BitArray representing the dimensions to be considered
         * @return the centroid of the specified objects stored in the given database
         *         w.r.t. the specified subspace
         * @throws IllegalArgumentException if the id list is empty
         */
        public static V Centroid<V>(IRelation relation, IDbIds ids, BitArray dimensions) where V : INumberVector
        {
            return ProjectedCentroid.Make<V>(dimensions, relation, ids).ToVector<V>(relation);
        }

        /**
         * Determines the covariance matrix of the objects stored in the given
         * database.
         * 
         * @param <V> Vector type
         * @param database the database storing the objects
         * @param ids the ids of the objects
         * @return the covariance matrix of the specified objects
         */
        public static Matrix CovarianceMatrix(IRelation database, IDbIds ids)
        {
            return Maths.LinearAlgebra.CovarianceMatrix.Make(database, ids).DestroyToNaiveMatrix();
        }

        /**
         * Determines the d x d covariance matrix of the given n x d data matrix.
         * 
         * @param data the database storing the objects
         * @return the covariance matrix of the given data matrix.
         */
        public static Matrix CovarianceMatrix(Matrix data)
        {
            return Maths.LinearAlgebra.CovarianceMatrix.Make(data).DestroyToNaiveMatrix();
        }

        /**
         * Determines the variances in each dimension of all objects stored in the
         * given database.
         * 
         * @param database the database storing the objects
         * @return the variances in each dimension of all objects stored in the given
         *         database
         */
        public static double[] Variances(IRelation database)
        {
            INumberVector centroid = Centroid<INumberVector>(database);
            double[] variances = new double[centroid.Count];

            for (int d = 1; d <= centroid.Count; d++)
            {
                double mu = centroid[d];
                foreach (IDbId id in database.GetDbIds())
                {
                    //for(DbIdIter it = database.iterDbIds(); it.valid(); it.advance()) {
                    INumberVector o = database[id] as INumberVector;
                    double diff = o[d] - mu;
                    variances[d - 1] += diff * diff;
                }

                variances[d - 1] /= database.Count;
            }
            return variances;
        }

        /**
         * Determines the variances in each dimension of the specified objects stored
         * in the given database. Returns
         * <code>variances(database, centroid(database, ids), ids)</code>
         * 
         * @param database the database storing the objects
         * @param ids the ids of the objects
         * @return the variances in each dimension of the specified objects
         */
        public static double[] Variances(IRelation database, IDbIds ids)
        {
            return Variances(database, Centroid<INumberVector>(database, ids), ids);
        }

        /**
         * Determines the variances in each dimension of the specified objects stored
         * in the given database.
         * 
         * @param database the database storing the objects
         * @param ids the ids of the objects
         * @param centroid the centroid or reference vector of the ids
         * @return the variances in each dimension of the specified objects
         */
        public static double[] Variances(IRelation database, INumberVector centroid, IDbIds ids)
        {
            double[] variances = new double[centroid.Count];

            for (int d = 1; d <= centroid.Count; d++)
            {
                double mu = centroid[d];
                foreach (IDbId id in ids)
                {
                    //for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
                    INumberVector o = (INumberVector)database[id];
                    double diff = o[d] - mu;
                    variances[d - 1] += diff * diff;
                }

                variances[d - 1] /= ids.Count;
            }
            return variances;
        }

        /**
         * Determines the minimum and maximum values in each dimension of all objects
         * stored in the given database.
         * 
         * @param <NV> vector type
         * @param database the database storing the objects
         * @return Minimum and Maximum vector for the hyperrectangle
         */
        public static IPair<NV, NV> ComputeMinMax<NV>(IRelation database) where NV : INumberVector
        {
            int dim = Dimensionality(database);
            double[] mins = new double[dim];
            double[] maxs = new double[dim];
            for (int i = 0; i < dim; i++)
            {
                mins[i] = Double.MaxValue;
                maxs[i] = -Double.MaxValue;
            }
            foreach (IDbId id in database.GetDbIds())
            {
                //for(DbIdIter iditer = database.iterDbIds(); iditer.valid(); iditer.advance()) {
                NV o = (NV)database[id];
                for (int d = 0; d < dim; d++)
                {
                    double v = o[d];
                    mins[d] = Math.Min(mins[d], v);
                    maxs[d] = Math.Max(maxs[d], v);
                }
            }
            NV factory = AssumeVectorField<NV>(database).GetFactory();
            NV min = (NV)factory.NewNumberVector(mins);
            NV max = (NV)factory.NewNumberVector(maxs);
            return new Pair<NV, NV>(min, max);
        }

        /**
         * Returns the median of a data set in the given dimension by using a sampling
         * method.
         * 
         * @param relation IRelation to process
         * @param ids IDbIds to process
         * @param dimension Dimensionality
         * @param numberOfSamples Number of samples to draw
         * @return Median value
         */
        public static double QuickMedian(IRelation relation,
            IArrayDbIds ids, int dimension, int numberOfSamples)
        {
            int everyNthItem = (int)Math.Max(1, Math.Floor(ids.Count / (double)numberOfSamples));
            double[] vals = new double[numberOfSamples];
            for (int i = 0; i < numberOfSamples; i++)
            {
                IDbId id = ids[i * everyNthItem];
                vals[i] = (relation[id] as INumberVector)[dimension];
            }
            Array.Sort(vals);
            if (vals.Length % 2 == 1)
            {
                return vals[((vals.Length + 1) / 2) - 1];
            }
            else
            {
                double v1 = vals[vals.Length / 2];
                double v2 = vals[(vals.Length / 2) - 1];
                return (v1 + v2) / 2.0;
            }
        }

        /**
         * Returns the median of a data set in the given dimension.
         * 
         * @param relation IRelation to process
         * @param ids IDbIds to process
         * @param dimension Dimensionality
         * @return Median value
         */
        public static double ExactMedian(IRelation relation, IDbIds ids, int dimension)
        {
            double[] vals = new double[ids.Count];
            int i = 0;
            foreach (IDbId id in ids)
            {
                //for (DbIdIter iter = ids.iter(); iter.valid(); iter.advance()) {
                vals[i] = (relation[id] as INumberVector)[dimension];
                i++;
            }
            Array.Sort(vals);
            if (vals.Length % 2 == 1)
            {
                return vals[((vals.Length + 1) / 2) - 1];
            }
            else
            {
                double v1 = vals[vals.Length / 2];
                double v2 = vals[(vals.Length / 2) - 1];
                return (v1 + v2) / 2.0;
            }
        }

        /**
         * Guess a potentially label-like representation, preferring class labels.
         * 
         * @param database
         * @return string representation
         */
        public static IRelation GuessLabelRepresentation(IDatabase database)
        {
            try
            {
                IRelation classrep = database.GetRelation(TypeUtil.CLASSLABEL);
                if (classrep != null)
                {
                    return new ConvertToStringView<ClassLabel>(classrep);
                }
            }
            catch (NoSupportedDataTypeException)
            {
                // retry.
            }
            try
            {
                IRelation labelsrep = database.GetRelation(TypeUtil.LABELLIST);
                if (labelsrep != null)
                {
                    return new ConvertToStringView<LabelList>(labelsrep);
                }
            }
            catch (NoSupportedDataTypeException)
            {
                // retry.
            }
            try
            {
                IRelation stringrep = database.GetRelation(TypeUtil.STRING);
                if (stringrep != null)
                {
                    return stringrep;
                }
            }
            catch (NoSupportedDataTypeException)
            {
                // retry.
            }
            throw new NoSupportedDataTypeException("No label-like representation was found.");
        }

        /**
         * Guess a potentially object label-like representation.
         * 
         * @param database
         * @return string representation
         */
        public static IRelation GuessObjectLabelRepresentation(IDatabase database)
        {
            try
            {
                IRelation labelsrep = database.GetRelation(TypeUtil.LABELLIST);
                if (labelsrep != null)
                {
                    return new ConvertToStringView<LabelList>(labelsrep);
                }
            }
            catch (NoSupportedDataTypeException)
            {
                // retry.
            }
            try
            {
                IRelation stringrep = database.GetRelation(TypeUtil.STRING);
                if (stringrep != null)
                {
                    return stringrep;
                }
            }
            catch (NoSupportedDataTypeException)
            {
                // retry.
            }
            try
            {
                IRelation classrep = database.GetRelation(TypeUtil.CLASSLABEL);
                if (classrep != null)
                {
                    return new ConvertToStringView<ClassLabel>(classrep);
                }
            }
            catch (NoSupportedDataTypeException)
            {
                // retry.
            }
            throw new NoSupportedDataTypeException("No label-like representation was found.");
        }

        /**
         * Retrieves all class labels within the database.
         * 
         * @param database the database to be scanned for class labels
         * @return a set comprising all class labels that are currently set in the
         *         database
         */
        public static SortedSet<ClassLabel> GetClassLabels(IRelation database)
        {
            SortedSet<ClassLabel> labels = new SortedSet<ClassLabel>();
            foreach (IDbId id in database.GetDbIds())
            {
                //for(DbIdIter it = database.iterDbIds(); it.valid(); it.advance()) {
                labels.Add((ClassLabel)database[id]);
            }
            return labels;
        }

        /**
         * Retrieves all class labels within the database.
         * 
         * @param database the database to be scanned for class labels
         * @return a set comprising all class labels that are currently set in the
         *         database
         */
        public static SortedSet<ClassLabel> GetClassLabels(IDatabase database)
        {
            IRelation relation = database.GetRelation(TypeUtil.CLASSLABEL);
            return GetClassLabels(relation);
        }

        /**
         * Do a cheap guess at the databases object class.
         * 
         * @param <O> Restriction type
         * @param database Database
         * @return Class of first object in the Database.
         */

        public static Type GuessObjectClass<O>(IRelation database)
        {
            return (Type)database[(database.GetDbIds().ElementAt(0))].GetType();
        }

        /**
         * Do a full inspection of the database to find the base object class.
         * 
         * Note: this can be an abstract class or interface!
         * 
         * TODO: Implement a full search for shared superclasses. But since currently
         * the databases will always use only once class, this is not yet implemented.
         * 
         * @param <O> Restriction type
         * @param database Database
         * @return Superclass of all objects in the database
         */
        public static Type GetBaseObjectClassExpensive<O>(IRelation database)
        {
            List<Type> candidates = new List<Type>();
            var iditer = database.GetEnumerator();
            // empty database?!
            if (iditer == null)
            {
                return null;
            }
            // put first class into result set.
            candidates.Add(database[(iditer.Current)].GetType());
            iditer.MoveNext();
            // other objects
            for (; iditer != null; iditer.MoveNext())
            {
                Type newcls = database[(iditer.Current)].GetType();
                // validate all candidates
                for (int i = 0; i < candidates.Count; i++)
                {

                    Type cand = candidates[i];
                    if (cand.IsAssignableFrom(newcls))
                    {
                        continue;
                    }
                    // TODO: resolve conflicts by finding all superclasses!
                    // Does this code here work?
                    foreach (Type interf in cand.GetInterfaces())
                    {
                        candidates.Add(interf);
                    }
                    candidates.Add(cand.BaseType);
                    candidates.RemoveAt(i);
                }

            }
            // if we have any candidates left ...
            if (candidates != null && candidates.Count > 0)
            {
                // remove subclasses
                for (int ii = 0; ii < candidates.Count; ii++)
                {
                    //Iterator<Type> ci = candidates.iterator();
                    //while(ci.hasNext()) {
                    Type cand = candidates[ii];
                    foreach (Type oc in candidates)
                    {
                        if (oc != cand && cand.IsAssignableFrom(oc))
                        {
                            candidates.Remove(cand);
                            break;
                        }
                    }
                }
                Debug.Assert(candidates.Count > 0);
                try
                {
                    return candidates[0];
                }
                catch (Exception)
                {
                    // ignore, and retry with next
                }
            }
            // no resulting class.
            return null;
        }

        /**
         * Find object by matching their labels.
         * 
         * @param database Database to search in
         * @param name_pattern Name to match against class or object label
         * @return found cluster or it throws an exception.
         */
        public static IArrayModifiableDbIds GetObjectsByLabelMatch(IDatabase database, Regex name_pattern)
        {
            IRelation relation = GuessLabelRepresentation(database);
            if (name_pattern == null)
            {
                return DbIdUtil.NewArray();
            }
            IArrayModifiableDbIds ret = DbIdUtil.NewArray();
            foreach (IDbId id in relation.GetDbIds())
            {
                //for(DbIdIter iditer = relation.iterDbIds(); iditer.valid(); iditer.advance()) {
                if (name_pattern.IsMatch(relation[id].ToString()))
                {
                    ret.Add(id);
                }
            }
            return ret;
        }

        /**
         * An ugly vector type cast unavoidable in some situations due to Generics.
         * 
         * @param <V> Base vector type
         * @param <T> Derived vector type (is actually V, too)
         * @param database Database
         * @return Database
         */

        public static IRelation RelationUglyVectorCast<V, T>(IRelation database)
        {
            return (IRelation)database;
        }

        /**
         * Get the column name or produce a generic label "Column XY".
         * 
         * @param rel IRelation
         * @param col Column
         * @return Label
         */
        public static String GetColumnLabel(IRelation rel, int col)
        {
            String lbl = AssumeVectorField<INumberVector>(rel).GetLabel(col);
            if (lbl != null)
            {
                return lbl;
            }
            else
            {
                return "Column " + col;
            }
        }

        /**
         * Iterator class that retrieves the given objects from the database.
         * 
         * @author Erich Schubert
         */
        //public static class RelationObjectIterator<O> implements Iterator<O> {
        //  /**
        //   * The real iterator.
        //   */
        //   DbIdIter iter;

        //  /**
        //   * The database we use
        //   */
        //   IRelation database;

        //  /**
        //   * Full Constructor.
        //   * 
        //   * @param iter Original iterator.
        //   * @param database Database
        //   */
        //  public RelationObjectIterator(DbIdIter iter, IRelation database) {
        //    base();
        //    this.iter = iter;
        //    this.database = database;
        //  }

        //  /**
        //   * Simplified constructor.
        //   * 
        //   * @param database Database
        //   */
        //  public RelationObjectIterator(IRelation database) {
        //    base();
        //    this.database = database;
        //    this.iter = database.iterDbIds();
        //  }

        //  
        //  public bool hasNext() {
        //    return iter.valid();
        //  }

        //  
        //  public O next() {
        //    O ret = database.Get(iter);
        //    iter.advance();
        //    return ret;
        //  }

        //  
        //  public void remove() {
        //    throw new UnsupportedOperationException();
        //  }
        //}

        /**
         * Collection view on a database that retrieves the objects when needed.
         * 
         * @author Erich Schubert
         */
        public class CollectionFromRelation<O> : ICollection<O> where O : IDbId
        {
            /**
             * The database we query
             */
            IRelation db;

            /**
             * Constructor.
             * 
             * @param db Database
             */
            public CollectionFromRelation(IRelation db)
            {

                this.db = db;
            }



            public void Add(O item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(O item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(O[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            public bool Remove(O item)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<O> GetEnumerator()
            {
                return (IEnumerator<O>)db.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return db.GetEnumerator();
            }
        }


        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
