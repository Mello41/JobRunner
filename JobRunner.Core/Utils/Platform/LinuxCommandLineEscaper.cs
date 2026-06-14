using JobRunner.Core.Interfaces.Detectors.Platform.CommandLine;
using JobRunner.Core.Interfaces.Platform;
using System;
using System.Linq;

namespace JobRunner.Core.Utils.Platform
{
    public sealed class LinuxCommandLineEscaper : ICommandLineEscaper
    {
        public string EscapeArgument(string argument)
        {
            if (string.IsNullOrEmpty(argument)) return "''";
            argument = argument.Replace("'", "'\\''");
            return argument.Contains(' ') ? $"'{argument}'" : argument;
        }

        public string EscapePath(string path) => path;
    }
}
