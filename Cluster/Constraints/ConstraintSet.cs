using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Datasets;
using Socona.Clustering.Clusters;

namespace Socona.Clustering.Constraints
{
    public abstract  class ConstraintSet:ISet<PairConstraint>
    {
        protected SortedSet<PairConstraint> dataset = new SortedSet<PairConstraint>();
        /// <summary>
        /// 获得将数据r分配置cls中第k个类时违反的约束个数
        /// </summary>
        /// <param name="r">查询的数据</param>
        /// <param name="cls">聚类列表</param>
        /// <param name="k">当前聚类</param>
        /// <returns></returns>
        public abstract int GetViolations(Record r, Cluster[] cls,int k);

        public bool Add(PairConstraint item)
        {
            return dataset.Add(item);
        }

        public void ExceptWith(IEnumerable<PairConstraint> other)
        {
            dataset.ExceptWith(other);
        }

        public void IntersectWith(IEnumerable<PairConstraint> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSubsetOf(IEnumerable<PairConstraint> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<PairConstraint> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<PairConstraint> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<PairConstraint> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<PairConstraint> other)
        {
            throw new NotImplementedException();
        }

        public bool SetEquals(IEnumerable<PairConstraint> other)
        {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<PairConstraint> other)
        {
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<PairConstraint> other)
        {
            throw new NotImplementedException();
        }

        void ICollection<PairConstraint>.Add(PairConstraint item)
        {
            dataset.Add(item);
        }

        public void Clear()
        {
            dataset.Clear();
        }

        public bool Contains(PairConstraint item)
        {
            return dataset.Contains(item);
        }

        public void CopyTo(PairConstraint[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return dataset.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(PairConstraint item)
        {
            return dataset.Remove(item);
        }

        public IEnumerator<PairConstraint> GetEnumerator()
        {
            return dataset.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return dataset.GetEnumerator();
        }
    }
}
