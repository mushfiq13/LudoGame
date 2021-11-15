using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public static class DictionaryListExtension
    {
        public static void Add<Tkey, TCollection, TValue>(this IDictionary<Tkey, TCollection> keyValues, Tkey key, TValue value)
                where Tkey : notnull
                where TCollection : ICollection<TValue>, new()
        {
            TCollection? collection;

            if (!keyValues.TryGetValue(key, out collection))
            {
                collection = new TCollection();
                keyValues.Add(key, collection);
            }

            collection.Add(value);
        }

        public static bool Remove<Tkey, TCollection, TValue>(this IDictionary<Tkey, TCollection> keyValues, Tkey key, TValue value)
                where Tkey : notnull
                where TCollection : ICollection<TValue>, new()
        {
            TCollection? collection;

            if (!keyValues.TryGetValue(key, out collection))
            {
                return false;
            }

            if (!collection.Remove(value))
            {
                return false;
            }
            
            if (!collection.Any())
            {
                keyValues.Remove(key);
            }

            return true;
        }
    }
}
