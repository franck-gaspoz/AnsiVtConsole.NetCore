using System.Collections;
using System.Text;

namespace AnsiVtConsole.NetCore.Component.Console
{
    internal sealed class EchoSequenceList : IEnumerable<EchoSequence>
    {
        public readonly List<EchoSequence> List
            = new List<EchoSequence>();

        public void Add(EchoSequence printSequence) => List.Add(printSequence);

        public override string ToString()
        {
            var r = new StringBuilder();
            foreach (var printSequence in List)
                r.AppendLine(printSequence.ToString());
            return r.ToString();
        }

        public string ToStringPattern()
        {
            var r = new StringBuilder();
            foreach (var printSequence in List)
                r.Append(printSequence.ToStringPattern());
            return r.ToString();
        }

        public IEnumerator<EchoSequence> GetEnumerator() => List.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => List.GetEnumerator();

        public int TextLength
        {
            get
            {
                var n = 0;
                foreach (var seq in List)
                    if (seq.IsText)
                        n += seq.Length;
                return n;
            }
        }
    }
}
