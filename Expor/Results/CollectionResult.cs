using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Results
{

    public class CollectionResult<O> : BasicResult, IIterableResult<O>
    {
        /**
         * The collection represented.
         */
        private ICollection<O> col;

        /**
         * Meta information (printed into the header)
         */
        private ICollection<String> header;

        /**
         * Constructor
         * 
         * @param name The long name (for pretty printing)
         * @param shortname the short name (for filenames etc.)
         * @param col ICollection represented
         * @param header Auxiliary information for result headers
         */
        public CollectionResult(String name, String shortname, ICollection<O> col, ICollection<String> header) :
            base(name, shortname)
        {
            this.col = col;
            this.header = header;
        }

        /**
         * Constructor
         * 
         * @param name The long name (for pretty printing)
         * @param shortname the short name (for filenames etc.)
         * @param col ICollection represented
         */
        public CollectionResult(String name, String shortname, ICollection<O> col) :
            this(name, shortname, col, new List<String>())
        {
        }

        /**
         * Add header information
         * 
         * @param s Header information string
         */
        public void AddHeader(String s)
        {
            header.Add(s);
        }

        /**
         * Get header information
         * 
         * @return header information of the result
         */
        public ICollection<String> GetHeader()
        {
            return header;
        }

        /**
         * Implementation of the {@link IterableResult} interface, using the backing collection.
         */


        public IEnumerator<O> GetEnumerator()
        {
            return col.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return col.GetEnumerator();
        }
    }
}
