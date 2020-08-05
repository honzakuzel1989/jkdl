﻿using System;

namespace jkdl
{
    public class DownloadProcessInfo
    {
        public DownloadProcessInfo(DateTime start, string guid, string link, string filename)
        {
            Key = guid;
            Link = link;
            Filename = filename;
            StartTime = start;
            EndTime = start;
        }

        public DownloadProcessInfo(DateTime start, string guid, string link, string filename, long bytesReceived, long totalBytesToReceive, int progressPercentage, bool running)
            : this(start, guid, link, filename)
        {
            BytesReceived = bytesReceived;
            TotalBytesToReceive = totalBytesToReceive;
            ProgressPercentage = progressPercentage;
            Running = running;
        }

        public DownloadProcessInfo(DateTime start, string guid, string link, string filename, long bytesReceived, long totalBytesToReceive, int progressPercentage, bool cancelled, Exception error, bool completed, DateTime end)
            : this(start, guid, link, filename, bytesReceived, totalBytesToReceive, progressPercentage, running: false)
        {
            Completed = completed;
            Cancelled = cancelled;
            Error = error;
            Failed = error != null;
        }

        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
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
        public TimeSpan Duration => EndTime > StartTime ? EndTime.Subtract(StartTime) : DateTime.Now.Subtract(StartTime);
    }
}
