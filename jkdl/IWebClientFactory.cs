namespace jkdl
{
    public interface IWebClientFactory
    {
        IWebClient CreateWebClient(DownloadProcessInfo info);
    }
}