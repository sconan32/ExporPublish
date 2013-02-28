using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Hierarchy
{

    /**
     * Centralized hierarchy implementation, using a HashMap of Lists.
     * 
     * @author Erich Schubert
     * 
     * @param <O> Object type (arbitrary!)
     */
    public class HashMapHierarchy<O> : IModifiableHierarchy<O>
    {
        /**
         * Reference storage.
         */
        private Dictionary<O, Rec> graph;

        /**
         * Constructor.
         */
        public HashMapHierarchy()
        {

            this.graph = new Dictionary<O, Rec>();
        }


        public int Size()
        {
            return graph.Count;
        }


        public void Add(O parent, O child)
        {
            // Add child to parent.
            {
                Rec rec = graph[(parent)];
                if (rec == null)
                {
                    rec = new Rec();
                    graph[parent] = rec;
                }
                rec.AddChild(child);
            }
            // Add child to parent
            {
                Rec rec = graph[(child)];
                if (rec == null)
                {
                    rec = new Rec();
                    graph[child] = rec;
                }
                rec.AddParent(parent);
            }
        }


        public void Add(O entry)
        {
            Rec rec;
            graph.TryGetValue(entry, out rec);
            if (rec == null)
            {
                rec = new Rec();
                graph[entry] = rec;
            }
        }


        public void Remove(O parent, O child)
        {
            // Remove child from parent.
            {
                Rec rec = graph[(parent)];
                if (rec != null)
                {
                    rec.RemoveChild(child);
                }
            }
            // Remove parent from child
            {
                Rec rec = graph[(child)];
                if (rec != null)
                {
                    rec.RemoveParent(parent);
                }
            }
        }


        public void Remove(O entry)
        {
            Rec rec = graph[(entry)];
            if (rec == null)
            {
                return;
            }
            for (int i = 0; i < rec.Nump; i++)
            {
                graph[rec.Parents[i]].RemoveChild(entry);
                rec.Parents[i] = default(O);
            }
            for (int i = 0; i < rec.Numc; i++)
            {
                graph[(rec.Children[i])].RemoveParent(entry);
                rec.Children[i] = default(O);
            }
            graph.Remove(entry);
        }


        public void RemoveSubtree(O entry)
        {
            Rec rec = graph[(entry)];
            if (rec == null)
            {
                return;
            }
            for (int i = 0; i < rec.Nump; i++)
            {
                graph[rec.Parents[i]].RemoveChild(entry);
                rec.Parents[i] = default(O);
            }
            for (int i = 0; i < rec.Numc; i++)
            {
                Rec crec = graph[(rec.Children[i])];
                crec.RemoveParent(entry);
                if (crec.Nump == 0)
                {
                    RemoveSubtree((O)rec.Children[i]);
                }
                rec.Children[i] = default(O);
            }
        }


        public int NumChildren(O obj)
        {
            Rec rec = graph[(obj)];
            if (rec == null)
            {
                return 0;
            }
            return rec.Numc;
        }


        public IEnumerator<O> IterChildren(O obj)
        {
            Rec rec = graph[(obj)];
            if (rec == null)
            {
                return null;
            }
            return rec.IterChildren();
        }


        public IEnumerator<O> IterDescendants(O obj)
        {
            return new ItrDesc(this, obj);
        }


        public int NumParents(O obj)
        {
            Rec rec = graph[(obj)];
            if (rec == null)
            {
                return 0;
            }
            return rec.Nump;
        }


        public IEnumerator<O> IterParents(O obj)
        {
            Rec rec = graph[(obj)];
            if (rec == null)
            {
                return null;
            }
            return rec.IterParents();
        }


        public IEnumerator<O> IterAncestors(O obj)
        {
            return new ItrAnc(this, obj);
        }


        public IEnumerator<O> IterAll()
        {
            return new ItrAll(this);
        }

        /**
         * Hierarchy pointers for an object.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         * 
         * @param <O> object type
         */
        private class Rec
        {
            /**
             * Number of parents, number of children.
             */
            int nump = 0, numc = 0;

            public int Numc
            {
                get { return numc; }
                set { numc = value; }
            }

            public int Nump
            {
                get { return nump; }
                set { nump = value; }
            }

            /**
             * Parents.
             */
            O[] parents = null;

            public O[] Parents
            {
                get { return parents; }
                set { parents = value; }
            }

            /**
             * Children.
             */
            O[] children = null;

            public O[] Children
            {
                get { return children; }
                set { children = value; }
            }

            /**
             * Add a parent.
             * 
             * @param parent Parent to add.
             */
            public void AddParent(O parent)
            {
                if (parents == null)
                {
                    parents = new O[1];
                    parents[0] = parent;
                    nump = 1;
                }
                else
                {
                    for (int i = 0; i < nump; i++)
                    {
                        if (parent.Equals(parents[i]))
                        {
                            return;
                        }
                    }
                    if (parents.Length == nump)
                    {
                        int newsize = Math.Min(5, (parents.Length << 1) + 1);

                        O[] nparents = new O[newsize];
                        Array.Copy(parents, nparents, parents.Length);
                        this.parents = nparents;
                    }
                    parents[nump] = parent;
                    nump++;
                }
            }

            /**
             * Add a child.
             * 
             * @param child Child to add
             */
            public void AddChild(O child)
            {
                if (children == null)
                {
                    children = new O[5];
                    children[0] = child;
                    numc = 1;
                }
                else
                {
                    for (int i = 0; i < numc; i++)
                    {
                        if (child.Equals(children[i]))
                        {
                            return;
                        }
                    }
                    if (children.Length == numc)
                    {
                        O [] nc=new O[ (children.Length << 1) + 1];
                        Array.Copy(children,nc,children.Length);
                        children = nc;
                    }
                    children[numc] = child;
                    numc++;
                }
            }

            /**
             * Remove a parent.
             * 
             * @param parent Parent to remove.
             */
            public void RemoveParent(O parent)
            {
                if (parents == null)
                {
                    return;
                }
                for (int i = 0; i < nump; i++)
                {
                    if (parent.Equals(parents[i]))
                    {
                        Array.Copy(parents, i + 1, parents, i, nump - 1 - i);
                        parents[nump] = default(O);
                        nump--;
                        break;
                    }
                }
                if (nump == 0)
                {
                    parents = null;
                }
            }

            /**
             * Remove a child.
             * 
             * @param child Child to remove.
             */
            public void RemoveChild(O child)
            {
                if (children == null)
                {
                    return;
                }
                for (int i = 0; i < numc; i++)
                {
                    if (child.Equals(children[i]))
                    {
                        Array.Copy(children, i + 1, children, i, numc - 1 - i);
                        children[numc] = default(O);
                        numc--;
                        break;
                    }
                }
                if (numc == 0)
                {
                    children = null;
                }
            }

            /**
             * Iterate over parents.
             * 
             * @return Iterator for parents.
             */

            public IEnumerator<O> IterParents()
            {
                if (nump == 0)
                {
                    return null;
                }
                return new ItrParents(this);
            }

            /**
             * Iterate over parents.
             * 
             * @return Iterator for parents.
             */

            public IEnumerator<O> IterChildren()
            {
                if (numc == 0)
                {
                    return null;
                }
                return new ItrChildren(this);
            }

            /**
             * Parent iterator.
             * 
             * @author Erich Schubert
             * 
             * @apiviz.exclude
             */
            class ItrParents : IEnumerator<O>
            {
                Rec record;
                int pos = 0;

            //    O current;

                public ItrParents(Rec r)
                {
                    this.record = r;
                }
                public O Current
                {
                    get { return (O)record.Parents[pos]; }
                }

                public void Dispose()
                { }

                object System.Collections.IEnumerator.Current
                {
                    get { return record.parents[pos]; }
                }

                public bool MoveNext()
                {
                    if (++pos < record.Nump)
                    {
                        return true;
                    }
                    return false;
                }

                public void Reset()
                { }
            }

            /**
             * Child iterator.
             * 
             * @author Erich Schubert
             * 
             * @apiviz.exclude
             */
            class ItrChildren : IEnumerator<O>
            {
                Rec record;
                int pos = 0;


                public ItrChildren(Rec r)
                {
                    this.record = r;

                }

                public O Current
                {
                    get { return (O)record.children[pos]; }
                }

                public void Dispose()
                { }

                object System.Collections.IEnumerator.Current
                {
                    get { return record.children[pos]; }
                }

                public bool MoveNext()
                {
                    if (++pos < record.Numc)
                    { return true; }
                    return false;
                }

                public void Reset()
                { }
            }
        }

        /**
         * Iterator to collect into the descendants.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        private class ItrDesc : IEnumerator<O>
        {
            HashMapHierarchy<O> h;
            /**
             * Iterator over children
             */
            IEnumerator<O> childiter;

            /**
             * Iterator of current child
             */
            IEnumerator<O> subiter = null;

            O current;
            /**
             * Starting element.
             * 
             * @param start
             */
            public ItrDesc(HashMapHierarchy<O> h, O start)
            {
                this.h = h;
                childiter = h.IterChildren(start);
            }
            public O Current
            {
                get { return current; }
            }

            public void Dispose()
            { }

            object System.Collections.IEnumerator.Current
            {
                get { return current; }
            }

            public bool MoveNext()
            {
                if (subiter != null && subiter.MoveNext())
                {
                    this.current = subiter.Current;
                    return true;
                }
                else if (childiter.MoveNext())
                {
                    // Not yet descended
                    //Debug.Assert(childiter.valid());
                    this.current = childiter.Current;
                    subiter = h.IterDescendants(childiter.Current);
                    return true;
                }
                return false;


            }

            public void Reset()
            { }
        }

        /**
         * Iterator over all Ancestors.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        private class ItrAnc : IEnumerator<O>
        {
            HashMapHierarchy<O> h;
            /**
             * Iterator over children
             */
            IEnumerator<O> parentiter;

            /**
             * Iterator of current child
             */
            IEnumerator<O> subiter = null;
            O current;
            /**
             * Starting element.
             * 
             * @param start
             */
            public ItrAnc(HashMapHierarchy<O> h, O start)
            {
                this.h = h;
                parentiter = h.IterParents(start);
            }

            public O Current
            {
                get { return current; }
            }

            public void Dispose()
            { }

            object System.Collections.IEnumerator.Current
            {
                get { return current; }
            }

            public bool MoveNext()
            {
                if (subiter != null && subiter.MoveNext())
                {
                    // Continue with subtree
                    this.current = subiter.Current;

                }
                else if (parentiter.MoveNext())
                {  // Not yet descended
                    // assert(parentiter.valid());
                    subiter = h.IterAncestors(parentiter.Current);
                    this.current = parentiter.Current;
                    return true;
                }
                return false;
            }

            public void Reset()
            { }
        }

        /**
         * Iterator over all members of the hierarchy.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public class ItrAll : IEnumerator<O>
        {
            Dictionary<O, Rec> graph;
            /**
             * The true iterator.
             */
            IEnumerator<O> iter;

            /**
             * Current object.
             */
            O cur = default(O);

            /**
             * Constructor.
             */
            public ItrAll(HashMapHierarchy<O> h)
            {
                this.graph = h.graph;
                iter = graph.Keys.GetEnumerator();

            }

            public O Current
            {
                get { return cur; }
            }

            public void Dispose()
            { }

            object System.Collections.IEnumerator.Current
            {
                get { return cur; }
            }

            public bool MoveNext()
            {
                if (iter.MoveNext())
                {
                    cur = iter.Current;
                    return true;
                }
                return false;
            }

            public void Reset()
            { }
        }

        /**
         * Empty iterator.
         */
        //private static final Iter<?> EMPTY_ITERATOR = new Iter<Object>() {
        //  @Override
        //  public boolean valid() {
        //    return false;
        //  }

        //  @Override
        //  public void advance() {
        //    throw new UnsupportedOperationException("Empty iterators must not be advanced.");
        //  }

        //  @Override
        //  public Object get() {
        //    throw new UnsupportedOperationException("Iterator is empty.");
        //  }
        //};


        public IList<O> GetChildren(O self)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<O> Descendants(O self)
        {
            throw new NotImplementedException();
        }

        public IList<O> GetParents(O self)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<O> Ancestors(O self)
        {
            throw new NotImplementedException();
        }
    }

}
