using System;

namespace jkdl
{
    public interface IDownloadProgressMonitor : IDisposable
    {
        void StartMonitor();
        void StopMonitor();
    }
}