using AnsiVtConsole.NetCore.Component.Parser.NonRecursiveFunctionalGrammar;

namespace AnsiVtConsole.NetCore.Component.Parser.ANSI
{
    /// <summary>
    /// ANSI parser. Use grammar defined in Component/Parser/ANSI/ansi-seq-patterns.txt
    /// </summary>
    public static class ANSIParser
    {
        #region attributes

        /// <summary>
        /// the grammar file name that must be loaded
        /// </summary>
        public const string GrammarFileName = "ansi-seq-patterns.txt";

        static readonly NonRecursiveFunctionGrammarParser _parser;

        #endregion

        #region init

        static ANSIParser()
        {
            var ap = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var p = Path.Combine(
                Path.GetDirectoryName(ap)!,
                "Component",
                "Parser",
                "ANSI",
                GrammarFileName);
            var lines = File.ReadLines(p);
            _parser = new NonRecursiveFunctionGrammarParser(lines);
        }

        #endregion

        /// <summary>
        /// get the syntax block list of a text
        /// </summary>
        /// <param name="s">text to be parsed</param>
        /// <returns>syntax block list</returns>
        public static SyntacticBlockList Parse(string s) => _parser.Parse(s);

        /// <summary>
        /// get the real length of the text without ansi sequences non printed characters
        /// </summary>
        /// <param name="s">text to be analyzed</param>
        /// <returns>length of visible part of the text</returns>
        public static int GetTextLength(string s) => GetText(s).Length;

        /// <summary>
        /// gets the text part of the syntactic elements
        /// </summary>
        /// <param name="s">text to be analyzed</param>
        /// <returns>string without ansi sequences</returns>
        public static string GetText(string s) => _parser.Parse(s).GetText();

        /// <summary>
        /// indicates wether or not a string starts with a known ansi sequence. the parsed syntax is assigned in the out parameter 'syntax'
        /// </summary>
        /// <param name="s">text to be parsed</param>
        /// <param name="syntax">parsed syntax</param>
        /// <returns>true if the given text starts with a known ansi sequence.</returns>
        public static bool StartsWithANSISequence(string s, out SyntacticBlockList syntax)
        {
            syntax = _parser.Parse(s);
            if (syntax.Count == 0)
                return false;
            return syntax[0].IsANSISequence;
        }
    }
}