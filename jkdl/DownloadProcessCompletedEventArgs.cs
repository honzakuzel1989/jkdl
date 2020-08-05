using System;
using System.ComponentModel;

namespace jkdl
{
    public class DownloadProcessCompletedEventArgs : EventArgs
    {
        public DownloadProcessInfo Info { get; }

        public DownloadProcessCompletedEventArgs(AsyncCompletedEventArgs e, DownloadProcessInfo info)
        {
            Info = new DownloadProcessInfo(info.Link, 
                info.Filename,
                info.BytesReceived,
                info.TotalBytesToReceive,
                info.ProgressPercentage,
                e.Cancelled,
                e.Error,
                completed: true);
        }
    }
}