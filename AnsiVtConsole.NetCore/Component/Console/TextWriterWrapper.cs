using System.Runtime.CompilerServices;
using System.Text;

namespace AnsiVtConsole.NetCore.Component.Console
{
    public class TextWriterWrapper
    {
        #region attributes

        private static int _instanceCounter = 1000;

        private static readonly object _instanceLock = new object();

        /// <summary>
        /// id
        /// </summary>
        public int ID;

        public override string ToString() => $"[text writer wrapper: id={ID} isMute={IsMute}]";

        /// <summary>
        /// is oputput not muted
        /// </summary>
        public bool IsNotMute => !IsMute;

        /// <summary>
        /// is output muted
        /// </summary>
        public bool IsMute { get; set; }

        /// <summary>
        /// is modified
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        /// is redirected
        /// </summary>
        public bool IsRedirected { get; protected set; }

        /// <summary>
        /// is buffer enabled
        /// </summary>
        public bool IsBufferEnabled { get; protected set; }

        /// <summary>
        /// initial buffer capacity
        /// </summary>
        public static int InitialBufferCapacity { get; set; } = 1_000_000;

        /// <summary>
        /// get lock
        /// </summary>
        public object? Lock => _textWriter;

        /// <summary>
        /// initial TextWriterInitialCapacity buffer capacity
        /// </summary>
        public int TextWriterInitialCapacity = 1_000_000;

        /// <summary>
        /// text writer
        /// </summary>
        private TextWriter? _textWriter;

        /// <summary>
        /// redirected text writer
        /// </summary>
        protected TextWriter? RedirectedTextWriter;

        /// <summary>
        /// memory buffer
        /// </summary>
        protected readonly MemoryStream Buffer = new(InitialBufferCapacity);

        /// <summary>
        /// buffer writer
        /// </summary>
        protected TextWriter? BufferWriter;

        /// <summary>
        /// record
        /// </summary>
        protected StringBuilder Recording = new(InitialBufferCapacity);

        /// <summary>
        /// is recording enabled
        /// </summary>
        public bool IsRecordingEnabled { get; protected set; }

        #region echo to filestream

        /// <summary>
        /// FileEchoDebugDumpDebugInfo
        /// </summary>
        public bool FileEchoDebugDumpDebugInfo = true;

        /// <summary>
        /// FileEchoDebugCommands
        /// </summary>
        public bool FileEchoDebugCommands = true;

        /// <summary>
        /// FileEchoDebugAutoFlush
        /// </summary>
        public bool FileEchoDebugAutoFlush = true;

        /// <summary>
        /// FileEchoDebugAutoLineBreak
        /// </summary>
        public bool FileEchoDebugAutoLineBreak = true;

        /// <summary>
        /// FileEchoDebugEnabled
        /// </summary>
        public bool FileEchoDebugEnabled => DebugEchoStreamWriter != null;

        /// <summary>
        /// debug stream writer
        /// </summary>
        protected StreamWriter? DebugEchoStreamWriter;

        /// <summary>
        /// debug file stream writer
        /// </summary>
        protected FileStream? DebugEchoFileStream;

        #endregion

        #region echo to memory

        /// <summary>
        /// replicate auto flush
        /// </summary>
        public bool ReplicateAutoFlush = true;

        /// <summary>
        /// is replication enbaled
        /// </summary>
        public bool IsReplicationEnabled => ReplicateStreamWriter != null;

        /// <summary>
        /// Replicate stream writer
        /// </summary>
        protected StreamWriter? ReplicateStreamWriter;

        /// <summary>
        /// Memory stream
        /// </summary>
        protected MemoryStream? ReplicateMemoryStream;

        /// <summary>
        /// capture memory stream
        /// </summary>
        protected MemoryStream? CaptureMemoryStream;

        /// <summary>
        /// replicate file stream
        /// </summary>
        protected FileStream? ReplicateFileStream;

        #endregion

        #endregion

        #region construction & init

        /// <summary>
        /// TextWriterWrapper
        /// </summary>
        public TextWriterWrapper()
        {
            Init();
            _textWriter = new StreamWriter(new MemoryStream(TextWriterInitialCapacity));
        }

        /// <summary>
        /// TextWriterWrapper
        /// </summary>
        /// <param name="textWriter">text writter</param>
        public TextWriterWrapper(TextWriter textWriter)
        {
            Init();
            _textWriter = textWriter;
        }

        private void Init()
        {
            lock (_instanceLock)
            {
                ID = _instanceCounter;
                _instanceCounter++;
            }
        }

        #endregion

        #region recording

        /// <summary>
        /// enable recording the stream in a string builder. clear the record on start
        /// </summary>
        public void StartRecording()
        {
            Recording.Clear();
            IsRecordingEnabled = true;
        }

        /// <summary>
        /// stop recording the stream. returns &amp; clear the record
        /// </summary>
        /// <returns>what has been recorded</returns>
        public string StopRecording()
        {
            IsRecordingEnabled = false;
            var r = Recording.ToString();
            Recording.Clear();
            return r;
        }

        #endregion

        #region stream operations

        /// <summary>
        /// capture the output stream to a string
        /// </summary>
        public void Capture()
        {
            lock (this)
            {
                if (RedirectedTextWriter == null && CaptureMemoryStream == null)
                {
                    CaptureMemoryStream = new MemoryStream();
                    var sw = new StreamWriter(CaptureMemoryStream);
                    RedirectedTextWriter = _textWriter;
                    _textWriter = sw;
                    IsRedirected = true;
                }
            }
        }

        /// <summary>
        /// stop capture
        /// </summary>
        /// <returns>captured stream</returns>
        public string? StopCapture()
        {
            lock (this)
            {
                if (CaptureMemoryStream != null)
                {
                    _textWriter!.Flush();

                    CaptureMemoryStream.Position = 0;
                    var str = Encoding.Default.GetString(CaptureMemoryStream.ToArray());

                    _textWriter.Close();
                    _textWriter = RedirectedTextWriter;
                    RedirectedTextWriter = null;
                    CaptureMemoryStream = null;
                    IsRedirected = false;
                    return str;
                }
                return null;
            }
        }

        /// <summary>
        /// redirect to text writer
        /// </summary>
        /// <param name="sw">text witer</param>
        public void Redirect(TextWriter? sw)
        {
            if (sw != null)
            {
                RedirectedTextWriter = _textWriter;
                _textWriter = sw;
                IsRedirected = true;
            }
            else
            {
                _textWriter!.Flush();
                _textWriter.Close();
                _textWriter = RedirectedTextWriter;
                RedirectedTextWriter = null;
                IsRedirected = false;
            }
        }

        /// <summary>
        /// redirect to file
        /// </summary>
        /// <param name="filePath">output file</param>
        public void Redirect(string? filePath = null)
        {
            if (filePath != null)
            {
                RedirectedTextWriter = _textWriter;
                _textWriter = new StreamWriter(new FileStream(filePath, FileMode.Append, FileAccess.Write));
                IsRedirected = true;
            }
            else
            {
                _textWriter!.Flush();
                _textWriter.Close();
                _textWriter = RedirectedTextWriter;
                RedirectedTextWriter = null;
                IsRedirected = false;
            }
        }

        /// <summary>
        /// echo Out to a memory stream
        /// </summary>
        /// <param name="autoFlush">auto flush</param>
        public void ReplicateToMem(
            bool autoFlush = false
            )
        {
            lock (this)
            {
                StopReplicate();
                ReplicateAutoFlush = autoFlush;
                ReplicateMemoryStream = new MemoryStream();
                ReplicateStreamWriter = new StreamWriter(ReplicateMemoryStream);
            }
        }

        /// <summary>
        /// echo Out to a file
        /// </summary>
        /// <param name="filepath">file path where to echo Out</param>
        /// <param name="autoFlush">if set, flush Out before each echo</param>
        public void ReplicateToFile(
            string filepath,
            bool autoFlush = false)
        {
            lock (this)
            {
                if (!string.IsNullOrWhiteSpace(filepath) && DebugEchoFileStream == null)
                {
                    StopReplicate();
                    ReplicateAutoFlush = autoFlush;
                    ReplicateFileStream = new FileStream(filepath, FileMode.Append, FileAccess.Write);
                    ReplicateStreamWriter = new StreamWriter(ReplicateFileStream);
                }
            }
        }

        /// <summary>
        /// stop replicate
        /// </summary>
        /// <returns>captured stream</returns>
        public string? StopReplicate()
        {
            if (ReplicateFileStream != null)
            {
                ReplicateStreamWriter!.Flush();
                ReplicateStreamWriter.Close();
                ReplicateFileStream = null;
                ReplicateStreamWriter = null;
                return null;
            }
            if (ReplicateMemoryStream != null)
            {
                ReplicateStreamWriter!.Flush();
                ReplicateMemoryStream.Position = 0;
                var str = Encoding.Default.GetString(ReplicateMemoryStream.ToArray());
                ReplicateStreamWriter.Close();
                ReplicateMemoryStream = null;
                ReplicateStreamWriter = null;
                return str;
            }
            return null;
        }

        #endregion

        #region buffering operations

        /// <summary>
        /// flush text writter
        /// </summary>
        public virtual void Flush()
        {
            lock (Lock!)
            {
                if (IsBufferEnabled)
                    return;
                _textWriter!.Flush();
            }
        }

        /// <summary>
        /// enabled buffer
        /// </summary>
        public virtual void EnableBuffer()
        {
            lock (Lock!)
            {
                if (IsBufferEnabled)
                    return;
                if (BufferWriter == null)
                    BufferWriter = new StreamWriter(Buffer);
                IsBufferEnabled = true;
            }
        }

        /// <summary>
        /// close buffer
        /// </summary>
        public virtual void CloseBuffer()
        {
            lock (Lock!)
            {
                if (!IsBufferEnabled)
                    return;
                Buffer.Seek(0, SeekOrigin.Begin);
                var txt = Encoding.Default.GetString(Buffer.ToArray());
                _textWriter!.Write(txt);
                Buffer.SetLength(0);
                IsBufferEnabled = false;
            }
        }

        #endregion

        #region stream write operations

        /// <summary>
        /// writes a string to the stream
        /// </summary>
        /// <param name="s">string to be written to the stream</param>
        public virtual void WriteStream(string s)
        {
            if (IsMute)
                return;

            var modifiantStr = !string.IsNullOrEmpty(s);
            IsModified |= modifiantStr;

            if (modifiantStr && IsRecordingEnabled)
                Recording.Append(s);

            if (IsReplicationEnabled)
                ReplicateStreamWriter!.Write(s);

            if (IsBufferEnabled)
            {
                BufferWriter!.Write(s);
            }
            else
            {
                _textWriter!.Write(s);
            }
        }

        /// <summary>
        /// writes a string to the stream
        /// </summary>
        /// <param name="s">string to be written to the stream</param>
        public virtual Task WriteAsync(string s)
        {
            if (IsMute)
                return Task.CompletedTask;

            IsModified = !string.IsNullOrWhiteSpace(s);

            if (IsModified && IsRecordingEnabled)
                Recording.Append(s);

            if (IsReplicationEnabled)
                ReplicateStreamWriter!.WriteAsync(s);

            if (IsBufferEnabled)
            {
                return BufferWriter!.WriteAsync(s);
            }
            else
            {
                return _textWriter!.WriteAsync(s);
            }
        }

        /// <summary>
        /// writes a string to the stream
        /// </summary>
        /// <param name="s">string to be written to the stream</param>
        public virtual void WriteLineStream(string s)
        {
            if (IsMute)
                return;

            var modifiantStr = !string.IsNullOrEmpty(s);
            IsModified |= modifiantStr;

            if (modifiantStr && IsRecordingEnabled)
                Recording.AppendLine(s);

            if (IsReplicationEnabled)
                ReplicateStreamWriter!.WriteLine(s);

            if (IsBufferEnabled)
            {
                BufferWriter!.WriteLine(s);
            }
            else
            {
                _textWriter!.WriteLine(s);
            }
        }

        /// <summary>
        /// internal debug method about echo
        /// </summary>
        /// <param name="s"></param>
        /// <param name="lineBreak"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerLineNumber"></param>
        internal virtual void EchoDebug(
            string s,
            bool lineBreak = false,
            [CallerMemberName] string callerMemberName = "",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            if (IsMute)
                return;
            if (!FileEchoDebugEnabled)
                return;
            if (FileEchoDebugDumpDebugInfo)
                DebugEchoStreamWriter?.Write($"l={s.Length},br={lineBreak} [{callerMemberName}:{callerLineNumber}] :");
            DebugEchoStreamWriter?.Write(s);
            if (lineBreak | FileEchoDebugAutoLineBreak)
                DebugEchoStreamWriter?.WriteLine(string.Empty);
            if (FileEchoDebugAutoFlush)
                DebugEchoStreamWriter?.Flush();
        }

        #endregion
    }
}
