using System;

namespace Functional.Utils;

public static partial class Functional
{
    public static Action<T> Combine<T>(this Action<T> nullableAction, Action<T> defaultAction) =>
        nullableAction
            .MakeOption()
            .Match(_ => nullableAction + defaultAction,
                   defaultAction);
}
