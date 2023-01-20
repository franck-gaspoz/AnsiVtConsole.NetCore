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
    public T? Value { get; protected set; }

    /// <inheritdoc/>
    public T? From { get; protected set; }

    /// <inheritdoc/>
    public T? To { get; protected set; }

    /// <inheritdoc/>
    public double Duration { get; protected set; }

    /// <inheritdoc/>
    public Easing Easing { get; protected set; }

    /// <inheritdoc/>
    public bool IsLoop { get; }

    /// <inheritdoc/>
    public bool IsAutoReverse { get; }

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
    double IAnimation.Duration => Duration;

    /// <summary>
    /// animation
    /// </summary>
    /// <param name="from">from</param>
    /// <param name="to">to</param>
    /// <param name="duration">duration</param>
    /// <param name="easing">easing function (default linear)</param>
    public ValueAnimation(
        T from,
        T to,
        double duration,
        Easing? easing = null)
    {
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
    /// <param name="easing">easing function (default linear)</param>
    public ValueAnimation(
        T to,
        double duration,
        Easing? easing = null)
    {
        To = to;
        Duration = duration;
        Easing = easing ?? new Linear();
    }

    /// <summary>
    /// add target property of an object to animation
    /// </summary>
    /// <param name="target">target</param>
    /// <param name="propertyName">property name</param>
    /// <returns>this object</returns>
    public ValueAnimation<T> For(object target, string propertyName)
    {
        _propertyInfo = target.GetType()
            .GetProperty(propertyName)
                ?? throw new ArgumentException($"property {propertyName} not found in object: {target}");

        _targets.Add(_targets);

        return this;
    }

    /// <summary>
    /// add target property of a class to animation
    /// </summary>
    /// <param name="propertyName">property name</param>
    /// <returns>this object</returns>
    public ValueAnimation<T> For<TargetType>(string propertyName)
    {
        _propertyInfo = typeof(TargetType)
            .GetProperty(propertyName)
                ?? throw new ArgumentException($"property {propertyName} not found in object: {typeof(TargetType)}");

        return this;
    }

    /// <summary>
    /// add target property of an object to animation
    /// </summary>
    /// <param name="target">animation target object</param>
    /// <param name="expression">linq expression that reference the target property of an object: () => obj.a.b.. Expression0&lt;Func&lt;ValueTypeglt;&gt;&gt;</param>
    /// <returns>this object</returns>
    public ValueAnimation<T> For(object target, LambdaExpression expression)
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

    /// <summary>
    /// add target property of a class to animation
    /// </summary>
    /// <param name="expression">linq expression that reference the target property of an object: () => obj.a.b.. Expression0&lt;Func&lt;ValueTypeglt;&gt;&gt;</param>
    /// <returns>this object</returns>
    public ValueAnimation<T> For(LambdaExpression expression)
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

    /// <summary>
    /// setup target(s) for this animation
    /// </summary>
    /// <param name="targets">one or several targets</param>
    /// <returns>this object</returns>
    public ValueAnimation<T> Target(params object[] targets)
    {
        _targets.AddRange(targets);
        return this;
    }

    /// <inheritdoc/>
    public abstract void SetValueAt(double position);

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
}
