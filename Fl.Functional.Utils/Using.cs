using System;
using System.Threading.Tasks;
using LanguageExt;

namespace Fl.Functional.Utils;

public static partial class Functional
{
    /// <summary>
    /// Executes <paramref name="action"/> with <paramref name="disposable"/>, disposing it afterwards,
    /// and returns <see cref="Unit"/>.
    /// </summary>
    /// <typeparam name="TD">The disposable type.</typeparam>
    /// <param name="disposable">The resource to use and then dispose.</param>
    /// <param name="action">The side-effecting action to execute with <paramref name="disposable"/>.</param>
    /// <returns><see cref="Unit.Default"/> after the action completes and the resource is disposed.</returns>
    public static Unit Using<TD>(TD disposable, Action<TD> action)
        where TD : IDisposable
    {
        using (disposable)
            action(disposable);
        return Unit.Default;
    }

    /// <summary>
    /// Executes <paramref name="func"/> with <paramref name="disposable"/>, disposes it, and returns the result.
    /// </summary>
    /// <typeparam name="TD">The disposable type.</typeparam>
    /// <typeparam name="T">The type of the value returned by <paramref name="func"/>.</typeparam>
    /// <param name="disposable">The resource to use and then dispose.</param>
    /// <param name="func">A function that produces a value from <paramref name="disposable"/>.</param>
    /// <returns>The value returned by <paramref name="func"/>.</returns>
    public static T Using<TD, T>(TD disposable, Func<TD, T> func)
        where TD : IDisposable
    {
        using (disposable)
            return func(disposable);
    }

    /// <summary>
    /// Creates a second disposable from the first using <paramref name="createDisposable2"/>, executes
    /// <paramref name="action"/> with both, disposes them in reverse order, and returns <see cref="Unit"/>.
    /// </summary>
    /// <typeparam name="TD1">The type of the first disposable.</typeparam>
    /// <typeparam name="TD2">The type of the second disposable, created from <paramref name="disposable1"/>.</typeparam>
    /// <param name="disposable1">The primary resource.</param>
    /// <param name="createDisposable2">A factory that derives a second disposable from <paramref name="disposable1"/>.</param>
    /// <param name="action">The side-effecting action to execute with both resources.</param>
    /// <returns><see cref="Unit.Default"/> after both resources have been disposed.</returns>
    public static Unit Using<TD1, TD2>(TD1 disposable1, Func<TD1, TD2> createDisposable2, Action<TD1, TD2> action)
        where TD1 : IDisposable
        where TD2 : IDisposable
    {
        using (disposable1)
        using (var disposable2 = createDisposable2(disposable1))
            action(disposable1, disposable2);
        return Unit.Default;
    }

    /// <summary>
    /// Creates a second disposable from the first, executes <paramref name="func"/> with both,
    /// disposes them in reverse order, and returns the result.
    /// </summary>
    /// <typeparam name="TD1">The type of the first disposable.</typeparam>
    /// <typeparam name="TD2">The type of the second disposable, created from <paramref name="disposable1"/>.</typeparam>
    /// <typeparam name="T">The type of the value returned by <paramref name="func"/>.</typeparam>
    /// <param name="disposable1">The primary resource.</param>
    /// <param name="createDisposable2">A factory that derives a second disposable from <paramref name="disposable1"/>.</param>
    /// <param name="func">A function that produces a value from both disposables.</param>
    /// <returns>The value returned by <paramref name="func"/>.</returns>
    public static T Using<TD1, TD2, T>(TD1 disposable1, Func<TD1, TD2> createDisposable2, Func<TD1, TD2, T> func)
        where TD1 : IDisposable
        where TD2 : IDisposable
    {
        using (disposable1)
        using (var disposable2 = createDisposable2(disposable1))
            return func(disposable1, disposable2);
    }

    /// <summary>
    /// Asynchronously executes <paramref name="action"/> with <paramref name="disposable"/>, disposes it,
    /// and returns <see cref="Unit"/>.
    /// </summary>
    /// <typeparam name="TD">The disposable type.</typeparam>
    /// <param name="disposable">The resource to use and then dispose.</param>
    /// <param name="action">An async side-effecting function to execute with <paramref name="disposable"/>.</param>
    /// <returns>A task that resolves to <see cref="Unit.Default"/> after the action and disposal complete.</returns>
    public static async Task<Unit> UsingAsync<TD>(TD disposable, Func<TD, Task> action)
        where TD : IDisposable
    {
        using (disposable)
            await action(disposable);
        return Unit.Default;
    }

    /// <summary>
    /// Asynchronously executes <paramref name="func"/> with <paramref name="disposable"/>, disposes it,
    /// and returns the result.
    /// </summary>
    /// <typeparam name="TD">The disposable type.</typeparam>
    /// <typeparam name="T">The type of the value produced by <paramref name="func"/>.</typeparam>
    /// <param name="disposable">The resource to use and then dispose.</param>
    /// <param name="func">An async function that produces a value from <paramref name="disposable"/>.</param>
    /// <returns>A task that resolves to the value returned by <paramref name="func"/>.</returns>
    public static async Task<T> UsingAsync<TD, T>(TD disposable, Func<TD, Task<T>> func)
        where TD : IDisposable
    {
        using (disposable)
            return await func(disposable);
    }

    /// <summary>
    /// Asynchronously creates a second disposable from the first, executes <paramref name="action"/> with both,
    /// disposes them in reverse order, and returns <see cref="Unit"/>.
    /// </summary>
    /// <typeparam name="TD1">The type of the first disposable.</typeparam>
    /// <typeparam name="TD2">The type of the second disposable, derived from <paramref name="disposable1"/>.</typeparam>
    /// <param name="disposable1">The primary resource.</param>
    /// <param name="createDisposable2">A factory that derives a second disposable from <paramref name="disposable1"/>.</param>
    /// <param name="action">An async side-effecting function executed with both resources.</param>
    /// <returns>A task that resolves to <see cref="Unit.Default"/> after both resources are disposed.</returns>
    public static async Task<Unit> UsingAsync<TD1, TD2>(TD1 disposable1, Func<TD1, TD2> createDisposable2, Func<TD1, TD2, Task> action)
        where TD1 : IDisposable
        where TD2 : IDisposable
    {
        using (disposable1)
        using (var disposable2 = createDisposable2(disposable1))
            await action(disposable1, disposable2);
        return Unit.Default;
    }

    /// <summary>
    /// Asynchronously creates a second disposable from the first, executes <paramref name="func"/> with both,
    /// disposes them in reverse order, and returns the result.
    /// </summary>
    /// <typeparam name="TD1">The type of the first disposable.</typeparam>
    /// <typeparam name="TD2">The type of the second disposable, derived from <paramref name="disposable1"/>.</typeparam>
    /// <typeparam name="T">The type of the value produced by <paramref name="func"/>.</typeparam>
    /// <param name="disposable1">The primary resource.</param>
    /// <param name="createDisposable2">A factory that derives a second disposable from <paramref name="disposable1"/>.</param>
    /// <param name="func">An async function that produces a value from both disposables.</param>
    /// <returns>A task that resolves to the value returned by <paramref name="func"/>.</returns>
    public static async Task<T> UsingAsync<TD1, TD2, T>(TD1 disposable1, Func<TD1, TD2> createDisposable2, Func<TD1, TD2, Task<T>> func)
        where TD1 : IDisposable
        where TD2 : IDisposable
    {
        using (disposable1)
        using (var disposable2 = createDisposable2(disposable1))
            return await func(disposable1, disposable2);
    }
}
