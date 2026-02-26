using System;
using System.Collections.Generic;
using LanguageExt;

namespace Fl.Functional.Utils;

public static partial class Functional
{
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
