using LanguageExt;
using System;
using System.Threading.Tasks;

namespace Functional.Utils;

public static partial class Functional
{
    public static Task<TOutput> MatchAsync<TL, TR, TOutput>(this Task<Either<TL, TR>> @this,
                                                    Func<TR, Task<TOutput>> onRightAsync,
                                                    Func<TL, Task<TOutput>> onLeftAsync) =>
    @this
        .MapAsync(onRightAsync)
        .MapLeftAsync(onLeftAsync)
        .MatchAsync(
            Prelude.identity,
            Prelude.identity
        );

    public static Task<TOutput> MatchAsync<TL, TR, TOutput>(this Task<Either<TL, TR>> @this,
                                                        Func<TR, Task<TOutput>> onRightAsync,
                                                        Func<TL, TOutput> onLeft) =>
        @this
            .MapAsync(onRightAsync)
            .MatchAsync(
                Prelude.identity,
                onLeft
            );

    public static Task<TOutput> MatchAsync<TL, TR, TOutput>(this Task<Either<TL, TR>> @this,
                                                        Func<TR, TOutput> onRight,
                                                        Func<TL, Task<TOutput>> onLeftAsync) =>
        @this
            .MapLeftAsync(onLeftAsync)
            .MatchAsync(
                onRight,
                Prelude.identity
            );

    public static async Task<TOutput> MatchAsync<TL, TR, TOutput>(this Task<Either<TL, TR>> @this,
                                                        Func<TR, TOutput> onRight,
                                                        Func<TL, TOutput> onLeft) =>
        (await @this)
                .Match(
                    onRight,
                    onLeft
                );

    public static async Task<TOutput> MatchUnsafeAsync<TL, TR, TOutput>(this Task<Either<TL, TR>> @this,
                                                            Func<TR, TOutput> onRight,
                                                            Func<TL, TOutput> onLeft) =>
        (await @this)
            .MatchUnsafe(
                onRight,
                onLeft
            );

}
