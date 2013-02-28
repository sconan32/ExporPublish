using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.PrefixTree
{
    public class PrefixTree<TKey, TValue> : IPrefixTree
        where TValue : IEnumerable<TKey>
    {

        private Dictionary<TKey, PrefixTree<TKey, TValue>> _index = new Dictionary<TKey, PrefixTree<TKey, TValue>>();
        private HashSet<TValue> _storedItems;

        public void Add(TValue content)
        {
            var list = content.ToList();

            while (list.Count > 0)
            {
                Add(list, content);
                list.RemoveAt(0);

            }
        }

        internal IEnumerable<TValue> FindItems(IEnumerable<TKey> pattern, int limit)
        {
            var currentTrie = this;


            foreach (var key in pattern)
            {
                if (!currentTrie._index.TryGetValue(key, out currentTrie))
                    return Enumerable.Empty<TValue>();
            }

            return currentTrie.GetCurrentItems(limit);
        }

        private IEnumerable<TValue> GetCurrentItems(int limit)
        {
            if (_storedItems != null)
            {
                foreach (var item in _storedItems)
                {
                    yield return item;
                    limit--;

                    if (limit <= 0)
                        yield break;
                }
            }
        }

        private void Add(List<TKey> keylist, TValue content)
        {
            StoreContent(content);

            if (keylist.Count > 0)
                BuildIndex(content, keylist);
        }

        private void BuildIndex(TValue content, List<TKey> keylist)
        {
            TKey ckey = keylist[0];
            keylist.RemoveAt(0);

            if (!_index.ContainsKey(ckey))
                _index.Add(ckey, new PrefixTree<TKey, TValue>());

            _index[ckey].Add(keylist, content);
        }

        private void StoreContent(TValue content)
        {
            if (_storedItems == null)
                _storedItems = new HashSet<TValue>();

            _storedItems.Add(content);
        }
    }
}
