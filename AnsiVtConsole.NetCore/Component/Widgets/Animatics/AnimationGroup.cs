using System.Linq.Expressions;

namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics;

/// <summary>
/// value animation group - usefull to setup properties for a set of animations in one time
/// </summary>
public sealed class AnimationGroup
{
    /// <summary>
    /// animations
    /// </summary>
    public IReadOnlyList<IAnimation> Animations => _animations;

    readonly List<IAnimation> _animations = new();

    /// <summary>
    /// animation group
    /// </summary>
    /// <param name="animations">animations in the group</param>
    public AnimationGroup(params IAnimation[] animations)
        => _animations.AddRange(animations);

    /// <summary>
    /// add target property of a class to animations in the group
    /// </summary>
    /// <param name="expression">linq expression that reference the target property of an object: () => obj.a.b.. Expression0&lt;Func&lt;ValueTypeglt;&gt;&gt;</param>
    /// <returns>this object</returns>
    public AnimationGroup For(LambdaExpression expression)
    {
        foreach (var anim in _animations)
            anim.For(expression);
        return this;
    }

    /// <summary>
    /// add target property of a class to animation in the group
    /// </summary>
    /// <param name="propertyName">property name</param>
    /// <returns>this object</returns>
    public AnimationGroup For<TargetType>(string propertyName)
    {
        foreach (var anim in _animations)
            anim.For<TargetType>(propertyName);
        return this;
    }

    /// <summary>
    /// setup target(s) for animations in the group
    /// </summary>
    /// <param name="targets">one or several targets</param>
    /// <returns>this object</returns>
    public AnimationGroup Target(params object[] targets)
    {
        foreach (var anim in _animations)
            anim.Target(targets);
        return this;
    }
}
