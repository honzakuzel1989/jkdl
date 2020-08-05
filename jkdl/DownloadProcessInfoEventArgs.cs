using System;
using System.ComponentModel;
using System.Net;

namespace jkdl
{
    public class DownloadProcessInfoEventArgs : EventArgs
    {
        public DownloadProcessInfo Info { get; }

        public DownloadProcessInfoEventArgs(DownloadProgressChangedEventArgs e, DownloadProcessInfo info)
        {
            Info = new DownloadProcessInfo(
                info.Key,
                info.Link, 
                info.Filename, 
                e.BytesReceived, 
                e.TotalBytesToReceive, 
                e.ProgressPercentage,
                running: true);
        }
    }
}
