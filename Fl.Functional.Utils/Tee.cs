using System;
using System.Threading.Tasks;

namespace Fl.Functional.Utils;

public static partial class Functional
{
    /// <summary>
    /// Applies <paramref name="tee"/> to <paramref name="this"/> when <paramref name="when"/> is <c>true</c>,
    /// returning the original value either way.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="this">The value to pass through.</param>
    /// <param name="tee">A function that receives the value and returns a transformed copy.</param>
    /// <param name="when">A boolean flag that controls whether <paramref name="tee"/> is invoked.</param>
    /// <returns>The result of <paramref name="tee"/> when <paramref name="when"/> is <c>true</c>; otherwise <paramref name="this"/> unchanged.</returns>
    public static T TeeWhen<T>(this T @this, Func<T, T> tee, bool when) =>
        when ? tee(@this) : @this;

    /// <summary>
    /// Applies <paramref name="tee"/> to <paramref name="this"/> when <paramref name="when"/> returns <c>true</c>
    /// for the current value, returning the original value either way.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="this">The value to pass through.</param>
    /// <param name="tee">A function that receives the value and returns a transformed copy.</param>
    /// <param name="when">A predicate evaluated on <paramref name="this"/> to decide whether to apply <paramref name="tee"/>.</param>
    /// <returns>The result of <paramref name="tee"/> when the predicate holds; otherwise <paramref name="this"/> unchanged.</returns>
    public static T TeeWhen<T>(this T @this, Func<T, T> tee, Func<T, bool> when) =>
        @this.TeeWhen(tee, when(@this));

    /// <summary>
    /// Applies <paramref name="tee"/> to <paramref name="this"/> when the parameterless predicate <paramref name="when"/>
    /// returns <c>true</c>, returning the original value either way.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="this">The value to pass through.</param>
    /// <param name="tee">A function that receives the value and returns a transformed copy.</param>
    /// <param name="when">A parameterless predicate that controls whether <paramref name="tee"/> is invoked.</param>
    /// <returns>The result of <paramref name="tee"/> when the predicate holds; otherwise <paramref name="this"/> unchanged.</returns>
    public static T TeeWhen<T>(this T @this, Func<T, T> tee, Func<bool> when) =>
        @this.TeeWhen(tee, when());

    /// <summary>
    /// Invokes the side-effecting <paramref name="tee"/> action on <paramref name="this"/> when
    /// <paramref name="when"/> returns <c>true</c>, then returns the original value unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="this">The value to pass through.</param>
    /// <param name="tee">A side-effecting action invoked conditionally on the value.</param>
    /// <param name="when">A parameterless predicate that controls whether <paramref name="tee"/> is invoked.</param>
    /// <returns><paramref name="this"/> unchanged, regardless of whether the action ran.</returns>
    public static T TeeWhen<T>(this T @this, Action<T> tee, Func<bool> when) =>
        @this.TeeWhen(t => t.Tee(tee), when());

    /// <summary>
    /// Asynchronously invokes <paramref name="tee"/> on <paramref name="this"/> when <paramref name="when"/>
    /// is <c>true</c>, then returns the original value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="this">The value to pass through.</param>
    /// <param name="tee">An async side-effecting function invoked conditionally.</param>
    /// <param name="when">A boolean flag that controls whether <paramref name="tee"/> is awaited.</param>
    /// <returns>A task that resolves to <paramref name="this"/> unchanged.</returns>
    public static async Task<T> TeeWhenAsync<T>(this T @this, Func<T, Task> tee, bool when)
    {
        if (when)
            await tee(@this);
        return @this;
    }

    /// <summary>
    /// Asynchronously invokes <paramref name="tee"/> on <paramref name="this"/> when <paramref name="when"/>
    /// returns <c>true</c> for the current value, then returns the original value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="this">The value to pass through.</param>
    /// <param name="tee">An async side-effecting function invoked conditionally.</param>
    /// <param name="when">A predicate evaluated on <paramref name="this"/> to decide whether to invoke <paramref name="tee"/>.</param>
    /// <returns>A task that resolves to <paramref name="this"/> unchanged.</returns>
    public static Task<T> TeeWhenAsync<T>(this T @this, Func<T, Task> tee, Func<T, bool> when) =>
        @this.TeeWhenAsync(tee, when(@this));

    /// <summary>
    /// Awaits the task, then asynchronously invokes <paramref name="tee"/> on the result when
    /// <paramref name="when"/> returns <c>true</c>, returning the original value.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the task.</typeparam>
    /// <param name="thistask">A task whose result is evaluated.</param>
    /// <param name="tee">An async side-effecting function invoked conditionally.</param>
    /// <param name="when">A predicate evaluated on the awaited value to decide whether to invoke <paramref name="tee"/>.</param>
    /// <returns>A task that resolves to the awaited value unchanged.</returns>
    public static async Task<T> TeeWhenAsync<T>(this Task<T> thistask, Func<T, Task> tee, Func<T, bool> when)
    {
        var @this = await thistask;
        if (when(@this))
            await tee(@this);

        return @this;
    }

    /// <summary>
    /// Invokes the async transformation <paramref name="tee"/> when <paramref name="when"/> returns <c>true</c>,
    /// returning a task that resolves to the transformed value; otherwise resolves to the original value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="this">The value to pass through.</param>
    /// <param name="tee">An async function that transforms the value.</param>
    /// <param name="when">A predicate evaluated on <paramref name="this"/> to decide whether to call <paramref name="tee"/>.</param>
    /// <returns>A task that resolves to the result of <paramref name="tee"/> when the predicate holds; otherwise <paramref name="this"/> wrapped in a completed task.</returns>
    public static Task<T> TeeWhenAsync<T>(this T @this, Func<T, Task<T>> tee, Func<T, bool> when) =>
        when(@this) ? tee(@this) : Task.FromResult(@this);

    /// <summary>
    /// Applies <paramref name="tee"/> to <paramref name="this"/> unconditionally and returns the result.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="this">The value to transform.</param>
    /// <param name="tee">A function that receives the value and returns a transformed copy.</param>
    /// <returns>The result of applying <paramref name="tee"/> to <paramref name="this"/>.</returns>
    public static T Tee<T>(this T @this, Func<T, T> tee) =>
        tee(@this);

    /// <summary>
    /// Invokes the side-effecting <paramref name="tee"/> action on <paramref name="this"/> and returns
    /// <paramref name="this"/> unchanged, enabling pass-through pipelines.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="this">The value to pass through.</param>
    /// <param name="tee">A side-effecting action invoked on the value.</param>
    /// <returns><paramref name="this"/> unchanged after the action has run.</returns>
    public static T Tee<T>(this T @this, Action<T> tee) =>
        @this.Tee(t =>
        {
            tee(t);
            return t;
        });

    /// <summary>
    /// Invokes the parameterless side-effecting <paramref name="tee"/> action and returns
    /// <paramref name="this"/> unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="this">The value to pass through.</param>
    /// <param name="tee">A parameterless side-effecting action.</param>
    /// <returns><paramref name="this"/> unchanged after the action has run.</returns>
    public static T Tee<T>(this T @this, Action tee) =>
        @this.Tee(t =>
        {
            tee();
            return t;
        });
}
