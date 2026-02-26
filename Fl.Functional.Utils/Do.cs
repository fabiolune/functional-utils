using System;
using System.Threading.Tasks;

namespace Fl.Functional.Utils;

public static partial class Functional
{
    /// <summary>
    /// Invokes <paramref name="action"/> with <paramref name="this"/> purely for its side effect.
    /// Unlike <c>Tee</c>, this method returns <c>void</c> and does not pass the value further.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="this">The value to pass to <paramref name="action"/>.</param>
    /// <param name="action">The side-effecting action to execute.</param>
    public static void Do<T>(this T @this, Action<T> action) =>
        action(@this);

    /// <summary>
    /// Asynchronously invokes <paramref name="action"/> with <paramref name="this"/> purely for its side effect.
    /// Returns the resulting <see cref="Task"/> so the caller can <c>await</c> completion.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="this">The value to pass to <paramref name="action"/>.</param>
    /// <param name="action">The asynchronous side-effecting function to execute.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static Task DoAsync<T>(this T @this, Func<T, Task> action) =>
        action(@this);
}
