using LanguageExt;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace Functional.Utils;

public static partial class Functional
{
    public static Option<T> MakeOption<T>(this T @this) =>
        @this.MakeOption(_ => false);

    public static Option<T> MakeOption<T>(this T @this, Predicate<T> noneWhen) =>
        @this.MakeOption(identity, noneWhen);

    public static Option<TResult> MakeOption<TInput, TResult>(
        this TInput @this,
        Func<TInput, TResult> map,
        Predicate<TInput> noneWhen) =>
        !typeof(TInput).IsValueType && EqualityComparer<TInput>.Default.Equals(@this, default) || noneWhen(@this) ? None : Some(map(@this));

    public static Task<Option<T>> MakeOptionAsync<T>(this Task<T> @this) =>
        @this.MakeOptionAsync(_ => false);

    public static Task<Option<T>> MakeOptionAsync<T>(this Task<T> @this, Predicate<T> noneWhen) =>
        @this.MakeOptionAsync(identity, noneWhen);

    public static async Task<Option<TResult>> MakeOptionAsync<TInput, TResult>(this Task<TInput> @this,
                                                              Func<TInput, TResult> map,
                                                              Predicate<TInput> noneWhen) =>
        (await @this).MakeOption(map, noneWhen);
}
