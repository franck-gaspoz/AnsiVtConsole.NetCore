#pragma warning disable CS1591

using AnsiVtConsole.NetCore.Component.EchoDirective;

namespace AnsiVtConsole.NetCore.Component.Console
{
    public sealed class EchoSequence
    {
        public readonly EchoDirectives? PrintDirective;
        public readonly int FirstIndex;
        public readonly int LastIndex;
        public readonly string? Value;
        public readonly string? Text;
        public int Length => LastIndex - FirstIndex + 1;

        readonly IAnsiVtConsole _console;

        public EchoSequence(
            IAnsiVtConsole console,
            EchoDirectives? printDirective,
            int firstIndex,
            int lastIndex,
            string? value,
            string? text,
            int relIndex = 0)
        {
            _console = console;
            PrintDirective = printDirective;
            FirstIndex = firstIndex + relIndex;
            LastIndex = lastIndex + relIndex;
            Value = value;
            Text = text;
        }

        public EchoSequence(
            IAnsiVtConsole console,
            string? printDirective,
            int firstIndex,
            int lastIndex,
            string? value,
            string? text,
            int relIndex = 0)
        {
            _console = console;
            if (printDirective != null)
            {
                if (Enum.TryParse<EchoDirectives>(printDirective, out var pr))
                    PrintDirective = pr;
            }

            FirstIndex = firstIndex + relIndex;
            LastIndex = lastIndex + relIndex;
            Value = value;
            Text = text;
        }

        public string ToText()
        {
            var s = "";
            if (PrintDirective.HasValue && Value == null)
            {
                s += $"{_console.Settings.CommandBlockBeginChar}{PrintDirective}{_console.Settings.CommandBlockEndChar}";
            }
            else
            {
                if (PrintDirective.HasValue && Value != null)
                    s += $"{_console.Settings.CommandBlockBeginChar}{PrintDirective}{_console.Settings.CommandValueAssignationChar}{Value}{_console.Settings.CommandBlockEndChar}";
                else
                    s += Text;
            }
            return s;
        }

        public override string ToString()
        {
            var s = $"{FirstIndex}..{LastIndex}({Length})  ";
            if (PrintDirective.HasValue && Value == null)
            {
                s += $"{PrintDirective}";
            }
            else
            {
                if (PrintDirective.HasValue && Value != null)
                    s += $"{PrintDirective}={Value}";
                else
                    s += Text;
            }
            return s;
        }

        public string ToStringPattern()
        {
            var c = "-";
            var s = "";
            if (PrintDirective.HasValue && Value == null)
            {
                s += $"{c}{PrintDirective}{c}";
            }
            else
            {
                if (PrintDirective.HasValue && Value != null)
                    s += $"{c}{PrintDirective}={Value}{c}";
                else
                    s += Text;
            }
            return s;
        }

        public bool IsText => !PrintDirective.HasValue;
    }
}
