using System;
using System.Threading.Tasks;

namespace Functional.Utils.Recursion;

public static class TailRecursion
{
    public static async Task<T> ExecuteAsync<T>(Func<Task<RecursionResult<T>>> func)
    {
        do
        {
            var recursionResult = await func();
            if (recursionResult.IsFinalResult)
                return recursionResult.Result;
            func = recursionResult.NextStepAync;
        } while (true);
    }

    public static Task<RecursionResult<T>> ReturnAsync<T>(T result) =>
        RecursionResult<T>.CreateLastAsync(result, null);

    public static Task<RecursionResult<T>> NextAsync<T>(Func<Task<RecursionResult<T>>> nextStep) =>
        RecursionResult<T>.CreateNextAsync(default, nextStep);

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

    public static RecursionResult<T> Return<T>(T result) =>
        RecursionResult<T>.CreateLast(result, null);

    public static RecursionResult<T> Next<T>(Func<RecursionResult<T>> nextStep) =>
        RecursionResult<T>.CreateNext(default, nextStep);
}
