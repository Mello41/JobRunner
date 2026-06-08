using System;
using System.Linq;

namespace JobRunner.Core.Platform
{
    public sealed class WindowsCommandLineEscaper : ICommandLineEscaper
    {
        public string EscapeArgument(string argument)
        {
            if (string.IsNullOrEmpty(argument)) return "\"\"";
            argument = argument.Replace("\\", "\\\\").Replace("\"", "\\\"");
            return argument.Contains(' ') ? $"\"{argument}\"" : argument;
        }

        public string EscapePath(string path) => EscapeArgument(path);
    }
}
