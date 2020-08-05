using System;
using System.ComponentModel;
using System.Net;

namespace jkdl
{
    public class DownloadProcessInfoEventArgs : EventArgs
    {
        public DownloadProcessInfo Info { get; }

        public DownloadProcessInfoEventArgs(DownloadProgressChangedEventArgs e, string link, string filename)
        {
            Info = new DownloadProcessInfo(link, 
                filename, 
                e.BytesReceived, 
                e.TotalBytesToReceive, 
                e.ProgressPercentage,
                running: true);
        }
    }
}
