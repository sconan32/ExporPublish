using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Algorithms.Clustering;
using Socona.Expor.Algorithms.Clustering.Trivial;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;
using Socona.Expor.Databases;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Results.Outliers;
using Socona.Expor.Utilities;

namespace Socona.Expor.Results
{

    public class ResultUtil
    {
        /**
         * Collect all Annotation results from a IResult
         * 
         * @param r IResult
         * @return List of all annotation results
         */
        public static IList<IRelation> GetRelations<V>(IResult r)
        {
            if (r is IRelation)
            {
                IList<IRelation> anns = new List<IRelation>(1);
                anns.Add((IRelation)r);
                return anns;
            }
            if (r is IHierarchicalResult)
            {
                return ClassGenericsUtil.CastWithGenericsOrNull<IList<IRelation>>(typeof(IList<V>),
                    FilterResults<IRelation>((IHierarchicalResult)r, typeof(IRelation)));
            }
            return new List<IRelation>();
        }

        /**
         * Collect all ordering results from a IResult
         * 
         * @param r IResult
         * @return List of ordering results
         */
        public static IList<IOrderingResult> GetOrderingResults(IResult r)
        {
            if (r is IOrderingResult)
            {
                IList<IOrderingResult> ors = new List<IOrderingResult>(1);
                ors.Add((IOrderingResult)r);
                return ors;
            }
            if (r is IHierarchicalResult)
            {
                return FilterResults<IOrderingResult>((IHierarchicalResult)r, typeof(IOrderingResult));
            }
            return new List<IOrderingResult>();
        }

        /**
         * Collect all clustering results from a IResult
         * 
         * @param r IResult
         * @return List of clustering results
         */
        public static IList<ClusterList> GetClusteringResults(IResult r)
        {
            if (r is ClusterList)
            {
                List<ClusterList> crs = new List<ClusterList>(1);
                crs.Add((ClusterList)r);
                return crs;
            }
            if (r is IHierarchicalResult)
            {
                return ClassGenericsUtil.CastWithGenericsOrNull<IList<ClusterList>>(
                    typeof(IList<ClusterList>),
                    FilterResults<ClusterList>((IHierarchicalResult)r, typeof(ClusterList)));
            }
            return new List<ClusterList>();
        }

        /**
         * Collect all collection results from a IResult
         * 
         * @param r IResult
         * @return List of collection results
         */
        public static IList<CollectionResult<O>> GetCollectionResults<O>(IResult r)
        {
            if (r is CollectionResult<O>)
            {
                IList<CollectionResult<O>> crs = new List<CollectionResult<O>>(1);
                crs.Add((CollectionResult<O>)r);
                return crs;
            }
            if (r is IHierarchicalResult)
            {
                return ClassGenericsUtil.CastWithGenericsOrNull<IList<CollectionResult<O>>>
                    (typeof(IList<CollectionResult<O>>),
                    FilterResults<CollectionResult<O>>((IHierarchicalResult)r, typeof(CollectionResult<O>)));
            }
            return new List<CollectionResult<O>>();
        }

        /**
         * Return all Iterable results
         * 
         * @param r IResult
         * @return List of iterable results
         */
        public static IList<IIterableResult<O>> GetIterableResults<O>(IResult r)
        {
            if (r is IIterableResult<O>)
            {
                IList<IIterableResult<O>> irs = new List<IIterableResult<O>>(1);
                irs.Add((IIterableResult<O>)r);
                return irs;
            }
            if (r is IHierarchicalResult)
            {
                return ClassGenericsUtil.CastWithGenericsOrNull<IList<IIterableResult<O>>>(
                    typeof(IList<IIterableResult<O>>),
                    FilterResults<IIterableResult<O>>((IHierarchicalResult)r,
                    typeof(IIterableResult<O>)));
            }
            return new List<IIterableResult<O>>();
        }

        /**
         * Collect all outlier results from a IResult
         * 
         * @param r IResult
         * @return List of outlier results
         */
        public static IList<OutlierResult> GetOutlierResults(IResult r)
        {
            if (r is OutlierResult)
            {
                IList<OutlierResult> ors = new List<OutlierResult>(1);
                ors.Add((OutlierResult)r);
                return ors;
            }
            if (r is IHierarchicalResult)
            {
                return FilterResults<OutlierResult>((IHierarchicalResult)r, typeof(OutlierResult));
            }
            return new List<OutlierResult>();
        }

        /**
         * Collect all settings results from a IResult
         * 
         * @param r IResult
         * @return List of settings results
         */
        public static IList<SettingsResult> GetSettingsResults(IResult r)
        {
            if (r is SettingsResult)
            {
                List<SettingsResult> ors = new List<SettingsResult>(1);
                ors.Add((SettingsResult)r);
                return ors;
            }
            if (r is IHierarchicalResult)
            {
                return FilterResults<SettingsResult>((IHierarchicalResult)r, typeof(SettingsResult));
            }
            return new List<SettingsResult>();
        }

        /**
         * Return only results of the given restriction class
         * 
         * @param <C> Class type
         * @param restrictionClass Class restriction
         * @return filtered results list
         */
        // We can't ensure that restrictionClass matches C.
        //@SuppressWarnings("unchecked")
        public static IList<C> FilterResults<C>(IResult r, Type restrictionClass)
        {
            IList<C> res = new List<C>();
            if (restrictionClass.IsInstanceOfType(r))
            {
                res.Add((C)r);
            }
            if (r is IHierarchicalResult)
            {
                var childrem = (r as IHierarchicalResult).Hierarchy.Descendants(r);
                foreach (var result in childrem)
                {


                    if (restrictionClass.IsInstanceOfType(result))
                    {
                        res.Add((C)result);
                    }
                }
            }

            return res;
        }

        /**
         * Ensure that the result contains at least one ClusterList.
         * 
         * @param <O> IDatabase type
         * @param db IDatabase to process
         * @param result result
         */
        public static void EnsureClusteringResult<O>(IDatabase db, IResult result)
            where O : IModel
        {
            ICollection<ClusterList> clusterings =
                ResultUtil.FilterResults<ClusterList>(result, typeof(ClusterList));
            if (clusterings.Count == 0)
            {
                IClusteringAlgorithm split = new ByLabelOrAllInOneClustering();
                ClusterList c = (ClusterList)split.Run(db);
                AddChildResult(db, c);
            }
        }

        /**
         * Ensure that there also is a selection container object.
         * 
         * @param db IDatabase
         * @return selection result
         */
        public static SelectionResult EnsureSelectionResult(IDatabase db)
        {
            IList<SelectionResult> selections =
                ResultUtil.FilterResults<SelectionResult>(db, typeof(SelectionResult));
            if (selections.Count > 0)
            {
                return selections[(0)];
            }
            SelectionResult sel = new SelectionResult();
            AddChildResult(db, sel);
            return sel;
        }

        /**
         * Get the sampling result attached to a relation
         * 
         * @param rel Relation
         * @return Sampling result.
         */
        public static SamplingResult GetSamplingResult<V>(IRelation rel)
        {
            ICollection<SamplingResult> selections =
                ResultUtil.FilterResults<SamplingResult>(rel, typeof(SamplingResult));
            if (selections.Count == 0)
            {
                SamplingResult newsam = new SamplingResult((IRelation)rel);
                AddChildResult(rel, newsam);
                return newsam;
            }
            return selections.ElementAt(0);
        }

        /**
         * Get (or create) a scales result for a relation.
         * 
         * @param rel Relation
         * @return associated scales result
         */
        public static ScalesResult GetScalesResult(IRelation rel)
        {
            ICollection<ScalesResult> scas =
                ResultUtil.FilterResults<ScalesResult>(rel, typeof(ScalesResult));
            if (scas.Count == 0)
            {
                ScalesResult newsca = new ScalesResult(rel);
                AddChildResult(rel, newsca);
                return newsca;
            }
            return scas.GetEnumerator().Current;
        }

        /**
         * Add a child result.
         * 
         * @param parent Parent
         * @param child Child
         */
        public static void AddChildResult(IHierarchicalResult parent, IResult child)
        {
            parent.Hierarchy.Add(parent, child);
        }

        /**
         * Find the first database result in the tree.
         * 
         * @param baseResult IResult tree base.
         * @return IDatabase
         */
        public static IDatabase FindDatabase(IResult baseResult)
        {
            IList<IDatabase> dbs = FilterResults<IDatabase>(baseResult, typeof(IDatabase));
            if (dbs.Count > 0)
            {
                return dbs[0];
            }
            else
            {
                return null;
            }
        }

        /**
         * Recursively remove a result and its children.
         * 
         * @param hierarchy IResult hierarchy
         * @param child IResult to remove
         */
        public static void RemoveRecursive(ResultHierarchy hierarchy, IResult child)
        {
            foreach (IResult parent in hierarchy.GetParents(child))
            {
                hierarchy.Remove(parent, child);
            }
            foreach (IResult sub in hierarchy.GetChildren(child))
            {
                RemoveRecursive(hierarchy, sub);
            }
        }

    }
}
