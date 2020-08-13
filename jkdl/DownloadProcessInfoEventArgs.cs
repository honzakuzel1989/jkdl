using System;

namespace jkdl
{
    public class DownloadProcessInfoEventArgs : EventArgs
    {
        public DownloadProcessInfo Info { get; }

        public DownloadProcessInfoEventArgs(long received, long toReceive, int percentage, DownloadProcessInfo info)
        {
            Info = new DownloadProcessInfo(
                info.StartTime,
                info.Key,
                info.Link, 
                info.Filename,
                received, 
                toReceive,
                percentage,
                running: true);
        }
    }
}
