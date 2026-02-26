using LanguageExt;
using System;
using System.Threading.Tasks;

namespace Fl.Functional.Utils;

public static partial class Functional
{
    /// <summary>
    /// Asynchronously pattern-matches an <see cref="Either{TL, TR}"/> by awaiting both branch functions.
    /// </summary>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <typeparam name="TR">The right (success) type.</typeparam>
    /// <typeparam name="TOutput">The type produced by both branch functions.</typeparam>
    /// <param name="this">A task that resolves to an <see cref="Either{TL, TR}"/>.</param>
    /// <param name="onRightAsync">An async function invoked when the either is <c>Right</c>.</param>
    /// <param name="onLeftAsync">An async function invoked when the either is <c>Left</c>.</param>
    /// <returns>A task that resolves to the output of whichever branch was taken.</returns>
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

    /// <summary>
    /// Asynchronously pattern-matches an <see cref="Either{TL, TR}"/> where only the right branch is async.
    /// </summary>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <typeparam name="TR">The right (success) type.</typeparam>
    /// <typeparam name="TOutput">The type produced by both branch functions.</typeparam>
    /// <param name="this">A task that resolves to an <see cref="Either{TL, TR}"/>.</param>
    /// <param name="onRightAsync">An async function invoked when the either is <c>Right</c>.</param>
    /// <param name="onLeft">A synchronous function invoked when the either is <c>Left</c>.</param>
    /// <returns>A task that resolves to the output of whichever branch was taken.</returns>
    public static Task<TOutput> MatchAsync<TL, TR, TOutput>(this Task<Either<TL, TR>> @this,
                                                        Func<TR, Task<TOutput>> onRightAsync,
                                                        Func<TL, TOutput> onLeft) =>
        @this
            .MapAsync(onRightAsync)
            .MatchAsync(
                Prelude.identity,
                onLeft
            );

    /// <summary>
    /// Asynchronously pattern-matches an <see cref="Either{TL, TR}"/> where only the left branch is async.
    /// </summary>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <typeparam name="TR">The right (success) type.</typeparam>
    /// <typeparam name="TOutput">The type produced by both branch functions.</typeparam>
    /// <param name="this">A task that resolves to an <see cref="Either{TL, TR}"/>.</param>
    /// <param name="onRight">A synchronous function invoked when the either is <c>Right</c>.</param>
    /// <param name="onLeftAsync">An async function invoked when the either is <c>Left</c>.</param>
    /// <returns>A task that resolves to the output of whichever branch was taken.</returns>
    public static Task<TOutput> MatchAsync<TL, TR, TOutput>(this Task<Either<TL, TR>> @this,
                                                        Func<TR, TOutput> onRight,
                                                        Func<TL, Task<TOutput>> onLeftAsync) =>
        @this
            .MapLeftAsync(onLeftAsync)
            .MatchAsync(
                onRight,
                Prelude.identity
            );

    /// <summary>
    /// Awaits the <see cref="Either{TL, TR}"/> and synchronously pattern-matches it, invoking the
    /// appropriate branch function and returning the result.
    /// </summary>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <typeparam name="TR">The right (success) type.</typeparam>
    /// <typeparam name="TOutput">The type produced by both branch functions.</typeparam>
    /// <param name="this">A task that resolves to an <see cref="Either{TL, TR}"/>.</param>
    /// <param name="onRight">A synchronous function invoked when the either is <c>Right</c>.</param>
    /// <param name="onLeft">A synchronous function invoked when the either is <c>Left</c>.</param>
    /// <returns>A task that resolves to the output of whichever branch was taken.</returns>
    public static async Task<TOutput> MatchAsync<TL, TR, TOutput>(this Task<Either<TL, TR>> @this,
                                                        Func<TR, TOutput> onRight,
                                                        Func<TL, TOutput> onLeft) =>
        (await @this)
                .Match(
                    onRight,
                    onLeft
                );

    /// <summary>
    /// Awaits the <see cref="Either{TL, TR}"/> and synchronously pattern-matches it using
    /// <c>MatchUnsafe</c>, which allows <c>null</c> return values from branch functions.
    /// </summary>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <typeparam name="TR">The right (success) type.</typeparam>
    /// <typeparam name="TOutput">The type produced by both branch functions. May be <c>null</c>.</typeparam>
    /// <param name="this">A task that resolves to an <see cref="Either{TL, TR}"/>.</param>
    /// <param name="onRight">A function invoked when the either is <c>Right</c>; may return <c>null</c>.</param>
    /// <param name="onLeft">A function invoked when the either is <c>Left</c>; may return <c>null</c>.</param>
    /// <returns>A task that resolves to the output of whichever branch was taken, potentially <c>null</c>.</returns>
    public static async Task<TOutput> MatchUnsafeAsync<TL, TR, TOutput>(this Task<Either<TL, TR>> @this,
                                                            Func<TR, TOutput> onRight,
                                                            Func<TL, TOutput> onLeft) =>
        (await @this)
            .MatchUnsafe(
                onRight,
                onLeft
            );

}
