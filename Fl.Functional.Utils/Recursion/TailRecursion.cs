using System;
using System.Threading.Tasks;

namespace Fl.Functional.Utils.Recursion;

public static class TailRecursion
{
    /// <summary>
    /// Executes an asynchronous tail-recursive computation by repeatedly invoking <paramref name="func"/>
    /// until a <see cref="RecursionResult{T}"/> with <see cref="RecursionResult{T}.IsFinalResult"/> set to
    /// <c>true</c> is returned, avoiding stack overflows via a trampoline loop.
    /// </summary>
    /// <typeparam name="T">The type of the final result.</typeparam>
    /// <param name="func">A factory that produces the next async recursion step.</param>
    /// <returns>A task that resolves to the final result of the recursion.</returns>
    public static async Task<T> ExecuteAsync<T>(Func<Task<RecursionResult<T>>> func)
    {
        do
        {
            var recursionResult = await func();
            if (recursionResult.IsFinalResult)
                return recursionResult.Result;
            func = recursionResult.NextStepAsync;
        } while (true);
    }

    /// <summary>
    /// Creates a terminal <see cref="RecursionResult{T}"/> that signals the end of an async recursion,
    /// wrapping <paramref name="result"/> as the final value.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="result">The final value to return from the recursive computation.</param>
    /// <returns>A task that resolves to a <see cref="RecursionResult{T}"/> marked as the final result.</returns>
    public static Task<RecursionResult<T>> ReturnAsync<T>(T result) =>
        RecursionResult<T>.CreateLastAsync(result, null);

    /// <summary>
    /// Creates an intermediate <see cref="RecursionResult{T}"/> that continues the async recursion
    /// by invoking <paramref name="nextStep"/> on the next iteration.
    /// </summary>
    /// <typeparam name="T">The type of the eventual result.</typeparam>
    /// <param name="nextStep">A factory producing the following recursion step.</param>
    /// <returns>A task that resolves to a <see cref="RecursionResult{T}"/> pointing to the next step.</returns>
    public static Task<RecursionResult<T>> NextAsync<T>(Func<Task<RecursionResult<T>>> nextStep) =>
        RecursionResult<T>.CreateNextAsync(default, nextStep);

    /// <summary>
    /// Executes a synchronous tail-recursive computation by repeatedly invoking <paramref name="func"/>
    /// until a <see cref="RecursionResult{T}"/> with <see cref="RecursionResult{T}.IsFinalResult"/> set to
    /// <c>true</c> is returned, avoiding stack overflows via a trampoline loop.
    /// </summary>
    /// <typeparam name="T">The type of the final result.</typeparam>
    /// <param name="func">A factory that produces the next recursion step.</param>
    /// <returns>The final result of the recursive computation.</returns>
    public static T Execute<T>(Func<RecursionResult<T>> func)
    {
        do
        {
            var recursionResult = func();
            if (recursionResult.IsFinalResult)
                return recursionResult.Result;
            func = recursionResult.NextStep;
        } while (true);
    }

    /// <summary>
    /// Creates a terminal <see cref="RecursionResult{T}"/> that signals the end of a synchronous recursion,
    /// wrapping <paramref name="result"/> as the final value.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="result">The final value to return from the recursive computation.</param>
    /// <returns>A <see cref="RecursionResult{T}"/> marked as the final result.</returns>
    public static RecursionResult<T> Return<T>(T result) =>
        RecursionResult<T>.CreateLast(result, null);

    /// <summary>
    /// Creates an intermediate <see cref="RecursionResult{T}"/> that continues the synchronous recursion
    /// by invoking <paramref name="nextStep"/> on the next iteration.
    /// </summary>
    /// <typeparam name="T">The type of the eventual result.</typeparam>
    /// <param name="nextStep">A factory producing the following recursion step.</param>
    /// <returns>A <see cref="RecursionResult{T}"/> pointing to the next step.</returns>
    public static RecursionResult<T> Next<T>(Func<RecursionResult<T>> nextStep) =>
        RecursionResult<T>.CreateNext(default, nextStep);
}
