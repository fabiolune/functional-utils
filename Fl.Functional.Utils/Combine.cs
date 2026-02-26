using System;

namespace Fl.Functional.Utils;

public static partial class Functional
{
    /// <summary>
    /// Merges two <see cref="Action{T}"/> delegates into one.
    /// When <paramref name="nullableAction"/> is <c>null</c>, only <paramref name="defaultAction"/> is returned;
    /// otherwise both actions are composed and executed in sequence.
    /// </summary>
    /// <typeparam name="T">The type of the argument accepted by both actions.</typeparam>
    /// <param name="nullableAction">The primary action, which may be <c>null</c>.</param>
    /// <param name="defaultAction">The fallback action used when <paramref name="nullableAction"/> is <c>null</c>,
    /// and appended after it when it is not.</param>
    /// <returns>
    /// A combined <see cref="Action{T}"/> that runs <paramref name="nullableAction"/> followed by
    /// <paramref name="defaultAction"/>, or just <paramref name="defaultAction"/> when the former is <c>null</c>.
    /// </returns>
    public static Action<T> Combine<T>(this Action<T> nullableAction, Action<T> defaultAction) =>
        nullableAction
            .MakeOption()
            .Match(_ => nullableAction + defaultAction,
                   defaultAction);
}
