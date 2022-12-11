﻿using System.Drawing;

using sc = System.Console;

namespace AnsiVtConsole.NetCore.Component.Console
{
    /// <summary>
    /// work area
    /// </summary>
    public sealed class WorkArea
    {
        private readonly IAnsiVtConsole _console;

        /// <summary>
        /// id
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// rect
        /// </summary>
        public readonly Rectangle Rect = Rectangle.Empty;

        /// <summary>
        /// this setting limit wide of lines (available width -1) to prevent sys console to automatically put a line break when reaching end of line (console bug ?)
        /// </summary>
        public bool AvoidConsoleAutoLineBreakAtEndOfLine = false;

        public WorkArea(IAnsiVtConsole console)
        {
            _console = console;
            Id = string.Empty;
        }

        /// <summary>
        /// WorkArea
        /// </summary>
        public WorkArea(
            IAnsiVtConsole console,
            string id,
            int x,
            int y,
            int width,
            int height)
        {
            _console = console;
            Id = id;
            Rect = new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// WorkArea
        /// </summary>
        public WorkArea(
            IAnsiVtConsole console,
            WorkArea workArea)
        {
            _console = console;
            Id = workArea.Id;
            Rect = new Rectangle(workArea.Rect.X, workArea.Rect.Y, workArea.Rect.Width, workArea.Rect.Height);
        }

        /// <summary>
        /// is empty
        /// </summary>
        public bool IsEmpty => Rect.IsEmpty;

        public bool InWorkArea => !Rect.IsEmpty;

        /// <summary>
        /// true until the contrary is detected (exception in GetCoords : sc.WindowLeft)
        /// </summary>
        public bool IsConsoleGeometryEnabled { get; private set; } = true;

        /// <summary>
        /// update the IsConsoleGeometryEnabled field
        /// </summary>
        /// <returns>value of the field</returns>
        public bool CheckConsoleHasGeometry()
        {
            try
            {
                var x = sc.WindowLeft;
            }
            catch (Exception)
            {
                IsConsoleGeometryEnabled = false;
                return false;
            }
            return true;
        }

        public (int x, int y, int w, int h) GetCoords(int x, int y, int w, int h, bool fitToVisibleArea = true)
        {
            // (1) dos console (eg. vs debug consolehep) set WindowTop as y scroll position. WSL console doesn't (still 0)
            // scroll -> native dos console set WindowTop and WindowLeft as base scroll coordinates
            // if WorkArea defined, we must use absolute coordinates and not related
            // CursorLeft and CursorTop are always good
            lock (_console.Out.Lock!)
            {
                if (!IsConsoleGeometryEnabled)
                    return (x, y, 1000, 1000);

                if (x < 0)
                    x = sc.WindowLeft + sc.WindowWidth + x;

                if (y < 0)
                    y = /*sc.WindowTop (fix 1) */ +sc.WindowHeight + y;

                if (fitToVisibleArea)
                {
                    if (w < 0)
                    {
                        w = sc.WindowWidth + ((AvoidConsoleAutoLineBreakAtEndOfLine) ? -1 : 0) + (w + 1)     // 1 POS TOO MUCH !!
                            /*+ sc.WindowLeft*/;
                    }

                    if (h < 0)
                    {
                        h = sc.WindowHeight + h
                            + sc.WindowTop; /* ?? */
                    }
                }
                else
                {
                    if (w < 0)
                        w = sc.BufferWidth + ((AvoidConsoleAutoLineBreakAtEndOfLine) ? -1 : 0) + (w + 1);

                    if (h < 0)
                        h = sc.WindowHeight + h + sc.WindowTop;
                }
                return (x, y, w, h);
            }
        }

        public ActualWorkArea ActualWorkArea(bool fitToVisibleArea = true)
        {
            if (!IsConsoleGeometryEnabled)
                return new ActualWorkArea(Id, 0, 0, 0, 0);
            var x0 = Rect.IsEmpty ? 0 : Rect.X;
            var y0 = Rect.IsEmpty ? 0 : Rect.Y;
            var w0 = Rect.IsEmpty ? -1 : Rect.Width;
            var h0 = Rect.IsEmpty ? -1 : Rect.Height;
            var (x, y, w, h) = GetCoords(x0, y0, w0, h0, fitToVisibleArea);
            return new ActualWorkArea(Id, x, y, w, h);
        }

        public void SetCursorAtWorkAreaTop()
        {
            if (!IsConsoleGeometryEnabled || Rect.IsEmpty)
                return;     // TODO: set cursor even if workarea empty?
            lock (_console.Out.Lock!)
            {
                _console.Out.SetCursorPos(Rect.X, Rect.Y);
            }
        }
    }
}
