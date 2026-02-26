using LanguageExt;
using System;
using System.Threading.Tasks;

namespace Fl.Functional.Utils;

public static partial class Functional
{
    /// <summary>
    /// Applies <paramref name="fn"/> to <paramref name="this"/>, enabling fluent transformation pipelines.
    /// </summary>
    /// <typeparam name="TSource">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result produced by <paramref name="fn"/>.</typeparam>
    /// <param name="this">The value to transform.</param>
    /// <param name="fn">The transformation function to apply.</param>
    /// <returns>The result of applying <paramref name="fn"/> to <paramref name="this"/>.</returns>
    public static TResult Map<TSource, TResult>(this TSource @this, Func<TSource, TResult> fn) =>
        fn(@this);

    /// <summary>
    /// Applies the same function <paramref name="fn"/> to both elements of a same-typed tuple.
    /// </summary>
    /// <typeparam name="TSource">The element type of the source tuple.</typeparam>
    /// <typeparam name="TResult">The element type of the result tuple.</typeparam>
    /// <param name="this">A tuple whose two elements are both of type <typeparamref name="TSource"/>.</param>
    /// <param name="fn">The function applied independently to each element.</param>
    /// <returns>A new tuple containing <c>(fn(Item1), fn(Item2))</c>.</returns>
    public static (TResult, TResult) SameMap<TSource, TResult>(this (TSource, TSource) @this,
        Func<TSource, TResult> fn) =>
        (fn(@this.Item1), fn(@this.Item2));

    /// <summary>
    /// Awaits <paramref name="this"/>, then passes the result to the asynchronous function <paramref name="fn"/>
    /// and awaits the produced task.
    /// </summary>
    /// <typeparam name="TSource">The type of the value produced by <paramref name="this"/>.</typeparam>
    /// <typeparam name="TResult">The type of the value produced by <paramref name="fn"/>.</typeparam>
    /// <param name="this">A task whose result is passed to <paramref name="fn"/>.</param>
    /// <param name="fn">An asynchronous transformation function.</param>
    /// <returns>A task that resolves to the result of <paramref name="fn"/>.</returns>
    public static async Task<TResult> MapAsync<TSource, TResult>(this Task<TSource> @this,
                                                                 Func<TSource, Task<TResult>> fn) =>
        await fn(await @this);

    /// <summary>
    /// Passes <paramref name="this"/> directly to the asynchronous function <paramref name="fn"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the value produced by <paramref name="fn"/>.</typeparam>
    /// <param name="this">The value to pass to <paramref name="fn"/>.</param>
    /// <param name="fn">An asynchronous transformation function.</param>
    /// <returns>A task that resolves to the result of <paramref name="fn"/>.</returns>
    public static Task<TResult> MapAsync<TSource, TResult>(this TSource @this,
                                                           Func<TSource, Task<TResult>> fn) =>
        fn(@this);

    /// <summary>
    /// Awaits <paramref name="this"/> and applies the synchronous function <paramref name="fn"/> to the result.
    /// </summary>
    /// <typeparam name="TSource">The type of the value produced by <paramref name="this"/>.</typeparam>
    /// <typeparam name="TResult">The type of the value produced by <paramref name="fn"/>.</typeparam>
    /// <param name="this">A task whose result is passed to <paramref name="fn"/>.</param>
    /// <param name="fn">A synchronous transformation function applied to the awaited value.</param>
    /// <returns>A task that resolves to the result of <paramref name="fn"/>.</returns>
    public static async Task<TResult> MapAsync<TSource, TResult>(this Task<TSource> @this,
                                                                 Func<TSource, TResult> fn) =>
        fn(await @this);

    /// <summary>
    /// Asynchronously maps the <c>Left</c> side of an <see cref="Either{TL, TR}"/> using an async function,
    /// leaving any <c>Right</c> value unchanged.
    /// </summary>
    /// <typeparam name="TL">The original left type.</typeparam>
    /// <typeparam name="TR">The right type, unchanged by this operation.</typeparam>
    /// <typeparam name="TMl">The new left type produced by <paramref name="onLeftAsync"/>.</typeparam>
    /// <param name="this">A task that resolves to an <see cref="Either{TL, TR}"/>.</param>
    /// <param name="onLeftAsync">An async function that transforms the left value.</param>
    /// <returns>A task that resolves to an <see cref="Either{TMl, TR}"/> with the left type remapped.</returns>
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

    /// <summary>
    /// Asynchronously maps the <c>Left</c> side of an <see cref="Either{TL, TR}"/> using a synchronous function,
    /// leaving any <c>Right</c> value unchanged.
    /// </summary>
    /// <typeparam name="TL">The original left type.</typeparam>
    /// <typeparam name="TR">The right type, unchanged by this operation.</typeparam>
    /// <typeparam name="TMl">The new left type produced by <paramref name="onLeft"/>.</typeparam>
    /// <param name="this">A task that resolves to an <see cref="Either{TL, TR}"/>.</param>
    /// <param name="onLeft">A synchronous function that transforms the left value.</param>
    /// <returns>A task that resolves to an <see cref="Either{TMl, TR}"/> with the left type remapped.</returns>
    public static async Task<Either<TMl, TR>> MapLeftAsync<TL, TR, TMl>(this Task<Either<TL, TR>> @this, Func<TL, TMl> onLeft) =>
        (await @this).BindLeft(left => (Either<TMl, TR>)onLeft(left));

}
