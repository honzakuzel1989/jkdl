namespace jkdl
{
    internal class DownloadInfo
    {
        public DownloadInfo(string link)
        {
            Link = link;
        }

        public string Link { get; set; }
        public int ProgressPercentage { get; set; }
        public long BytesReceived { get; set; }
        public long TotalBytesToReceive { get; set; }
    }
}
