namespace AnsiVtConsole.NetCore.Component.UI
{
    public sealed class WorkAreaScrollEventArgs
        : EventArgs
    {
        public readonly int DeltaX;
        public readonly int DeltaY;

        public WorkAreaScrollEventArgs(int deltaX, int deltaY)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
        }
    }
}
