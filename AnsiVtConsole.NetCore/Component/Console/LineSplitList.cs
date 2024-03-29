﻿namespace AnsiVtConsole.NetCore.Component.Console
{
    sealed class LineSplitList
    {
        public readonly List<StringSegment> Splits;

        public readonly EchoSequenceList? PrintSequences;

        public readonly int CursorIndex;

        public readonly int CursorLineIndex;

        public LineSplitList(
            List<StringSegment> splits,
            EchoSequenceList? printSequences,
            int cursorIndex = -1,
            int cursorLineIndex = -1)
        {
            Splits = splits;
            PrintSequences = printSequences;
            CursorIndex = cursorIndex;
            CursorLineIndex = cursorLineIndex;
        }

    }
}
