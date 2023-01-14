namespace AnsiVtConsole.NetCore.Component.Parser.NonRecursiveFunctionalGrammar
{
    /// <summary>
    /// rule
    /// </summary>
    public sealed class Rule : List<string>
    {
        static int _counter = 0;
        static readonly object _counterLock = new();
#pragma warning disable CS1591 
        public int ID;

        public TreePath TreePath;

        public string Key => TreePath.Key;

#pragma warning disable CS8618
        public Rule() => Init();
#pragma warning restore CS8618 

#pragma warning disable CS8618 
        public Rule(IEnumerable<string> range) : base(range) => Init();
#pragma warning restore CS8618
#pragma warning restore CS1591

        void Init()
        {
            TreePath = new TreePath(this, -1);
            lock (_counterLock)
            {
                ID = _counter;
                _counter++;
            }
        }
    }
}