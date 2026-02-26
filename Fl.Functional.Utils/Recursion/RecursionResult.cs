using System;
using System.Threading.Tasks;

namespace Fl.Functional.Utils.Recursion;

public class RecursionResult<T>
{
    private RecursionResult(bool isFinalResult,
                            T result,
                            Func<Task<RecursionResult<T>>> nextStep)
    {
        IsFinalResult = isFinalResult;
        Result = result;
        NextStepAsync = nextStep;
    }
    private RecursionResult(bool isFinalResult,
                            T result,
                            Func<RecursionResult<T>> nextStep)
    {
        IsFinalResult = isFinalResult;
        Result = result;
        NextStep = nextStep;
    }

    /// <summary>
    /// Creates an intermediate async recursion result that signals the trampoline to continue
    /// by invoking <paramref name="nextStep"/> on the next iteration.
    /// </summary>
    /// <typeparam name="T">The type of the eventual result.</typeparam>
    /// <param name="result">An intermediate value (typically <c>default</c>); not used as the final result.</param>
    /// <param name="nextStep">A factory that produces the following async recursion step.</param>
    /// <returns>A task that resolves to a <see cref="RecursionResult{T}"/> with <see cref="IsFinalResult"/> set to <c>false</c>.</returns>
    public static Task<RecursionResult<T>> CreateNextAsync(T result, Func<Task<RecursionResult<T>>> nextStep) =>
        Task.FromResult(new RecursionResult<T>(false, result, nextStep));

    /// <summary>
    /// Creates a terminal async recursion result that signals the trampoline to stop and return
    /// <paramref name="result"/> as the final value.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="result">The final value of the recursive computation.</param>
    /// <param name="nextStep">Ignored by the trampoline; may be <c>null</c>.</param>
    /// <returns>A task that resolves to a <see cref="RecursionResult{T}"/> with <see cref="IsFinalResult"/> set to <c>true</c>.</returns>
    public static Task<RecursionResult<T>> CreateLastAsync(T result, Func<Task<RecursionResult<T>>> nextStep) =>
        Task.FromResult(new RecursionResult<T>(true, result, nextStep));

    /// <summary>
    /// Creates an intermediate synchronous recursion result that signals the trampoline to continue
    /// by invoking <paramref name="nextStep"/> on the next iteration.
    /// </summary>
    /// <typeparam name="T">The type of the eventual result.</typeparam>
    /// <param name="result">An intermediate value (typically <c>default</c>); not used as the final result.</param>
    /// <param name="nextStep">A factory that produces the following synchronous recursion step.</param>
    /// <returns>A <see cref="RecursionResult{T}"/> with <see cref="IsFinalResult"/> set to <c>false</c>.</returns>
    public static RecursionResult<T> CreateNext(T result, Func<RecursionResult<T>> nextStep) =>
        new(false, result, nextStep);

    /// <summary>
    /// Creates a terminal synchronous recursion result that signals the trampoline to stop and return
    /// <paramref name="result"/> as the final value.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="result">The final value of the recursive computation.</param>
    /// <param name="nextStep">Ignored by the trampoline; may be <c>null</c>.</param>
    /// <returns>A <see cref="RecursionResult{T}"/> with <see cref="IsFinalResult"/> set to <c>true</c>.</returns>
    public static RecursionResult<T> CreateLast(T result, Func<RecursionResult<T>> nextStep) =>
        new(true, result, nextStep);

    /// <summary>
    /// Gets a value indicating whether this is the terminal result of the recursive computation.
    /// When <c>true</c>, the trampoline stops and returns <see cref="Result"/>.
    /// </summary>
    public bool IsFinalResult { get; private set; }

    /// <summary>
    /// Gets the value carried by this recursion step.
    /// Only meaningful as the final output when <see cref="IsFinalResult"/> is <c>true</c>.
    /// </summary>
    public T Result { get; private set; }

    /// <summary>
    /// Gets the factory for the next async recursion step.
    /// Used by the trampoline when <see cref="IsFinalResult"/> is <c>false</c>.
    /// </summary>
    public Func<Task<RecursionResult<T>>> NextStepAsync { get; private set; }

    /// <summary>
    /// Gets the factory for the next synchronous recursion step.
    /// Used by the trampoline when <see cref="IsFinalResult"/> is <c>false</c>.
    /// </summary>
    public Func<RecursionResult<T>> NextStep { get; private set; }
}
