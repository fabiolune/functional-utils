using System;
using System.Threading.Tasks;

namespace Functional.Utils;

public static partial class Functional
{
    public static T TeeWhen<T>(this T @this, Func<T, T> tee, bool when) =>
        when ? tee(@this) : @this;

    public static T TeeWhen<T>(this T @this, Func<T, T> tee, Func<T, bool> when) =>
        @this.TeeWhen(tee, when(@this));

    public static T TeeWhen<T>(this T @this, Func<T, T> tee, Func<bool> when) =>
        @this.TeeWhen(tee, when());

    public static T TeeWhen<T>(this T @this, Action<T> tee, Func<bool> when) =>
        @this.TeeWhen(t => t.Tee(tee), when());

    public static async Task<T> TeeWhenAsync<T>(this T @this, Func<T, Task> tee, bool when)
    {
        if (when)
            await tee(@this);
        return @this;
    }

    public static Task<T> TeeWhenAsync<T>(this T @this, Func<T, Task> tee, Func<T, bool> when) =>
        @this.TeeWhenAsync(tee, when(@this));

    public static async Task<T> TeeWhenAsync<T>(this Task<T> thistask, Func<T, Task> tee, Func<T, bool> when)
    {
        var @this = await thistask;
        if (when(@this))
            await tee(@this);

        return @this;
    }

    public static Task<T> TeeWhenAsync<T>(this T @this, Func<T, Task<T>> tee, Func<T, bool> when) =>
        when(@this) ? tee(@this) : Task.FromResult(@this);

    public static T Tee<T>(this T @this, Func<T, T> tee) =>
        tee(@this);

    public static T Tee<T>(this T @this, Action<T> tee) =>
        @this.Tee(t =>
        {
            tee(t);
            return t;
        });

    public static T Tee<T>(this T @this, Action tee) =>
        @this.Tee(t =>
        {
            tee();
            return t;
        });
}
