﻿#pragma warning disable CS1591

using System.Runtime.InteropServices;

namespace AnsiVtConsole.NetCore.Lib
{
    public static class RuntimeEnvironment
    {
        public static OSPlatform? OSType
        {
            get
            {
                OSPlatform? oSPlatform = null;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                    oSPlatform = OSPlatform.FreeBSD;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    oSPlatform = OSPlatform.Windows;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    oSPlatform = OSPlatform.OSX;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    oSPlatform = OSPlatform.Linux;
                return oSPlatform;
            }
        }
    }
}
