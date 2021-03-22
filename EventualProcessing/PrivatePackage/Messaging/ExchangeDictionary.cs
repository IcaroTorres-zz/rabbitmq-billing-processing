using System;
using System.Collections.Generic;

namespace PrivatePackage.Messaging
{
    public class ExchangeDictionary : Dictionary<string, ExchangeSettings>
    {
        public ExchangeSettings GetSettings(string key) => TryGetValue(key, out var settings)
            ? settings
            : throw new ArgumentException($"ExchangeSettings not found. Ensure your appsettings has a entry for given key {key}");
    }
}
