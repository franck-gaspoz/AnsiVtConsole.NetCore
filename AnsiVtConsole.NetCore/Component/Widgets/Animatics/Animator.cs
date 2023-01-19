namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics;

sealed class Animator
{
    public bool IsRunning { get; private set; }

    Thread? _thread;
    bool _end = false;
    readonly Animation _animation;
    TimeLine? _timeLine;
    int _timeLineIndex;
    double _timeLapse;
    DateTime? _timeLineStartTime;
    DateTime? _timeLineEndTime;
#if DEBUG
    int _tick;
#endif

    public event EventHandler OnStart;
    public event EventHandler OnStop;

#pragma warning disable CS8618
    public Animator(Animation animation) => _animation = animation;
#pragma warning restore CS8618

    /// <summary>
    /// starts an animation
    /// </summary>
    public void Start()
    {
        IsRunning = true;
        _timeLapse = GetTimeLapse();

        if (!_animation.TimeLines.Any())
            return;

        OnStart?.Invoke(this, EventArgs.Empty);

        (_thread = new(() => RunAnimation(_animation)))
             .Start();
    }

    public void Stop()
    {
        if (_end) _end = false;
        OnStop?.Invoke(this, EventArgs.Empty);
    }

    void Start(TimeLine timeLine)
    {
#if DEBUG
        _tick = 0;
#endif
        _timeLineIndex = 0;
        _timeLine = timeLine;
        _timeLineStartTime = DateTime.Now;
        _timeLineEndTime = _timeLineStartTime!.Value.Add(
            TimeSpan.FromMilliseconds(_timeLine.Duration));
    }

    double GetTimeLapse()
        => 1 / _animation.Fps * 1000;

    void RunAnimation(object? obj)
    {
#if DEBUG
        Dbg($"start animation | fps={_animation.Fps} | timeLapse = {_timeLapse} ms");
#endif

        while (_timeLineIndex < _animation.TimeLines.Count)
        {
            Start(_animation.TimeLines[_timeLineIndex]);

#if DEBUG
            Dbg($"start timeline {_timeLineIndex} : {_timeLine!.Duration} ms");
#endif

            while (!_end)
            {
                var time = DateTime.Now;
                _end = time > _timeLineEndTime;
                var position = (time - _timeLineStartTime!.Value)
                    .TotalMilliseconds;

                foreach (var animation in _timeLine!.Animations)
                {
#if DEBUG
                    Dbg($"animate tick {_tick} position {position} : {animation} # {DateStr(DateTime.Now)} (-> {DateStr(_timeLineEndTime!.Value)})");
                    _tick++;
#endif
                    animation.SetValueAt(position);
                }

                Thread.Sleep((int)_timeLapse);

            }

            _timeLineIndex++;
        }

        IsRunning = false;
        OnStop?.Invoke(this, EventArgs.Empty);
    }

#if DEBUG
    static string DateStr(DateTime d)
        => $"{d.Hour}:{d.Month}:{d.Second}:{d.Millisecond}";

    static void Dbg(string text) => System.Diagnostics.Debug.WriteLine(text);
#endif
}
