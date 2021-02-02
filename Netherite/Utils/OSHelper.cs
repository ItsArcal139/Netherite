using System.Runtime.InteropServices;

namespace Netherite.Utils
{
    public static class OSHelper
    {
        public static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsArm64 => RuntimeInformation.OSArchitecture == Architecture.Arm64;

        private static bool InternalCheckAppleSilicon()
        {
            if (!IsMacOS || !IsX64) return false;
            return RuntimeInformation.OSDescription.Contains("ARM64");
        }

        public static bool IsX64 => RuntimeInformation.OSArchitecture == Architecture.X64;

        public static bool IsRosetta2 => InternalCheckAppleSilicon();
    }
}
