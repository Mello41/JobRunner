using JobRunner.Core.Interfaces.Platform;
using JobRunner.Core.Interfaces.Platform.CommandLine;
using System.Runtime.InteropServices;

namespace JobRunner.Core.Utils.Platform
{
    public static class PlatformDetector //: IPlatformDetector
    {
        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        public static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static ICommandLineEscaper CreateEscaper()
        {
            if (IsWindows) return new WindowsCommandLineEscaper();
            if (IsLinux || IsMacOS) return new LinuxCommandLineEscaper();
            return new WindowsCommandLineEscaper(); // fallback
        }
    }
}