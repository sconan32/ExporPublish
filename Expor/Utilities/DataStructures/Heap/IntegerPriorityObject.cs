using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Utilities.DataStructures.Heap
{

    public class IntegerPriorityObject<O> : IPair<Int32, O>, IComparable<IntegerPriorityObject<O>>
    {
        /**
         * Priority.
         */
        int priority;

        /**
         * Stored object. Private; since changing this will break an
         * {@link de.lmu.ifi.dbs.elki.utilities.datastructures.heap.UpdatableHeap
         * UpdatableHeap}s Hash IDictionary!
         */
        private O obj;

        /**
         * Constructor.
         * 
         * @param priority Priority
         * @param object Payload
         */
        public IntegerPriorityObject(int priority, O obj) :
            base()
        {
            this.priority = priority;
            this.obj = obj;
        }

        /**
         * Get the priority.
         * 
         * @return Priority
         */
        public int GetPriority()
        {
            return priority;
        }

        /**
         * Get the stored object payload
         * 
         * @return object data
         */
        public O GetObject()
        {
            return obj;
        }


        public Int32 GetFirst()
        {
            return priority;
        }


        public O GetSecond()
        {
            return obj;
        }
        public int First
        {
            get { return priority; }
            set { priority = value; }
        }
        public O Second
        {
            get { return obj; }
            set { obj = value; }
        }

        public override int GetHashCode()
        {
            return ((obj == null) ? 0 : obj.GetHashCode());
        }


        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (!(obj is IntegerPriorityObject<O>))
            {
                return false;
            }
            IntegerPriorityObject<O> other = (IntegerPriorityObject<O>)obj;
            if (obj == null)
            {
                return (other.obj == null);
            }
            else
            {
                return obj.Equals(other.obj);
            }
        }


        public int CompareTo(IntegerPriorityObject<O> o)
        {
            return o.priority - this.priority;
        }


        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(priority).Append(":").Append(obj.ToString());
            return buf.ToString();
        }
    }
}
