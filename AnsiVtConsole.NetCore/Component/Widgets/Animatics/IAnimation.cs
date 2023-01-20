using System.Linq.Expressions;

using AnsiVtConsole.NetCore.Component.Widgets.Animatics.Easings;

namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics;

/// <summary>
/// animation
/// </summary>
public interface IAnimation
{
    /// <summary>
    /// current value
    /// </summary>
    /// <typeparam name="TValue">value type</typeparam>
    /// <returns>value</returns>
    TValue? Value<TValue>()
        where TValue : class;

    /// <summary>
    /// from value
    /// </summary>
    /// <typeparam name="TValue">value type</typeparam>
    /// <returns>value</returns>
    TValue? From<TValue>()
        where TValue : class;

    /// <summary>
    /// to value
    /// </summary>
    /// <typeparam name="TValue">value type</typeparam>
    /// <returns>value</returns>
    TValue? To<TValue>()
        where TValue : class;

    /// <summary>
    /// add target property of a class to animation
    /// </summary>
    /// <param name="expression">linq expression that reference the target property of an object: () => obj.a.b.. Expression0&lt;Func&lt;ValueTypeglt;&gt;&gt;</param>
    /// <returns>this object</returns>
    IAnimation For(LambdaExpression expression);

    /// <summary>
    /// add target property of a class to animation
    /// </summary>
    /// <param name="propertyName">property name</param>
    /// <returns>this object</returns>
    IAnimation For<TargetType>(string propertyName);

    /// <summary>
    /// add target property of an object to animation
    /// </summary>
    /// <param name="target">target</param>
    /// <param name="propertyName">property name</param>
    /// <returns>this object</returns>
    IAnimation For(object target, string propertyName);

    /// <summary>
    /// add target property of an object to animation
    /// </summary>
    /// <param name="target">animation target object</param>
    /// <param name="expression">linq expression that reference the target property of an object: () => obj.a.b.. Expression0&lt;Func&lt;ValueTypeglt;&gt;&gt;</param>
    /// <returns>this object</returns>
    IAnimation For(object target, LambdaExpression expression);

    /// <summary>
    /// setup target(s) for this animation
    /// </summary>
    /// <param name="targets">one or several targets</param>
    /// <returns>this object</returns>
    IAnimation Target(params object[] targets);

    /// <summary>
    /// duration (ms)
    /// </summary>
    /// <returns>value</returns>
    double Duration { get; }

    /// <summary>
    /// easing function
    /// </summary>
    public Easing Easing { get; }

    /// <summary>
    /// is cyclic
    /// </summary>
    bool IsLoop { get; }

    /// <summary>
    /// auto reverse or not when cyclic
    /// </summary>
    bool IsAutoReverse { get; }

    /// <summary>
    /// set the value for any position in the animation time line
    /// </summary>
    /// <param name="position">position (ms)</param>
    abstract void SetValueAt(double position);
}