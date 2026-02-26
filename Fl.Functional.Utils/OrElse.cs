using LanguageExt;

namespace Fl.Functional.Utils;

public static partial class Functional
{
    /// <summary>
    /// Extracts the value from <paramref name="this"/> when it is <c>Some</c>,
    /// or returns <paramref name="defaultValue"/> when it is <c>None</c>.
    /// </summary>
    /// <typeparam name="T">The type of the optional value.</typeparam>
    /// <param name="this">The option to extract a value from.</param>
    /// <param name="defaultValue">The fallback value returned when <paramref name="this"/> is <c>None</c>.</param>
    /// <returns>The inner value if <c>Some</c>; otherwise <paramref name="defaultValue"/>.</returns>
    public static T OrElse<T>(this Option<T> @this, T defaultValue) => @this.IfNone(defaultValue);
}
