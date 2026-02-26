using LanguageExt;
using System;
using System.Threading.Tasks;

namespace Functional.Utils;

public static partial class Functional
{
    public static Either<TL, TR> MakeEither<TL, TR>(this TR @this, TL leftValue) =>
        @this
            .MakeOption(_ => false)
            .ToEither(leftValue);

    public static Either<TL, TR> MakeEither<TL, TR>(this TR @this, Predicate<TR> leftWhen, TL leftValue) =>
        @this
            .MakeOption(Prelude.identity, leftWhen)
            .ToEither(leftValue);

    public static Either<TL, TROutput> MakeEither<TRInput, TROutput, TL>(
        this TRInput @this,
        Func<TRInput, TROutput> map,
        Predicate<TRInput> leftWhen,
        TL leftValue) =>
        @this
            .MakeOption(map, leftWhen)
            .ToEither(leftValue);

    public static Either<TL, TR> MakeEither<TL, TR>(this TR @this, Func<TL> leftFunc) =>
        @this
            .MakeOption(_ => false)
            .ToEither(leftFunc);

    public static Either<TL, TR> MakeEither<TL, TR>(this TR @this, Predicate<TR> leftWhen, Func<TL> leftFunc) =>
        @this
            .MakeOption(Prelude.identity, leftWhen)
            .ToEither(leftFunc);

    public static Either<TL, TR> MakeEither<T, TR, TL>(
        this T @this,
        Func<T, TR> map,
        Predicate<T> leftWhen,
        Func<TL> leftFunc) =>
        @this.MakeOption(map, leftWhen)
            .ToEither(leftFunc);

    public static Either<TL, TR> MakeEither<T, TR, TL>(
        this T @this,
        Func<T, TR> map,
        Predicate<T> leftWhen,
        Func<T, TL> leftMap) =>
        @this
            .MakeOption(map, leftWhen)
            .ToEither(leftMap(@this));

    public static Task<Either<TL, TR>> MakeEitherAsync<TL, TR>(this Task<TR> @this, TL leftValue) =>
        @this
            .MakeEitherAsync(_ => false, leftValue);

    public static Task<Either<TL, TR>> MakeEitherAsync<TL, TR>(this Task<TR> @this, Predicate<TR> leftWhen, TL leftValue) =>
        @this
            .MakeEitherAsync(Prelude.identity, leftWhen, leftValue);

    public static async Task<Either<TL, TROutput>> MakeEitherAsync<TL, TRInput, TROutput>(this Task<TRInput> @this,
                                                              Func<TRInput, TROutput> map,
                                                              Predicate<TRInput> leftWhen,
                                                              TL leftValue)
        => (await @this.MakeOptionAsync(map, leftWhen))
            .ToEither(leftValue);

    public static Task<Either<TL, TR>> MakeEitherAsync<TL, TR>(this Task<TR> @this, Func<TL> leftFunc)
        => @this.MakeEitherAsync(_ => false, leftFunc);

    public static Task<Either<TL, TR>> MakeEitherAsync<TL, TR>(this Task<TR> @this, Predicate<TR> leftWhen, Func<TL> leftFunc)
        => @this.MakeEitherAsync(Prelude.identity, leftWhen, leftFunc);

    public static async Task<Either<TL, TROutput>> MakeEitherAsync<TL, TRInput, TROutput>(this Task<TRInput> @this,
                                                              Func<TRInput, TROutput> map,
                                                              Predicate<TRInput> leftWhen,
                                                              Func<TL> leftFunc) =>
        (await @this.MakeOptionAsync(map, leftWhen))
            .ToEither(leftFunc);
}
