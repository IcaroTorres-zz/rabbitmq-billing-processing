using System.Collections.Generic;

namespace PrivatePackage.Messaging
{
    public class ConsumerQueueDictionary : Dictionary<string, string>
    {
        public string GetQueue(string queueKey) => TryGetValue(queueKey, out string queueName) ? queueName : "";
    }
}
