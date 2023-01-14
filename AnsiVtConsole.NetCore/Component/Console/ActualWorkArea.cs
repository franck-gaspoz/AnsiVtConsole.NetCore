#pragma warning disable CS1591

namespace AnsiVtConsole.NetCore.Component.Console
{
    /// <summary>
    /// acutal work area
    /// </summary>
    public sealed class ActualWorkArea
    {
        public readonly string Id;
        public readonly int Left;
        public readonly int Top;
        public readonly int Right;
        public readonly int Bottom;

        public ActualWorkArea(string id, int left, int top, int right, int bottom)
        {
            Id = id;
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public void Deconstruct(out string id, out int left, out int top, out int right, out int bottom)
        {
            id = Id;
            left = Left;
            right = Right;
            top = Top;
            bottom = Bottom;
        }
    }
}
#pragma warning restore CS1591