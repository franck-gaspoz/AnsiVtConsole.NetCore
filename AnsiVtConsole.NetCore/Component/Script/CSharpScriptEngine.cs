using AnsiVtConsole.NetCore.Component.Console;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace AnsiVtConsole.NetCore.Component.Script
{
    /// <summary>
    /// c# script engine
    /// </summary>
    public sealed class CSharpScriptEngine
    {
        private readonly Dictionary<string, Script<object>> _csscripts = new();

        /// <summary>
        /// default script options
        /// </summary>
        public ScriptOptions DefaultScriptOptions;

#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        /// <summary>
        /// CSharpScriptEngine
        /// </summary>
        /// <param name="console"></param>
        public CSharpScriptEngine(IAnsiVtConsole console) => Init(console);
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

        /// <summary>
        /// CSharpScriptEngine
        /// </summary>
        /// <param name="console"></param>
        /// <param name="defaultScriptOptions"></param>
        public CSharpScriptEngine(IAnsiVtConsole console, ScriptOptions defaultScriptOptions)
        {
            DefaultScriptOptions = defaultScriptOptions;
            Init(console);
        }

        private void Init(IAnsiVtConsole console)
        {
            DefaultScriptOptions ??= ScriptOptions.Default;
            DefaultScriptOptions = DefaultScriptOptions
                .AddImports("System")
                .AddReferences(console.GetType().Assembly);
        }

        /// <summary>
        /// execute a csharp script
        /// </summary>
        /// <param name="csharpText">source code</param>
        /// <param name="out">output</param>
        /// <param name="scriptOptions">script options</param>
        /// <returns>script return value</returns>
        public object? ExecCSharp(
            string csharpText,
            ConsoleTextWriterWrapper @out,
            ScriptOptions? scriptOptions = null
            )
        {
            try
            {
                scriptOptions ??= DefaultScriptOptions;
                var scriptKey = csharpText;
                if (!_csscripts.TryGetValue(scriptKey, out var script))
                {
                    script = CSharpScript.Create<object>(
                        csharpText,
                        scriptOptions
                        );
                    var cpl = script.Compile();
                    _csscripts[scriptKey] = script;
                }
                var res = script.RunAsync();
                return res.Result.ReturnValue;
            }
            catch (CompilationErrorException ex)
            {
                @out?.Errorln($"{csharpText}");
                @out?.Errorln(string.Join(Environment.NewLine, ex.Diagnostics));
                return null;
            }
        }
    }
}
