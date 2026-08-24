namespace UnityRPG.DeveloperTools
{
    public readonly struct ParsedCommand
    {
        public string Name { get; }
        public string[] Args { get; }

        public ParsedCommand(string name, string[] args)
        {
            Name = name;
            Args = args;
        }
    }
}