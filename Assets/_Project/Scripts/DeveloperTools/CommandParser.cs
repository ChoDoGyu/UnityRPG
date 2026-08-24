using System;

namespace UnityRPG.DeveloperTools
{
    public static class CommandParser
    {
        public static bool TryParse(string input, out ParsedCommand command)
        {
            command = default;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            string[] tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length == 0)
                return false;

            string name = tokens[0].ToLowerInvariant();
            string[] args = new string[tokens.Length - 1];

            if (args.Length > 0)
                Array.Copy(tokens, 1, args, 0, args.Length);

            command = new ParsedCommand(name, args);
            return true;
        }
    }
}