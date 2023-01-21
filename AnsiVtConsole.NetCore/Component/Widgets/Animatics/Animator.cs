using System.Diagnostics;

using Microsoft.CodeAnalysis;

namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics;

/// <summary>
/// sequential play of timelines
/// </summary>
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
    double _sumAnimationDuration;
    int _tick;

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

    /// <summary>
    /// stops the animation
    /// </summary>
    public void Stop()
    {
        if (_end) _end = false;
        OnStop?.Invoke(this, EventArgs.Empty);
        _thread = null;
    }

    void Start(bool reverse, TimeLine timeLine)
    {
#if DEBUG
        _tick = 0;
#endif
        _sumAnimationDuration = 0;
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
        Dbg($"start timeline {_timeLineIndex} | fps={_animation.Fps} | timeLapse = {_timeLapse} ms");
#endif

        var reverse = false;

        while (_timeLineIndex < _animation.TimeLines.Count)
        {
            Start(reverse, _animation.TimeLines[_timeLineIndex]);

#if DEBUG
            Dbg($"start timeline {_timeLineIndex} : {_timeLine!.Duration} ms");
#endif

            while (!_end)
            {
                var time = DateTime.Now;
                _end = time > _timeLineEndTime;

                var position = !reverse ?
                    (time - _timeLineStartTime!.Value)
                        .TotalMilliseconds
                    : (_timeLineEndTime!.Value - time)
                        .TotalMilliseconds;

                var frameStartTime = DateTime.Now;

                foreach (var animation in _timeLine!.Animations)
                {
#if DEBUG
                    Dbg($"animate tick {_tick} position {position} : {animation} # {DateStr(DateTime.Now)} (-> {DateStr(_timeLineEndTime!.Value)})");
#endif
                    animation.SetValueAt(
                        Math.Min(
                            animation.Duration, position),
                        reverse);
                }

                _tick++;
                _timeLine.Render();

                var frameDuration = (DateTime.Now - frameStartTime).TotalMilliseconds;
                _sumAnimationDuration += frameDuration;

                if (frameDuration > _timeLapse)
                    Debug.WriteLine(
                        $"animation frame time overriden: took {frameDuration}ms but frame length is {_timeLapse} ms");

                var wait = Math.Max(0, _timeLapse - frameDuration);

                Thread.Sleep((int)wait);
            }

            if (_timeLine.IsAutoReverse)
            {
                reverse = !reverse;
            }

            if (_timeLine.IsLoop)
            {

            }
#if DEBUG
            if (_tick > 0)
                Debug.WriteLine($"average animation duration = {_sumAnimationDuration / _tick} ms. frame delay = {_timeLapse} ms");
#endif
            if (!_timeLine.IsAutoReverse && !_timeLine.IsLoop)
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
