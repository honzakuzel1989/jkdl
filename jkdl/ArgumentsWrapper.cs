namespace jkdl
{
    class ArgumentsWrapper : IArgumentsWrapper
    {
        public string[] Arguments { get; }

        public ArgumentsWrapper(string[] arguments)
        {
            Arguments = arguments;
        }
    }
}
