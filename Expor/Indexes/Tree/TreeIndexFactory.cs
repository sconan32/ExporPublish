using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Persistent;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Utilities.Options.Constraints;
using Socona.Expor.Utilities.Options.Parameterizations;
using Socona.Expor.Utilities.Options.Parameters;

namespace Socona.Expor.Indexes.Tree
{

    /**
     * Abstract base class for tree-based indexes.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.stereotype factory,interface
     * @apiviz.has Index oneway - - 芦create禄
     * 
     * @param <O> Object type
     * @param <I> Index type
     */
    // TODO: actually, this class should be called PagedIndexFactory?
    public abstract class TreeIndexFactory : IIndexFactory
    {
        /**
         * Optional parameter that specifies the name of the file storing the index.
         * If this parameter is not set the index is hold in the main memory.
         * <p>
         * Key: {@code -treeindex.file}
         * </p>
         */
        public static OptionDescription FILE_ID = OptionDescription.GetOrCreate("treeindex.file",
            "The name of the file storing the index. " +
            "If this parameter is not set the index is hold in the main memory.");

        /**
         * Parameter to specify the size of a page in bytes, must be an integer
         * greater than 0.
         * <p>
         * Default value: {@code 4000}
         * </p>
         * <p>
         * Key: {@code -treeindex.pagesize}
         * </p>
         */
        public static OptionDescription PAGE_SIZE_ID = OptionDescription.GetOrCreate("treeindex.pagesize",
            "The size of a page in bytes.");

        /**
         * Parameter to specify the size of the cache in bytes, must be an integer
         * equal to or greater than 0.
         * <p>
         * Default value: {@link Integer#MAX_VALUE}
         * </p>
         * <p>
         * Key: {@code -treeindex.cachesize}
         * </p>
         */
        public static OptionDescription CACHE_SIZE_ID = OptionDescription.GetOrCreate("treeindex.cachesize",
            "The size of the cache in bytes.");

        /**
         * Holds the name of the file storing the index specified by {@link #FILE_ID},
         * null if {@link #FILE_ID} is not specified.
         */
        protected String fileName = null;

        /**
         * Holds the value of {@link #PAGE_SIZE_ID}.
         */
        protected int pageSize;

        /**
         * Holds the value of {@link #CACHE_SIZE_ID}.
         */
        protected long cacheSize;

        /**
         * Constructor.
         * 
         * @param fileName
         * @param pageSize
         * @param cacheSize
         */
        public TreeIndexFactory(String fileName, int pageSize, long cacheSize) :
            base()
        {
            this.fileName = fileName;
            this.pageSize = pageSize;
            this.cacheSize = cacheSize;
        }

        /**
         * Make the page file for this index.
         * 
         * @param <N> page type
         * @param cls Class information
         * @return Page file
         */
        // FIXME: make this single-shot when filename is set!
        protected IPageFile<N> MakePageFile<N>(Type cls) where N : IExternalizablePage
        {
            IPageFile<N> inner;
            if (fileName == null)
            {
                inner = new MemoryPageFile<N>(pageSize);
            }
            else
            {
                inner = new PersistentPageFile<N>(pageSize, fileName, cls);
            }
            if (cacheSize >= int.MaxValue)
            {
                return inner;
            }
            return new LRUCache<N>(cacheSize, inner);
        }


        abstract public IIndex Instantiate(IRelation relation);
        public abstract ITypeInformation GetInputTypeRestriction();
      
        /**
         * Parameterization class.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public abstract class Parameterizer : AbstractParameterizer
        {
            protected String fileName = null;

            protected int pageSize;

            protected long cacheSize;


            protected override void MakeOptions(IParameterization config)
            {
                base.MakeOptions(config);
                FileParameter FILE_PARAM = new FileParameter(FILE_ID, FileParameter.FileType.OUTPUT_FILE, true);
                if (config.Grab(FILE_PARAM))
                {
                    fileName = FILE_PARAM.GetValue().FullName;
                }
                else
                {
                    fileName = null;
                }

                IntParameter PAGE_SIZE_PARAM = new IntParameter(PAGE_SIZE_ID, new GreaterConstraint<int>(0), 4000);
                if (config.Grab(PAGE_SIZE_PARAM))
                {
                    pageSize = PAGE_SIZE_PARAM.GetValue();
                }

                LongParameter CACHE_SIZE_PARAM = new LongParameter(CACHE_SIZE_ID, new GreaterEqualConstraint<long>(0), 
                    int.MaxValue);
                if (config.Grab(CACHE_SIZE_PARAM))
                {
                    cacheSize = CACHE_SIZE_PARAM.GetValue();
                }
            }


            protected override abstract object MakeInstance();
        }
    }
}
