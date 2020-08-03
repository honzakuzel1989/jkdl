namespace jkdl
{
    public interface IWebClientFactory
    {
        IWebClient CreateWebClient(string link, string filename);
    }
}