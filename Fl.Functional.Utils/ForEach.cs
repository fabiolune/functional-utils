using System;
using System.Collections.Generic;
using LanguageExt;

namespace Fl.Functional.Utils;

public static partial class Functional
{
    /// <summary>
    /// Iterates over <paramref name="items"/> and invokes <paramref name="action"/> for each element.
    /// Returns <c>None</c> when <paramref name="items"/> is <c>null</c>, so callers can safely handle
    /// the missing-collection case via <c>Option</c> pattern matching.
    /// </summary>
    /// <typeparam name="T">The element type of the collection.</typeparam>
    /// <param name="items">The collection to iterate. May be <c>null</c>.</param>
    /// <param name="action">The action to invoke for each element.</param>
    /// <returns>
    /// <c>Some(<see cref="Unit"/>)</c> after the iteration completes, or <c>None</c> when
    /// <paramref name="items"/> is <c>null</c>.
    /// </returns>
    public static Option<Unit> ForEach<T>(this IEnumerable<T> items, Action<T> action) =>
        items
            .MakeOption()
            .Match(_ =>
            {
                foreach (var obj in _)
                    action(obj);
                return Unit.Default;
            },
            Option<Unit>.None);
}
