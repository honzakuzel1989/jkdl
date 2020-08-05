using System;

namespace jkdl
{
    public class DownloadProcessInfo
    {
        public DownloadProcessInfo(string link, string filename, long bytesReceived, long totalBytesToReceive, int progressPercentage)
        {
            Link = link;
            Filename = filename;
            BytesReceived = bytesReceived;
            TotalBytesToReceive = totalBytesToReceive;
            ProgressPercentage = progressPercentage;
        }

        public DownloadProcessInfo(string link, string filename, long bytesReceived, long totalBytesToReceive, int progressPercentage, bool cancelled, Exception error, bool completed) 
            : this(link, filename, bytesReceived, totalBytesToReceive, progressPercentage)
        {
            Completed = completed;
            Cancelled = cancelled;
            Error = error;
        }

        public bool Completed { get; }
        public string Link { get; }
        public string Filename { get; }
        public long BytesReceived { get; }
        public long TotalBytesToReceive { get; }
        public int ProgressPercentage { get; }
        public bool Cancelled { get; }
        public Exception Error { get; }
    }
}
