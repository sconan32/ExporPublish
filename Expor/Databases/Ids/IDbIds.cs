using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids
{

    /// <summary>
    /// 用于 IDbId 集合的接口
    /// </summary>
    public interface IDbIds : IEnumerable<IDbId>
    {


        /**
         * Retrieve the collection / data size.
         * 
         * @return collection size
         */
        int Count { get; }

        /**
         * Test whether an ID is contained.
         * 
         * @param o object to test
         * @return true when contained
         */
        bool Contains(IDbIdRef o);

        /**
         * Test for an empty DBID collection.
         * 
         * @return true when empty.
         */
        bool IsEmpty();


    }

}
