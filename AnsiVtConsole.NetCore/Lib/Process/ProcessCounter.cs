namespace AnsiVtConsole.NetCore.Lib.Process
{
    /// <summary>
    /// process counter usable with ProcessWrapper
    /// </summary>
    sealed class ProcessCounter
    {
        int _counter;

        readonly object _counterLock = new();

        public static bool Log { get; set; } = true;

        public int Counter
        {
            get
            {
                lock (_counterLock)
                {
                    return _counter;
                }
            }
        }

        public int GetCounter() => _counter;

        public ProcessCounter() => _counter = 0;

        public void Increase()
        {
            lock (_counterLock)
            {
                _counter++;
                if (Log)
                    System.Diagnostics.Debug.WriteLine("ProcessCounter:Increased = " + _counter);
            }
        }

        public void Decrease()
        {
            lock (_counterLock)
            {
                _counter--;
                if (Log)
                    System.Diagnostics.Debug.WriteLine("ProcessCounter:Decreased = " + _counter);
            }
        }

        public void WaitForLessThan(int N)
        {
            if (Log)
                System.Diagnostics.Debug.WriteLine("ProcessCounter:WaitForLessThan " + N);
            var t = new Thread(() => WaitForLessThanNInternal(N));
            t.Start();
            t.Join();
        }

        void WaitForLessThanNInternal(int n)
        {
            var end = false;
            while (!end)
            {
                int v;
                lock (_counterLock)
                {
                    v = GetCounter();
                    if (v < n)
                        end = true;
                    if (!end)
                    {
                        Thread.Yield();
                        Thread.Sleep(50);
                    }
                }
            }
        }
    }
}
