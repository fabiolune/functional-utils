using System;
using System.Threading.Tasks;

namespace Functional.Utils;

public static partial class Functional
{
    public static void Do<T>(this T @this, Action<T> action) =>
        action(@this);

    public static Task DoAsync<T>(this T @this, Func<T, Task> action) =>
        action(@this);
}
