using System.Drawing;

namespace AnsiVtConsole.NetCore.Component.Console
{
    /// <summary>
    /// work area
    /// </summary>
    public sealed class WorkArea
    {
        /// <summary>
        /// id
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// rect
        /// </summary>
        public readonly Rectangle Rect = Rectangle.Empty;

        /// <summary>
        /// WorkArea
        /// </summary>
        public WorkArea() => Id = string.Empty;

        /// <summary>
        /// WorkArea
        /// </summary>
        public WorkArea(string id, int x, int y, int width, int height)
        {
            Id = id;
            Rect = new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// WorkArea
        /// </summary>
        public WorkArea(WorkArea workArea)
        {
            Id = workArea.Id;
            Rect = new Rectangle(workArea.Rect.X, workArea.Rect.Y, workArea.Rect.Width, workArea.Rect.Height);
        }

        /// <summary>
        /// is empty
        /// </summary>
        public bool IsEmpty => Rect.IsEmpty;

        public bool InWorkArea => !Rect.IsEmpty;
    }
}
