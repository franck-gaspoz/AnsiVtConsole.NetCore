namespace AnsiVtConsole.NetCore.Component.Parser.NonRecursiveFunctionalGrammar
{
    internal sealed class Rule : List<string>
    {
        private static int _counter = 0;
        private static readonly object _counterLock = new();
        public int ID;

        public TreePath TreePath;

        public string Key => TreePath.Key;

#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        public Rule() => Init();
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        public Rule(IEnumerable<string> range) : base(range) => Init();
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

        private void Init()
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