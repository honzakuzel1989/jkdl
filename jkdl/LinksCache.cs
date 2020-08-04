using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace jkdl
{
    internal class LinksCache : ILinksCache
    {
        private readonly BlockingCollection<string> _cache = new BlockingCollection<string>();

        public void AddLink(string link)
        {
            _cache.Add(link);
        }

        public IEnumerable<string> GetLinks(CancellationToken cancellationToken)
        {
            return _cache.GetConsumingEnumerable(cancellationToken);
        }
    }
}