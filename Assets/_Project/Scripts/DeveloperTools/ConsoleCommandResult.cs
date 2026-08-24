namespace UnityRPG.DeveloperTools
{
    public readonly struct ConsoleCommandResult
    {
        public bool Success { get; }
        public string Message { get; }

        public ConsoleCommandResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static ConsoleCommandResult Succeed(string message) => new ConsoleCommandResult(true, message);
        public static ConsoleCommandResult Fail(string message) => new ConsoleCommandResult(false, message);
    }
}