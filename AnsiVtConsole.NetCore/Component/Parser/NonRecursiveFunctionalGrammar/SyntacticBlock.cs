#pragma warning disable CS1591

using AnsiVtConsole.NetCore.Component.Console;

namespace AnsiVtConsole.NetCore.Component.Parser.NonRecursiveFunctionalGrammar
{
    public sealed class SyntacticBlock
    {
        public string Text;

        public int Index;

        public TreePath? SyntacticRule;

        public bool IsSelected;

        public bool IsANSISequence;

        public SyntacticBlock(
            int index,
            TreePath? syntacticRule,
            string text,
            bool isSelected = false,
            bool isANSISequence = true)
        {
            Index = index;
            SyntacticRule = syntacticRule;
            Text = text;
            IsSelected = isSelected;
            IsANSISequence = isANSISequence;
        }

        public override string ToString() => $"{Index}->{Index + Text.Length - 1} : \"{ASCII.GetNonPrintablesCodesAsLabel(Text, false)}\"  ==  {SyntacticRule}";
    }
}