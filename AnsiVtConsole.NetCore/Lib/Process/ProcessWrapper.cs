﻿using System.Text;

using sys = System.Diagnostics;

namespace AnsiVtConsole.NetCore.Lib.Process
{
    /// <summary>
    /// process wrapper
    /// </summary>
    sealed class ProcessWrapper
    {
        public sys.Process? Process = null;

        public Thread? ThreadRunner = null;

        public Action? CallBack = null;

        public ProcessCounter? PC = null;

        public Action<string>? StdOutCallBack = null;

        public Action<string>? StdErrCallBack = null;

        readonly StringBuilder _out = new();
        readonly StringBuilder _err = new();

        public int EndOfStreams { get; private set; } = 0;
        public Thread? StdOutCallBackThread;
        public Thread? StdErrCallBackThread;

        ProcessWrapper() { }

        public static int RunCommand(
            string exePath,
            string exeFileName,
            string workingDir,
            bool createNoWindow,
            Action<string> logInfo,
            Action<string> logError,
            params string[] args
            )
        {
            var a = args.Select(x => $"\"{x}\"").ToList();
            var sa = string.Join(" ", a);
            var cmdline = $"{exePath}\\{exeFileName}";

            var psi = new sys.ProcessStartInfo(
                cmdline,
                sa
                )
            {
                CreateNoWindow = createNoWindow,
                WorkingDirectory = workingDir ?? exePath,
                WindowStyle = sys.ProcessWindowStyle.Normal
            };
            var tr = ProcessWrapper.ThreadRunWaitAndReturnExitCode(
                psi,
                null,
                null,
                logInfo,
                logError
                );
            return tr;
        }

        public void ReadStdOut()
        {
            if (StdOutCallBack != null)
            {
                string? str;
                while ((str = Process!.StandardOutput.ReadLine()) != null)
                {
                    _out.AppendLine(str);
                    StdOutCallBack(str);
                }
                EndOfStreams += 1;
            }
        }

        public void ReadStdErr()
        {
            if (StdErrCallBack != null)
            {
                string? str;
                while ((str = Process!.StandardError.ReadLine()) != null)
                {
                    _err.AppendLine(str);
                    StdErrCallBack(str);
                }
                EndOfStreams += 1;
            }
        }

        public void WaitForAll()
        {
            ThreadRunner!.Join();
            while (EndOfStreams < 2)
            {
                Thread.Yield();
            }
        }

        /// <summary>
        /// wait for the end of the process (that must be running) and then called the callback action if not null
        /// </summary>
        /// <param name="callBack"></param>
        public void WaitForExit(Action? callBack = null)
        {
            Process!.WaitForExit();
            if (callBack != null)
                callBack.Invoke();
            if (CallBack != null)
                CallBack.Invoke();
        }

        /// <summary>
        /// wait for the end of the process (that must be running) and then called the callback action if not null
        /// </summary>
        /// <param name="callBack"></param>
        public int WaitForExitGetExitCode(Action<int>? callBack = null)
        {
            Process!.WaitForExit();
            var res = Process.ExitCode;
            if (callBack != null)
                callBack.Invoke(res);
            return res;
        }

        /// <summary>
        /// build a process wrapper that wraps a process runned within the provided command line arguments
        /// </summary>
        public static ProcessWrapper Run(string fileName, string arguments, sys.ProcessWindowStyle windowStyle)
        {
            var psi = new sys.ProcessStartInfo(fileName, arguments)
            {
                WindowStyle = windowStyle
            };
            return Run(psi);
        }

        /// <summary>
        /// build a process wrapper that wraps a process runned within the provided process start infos
        /// </summary>
        public static ProcessWrapper Run(sys.ProcessStartInfo psi)
        {
            var sw = new ProcessWrapper
            {
                Process = sys.Process.Start(psi)
            };
            return sw;
        }

        /// <summary>
        /// build a process wrapper that wraps a process runned within the provided process start infos in a separated thread that wait the process end and then call the callback function if not null
        /// </summary>
        public static ProcessWrapper ThreadRun(sys.ProcessStartInfo psi, ProcessCounter? pc = null)
        {
            var sw = new ProcessWrapper
            {
                EndOfStreams = 0
            };
            if (pc != null)
                pc.Increase();
            InitPSI(psi);
            sw.Process = sys.Process.Start(psi);
            Action? callBack = null;
            if (pc != null)
                callBack = pc.Decrease;
            sw.PC = pc;
            sw.ThreadRunner = new Thread(() => sw.WaitForExit(callBack));
            sw.ThreadRunner.Start();
            return sw;
        }

        /// <summary>
        /// build a process wrapper that wraps a process runned within the provided process start infos in a separated thread that wait the process end and then call the callback function if not null
        /// </summary>
        public static ProcessWrapper ThreadRun(
            sys.ProcessStartInfo psi,
            ProcessCounter? pc = null,
            Action? callBack = null,
            Action<string>? stdOutCallBack = null,
            Action<string>? stdErrCallBack = null)
        {
            var sw = new ProcessWrapper
            {
                EndOfStreams = 0,
                PC = pc
            };
            if (pc != null)
                pc.Increase();
            InitPSI(psi);
            sw.StdOutCallBack = stdOutCallBack;
            sw.StdErrCallBack = stdErrCallBack;
            sw.Process = sys.Process.Start(psi);
            if (stdOutCallBack != null)
                (sw.StdOutCallBackThread = new Thread(() => sw.ReadStdOut())).Start();
            if (stdErrCallBack != null)
                (sw.StdErrCallBackThread = new Thread(() => sw.ReadStdErr())).Start();
            Action? lcallBack = null;
            if (pc != null)
                lcallBack = pc.Decrease;
            sw.CallBack = callBack;
            sw.ThreadRunner = new Thread(() => sw.WaitForExit(lcallBack));
            sw.ThreadRunner.Start();
            sw.ThreadRunner.Join();     // FGZ 19/5/2018 - découpler pour rétro compat
            return sw;
        }

        /// <summary>
        /// build a process wrapper that wraps a process runned within the provided process start infos in a separated thread that wait the process end and then call the callback function if not null
        /// </summary>
        public static ProcessWrapper ThreadRun(
            sys.ProcessStartInfo psi,
            ProcessCounter? pc = null,
            Action<string>? stdOutCallBack = null,
            Action<string>? stdErrCallBack = null)
        {
            var sw = new ProcessWrapper
            {
                EndOfStreams = 0,
                PC = pc
            };
            if (pc != null)
                pc.Increase();
            InitPSI(psi);
            sw.StdOutCallBack = stdOutCallBack;
            sw.StdErrCallBack = stdErrCallBack;
            sw.Process = sys.Process.Start(psi);
            if (stdOutCallBack != null)
                (sw.StdOutCallBackThread = new Thread(() => sw.ReadStdOut())).Start();
            if (stdErrCallBack != null)
                (sw.StdErrCallBackThread = new Thread(() => sw.ReadStdErr())).Start();
            Action? lcallBack = null;
            if (pc != null)
                lcallBack = pc.Decrease;

            return sw;
        }

        /// <summary>
        /// build a process wrapper that wraps a process runned within the provided process start infos in a separated thread that wait the process end and then call the callback function if not null
        /// </summary>
        public static int
            ThreadRunWaitAndReturnExitCode(
                sys.ProcessStartInfo psi,
                ProcessCounter? pc = null,
                Action? callBack = null,
                Action<string>? stdOutCallBack = null,
                Action<string>? stdErrCallBack = null)
        {
            var sw = new ProcessWrapper
            {
                EndOfStreams = 0,
                PC = pc
            };
            if (pc != null)
                pc.Increase();
            InitPSI(psi);
            sw.StdOutCallBack = stdOutCallBack;
            sw.StdErrCallBack = stdErrCallBack;
            sw.Process = sys.Process.Start(psi);
            if (stdOutCallBack != null)
                (sw.StdOutCallBackThread = new Thread(() => sw.ReadStdOut())).Start();
            if (stdErrCallBack != null)
                (sw.StdErrCallBackThread = new Thread(() => sw.ReadStdErr())).Start();
            Action? lcallBack = null;
            if (pc != null)
                lcallBack = pc.Decrease;
            sw.CallBack = callBack;
            sw.ThreadRunner = new Thread(() => sw.WaitForExit(lcallBack));
            sw.ThreadRunner.Start();
            var ret = sw.WaitForExitGetExitCode();
            return ret;
        }

        /// <summary>
        /// build a process wrapper that wraps a process runned within the provided process start infos in a separated thread that wait the process end and then call the callback function if not null
        /// </summary>
        public static ProcessWrapper ThreadRunGetExitCode(
            sys.ProcessStartInfo psi,
            ProcessCounter? pc = null,
            Action<int>? callBack = null,
            Action<string>? stdOutCallBack = null,
            Action<string>? stdErrCallBack = null)
        {
            var sw = new ProcessWrapper
            {
                EndOfStreams = 0,
                PC = pc
            };
            if (pc != null)
                pc.Increase();
            InitPSI(psi);
            sw.StdOutCallBack = stdOutCallBack;
            sw.StdErrCallBack = stdErrCallBack;
            sw.Process = sys.Process.Start(psi);
            if (stdOutCallBack != null)
                (sw.StdOutCallBackThread = new Thread(() => sw.ReadStdOut())).Start();
            if (stdErrCallBack != null)
                (sw.StdErrCallBackThread = new Thread(() => sw.ReadStdErr())).Start();
            Action? lcallBack = null;
            if (pc != null)
                lcallBack = pc.Decrease;
            sw.ThreadRunner = new Thread(() => sw.WaitForExitGetExitCode(callBack));
            sw.ThreadRunner.Start();
            return sw;
        }

        /// <summary>
        /// init a PSI from default values
        /// </summary>
        static void InitPSI(sys.ProcessStartInfo psi)
        {
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
        }
    }
}
