using System;

namespace jkdl
{
    public class DownloadProcessCompletedEventArgs : EventArgs
    {
        public DownloadProcessInfo Info { get; }

        public DownloadProcessCompletedEventArgs(Exception ex, bool cancelled, DownloadProcessInfo info)
        {
            Info = new DownloadProcessInfo(
                info.StartTime,
                info.Key,
                info.Link, 
                info.Filename,
                info.BytesReceived,
                info.TotalBytesToReceive,
                info.ProgressPercentage,
                cancelled,
                ex,
                completed: true,
                DateTime.Now);
        }
    }
}