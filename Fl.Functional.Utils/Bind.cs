using LanguageExt;
using System;
using System.Threading.Tasks;

namespace Fl.Functional.Utils;

public static partial class Functional
{
    /// <summary>
    /// Awaits <paramref name="this"/> and applies <paramref name="onLeft"/> to the <c>Left</c> value,
    /// allowing the error type to be transformed while keeping any <c>Right</c> value unchanged.
    /// </summary>
    /// <typeparam name="TL">The original left (error) type.</typeparam>
    /// <typeparam name="TR">The right (success) type, unchanged by this operation.</typeparam>
    /// <typeparam name="TMl">The new left type produced by <paramref name="onLeft"/>.</typeparam>
    /// <param name="this">A task that resolves to an <see cref="Either{TL, TR}"/>.</param>
    /// <param name="onLeft">A function that maps the left value to a new <see cref="Either{TMl, TR}"/>.</param>
    /// <returns>
    /// A task that resolves to the original <c>Right</c> value unchanged, or to the
    /// <see cref="Either{TMl, TR}"/> produced by <paramref name="onLeft"/> when the source is <c>Left</c>.
    /// </returns>
    public static async Task<Either<TMl, TR>> BindLeftAsync<TL, TR, TMl>(
        this Task<Either<TL, TR>> @this,
        Func<TL, Either<TMl, TR>> onLeft) =>
        (await @this).BindLeft(onLeft);
}