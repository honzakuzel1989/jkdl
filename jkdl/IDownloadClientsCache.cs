using System.Collections.Generic;

namespace jkdl
{
    internal interface IDownloadClientsCache
    {
        bool IsEmpty { get; }
        IEnumerable<IWebClient> Values { get; }
        IWebClient this[string key] { get; set; }
        bool TryGetValue(string key, out IWebClient client);
    }
}