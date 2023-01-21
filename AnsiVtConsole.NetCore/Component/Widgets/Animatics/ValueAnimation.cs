#define dbg

using System.Linq.Expressions;
using System.Reflection;

using AnsiVtConsole.NetCore.Component.Widgets.Animatics.Easings;

namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics;

/// <summary>
/// animation of value of type T
/// </summary>
/// <typeparam name="T">value type</typeparam>
public abstract class ValueAnimation<T> : IAnimation
{
    readonly List<object> _targets = new();

    PropertyInfo? _propertyInfo;

    /// <inheritdoc/>
    public T? Value { get; private set; }

    /// <inheritdoc/>
    public T? From { get; private set; }

    /// <inheritdoc/>
    public T To { get; private set; }

    /// <inheritdoc/>
    public T Increment { get; private set; }

    /// <inheritdoc/>
    public double Duration { get; private set; }

    /// <inheritdoc/>
    public Easing Easing { get; private set; }

    /// <inheritdoc/>
    TValue? IAnimation.Value<TValue>()
        where TValue : class
            => Value as TValue;

    /// <inheritdoc/>
    TValue? IAnimation.From<TValue>()
        where TValue : class
            => From as TValue;

    /// <inheritdoc/>
    TValue? IAnimation.To<TValue>()
        where TValue : class
            => To as TValue;

    /// <inheritdoc/>
    TValue? IAnimation.Increment<TValue>()
        where TValue : class
            => To as TValue;

    /// <inheritdoc/>
    double IAnimation.Duration => Duration;

    /// <summary>
    /// animation
    /// </summary>
    /// <param name="from">from</param>
    /// <param name="to">to</param>
    /// <param name="duration">duration</param>
    /// <param name="increment">increment</param>
    /// <param name="easing">easing function (default linear)</param>
    public ValueAnimation(
        T from,
        T to,
        double duration,
        T increment,
        Easing? easing = null)
    {
        Increment = increment;
        From = from;
        To = to;
        Duration = duration;
        Easing = easing ?? new Linear();
    }

    /// <summary>
    /// animation
    /// </summary>
    /// <param name="to">to</param>
    /// <param name="duration">duration</param>
    /// <param name="increment">increment</param>
    /// <param name="easing">easing function (default linear)</param>
    public ValueAnimation(
        T to,
        double duration,
        T increment,
        Easing? easing = null)
    {
        Increment = increment;
        To = to;
        Duration = duration;
        Easing = easing ?? new Linear();
        SetInitialValue();
    }

    /// <inheritdoc/>
    public IAnimation For(object target, string propertyName)
    {
        _propertyInfo = target.GetType()
            .GetProperty(propertyName)
                ?? throw new ArgumentException($"property {propertyName} not found in object: {target}");

        _targets.Add(_targets);

        return this;
    }

    /// <inheritdoc/>
    public IAnimation For<TargetType>(string propertyName)
    {
        _propertyInfo = typeof(TargetType)
            .GetProperty(propertyName)
                ?? throw new ArgumentException($"property {propertyName} not found in object: {typeof(TargetType)}");

        return this;
    }

    /// <inheritdoc/>
    public IAnimation For(object target, LambdaExpression expression)
    {
        Type? returnType;
        if (expression.Body is MemberExpression expr
            && expr.Member is PropertyInfo propertyInfo)
            returnType = expression.ReturnType;
        else
            throw new ArgumentException("expression is not valid", nameof(expression));
        if (returnType != typeof(T))
            throw new ArgumentException("expression is not valid. Expected return type: " + typeof(T).Name);

        _propertyInfo = propertyInfo;
        _targets.Add(target);

        return this;
    }

    /// <inheritdoc/>
    public IAnimation For(LambdaExpression expression)
    {
        Type? returnType;
        if (expression.Body is MemberExpression expr
            && expr.Member is PropertyInfo propertyInfo)
            returnType = expression.ReturnType;
        else
            throw new ArgumentException("expression is not valid", nameof(expression));
        if (returnType != typeof(T))
            throw new ArgumentException("expression is not valid. Expected return type: " + typeof(T).Name);

        _propertyInfo = propertyInfo;

        return this;
    }

    /// <inheritdoc/>
    /// <returns>this object</returns>
    public IAnimation Target(params object[] targets)
    {
        _targets.AddRange(targets);
        return this;
    }

    /// <inheritdoc/>
    public void SetValueAt(double position, bool reverse)
    {
        var to = reverse ? From : To;
        var from = reverse ? To : From;
        var toBack = To;
        var fromBack = From;
        To = to;
        From = from;

        SetValueAt(position);

        To = toBack;
        From = fromBack;
    }

    /// <inheritdoc/>
    public abstract void SetValueAt(double position);

    /// <inheritdoc/>
    protected void SetInitialValue()
    {
        if (_propertyInfo is null)
            throw new InvalidDataException("property is not defined");

        foreach (var target in _targets)
            From = GetValue(_propertyInfo, target);
    }

    /// <summary>
    /// set value of target
    /// </summary>
    /// <param name="value">value</param>
    protected void SetValue(T value)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine("value = " + value);
#endif
        if (_propertyInfo is null)
            throw new InvalidDataException("property is not defined");

        foreach (var target in _targets)
            SetValue(_propertyInfo, target, value);
    }

    static void SetValue(PropertyInfo propertyInfo, object target, T value)
        => propertyInfo.SetValue(target, value);

    static T? GetValue(PropertyInfo propertyInfo, object target)
        => (T?)propertyInfo.GetValue(target);
}
