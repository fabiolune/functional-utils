using System;
using System.Threading.Tasks;
using LanguageExt;

namespace Fl.Functional.Utils;

public static partial class Functional
{
    public static Unit Using<TD>(TD disposable, Action<TD> action)
        where TD : IDisposable
    {
        using (disposable)
            action(disposable);
        return Unit.Default;
    }

    public static T Using<TD, T>(TD disposable, Func<TD, T> func)
        where TD : IDisposable
    {
        using (disposable)
            return func(disposable);
    }

    public static Unit Using<TD1, TD2>(TD1 disposable1, Func<TD1, TD2> createDisposable2, Action<TD1, TD2> action)
        where TD1 : IDisposable
        where TD2 : IDisposable
    {
        using (disposable1)
        using (var disposable2 = createDisposable2(disposable1))
            action(disposable1, disposable2);
        return Unit.Default;
    }

    public static T Using<TD1, TD2, T>(TD1 disposable1, Func<TD1, TD2> createDisposable2, Func<TD1, TD2, T> func)
        where TD1 : IDisposable
        where TD2 : IDisposable
    {
        using (disposable1)
        using (var disposable2 = createDisposable2(disposable1))
            return func(disposable1, disposable2);
    }

    public static async Task<Unit> UsingAsync<TD>(TD disposable, Func<TD, Task> action)
        where TD : IDisposable
    {
        using (disposable)
            await action(disposable);
        return Unit.Default;
    }

    public static async Task<T> UsingAsync<TD, T>(TD disposable, Func<TD, Task<T>> func)
        where TD : IDisposable
    {
        using (disposable)
            return await func(disposable);
    }

    public static async Task<Unit> UsingAsync<TD1, TD2>(TD1 disposable1, Func<TD1, TD2> createDisposable2, Func<TD1, TD2, Task> action)
        where TD1 : IDisposable
        where TD2 : IDisposable
    {
        using (disposable1)
        using (var disposable2 = createDisposable2(disposable1))
            await action(disposable1, disposable2);
        return Unit.Default;
    }

    public static async Task<T> UsingAsync<TD1, TD2, T>(TD1 disposable1, Func<TD1, TD2> createDisposable2, Func<TD1, TD2, Task<T>> func)
        where TD1 : IDisposable
        where TD2 : IDisposable
    {
        using (disposable1)
        using (var disposable2 = createDisposable2(disposable1))
            return await func(disposable1, disposable2);
    }
}
