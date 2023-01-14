namespace AnsiVtConsole.NetCore.Lib
{
    /// <summary>
    /// generic event args + IsCanceled information
    /// </summary>
    /// <typeparam name="T">event arg type</typeparam>
    sealed class EventArgs<T> : EventArgs
    {
        public T? Value { get; set; }

        public bool IsCanceled { get; set; }

        public EventArgs(T? val) => Value = val;

        public EventArgs() { }

        public void Recycle()
        {
            Value = default;
            IsCanceled = false;
        }
    }
}
