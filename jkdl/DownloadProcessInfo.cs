using System;

namespace jkdl
{
    public class DownloadProcessInfo
    {
        public DownloadProcessInfo(string guid, string link, string filename)
        {
            Key = guid;
            Link = link;
            Filename = filename;
        }

        public DownloadProcessInfo(string guid, string link, string filename, long bytesReceived, long totalBytesToReceive, int progressPercentage, bool running)
            : this(guid, link, filename)
        {
            BytesReceived = bytesReceived;
            TotalBytesToReceive = totalBytesToReceive;
            ProgressPercentage = progressPercentage;
            Running = running;
        }

        public DownloadProcessInfo(string guid, string link, string filename, long bytesReceived, long totalBytesToReceive, int progressPercentage, bool cancelled, Exception error, bool completed)
            : this(guid, link, filename, bytesReceived, totalBytesToReceive, progressPercentage, running: false)
        {
            Completed = completed;
            Cancelled = cancelled;
            Error = error;
            Failed = error != null;
        }

        public bool Running { get; }
        public bool Completed { get; }
        public string Key { get; }
        public string Link { get; }
        public string Filename { get; }
        public long BytesReceived { get; }
        public long TotalBytesToReceive { get; }
        public int ProgressPercentage { get; }
        public bool Cancelled { get; }
        public bool Failed { get; }
        public Exception Error { get; }
    }
}
