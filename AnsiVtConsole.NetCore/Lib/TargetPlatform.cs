#pragma warning disable CS1591

namespace AnsiVtConsole.NetCore.Lib
{
    /// <summary>
    /// based on System.Runtime.RuntimeEnvironment
    /// </summary>
    public enum TargetPlatform
    {
        FreeBSD,
        Linux,
        OSX,
        Windows,
        Any,
        Unspecified
    }
}