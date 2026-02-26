using NUnit.Framework;
using Shouldly;
using Fl.Functional.Utils.Recursion;
using System.Threading.Tasks;

namespace Fl.Functional.Utils.Tests.Recursion;

[TestFixture]
public class TailRecursionTests
{
    [Test]
    public void ExecuteAsync_ShouldCallFunc()
    {
        var called = false;
        Task<RecursionResult<LanguageExt.Unit>> callback()
        {
            called = true;
            return TailRecursion.ReturnAsync(LanguageExt.Unit.Default);
        }

        var _ = TailRecursion.ExecuteAsync(callback).Result;

        called
            .ShouldBeTrue();
    }
    [Test]
    public void ExecuteAsync_ShouldCallNext()
    {
        var result = TailRecursion.ExecuteAsync(() => TestFuncAsync(0)).Result;

        result.ShouldBe(1);
    }

    [Test]
    public void Execute_ShouldCallFunc()
    {
        var called = false;
        RecursionResult<LanguageExt.Unit> callback()
        {
            called = true;
            return TailRecursion.Return(LanguageExt.Unit.Default);
        }

        var _ = TailRecursion.Execute(callback);

        called
            .ShouldBeTrue();
    }
    [Test]
    public void Execute_ShouldCallNext()
    {
        var result = TailRecursion.Execute(() => TestFunc(0));

        result.ShouldBe(1);
    }

    private static Task<RecursionResult<int>> TestFuncAsync(int current) =>
        current == 0
            ? TailRecursion.NextAsync(() => TestFuncAsync(current + 1))
            : TailRecursion.ReturnAsync(current);

    private static RecursionResult<int> TestFunc(int current) =>
        current == 0
            ? TailRecursion.Next(() => TestFunc(current + 1))
            : TailRecursion.Return(current);
}
