using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Heap
{

    /**
     * Basic in-memory heap interface, for double keys and int values.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.has UnsortedIter
     */
    public interface IDoubleInt32Heap : IEnumerable<KeyValuePair<double, int>>
    {
        /**
         * Add a key-value pair to the heap
         * 
         * @param key Key
         * @param val Value
         */
        void Add(double key, int val);

        /**
         * Add a key-value pair to the heap if it improves the top.
         * 
         * @param key Key
         * @param val Value
         * @param k Desired maximum size
         */
        void Add(double key, int val, int k);

        /**
         * Combined operation that removes the top element, and inserts a new element
         * instead.
         * 
         * @param key Key of new element
         * @param val Value of new element
         */
        void ReplaceTopElement(double key, int val);

        /**
         * Get the current top key
         * 
         * @return Top key
         */
        double PeekKey();

        /**
         * Get the current top value
         * 
         * @return Value
         */
        int PeekValue();

        /**
         * Remove the first element
         */
        void Poll();

        /**
         * Clear the heap contents.
         */
        void Clear();

        /**
         * Query the size
         * 
         * @return Size
         */
        int Count { get; }

        /**
         * Is the heap empty?
         * 
         * @return {@code true} when the size is 0.
         */
        bool IsEmpty();

        /**
         * Get an unsorted iterator to inspect the heap.
         * 
         * @return Iterator
         */

    }

}
