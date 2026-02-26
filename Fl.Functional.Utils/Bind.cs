using LanguageExt;
using System;
using System.Threading.Tasks;

namespace Fl.Functional.Utils;

public static partial class Functional
{
    public static async Task<Either<TMl, TR>> BindLeftAsync<TL, TR, TMl>(
        this Task<Either<TL, TR>> @this,
        Func<TL, Either<TMl, TR>> onLeft) =>
        (await @this).BindLeft(onLeft);
}