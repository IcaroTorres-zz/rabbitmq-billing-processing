using System;
using System.Collections.Generic;

namespace Library.DependencyInjection
{
    public class CollectionsDictionary : Dictionary<string, string>
    {
        public string GetCollectionName(string key) => TryGetValue(key, out string collectionName)
            ? collectionName
            : throw new ArgumentException(
                $"MongoDB Collection not found. Ensure your setting has an entry for given key {key}");
    }
}
