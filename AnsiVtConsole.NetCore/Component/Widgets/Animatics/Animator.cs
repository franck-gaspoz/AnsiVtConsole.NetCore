#define dbg

namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics;

sealed class Animator
{
    Thread? _thread;
    bool _end = false;
    readonly Animation _animation;
    TimeLine? _timeLine;
    int _timeLineIndex;
    double _timeLapse;
    DateTime? _timeLineStartTime;
    DateTime? _timeLineEndTime;
#if dbg
    int _tick;
#endif

    public EventHandler OnStart;
    public EventHandler OnStop;

    public Animator(Animation animation) => _animation = animation;

    /// <summary>
    /// starts an animation
    /// </summary>
    public void Start()
    {
        if (!_animation.TimeLines.Any())
            return;

        (_thread = new(() => RunAnimation(_animation)))
             .Start();

        OnStart?.Invoke(this, EventArgs.Empty);

        _thread!.Start();
    }

    public void Stop()
    {
        if (_end) _end = false;
        OnStop?.Invoke(this, EventArgs.Empty);
    }

    void Start(TimeLine timeLine)
    {
#if dbg
        _tick = 0;
#endif
        _timeLineIndex = 0;
        _timeLine = timeLine;
        _timeLapse = GetTimeLapse(timeLine);
        _timeLineStartTime = DateTime.Now;
        _timeLineEndTime = _timeLineStartTime!.Value.Add(
            TimeSpan.FromMilliseconds(_timeLapse));
    }

    double GetTimeLapse(TimeLine timeLine)
    {
        var totalDuration = timeLine.Duration;
        var nbFrames = totalDuration * _animation.Fps;
        var timeLapse = totalDuration / nbFrames;
        return timeLapse;
    }

    void RunAnimation(object? obj)
    {

#if dbg
        Dbg("start animation");
#endif

        while (_timeLineIndex < _animation.TimeLines.Count)
        {
            Start(_animation.TimeLines[_timeLineIndex]);

#if dbg
            Dbg($"start timeline {_timeLineIndex} : {_timeLine!.Duration} ms");
#endif

            while (!_end)
            {
                var time = DateTime.Now;
                _end = time > _timeLineEndTime;

                foreach (var animation in _timeLine!.Animations)
                {
#if dbg
                    Dbg($"animate tick {_tick} : {animation} # {DateStr(DateTime.Now)} (-> {DateStr(_timeLineEndTime!.Value)})");
                    _tick++;
#endif
                }

                Thread.Sleep((int)_timeLapse);

            }

            _timeLineIndex++;
        }
    }

    static string DateStr(DateTime d)
        => $"{d.Hour}:{d.Month}:{d.Second}:{d.Millisecond}";

    void Dbg(string text) => System.Diagnostics.Debug.WriteLine(text);
}
