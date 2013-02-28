using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Socona.Expor.Utilities.Concurrent
{
    public class AtomicInt32
    {
        private int value;

        public AtomicInt32(int initialValue)
        {
            value = initialValue;
        }

        public AtomicInt32()
            : this(0)
        {
        }

        public int Get()
        {
            return value;
        }

        public void Set(int newValue)
        {
            value = newValue;
        }

        public int GetAndSet(int newValue)
        {
            for (; ; )
            {
                int current = Get();
                if (CompareAndSet(current, newValue))
                    return current;
            }
        }

        public bool CompareAndSet(int expect, int update)
        {
            return Interlocked.CompareExchange(ref value, update, expect) == expect;
        }

        public int GetAndIncrement()
        {
            for (; ; )
            {
                int current = Get();
                int next = current + 1;
                if (CompareAndSet(current, next))
                    return current;
            }
        }

        public int GetAndDecrement()
        {
            for (; ; )
            {
                int current = Get();
                int next = current - 1;
                if (CompareAndSet(current, next))
                    return current;
            }
        }

        public int GetAndAdd(int delta)
        {
            for (; ; )
            {
                int current = Get();
                int next = current + delta;
                if (CompareAndSet(current, next))
                    return current;
            }
        }

        public int IncrementAndGet()
        {
            for (; ; )
            {
                int current = Get();
                int next = current + 1;
                if (CompareAndSet(current, next))
                    return next;
            }
        }

        public int DecrementAndGet()
        {
            for (; ; )
            {
                int current = Get();
                int next = current - 1;
                if (CompareAndSet(current, next))
                    return next;
            }
        }

        public int AddAndGet(int delta)
        {
            for (; ; )
            {
                int current = Get();
                int next = current + delta;
                if (CompareAndSet(current, next))
                    return next;
            }
        }

        public override String ToString()
        {
            return Convert.ToString(Get());
        }
    }

}
