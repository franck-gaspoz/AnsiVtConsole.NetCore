namespace AnsiVtConsole.NetCore.Component.Parser.NonRecursiveFunctionalGrammar
{
    public sealed class TreeNode
    {
        private static int _counter = 0;
        private static readonly object _counterLock = new();
        public int ID;
        public string Label;
        public readonly Dictionary<string, TreeNode> SubNodes = new();

        public bool IsRoot => Label == null;

#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        public TreeNode() => Init();
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

        public TreeNode(string label)
        {
            if (label == null)
                throw new Exception("label can't be null");
            Init();
            Label = label;
        }

        private void Init()
        {
            lock (_counterLock)
            {
                ID = _counter;
                _counter++;
            }
        }

        public override string ToString() => $" {ID}:  {Label}  -> {string.Join(" ", SubNodes.Values.Select(x => x.Label))}";
    }
}