using System;
using System.Collections.Generic;

namespace PrivatePackage.Messaging
{
    [Serializable]
    public class ConsumerQueueDictionary : Dictionary<string, string>
    {
        public string GetQueue(string queueKey) => TryGetValue(queueKey, out string queueName)
            ? queueName
            : throw new ArgumentException($"QueueSettings not found. Ensure your appsettings has a entry for given key {queueKey}");
    }
}
