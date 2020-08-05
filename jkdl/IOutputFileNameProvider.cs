namespace jkdl
{
    public interface IOutputFileNameProvider
    {
        string GetAbsoluteFileName(string link);
    }
}