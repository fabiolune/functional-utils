using LanguageExt;
using System;
using System.Threading.Tasks;

namespace Functional.Utils;

public static partial class Functional
{
    public static TResult Map<TSource, TResult>(this TSource @this, Func<TSource, TResult> fn) =>
        fn(@this);

    public static (TResult, TResult) SameMap<TSource, TResult>(this (TSource, TSource) @this,
        Func<TSource, TResult> fn) =>
        (fn(@this.Item1), fn(@this.Item2));

    public static async Task<TResult> MapAsync<TSource, TResult>(this Task<TSource> @this,
                                                                 Func<TSource, Task<TResult>> fn) =>
        await fn(await @this);

    public static Task<TResult> MapAsync<TSource, TResult>(this TSource @this,
                                                           Func<TSource, Task<TResult>> fn) =>
        fn(@this);

    public static async Task<TResult> MapAsync<TSource, TResult>(this Task<TSource> @this,
                                                                 Func<TSource, TResult> fn) =>
        fn(await @this);

    public static async Task<Either<TMl, TR>> MapLeftAsync<TL, TR, TMl>(this Task<Either<TL, TR>> @this,
                                                                       Func<TL, Task<TMl>> onLeftAsync)
    {
        var either = await @this;
        return await either
                .MatchAsync<Either<TMl, TR>>(
                    _ => _,
                    async left => await onLeftAsync(left)
                );
    }
    public static async Task<Either<TMl, TR>> MapLeftAsync<TL, TR, TMl>(this Task<Either<TL, TR>> @this, Func<TL, TMl> onLeft) =>
        (await @this).BindLeft(left => (Either<TMl, TR>)onLeft(left));

}
