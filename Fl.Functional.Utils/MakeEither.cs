using LanguageExt;
using System;
using System.Threading.Tasks;

namespace Fl.Functional.Utils;

public static partial class Functional
{
    /// <summary>
    /// Wraps <paramref name="this"/> in <c>Right</c>.
    /// Returns <c>Left(<paramref name="leftValue"/>)</c> only when the value is <c>null</c>.
    /// </summary>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <typeparam name="TR">The right (success) type.</typeparam>
    /// <param name="this">The value to wrap.</param>
    /// <param name="leftValue">The value used for <c>Left</c> when <paramref name="this"/> is <c>null</c>.</param>
    /// <returns><c>Right(<paramref name="this"/>)</c>, or <c>Left(<paramref name="leftValue"/>)</c> when <paramref name="this"/> is <c>null</c>.</returns>
    public static Either<TL, TR> MakeEither<TL, TR>(this TR @this, TL leftValue) =>
        @this
            .MakeOption(_ => false)
            .ToEither(leftValue);

    /// <summary>
    /// Wraps <paramref name="this"/> in <c>Right</c> when <paramref name="leftWhen"/> returns <c>false</c>;
    /// otherwise returns <c>Left(<paramref name="leftValue"/>)</c>.
    /// </summary>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <typeparam name="TR">The right (success) type.</typeparam>
    /// <param name="this">The value to evaluate.</param>
    /// <param name="leftWhen">A predicate that, when satisfied, produces the <c>Left</c> case.</param>
    /// <param name="leftValue">The value used for <c>Left</c> when <paramref name="leftWhen"/> is <c>true</c>.</param>
    /// <returns><c>Right(<paramref name="this"/>)</c>, or <c>Left(<paramref name="leftValue"/>)</c> when the predicate holds.</returns>
    public static Either<TL, TR> MakeEither<TL, TR>(this TR @this, Predicate<TR> leftWhen, TL leftValue) =>
        @this
            .MakeOption(Prelude.identity, leftWhen)
            .ToEither(leftValue);

    /// <summary>
    /// Applies <paramref name="map"/> to <paramref name="this"/> and wraps the result in <c>Right</c>
    /// when <paramref name="leftWhen"/> returns <c>false</c>; otherwise returns <c>Left(<paramref name="leftValue"/>)</c>.
    /// </summary>
    /// <typeparam name="TRInput">The type of the source value.</typeparam>
    /// <typeparam name="TROutput">The type of the mapped right value.</typeparam>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <param name="this">The source value to evaluate and potentially map.</param>
    /// <param name="map">A projection applied to <paramref name="this"/> when the result is <c>Right</c>.</param>
    /// <param name="leftWhen">A predicate evaluated on <paramref name="this"/>; when <c>true</c> the result is <c>Left</c>.</param>
    /// <param name="leftValue">The value used for <c>Left</c> when <paramref name="leftWhen"/> is <c>true</c>.</param>
    /// <returns><c>Right(map(<paramref name="this"/>))</c>, or <c>Left(<paramref name="leftValue"/>)</c> when the predicate holds.</returns>
    public static Either<TL, TROutput> MakeEither<TRInput, TROutput, TL>(
        this TRInput @this,
        Func<TRInput, TROutput> map,
        Predicate<TRInput> leftWhen,
        TL leftValue) =>
        @this
            .MakeOption(map, leftWhen)
            .ToEither(leftValue);

    /// <summary>
    /// Wraps <paramref name="this"/> in <c>Right</c>.
    /// Returns <c>Left</c> produced by invoking <paramref name="leftFunc"/> only when the value is <c>null</c>.
    /// </summary>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <typeparam name="TR">The right (success) type.</typeparam>
    /// <param name="this">The value to wrap.</param>
    /// <param name="leftFunc">A factory invoked to produce the <c>Left</c> value when <paramref name="this"/> is <c>null</c>.</param>
    /// <returns><c>Right(<paramref name="this"/>)</c>, or <c>Left</c> from <paramref name="leftFunc"/> when <paramref name="this"/> is <c>null</c>.</returns>
    public static Either<TL, TR> MakeEither<TL, TR>(this TR @this, Func<TL> leftFunc) =>
        @this
            .MakeOption(_ => false)
            .ToEither(leftFunc);

    /// <summary>
    /// Wraps <paramref name="this"/> in <c>Right</c> when <paramref name="leftWhen"/> returns <c>false</c>;
    /// otherwise returns <c>Left</c> produced by invoking <paramref name="leftFunc"/>.
    /// </summary>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <typeparam name="TR">The right (success) type.</typeparam>
    /// <param name="this">The value to evaluate.</param>
    /// <param name="leftWhen">A predicate that, when satisfied, triggers the <c>Left</c> case.</param>
    /// <param name="leftFunc">A factory invoked lazily to produce the <c>Left</c> value.</param>
    /// <returns><c>Right(<paramref name="this"/>)</c>, or <c>Left</c> from <paramref name="leftFunc"/> when the predicate holds.</returns>
    public static Either<TL, TR> MakeEither<TL, TR>(this TR @this, Predicate<TR> leftWhen, Func<TL> leftFunc) =>
        @this
            .MakeOption(Prelude.identity, leftWhen)
            .ToEither(leftFunc);

    /// <summary>
    /// Applies <paramref name="map"/> to <paramref name="this"/> and wraps the result in <c>Right</c>
    /// when <paramref name="leftWhen"/> returns <c>false</c>; otherwise returns <c>Left</c> from <paramref name="leftFunc"/>.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TR">The type of the mapped right value.</typeparam>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <param name="this">The source value to evaluate and potentially map.</param>
    /// <param name="map">A projection applied to <paramref name="this"/> when the result is <c>Right</c>.</param>
    /// <param name="leftWhen">A predicate evaluated on <paramref name="this"/>; when <c>true</c> the result is <c>Left</c>.</param>
    /// <param name="leftFunc">A factory invoked lazily to produce the <c>Left</c> value.</param>
    /// <returns><c>Right(map(<paramref name="this"/>))</c>, or <c>Left</c> from <paramref name="leftFunc"/> when the predicate holds.</returns>
    public static Either<TL, TR> MakeEither<T, TR, TL>(
        this T @this,
        Func<T, TR> map,
        Predicate<T> leftWhen,
        Func<TL> leftFunc) =>
        @this.MakeOption(map, leftWhen)
            .ToEither(leftFunc);

    /// <summary>
    /// Applies <paramref name="map"/> to <paramref name="this"/> and wraps the result in <c>Right</c>
    /// when <paramref name="leftWhen"/> returns <c>false</c>; otherwise returns <c>Left</c> produced
    /// by applying <paramref name="leftMap"/> to the original value.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TR">The type of the mapped right value.</typeparam>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <param name="this">The source value to evaluate and potentially map.</param>
    /// <param name="map">A projection applied to <paramref name="this"/> when the result is <c>Right</c>.</param>
    /// <param name="leftWhen">A predicate evaluated on <paramref name="this"/>; when <c>true</c> the result is <c>Left</c>.</param>
    /// <param name="leftMap">A function applied to <paramref name="this"/> to produce the <c>Left</c> value.</param>
    /// <returns><c>Right(map(<paramref name="this"/>))</c>, or <c>Left(leftMap(<paramref name="this"/>))</c> when the predicate holds.</returns>
    public static Either<TL, TR> MakeEither<T, TR, TL>(
        this T @this,
        Func<T, TR> map,
        Predicate<T> leftWhen,
        Func<T, TL> leftMap) =>
        @this
            .MakeOption(map, leftWhen)
            .ToEither(leftMap(@this));

    /// <summary>
    /// Asynchronously wraps the awaited value in <c>Right</c>.
    /// Returns <c>Left(<paramref name="leftValue"/>)</c> only when the awaited value is <c>null</c>.
    /// </summary>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <typeparam name="TR">The right (success) type.</typeparam>
    /// <param name="this">A task whose result is wrapped in an <see cref="Either{TL, TR}"/>.</param>
    /// <param name="leftValue">The value used for <c>Left</c> when the awaited value is <c>null</c>.</param>
    /// <returns>A task that resolves to <c>Right(value)</c> or <c>Left(<paramref name="leftValue"/>)</c>.</returns>
    public static Task<Either<TL, TR>> MakeEitherAsync<TL, TR>(this Task<TR> @this, TL leftValue) =>
        @this
            .MakeEitherAsync(_ => false, leftValue);

    /// <summary>
    /// Asynchronously wraps the awaited value in <c>Right</c> when <paramref name="leftWhen"/> returns <c>false</c>;
    /// otherwise returns <c>Left(<paramref name="leftValue"/>)</c>.
    /// </summary>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <typeparam name="TR">The right (success) type.</typeparam>
    /// <param name="this">A task whose result is evaluated.</param>
    /// <param name="leftWhen">A predicate that, when satisfied, produces the <c>Left</c> case.</param>
    /// <param name="leftValue">The value used for <c>Left</c> when the predicate is <c>true</c>.</param>
    /// <returns>A task that resolves to <c>Right(value)</c> or <c>Left(<paramref name="leftValue"/>)</c>.</returns>
    public static Task<Either<TL, TR>> MakeEitherAsync<TL, TR>(this Task<TR> @this, Predicate<TR> leftWhen, TL leftValue) =>
        @this
            .MakeEitherAsync(Prelude.identity, leftWhen, leftValue);

    /// <summary>
    /// Asynchronously applies <paramref name="map"/> to the awaited value and wraps the result in <c>Right</c>
    /// when <paramref name="leftWhen"/> returns <c>false</c>; otherwise returns <c>Left(<paramref name="leftValue"/>)</c>.
    /// </summary>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <typeparam name="TRInput">The type of the value produced by the task.</typeparam>
    /// <typeparam name="TROutput">The type of the mapped right value.</typeparam>
    /// <param name="this">A task whose result is evaluated and potentially mapped.</param>
    /// <param name="map">A projection applied to the awaited value when the result is <c>Right</c>.</param>
    /// <param name="leftWhen">A predicate evaluated on the awaited value; when <c>true</c> the result is <c>Left</c>.</param>
    /// <param name="leftValue">The value used for <c>Left</c> when the predicate is <c>true</c>.</param>
    /// <returns>A task that resolves to <c>Right(map(value))</c> or <c>Left(<paramref name="leftValue"/>)</c>.</returns>
    public static async Task<Either<TL, TROutput>> MakeEitherAsync<TL, TRInput, TROutput>(this Task<TRInput> @this,
                                                              Func<TRInput, TROutput> map,
                                                              Predicate<TRInput> leftWhen,
                                                              TL leftValue)
        => (await @this.MakeOptionAsync(map, leftWhen))
            .ToEither(leftValue);

    /// <summary>
    /// Asynchronously wraps the awaited value in <c>Right</c>.
    /// Returns <c>Left</c> produced by invoking <paramref name="leftFunc"/> only when the awaited value is <c>null</c>.
    /// </summary>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <typeparam name="TR">The right (success) type.</typeparam>
    /// <param name="this">A task whose result is wrapped in an <see cref="Either{TL, TR}"/>.</param>
    /// <param name="leftFunc">A factory invoked to produce the <c>Left</c> value when the awaited value is <c>null</c>.</param>
    /// <returns>A task that resolves to <c>Right(value)</c> or <c>Left</c> from <paramref name="leftFunc"/>.</returns>
    public static Task<Either<TL, TR>> MakeEitherAsync<TL, TR>(this Task<TR> @this, Func<TL> leftFunc)
        => @this.MakeEitherAsync(_ => false, leftFunc);

    /// <summary>
    /// Asynchronously wraps the awaited value in <c>Right</c> when <paramref name="leftWhen"/> returns <c>false</c>;
    /// otherwise returns <c>Left</c> produced by invoking <paramref name="leftFunc"/>.
    /// </summary>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <typeparam name="TR">The right (success) type.</typeparam>
    /// <param name="this">A task whose result is evaluated.</param>
    /// <param name="leftWhen">A predicate that, when satisfied, produces the <c>Left</c> case.</param>
    /// <param name="leftFunc">A factory invoked lazily to produce the <c>Left</c> value.</param>
    /// <returns>A task that resolves to <c>Right(value)</c> or <c>Left</c> from <paramref name="leftFunc"/>.</returns>
    public static Task<Either<TL, TR>> MakeEitherAsync<TL, TR>(this Task<TR> @this, Predicate<TR> leftWhen, Func<TL> leftFunc)
        => @this.MakeEitherAsync(Prelude.identity, leftWhen, leftFunc);

    /// <summary>
    /// Asynchronously applies <paramref name="map"/> to the awaited value and wraps the result in <c>Right</c>
    /// when <paramref name="leftWhen"/> returns <c>false</c>; otherwise returns <c>Left</c> from <paramref name="leftFunc"/>.
    /// </summary>
    /// <typeparam name="TL">The left (error) type.</typeparam>
    /// <typeparam name="TRInput">The type of the value produced by the task.</typeparam>
    /// <typeparam name="TROutput">The type of the mapped right value.</typeparam>
    /// <param name="this">A task whose result is evaluated and potentially mapped.</param>
    /// <param name="map">A projection applied to the awaited value when the result is <c>Right</c>.</param>
    /// <param name="leftWhen">A predicate evaluated on the awaited value; when <c>true</c> the result is <c>Left</c>.</param>
    /// <param name="leftFunc">A factory invoked lazily to produce the <c>Left</c> value.</param>
    /// <returns>A task that resolves to <c>Right(map(value))</c> or <c>Left</c> from <paramref name="leftFunc"/>.</returns>
    public static async Task<Either<TL, TROutput>> MakeEitherAsync<TL, TRInput, TROutput>(this Task<TRInput> @this,
                                                              Func<TRInput, TROutput> map,
                                                              Predicate<TRInput> leftWhen,
                                                              Func<TL> leftFunc) =>
        (await @this.MakeOptionAsync(map, leftWhen))
            .ToEither(leftFunc);
}
