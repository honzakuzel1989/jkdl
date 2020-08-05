﻿using System.Collections.Generic;
using System.Threading;

namespace jkdl
{
    public interface ILinksCache
    {
        void AddLink(string link, CancellationToken cancellationToken);
        IEnumerable<string> GetLinks(CancellationToken cancellationToken);
    }
}