using LanguageExt;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace Fl.Functional.Utils;

public static partial class Functional
{
    /// <summary>
    /// Wraps <paramref name="this"/> in <c>Some</c>, returning <c>None</c> only when the value is <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="this">The value to wrap.</param>
    /// <returns><c>Some(<paramref name="this"/>)</c>, or <c>None</c> when <paramref name="this"/> is <c>null</c>.</returns>
    public static Option<T> MakeOption<T>(this T @this) =>
        @this.MakeOption(_ => false);

    /// <summary>
    /// Wraps <paramref name="this"/> in <c>Some</c> unless the value is <c>null</c> or
    /// <paramref name="noneWhen"/> returns <c>true</c>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="this">The value to wrap.</param>
    /// <param name="noneWhen">A predicate that, when satisfied, produces <c>None</c> instead of <c>Some</c>.</param>
    /// <returns><c>Some(<paramref name="this"/>)</c>, or <c>None</c> when the value is <c>null</c> or <paramref name="noneWhen"/> returns <c>true</c>.</returns>
    public static Option<T> MakeOption<T>(this T @this, Predicate<T> noneWhen) =>
        @this.MakeOption(identity, noneWhen);

    /// <summary>
    /// Applies <paramref name="map"/> to <paramref name="this"/> and wraps the result in <c>Some</c>,
    /// unless the original value is <c>null</c> (for reference types) or <paramref name="noneWhen"/> returns <c>true</c>.
    /// </summary>
    /// <typeparam name="TInput">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the mapped value stored inside the option.</typeparam>
    /// <param name="this">The source value to evaluate and potentially map.</param>
    /// <param name="map">A projection applied to <paramref name="this"/> when the option is <c>Some</c>.</param>
    /// <param name="noneWhen">A predicate evaluated on <paramref name="this"/>; when <c>true</c> the result is <c>None</c>.</param>
    /// <returns><c>Some(map(<paramref name="this"/>))</c>, or <c>None</c> when the value is <c>null</c> or the predicate holds.</returns>
    public static Option<TResult> MakeOption<TInput, TResult>(
        this TInput @this,
        Func<TInput, TResult> map,
        Predicate<TInput> noneWhen) =>
        !typeof(TInput).IsValueType && EqualityComparer<TInput>.Default.Equals(@this, default) || noneWhen(@this) ? None : Some(map(@this));

    /// <summary>
    /// Asynchronously wraps the awaited value in <c>Some</c>, returning <c>None</c> only when the value is <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the task.</typeparam>
    /// <param name="this">A task whose result is wrapped in an option.</param>
    /// <returns>A task that resolves to <c>Some(value)</c>, or <c>None</c> when the awaited value is <c>null</c>.</returns>
    public static Task<Option<T>> MakeOptionAsync<T>(this Task<T> @this) =>
        @this.MakeOptionAsync(_ => false);

    /// <summary>
    /// Asynchronously wraps the awaited value in <c>Some</c> unless the value is <c>null</c> or
    /// <paramref name="noneWhen"/> returns <c>true</c>.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the task.</typeparam>
    /// <param name="this">A task whose result is wrapped in an option.</param>
    /// <param name="noneWhen">A predicate that, when satisfied, produces <c>None</c>.</param>
    /// <returns>A task that resolves to <c>Some(value)</c> or <c>None</c> depending on nullability and the predicate.</returns>
    public static Task<Option<T>> MakeOptionAsync<T>(this Task<T> @this, Predicate<T> noneWhen) =>
        @this.MakeOptionAsync(identity, noneWhen);

    /// <summary>
    /// Asynchronously applies <paramref name="map"/> to the awaited value and wraps the result in <c>Some</c>,
    /// unless the original value is <c>null</c> or <paramref name="noneWhen"/> returns <c>true</c>.
    /// </summary>
    /// <typeparam name="TInput">The type of the value produced by the task.</typeparam>
    /// <typeparam name="TResult">The type of the mapped value stored inside the option.</typeparam>
    /// <param name="this">A task whose result is evaluated and potentially mapped.</param>
    /// <param name="map">A projection applied to the awaited value when the option is <c>Some</c>.</param>
    /// <param name="noneWhen">A predicate evaluated on the awaited value; when <c>true</c> the result is <c>None</c>.</param>
    /// <returns>A task that resolves to <c>Some(map(value))</c>, or <c>None</c> when the value is <c>null</c> or the predicate holds.</returns>
    public static async Task<Option<TResult>> MakeOptionAsync<TInput, TResult>(this Task<TInput> @this,
                                                              Func<TInput, TResult> map,
                                                              Predicate<TInput> noneWhen) =>
        (await @this).MakeOption(map, noneWhen);
}
