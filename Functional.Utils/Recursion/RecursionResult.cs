using System;
using System.Threading.Tasks;

namespace Functional.Utils.Recursion;

public class RecursionResult<T>
{
    private RecursionResult(bool isFinalResult,
                            T result,
                            Func<Task<RecursionResult<T>>> nextStep)
    {
        IsFinalResult = isFinalResult;
        Result = result;
        NextStepAync = nextStep;
    }
    private RecursionResult(bool isFinalResult,
                            T result,
                            Func<RecursionResult<T>> nextStep)
    {
        IsFinalResult = isFinalResult;
        Result = result;
        NextStep = nextStep;
    }

    public static Task<RecursionResult<T>> CreateNextAsync(T result, Func<Task<RecursionResult<T>>> nextStep) =>
        Task.FromResult(new RecursionResult<T>(false, result, nextStep));

    public static Task<RecursionResult<T>> CreateLastAsync(T result, Func<Task<RecursionResult<T>>> nextStep) =>
        Task.FromResult(new RecursionResult<T>(true, result, nextStep));

    public static RecursionResult<T> CreateNext(T result, Func<RecursionResult<T>> nextStep) =>
        new(false, result, nextStep);

    public static RecursionResult<T> CreateLast(T result, Func<RecursionResult<T>> nextStep) =>
        new(true, result, nextStep);

    public bool IsFinalResult { get; private set; }
    public T Result { get; private set; }
    public Func<Task<RecursionResult<T>>> NextStepAync { get; private set; }
    public Func<RecursionResult<T>> NextStep { get; private set; }
}
