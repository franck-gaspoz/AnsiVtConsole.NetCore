namespace AnsiVtConsole.NetCore.Component.EchoDirective
{
    /// <summary>
    /// these map attribute a echo command delegate to an echo directive syntax
    /// </summary>
    internal sealed class CommandMap
    {
        public Dictionary<
            string,
            (EchoDirectiveProcessor.SimpleCommandDelegate? simpleCommand,
            EchoDirectiveProcessor.CommandDelegate? command,
            object? parameter)>? Map;

        public CommandMap(Dictionary<
            string,
            (EchoDirectiveProcessor.SimpleCommandDelegate? simpleCommand,
            EchoDirectiveProcessor.CommandDelegate? command,
            object? parameter)> map)
            => Map = map;

    }
}