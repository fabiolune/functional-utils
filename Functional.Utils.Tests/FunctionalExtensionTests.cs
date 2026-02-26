using LanguageExt;
using NSubstitute;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using static Functional.Utils.Functional;

namespace Functional.Utils.Tests;

[TestFixture]
public class FunctionalTests
{
    public interface ILog
    {
        void Log(string message);
    }

    public class CanBeDisposed(FunctionalTests.ILog logger) : IDisposable
    {
        private readonly ILog _logger = logger;

        public static bool GetTrue() => true;

        public static Task<bool> GetTrueAsync() => Task.FromResult(true);

        public static Task SetTrueAsync(ref bool value)
        {
            Task.Delay(1000);
            value = true;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _logger.Log("dispose");
        }
    }

    private class CanBeDisposedAlt(FunctionalTests.CanBeDisposed innerDisposable, FunctionalTests.ILog logger) : IDisposable
    {
        private readonly CanBeDisposed _innerDisposable = innerDisposable;
        private readonly ILog _logger = logger;

        public static string GetTrue() => CanBeDisposed.GetTrue().ToString();

        public static Task<string> GetTrueAsync() => Task.FromResult(CanBeDisposed.GetTrue().ToString());

        public static Task SetTrueAsync(ref string value)
        {
            Task.Delay(1000);
            value = CanBeDisposed.GetTrue().ToString();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _logger.Log("dispose-alt");
        }
    }

    public struct ValueTypeStruct
    {
        public bool Success { get; set; }
    }

    [Test]
    public void Combine_BothActionValid_ReturnsActionsMerged()
    {
        var data = new[] { "hello", "hi" };
        var result = new List<string>();
        Action<List<string>> a1 = _ => result.AddRange(data);
        void A2(List<string> _) => result.Add("goodbye");
        var actionResult = a1.Combine(A2);
        actionResult(result);
        result.ShouldBeEquivalentTo(new List<string> { "hello", "hi", "goodbye" });
    }

    [Test]
    public void Combine_NullableActionIsNull_ReturnsDefaultActionResult()
    {
        var result = new List<string>();
        void A2(List<string> _) => result.Add("goodbye");
        var actionResult = ((Action<List<string>>)null).Combine(A2);
        actionResult(result);
        result.ShouldBeEquivalentTo(new List<string> { "goodbye" });
    }

    [Test]
    public void ForEach_GivenNull_ReturnsNone()
    {
        List<int> result = [];
        ((IEnumerable<int>)null).ForEach(result.Add).IsNone.ShouldBeTrue();
        result.ShouldBeEmpty();
    }

    [Test]
    public void ForEach_GivenValidEnumerable_ShouldExecuteActionForEachItem()
    {
        IEnumerable<int> given = [1, 2, 3, 4];
        var expected = new[] { 2, 3, 4, 5 };
        List<int> result = [];
        given.ForEach(item => result.Add(item + 1)).IsSome.ShouldBeTrue();
        result.SequenceEqual(expected).ShouldBeTrue();
    }

    [Test]
    public void Using_WithActionDisposable_ShouldCallWithObject()
    {
        var log = Substitute.For<ILog>();
        var called = false;

        var result = Using(new CanBeDisposed(log),
                           _ => { called = CanBeDisposed.GetTrue(); });

        result.ShouldBe(LanguageExt.Unit.Default);
        called.ShouldBeTrue();
        log
            .Received(1)
            .Log("dispose");
    }

    [Test]
    public void UsingAsync_WithActionDisposable_ShouldCallWithObject()
    {
        var log = Substitute.For<ILog>();
        var called = false;

        var result = UsingAsync(new CanBeDisposed(log),
                                cbd => CanBeDisposed.SetTrueAsync(ref called)).Result;

        result.ShouldBe(LanguageExt.Unit.Default);
        called.ShouldBeTrue();
        log
            .Received(1)
            .Log("dispose");
    }

    [Test]
    public void Using_WithFunctionDisposable_ShouldCallWithObject()
    {
        var log = Substitute.For<ILog>();

        var result = Using(new CanBeDisposed(log),
                           cbd => CanBeDisposed.GetTrue());

        result.ShouldBeTrue();
        log
            .Received(1)
            .Log("dispose");
    }

    [Test]
    public void UsingAsync_WithFunctionDisposable_ShouldCallWithObject()
    {
        var log = Substitute.For<ILog>();

        var result = UsingAsync(new CanBeDisposed(log),
                                cbd => CanBeDisposed.GetTrueAsync()).Result;

        result.ShouldBeTrue();
        log
            .Received(1)
            .Log("dispose");
    }

    [Test]
    public void Using_WithActionWithTwoDisposable_ShouldCallWithObjects()
    {
        var log = Substitute.For<ILog>();
        var called = false;
        var calledAlt = "False";

        var result = Using(new CanBeDisposed(log),
                           cbd => new CanBeDisposedAlt(cbd, log),
                           (_, _) =>
                           {
                               called = CanBeDisposed.GetTrue();
                               calledAlt = CanBeDisposedAlt.GetTrue();
                           });

        result.ShouldBe(LanguageExt.Unit.Default);
        called.ShouldBeTrue();
        calledAlt.ShouldBe("True");
        log.Received(1).Log("dispose");
        log.Received(1).Log("dispose-alt");
    }

    [Test]
    public void UsingAsync_WithActionWithTwoDisposable_ShouldCallWithObjects()
    {
        var log = Substitute.For<ILog>();
        var called = false;
        var calledAlt = "false";

        var result = UsingAsync(new CanBeDisposed(log),
                                cbd => new CanBeDisposedAlt(cbd, log),
                                async (cbd, cbda) =>
                                {
                                    await CanBeDisposed.SetTrueAsync(ref called);
                                    await CanBeDisposedAlt.SetTrueAsync(ref calledAlt);
                                }).Result;

        result.ShouldBe(LanguageExt.Unit.Default);
        called.ShouldBeTrue();
        calledAlt.ShouldBe("True");
        log.Received(1).Log("dispose");
        log.Received(1).Log("dispose-alt");
    }

    [Test]
    public void Using_WithFunctionWithTwoDisposable_ShouldCallWithObject()
    {
        var log = Substitute.For<ILog>();

        var (result1, result2) = Using(new CanBeDisposed(log),
                                       cbd => new CanBeDisposedAlt(cbd, log),
                                       (cbd, cbda) =>
                                       (
                                           CanBeDisposed.GetTrue(),
                                           CanBeDisposedAlt.GetTrue()
                                       ));

        result1.ShouldBeTrue();
        result2.ShouldBe("True");
        log.Received(1).Log("dispose");
        log.Received(1).Log("dispose-alt");
    }

    [Test]
    public void UsingAsync_WithFunctionWithTwoDisposable_ShouldCallWithObject()
    {
        var log = Substitute.For<ILog>();

        var (result1, result2) = UsingAsync(new CanBeDisposed(log),
                                            cbd => new CanBeDisposedAlt(cbd, log),
                                            async (_, _) =>
                                            (
                                                await CanBeDisposed.GetTrueAsync(),
                                                await CanBeDisposedAlt.GetTrueAsync()
                                            )).Result;

        result1.ShouldBeTrue();
        result2.ShouldBe("True");
        log.Received(1).Log("dispose");
        log.Received(1).Log("dispose-alt");
    }

    [Test]
    public void ToOption_WithNull_ShouldBeNone()
    {
        var option = ((CanBeDisposed)null).MakeOption();

        option.IsNone.ShouldBeTrue();
    }

    [Test]
    public void MakeEither_WithNull_ShouldBeLeft()
    {
        var either = ((CanBeDisposed)null).MakeEither(10);
        either.IsLeft.ShouldBeTrue();
        either.IfLeft(i => i.ShouldBe(10));
    }

    [Test]
    public void MakeEitherLeftFunc_WithNull_ShouldBeLeft()
    {
        var either = ((CanBeDisposed)null).MakeEither(() => 10);
        either.IsLeft.ShouldBeTrue();
        either.IfLeft(i => i.ShouldBe(10));
    }

    [Test]
    public void MakeOption_WithValueAndNoneFunction_ShouldBeNone()
    {
        var canBeDisposed = new CanBeDisposed(Substitute.For<ILog>());

        var option = canBeDisposed.MakeOption(_ => true);

        option.IsNone.ShouldBeTrue();
    }

    [Test]
    public void MakeEither_WithValueAndNoneFunction_ShouldBeLeft()
    {
        var canBeDisposed = new CanBeDisposed(Substitute.For<ILog>());

        var either = canBeDisposed.MakeEither(_ => true, 10);

        either.IsLeft.ShouldBeTrue();
        either.IfLeft(i => i.ShouldBe(10));
    }

    [Test]
    public void MakeEitherLeftFunc_WithValueAndNoneFunction_ShouldBeLeft()
    {
        var canBeDisposed = new CanBeDisposed(Substitute.For<ILog>());

        var either = canBeDisposed.MakeEither(_ => true, () => 10);

        either.IsLeft.ShouldBeTrue();
        either.IfLeft(i => i.ShouldBe(10));
    }

    [Test]
    public void MakeEither_WithLeftMapAndRightValue_ShouldBeRight()
    {
        var input = "some string";

        var either = input.MakeEither(_ => true, _ => _ == "left value", _ => LanguageExt.Unit.Default);

        either.IsRight.ShouldBeTrue();
        either.IfRight(b => b.ShouldBeTrue());
    }

    [Test]
    public void MakeEither_WithLeftMapAndLeftValue_ShouldBeLeft()
    {
        var input = "left value";

        var either = input.MakeEither(_ => true, _ => _ == "left value", _ => 321);

        either.IsLeft.ShouldBeTrue();
        either.IfLeft(b => b.ShouldBe(321));
    }

    [Test]
    public void MakeOption_WithValue_ShouldBeSome()
    {
        var canBeDisposed = new CanBeDisposed(Substitute.For<ILog>());

        var option = canBeDisposed.MakeOption();

        option.IsSome.ShouldBeTrue();
    }

    [Test]
    public void ToEither_WithValue_ShouldBeRight()
    {
        var canBeDisposed = new CanBeDisposed(Substitute.For<ILog>());

        var either = canBeDisposed.MakeEither(10);

        either.IsRight.ShouldBeTrue();
        either.IfRight(i => i.ShouldBe(canBeDisposed));
    }

    [Test]
    public void ToEitherLeftFunc_WithValue_ShouldBeRight()
    {
        var canBeDisposed = new CanBeDisposed(Substitute.For<ILog>());

        var either = canBeDisposed.MakeEither(() => 10);

        either.IsRight.ShouldBeTrue();
        either.IfRight(i => i.ShouldBe(canBeDisposed));
    }

    [Test]
    public void ToOptionMapping_WithNull_ShouldBeNone()
    {
        var option = ((CanBeDisposed)null).MakeOption(_ => 1, _ => true);

        option.IsNone.ShouldBeTrue();
    }

    [Test]
    public void ToEitherMapping_WithNull_ShouldBeLeft()
    {
        var either = ((CanBeDisposed)null).MakeEither(_ => 1, _ => true, "something");

        either.IsLeft.ShouldBeTrue();
        either.IfLeft(i => i.ShouldBe("something"));
    }

    [Test]
    public void ToEitherLeftFuncMapping_WithNull_ShouldBeLeft()
    {
        var either = ((CanBeDisposed)null).MakeEither(_ => 1, _ => true, () => "something");

        either.IsLeft.ShouldBeTrue();
        either.IfLeft(i => i.ShouldBe("something"));
    }

    [Test]
    public void ToOptionMapping_WithBool_ShouldHandleProperly()
    {
        false.MakeOption(Prelude.identity).IsNone.ShouldBeFalse();
        true.MakeOption(Prelude.identity).IsNone.ShouldBeTrue();
    }

    [Test]
    public void ToEitherMapping_WithBool_ShouldHandleProperly()
    {
        false.MakeEither(Prelude.identity, 10).IsRight.ShouldBeTrue();
        true.MakeEither(Prelude.identity, 10).IsLeft.ShouldBeTrue();
    }

    [Test]
    public void ToEitherLeftFuncMapping_WithBool_ShouldHandleProperly()
    {
        false.MakeEither(Prelude.identity, () => 10).IsRight.ShouldBeTrue();
        true.MakeEither(Prelude.identity, () => 10).IsLeft.ShouldBeTrue();
    }

    [Test]
    public void ToOptionMapping_WithValueType_ShouldHandleProperly()
    {
        var value = new ValueTypeStruct { Success = true };

        var option = value.MakeOption(_ => _.Success == false);

        option.IsSome.ShouldBeTrue();
        option.IfSome(v => v.ShouldBe(value));
    }

    [Test]
    public void ToEitherMapping_WithValueType_ShouldHandleProperly()
    {
        var value = new ValueTypeStruct { Success = true };

        var either = value.MakeEither(_ => _.Success == false, 10);

        either.IsRight.ShouldBeTrue();
        either.IfRight(v => v.ShouldBe(value));
    }

    [Test]
    public void ToEitherLeftFuncMapping_WithValueType_ShouldHandleProperly()
    {
        var value = new ValueTypeStruct { Success = true };

        var either = value.MakeEither(_ => _.Success == false, () => 10);

        either.IsRight.ShouldBeTrue();
        either.IfRight(v => v.ShouldBe(value));
    }

    [Test]
    public void ToOptionMapping_WithDefaultValueType_ShouldHandleProperly()
    {
        var value = new ValueTypeStruct();

        var option = value.MakeOption(_ => _.Success);

        option.IsSome.ShouldBeTrue();
        option.IfSome(v => v.ShouldBe(value));
    }

    [Test]
    public void ToEitherMapping_WithDefaultValueType_ShouldHandleProperly()
    {
        var value = new ValueTypeStruct();

        var either = value.MakeEither(_ => _.Success, 10);

        either.IsRight.ShouldBeTrue();
        either.IfRight(v => v.ShouldBe(value));
    }

    [Test]
    public void ToEitherLeftFuncMapping_WithDefaultValueType_ShouldHandleProperly()
    {
        var value = new ValueTypeStruct();

        var either = value.MakeEither(_ => _.Success, () => 10);

        either.IsRight.ShouldBeTrue();
        either.IfRight(v => v.ShouldBe(value));
    }

    [Test]
    public void ToOptionMapping_WithValueAndNoneFunction_ShouldBeNone()
    {
        var canBeDisposed = new CanBeDisposed(Substitute.For<ILog>());

        var option = canBeDisposed.MakeOption(_ => 1, _ => true);

        option.IsNone.ShouldBeTrue();
    }

    [Test]
    public void ToEitherMapping_WithValueAndNoneFunction_ShouldBeLeft()
    {
        var canBeDisposed = new CanBeDisposed(Substitute.For<ILog>());

        var either = canBeDisposed.MakeEither(_ => 1, _ => true, 10);

        either.IsLeft.ShouldBeTrue();
        either.IfLeft(v => v.ShouldBe(10));
    }

    [Test]
    public void ToEitherLeftFuncMapping_WithValueAndNoneFunction_ShouldBeLeft()
    {
        var canBeDisposed = new CanBeDisposed(Substitute.For<ILog>());

        var either = canBeDisposed.MakeEither(_ => 1, _ => true, () => 10);

        either.IsLeft.ShouldBeTrue();
        either.IfLeft(v => v.ShouldBe(10));
    }

    [Test]
    public void ToOptionMapping_WithValue_ShouldBeSome()
    {
        var canBeDisposed = new CanBeDisposed(Substitute.For<ILog>());

        var option = canBeDisposed.MakeOption(_ => 1, _ => false);

        option.IsSome.ShouldBeTrue();
        option.IfSome(v => v.ShouldBe(1));
    }

    [Test]
    public void ToEitherMapping_WithValue_ShouldBeRight()
    {
        var canBeDisposed = new CanBeDisposed(Substitute.For<ILog>());

        var either = canBeDisposed.MakeEither(_ => 1, _ => false, 10);

        either.IsRight.ShouldBeTrue();
        either.IfRight(v => v.ShouldBe(1));
    }

    [Test]
    public void ToEitherLeftFuncMapping_WithValue_ShouldBeRight()
    {
        var canBeDisposed = new CanBeDisposed(Substitute.For<ILog>());

        var either = canBeDisposed.MakeEither(_ => 1, _ => false, () => 10);

        either.IsRight.ShouldBeTrue();
        either.IfRight(v => v.ShouldBe(1));
    }

    [Test]
    public void Tee_Should_Transform()
    {
        var result = "any".Tee(s => "teezed");

        result.ShouldBe("teezed");
    }

    internal class TestClass
    {
        internal string Status;
    }

    [Test]
    public void Tee_WithActionOfT_Should_ReturnSameObject()
    {
        var input = new TestClass
        {
            Status = "initial"
        };

        var result = input.Tee(i => i.Status = "final");

        result.ShouldBe(input);
        result.Status.ShouldBe("final");
    }

    [Test]
    public void Tee_WithAction_Should_ReturnSameObject()
    {
        var executed = false;
        var input = new TestClass
        {
            Status = "whatever"
        };

        var result = input.Tee(() => executed = true);

        result.ShouldBe(input);
        executed.ShouldBeTrue();
    }

    [TestCase(true, "teezed")]
    [TestCase(false, "any")]
    public void TeeWhen_WithTrueOrFalseCondition_ShouldTransformOrNot(bool whenResult, string expected)
    {
        var result = "any".TeeWhen(s => "teezed", () => whenResult);

        result.ShouldBe(expected);
    }

    [TestCase(false, "teezed")]
    [TestCase(true, "any")]
    public void TeeWhen_ActionWithTrueOrFalseCondition_ShouldTransformOrNot(bool whenResult, string expected)
    {
        var value = "teezed";
        var result = "any".TeeWhen(s => { value = s; }, () => whenResult);

        result.ShouldBe("any");
        value.ShouldBe(expected);
    }

    [TestCase(false, true)]
    [TestCase(true, false)]
    public void TeeWhenAsync_WithTrueOrFalseCondition_ActionShouldPerformOrNot(bool whenResult, bool expected)
    {
        var boolean = true;
        async Task teeAsync(string s)
        {
            await Task.Delay(2000);
            boolean = false;
        }

        var result = "any".TeeWhenAsync(teeAsync, s => whenResult);

        result.Result.ShouldBe("any");
        boolean.ShouldBe(expected);

    }

    [TestCase(false, true)]
    [TestCase(true, false)]
    public void TeeWhenAsync_WithTaskTrueOrFalseCondition_ActionShouldPerformOrNot(bool whenResult, bool expected)
    {
        var boolean = true;
        async Task teeAsync(string s)
        {
            await Task.Delay(2000);
            boolean = false;
        }

        var result = "any".AsTask().TeeWhenAsync(teeAsync, s => whenResult);

        result.Result.ShouldBe("any");
        boolean.ShouldBe(expected);

    }

    [TestCase(false, "any")]
    [TestCase(true, "teezed")]
    public void TeeWhenAsync_WithTrueOrFalseCondition_ShouldTransformOrNot(bool whenResult, string expected)
    {
        static Task<string> teeAsync(string s) => Task.FromResult("teezed");

        var result = "any".TeeWhenAsync(teeAsync, s => whenResult);

        result.Result.ShouldBe(expected);

    }

    [TestCase(true, "teezed")]
    [TestCase(false, "any")]
    public void TeeWhen_WithTrueOrFalseConditionOnInput_ShouldTransformOrNot(bool whenResult, string expected)
    {
        var result = "any".TeeWhen(s => "teezed", s => whenResult);

        result.ShouldBe(expected);
    }

    [Test]
    public void OrElse_ShouldExtractOptionValueOrElse()
    {
        var value = Some(10);

        var result = value.OrElse(0);

        result.ShouldBe(10);
    }

    [Test]
    public void OrElse_ShouldExtractOtherValueOrElse()
    {
        Option<int> value = None;

        var result = value.OrElse(0);

        result.ShouldBe(0);
    }

    [Test]
    public void ToOptionAsync_WithNull_ShouldBeNone()
    {
        var canBeDispose = Task.FromResult((CanBeDisposed)null);

        var option = canBeDispose.MakeOptionAsync().Result;

        option.IsNone.ShouldBeTrue();
    }

    [Test]
    public void ToEitherAsync_WithNull_ShouldBeLeft()
    {
        var canBeDispose = Task.FromResult((CanBeDisposed)null);

        var either = canBeDispose.MakeEitherAsync(10).Result;

        either.IsLeft.ShouldBeTrue();
        either.IfLeft(v => v.ShouldBe(10));
    }

    [Test]
    public void ToEitherAsyncLeftFunc_WithNull_ShouldBeLeft()
    {
        var canBeDispose = Task.FromResult((CanBeDisposed)null);

        var either = canBeDispose.MakeEitherAsync(() => 10).Result;

        either.IsLeft.ShouldBeTrue();
        either.IfLeft(v => v.ShouldBe(10));
    }

    [Test]
    public void ToOptionAsync_WithValueAndNoneFunction_ShouldBeNone()
    {
        var canBeDispose = Task.FromResult(new CanBeDisposed(Substitute.For<ILog>()));

        var option = canBeDispose.MakeOptionAsync(_ => true).Result;

        option.IsNone.ShouldBeTrue();
    }

    [Test]
    public void ToEitherAsync_WithValueAndNoneFunction_ShouldBeLeft()
    {
        var canBeDispose = Task.FromResult(new CanBeDisposed(Substitute.For<ILog>()));

        var either = canBeDispose.MakeEitherAsync(_ => true, 10).Result;

        either.IsLeft.ShouldBeTrue();
        either.IfLeft(v => v.ShouldBe(10));
    }

    [Test]
    public void ToEitherAsyncLeftFunc_WithValueAndNoneFunction_ShouldBeLeft()
    {
        var canBeDispose = Task.FromResult(new CanBeDisposed(Substitute.For<ILog>()));

        var either = canBeDispose.MakeEitherAsync(_ => true, () => 10).Result;

        either.IsLeft.ShouldBeTrue();
        either.IfLeft(v => v.ShouldBe(10));
    }

    [Test]
    public void ToOptionAsync_WithValue_ShouldBeSome()
    {
        var innerDispose = new CanBeDisposed(Substitute.For<ILog>());
        var canBeDispose = Task.FromResult(innerDispose);

        var option = canBeDispose.MakeOptionAsync().Result;

        option.IsSome.ShouldBeTrue();
        option.IfSome(v => v.ShouldBe(innerDispose));
    }

    [Test]
    public void ToEitherAsync_WithValue_ShouldBeRight()
    {
        var innerDispose = new CanBeDisposed(Substitute.For<ILog>());
        var canBeDispose = Task.FromResult(innerDispose);

        var either = canBeDispose.MakeEitherAsync(10).Result;

        either.IsRight.ShouldBeTrue();
        either.IfRight(v => v.ShouldBe(innerDispose));
    }

    [Test]
    public void ToEitherAsyncLeftFunc_WithValue_ShouldBeRight()
    {
        var innerDispose = new CanBeDisposed(Substitute.For<ILog>());
        var canBeDispose = Task.FromResult(innerDispose);

        var either = canBeDispose.MakeEitherAsync(() => 10).Result;

        either.IsRight.ShouldBeTrue();
        either.IfRight(v => v.ShouldBe(innerDispose));
    }

    [Test]
    public void ToOptionAsyncMapping_WithNull_ShouldBeNone()
    {
        var canBeDispose = Task.FromResult((CanBeDisposed)null);

        var option = canBeDispose.MakeOptionAsync(_ => 1, _ => true).Result;

        option.IsNone.ShouldBeTrue();
    }

    [Test]
    public void ToEitherAsyncMapping_WithNull_ShouldBeLeft()
    {
        var canBeDispose = Task.FromResult((CanBeDisposed)null);

        var either = canBeDispose.MakeEitherAsync(_ => 1, _ => true, 10).Result;

        either.IsLeft.ShouldBeTrue();
        either.IfLeft(v => v.ShouldBe(10));
    }

    [Test]
    public void ToEitherAsyncLeftFuncMapping_WithNull_ShouldBeLeft()
    {
        var canBeDispose = Task.FromResult((CanBeDisposed)null);

        var either = canBeDispose.MakeEitherAsync(_ => 1, _ => true, () => 10).Result;

        either.IsLeft.ShouldBeTrue();
        either.IfLeft(v => v.ShouldBe(10));
    }

    [Test]
    public void ToOptionAsyncMapping_WithBool_ShouldHandleProperly()
    {
        var optionSome = Task.FromResult(false).MakeOptionAsync(Prelude.identity).Result;
        var optionNone = Task.FromResult(true).MakeOptionAsync(Prelude.identity).Result;

        optionNone.IsNone.ShouldBeTrue();
        optionSome.IsSome.ShouldBeTrue();
    }

    [Test]
    public void ToEitherAsyncMapping_WithBool_ShouldHandleProperly()
    {
        var eitherRight = Task.FromResult(false).MakeEitherAsync(Prelude.identity, 10).Result;
        var eitherLeft = Task.FromResult(true).MakeEitherAsync(Prelude.identity, 10).Result;

        eitherLeft.IsLeft.ShouldBeTrue();
        eitherRight.IsRight.ShouldBeTrue();
    }

    [Test]
    public void ToEitherAsyncLeftFuncMapping_WithBool_ShouldHandleProperly()
    {
        var eitherRight = Task.FromResult(false).MakeEitherAsync(Prelude.identity, () => 10).Result;
        var eitherLeft = Task.FromResult(true).MakeEitherAsync(Prelude.identity, () => 10).Result;

        eitherLeft.IsLeft.ShouldBeTrue();
        eitherRight.IsRight.ShouldBeTrue();
    }

    [Test]
    public void ToOptionAsyncMapping_WithValueType_ShouldHandleProperly()
    {
        var value = Task.FromResult(new ValueTypeStruct { Success = true });

        var option = value.MakeOptionAsync(_ => _.Success == false).Result;

        option.IsSome.ShouldBeTrue();
    }

    [Test]
    public void ToEitherAsyncMapping_WithValueType_ShouldHandleProperly()
    {
        var value = Task.FromResult(new ValueTypeStruct { Success = true });

        var either = value.MakeEitherAsync(_ => _.Success == false, 10).Result;

        either.IsRight.ShouldBeTrue();
    }

    [Test]
    public void ToEitherAsyncLeftFuncMapping_WithValueType_ShouldHandleProperly()
    {
        var value = Task.FromResult(new ValueTypeStruct { Success = true });

        var either = value.MakeEitherAsync(_ => _.Success == false, () => 10).Result;

        either.IsRight.ShouldBeTrue();
    }

    [Test]
    public void ToOptionAsyncMapping_WithDefaultValueType_ShouldHandleProperly()
    {
        var value = Task.FromResult(new ValueTypeStruct());

        var option = value.MakeOptionAsync(_ => _.Success).Result;

        option.IsSome.ShouldBeTrue();
    }

    [Test]
    public void ToEitherAsyncMapping_WithDefaultValueType_ShouldHandleProperly()
    {
        var value = Task.FromResult(new ValueTypeStruct());

        var either = value.MakeEitherAsync(_ => _.Success, 10).Result;

        either.IsRight.ShouldBeTrue();
    }

    [Test]
    public void ToEitherAsyncLeftFuncMapping_WithDefaultValueType_ShouldHandleProperly()
    {
        var value = Task.FromResult(new ValueTypeStruct());

        var either = value.MakeEitherAsync(_ => _.Success, () => 10).Result;

        either.IsRight.ShouldBeTrue();
    }

    [Test]
    public void ToOptionAsyncMapping_WithValueAndNoneFunction_ShouldBeNone()
    {
        var canBeDispose = Task.FromResult(new CanBeDisposed(Substitute.For<ILog>()));

        var option = canBeDispose.MakeOptionAsync(_ => 1, _ => true).Result;

        option.IsNone.ShouldBeTrue();
    }

    [Test]
    public void ToEitherAsyncMapping_WithValueAndNoneFunction_ShouldBeLeft()
    {
        var canBeDispose = Task.FromResult(new CanBeDisposed(Substitute.For<ILog>()));

        var either = canBeDispose.MakeEitherAsync(_ => 1, _ => true, 10).Result;

        either.IsLeft.ShouldBeTrue();
        either.IfLeft(v => v.ShouldBe(10));
    }

    [Test]
    public void ToEitherAsyncLeftFuncMapping_WithValueAndNoneFunction_ShouldBeLeft()
    {
        var canBeDispose = Task.FromResult(new CanBeDisposed(Substitute.For<ILog>()));

        var either = canBeDispose.MakeEitherAsync(_ => 1, _ => true, () => 10).Result;

        either.IsLeft.ShouldBeTrue();
        either.IfLeft(v => v.ShouldBe(10));
    }

    [Test]
    public void ToOptionAsyncMapping_WithValue_ShouldBeSome()
    {
        var canBeDispose = Task.FromResult(new CanBeDisposed(Substitute.For<ILog>()));

        var option = canBeDispose.MakeOptionAsync(_ => 1, _ => false).Result;

        option.IsSome.ShouldBeTrue();
    }

    [Test]
    public void ToEitherAsyncMapping_WithValue_ShouldBeRight()
    {
        var canBeDispose = Task.FromResult(new CanBeDisposed(Substitute.For<ILog>()));

        var either = canBeDispose.MakeEitherAsync(_ => 1, _ => false, 10).Result;

        either.IsRight.ShouldBeTrue();
    }

    [Test]
    public void ToEitherAsyncLeftFuncMapping_WithValue_ShouldBeRight()
    {
        var canBeDispose = Task.FromResult(new CanBeDisposed(Substitute.For<ILog>()));

        var either = canBeDispose.MakeEitherAsync(_ => 1, _ => false, () => 10).Result;

        either.IsRight.ShouldBeTrue();
    }

    [Test]
    public void MapAsync_SourceFunc_ShouldCall()
    {
        var source = Task.FromResult(1);

        var result = source.MapAsync<int, string>(_ => Task.FromResult("value")).Result;

        result.ShouldBe("value");
    }

    [Test]
    public void MapAsync_TaskSourceFunc_ShouldCall()
    {
        var source = 0;

        var result = source.MapAsync(async _ => await Task.FromResult(1)).Result;

        result.ShouldBe(1);
    }

    [Test]
    public void MapAsync_TaskSourceFunc_NoAwait_ShouldCall()
    {
        var source = 0;

        var result = source.MapAsync(_ => Task.FromResult(1)).Result;

        result.ShouldBe(1);
    }

    [Test]
    public void MapAsync_TaskSourceTaskFunc_NoAwait_ShouldCall()
    {
        var canBeRemap = Task.FromResult(0);

        var result = canBeRemap.MapAsync(_ => 1).Result;

        result.ShouldBe(1);
    }

    [Test]
    public void Map_WithValue_ShouldMapToResultValue()
    {
        var source = 10;

        var result = source.Map(@this => @this.ToString());

        result.ShouldBe("10");
    }

    [Test]
    public void SameMap_WithValue_ShouldMapToResultValue()
    {
        var source = (10, 37);

        var result = source.SameMap(tuple => tuple.ToString());

        result.ShouldBe(("10", "37"));
    }

    [Test]
    public void Map_WithOptionValue_ShouldMapToResultValue()
    {
        var some = Option<int>.Some(10);
        var none = Option<int>.None;

        var resultSome = some.Map((Option<int> @this) => @this.ToString());
        var resultNone = none.Map((Option<int> @this) => @this.ToString());

        resultSome.ShouldBe("Some(10)");
        resultNone.ShouldBe("None");
    }

    [Test]
    public void Map_WithEitherValue_ShouldMapToResultValue()
    {
        var right = (Either<string, int>)10;
        var left = (Either<string, int>)"ERROR";

        var resultRight = right.Map((Either<string, int> @this) => @this.ToString());
        var resultLeft = left.Map((Either<string, int> @this) => @this.ToString());

        resultRight.ShouldBe("Right(10)");
        resultLeft.ShouldBe("Left(ERROR)");
    }

    [Test]
    public void EitherMapLeftAsync_WhenRight_ShouldNotCallTheLeftFunc()
    {
        var either = Task.FromResult((Either<int, string>)"Hello world!");

        var result = either.MapLeftAsync(intLeft => Task.FromResult(intLeft * 10)).Result;

        result.IsRight.ShouldBeTrue();
        result.IfRight(v => v.ShouldBe("Hello world!"));
    }

    [Test]
    public void EitherMapLeftAsync_WhenLeft_ShouldtCallTheLeftFunc()
    {
        var either = Task.FromResult((Either<int, string>)10);
        static async Task<int> mapLeft(int s)
        {
            await Task.Delay(1000);
            return s * 10;
        }

        var result = either.MapLeftAsync(mapLeft).Result;

        result.IsLeft.ShouldBeTrue();
        result.IfLeft(v => v.ShouldBe(100));
    }

    [Test]
    public void EitherMatchAsync_WhenRight_ShouldCallTheRightFunc()
    {
        var either = Task.FromResult((Either<int, string>)"Hello");

        var result = either
                        .MatchAsync(
                            strRight => (Either<int, string>)$"{strRight} world!",
                            intLeft => intLeft * 10
                        ).Result;

        result.IsRight.ShouldBeTrue();
        result.IfRight(v => v.ShouldBe("Hello world!"));
    }

    [Test]
    public void EitherMatchAsync_WhenLeft_ShouldCallTheLeftFunc()
    {
        var either = Task.FromResult((Either<int, string>)10);

        var result = either
                        .MatchAsync(
                            strRight => (Either<int, string>)$"{strRight} world!",
                            intLeft => intLeft * 10
                        ).Result;

        result.IsLeft.ShouldBeTrue();
        result.IfLeft(v => v.ShouldBe(100));
    }

    [Test]
    public void EitherMatchAsync_WhenRightWithRightTaskFunc_ShouldCallTheRightFunc()
    {
        var either = Task.FromResult((Either<int, string>)"Hello");

        var result = either
                        .MatchAsync(
                            strRight => Task.FromResult((Either<int, string>)$"{strRight} world!"),
                            intLeft => intLeft * 10
                        ).Result;

        result.IsRight.ShouldBeTrue();
        result.IfRight(v => v.ShouldBe("Hello world!"));
    }

    [Test]
    public void EitherMatchAsync_WhenLeftWithRightTaskFunc_ShouldCallTheRightFunc()
    {
        var either = Task.FromResult((Either<int, string>)10);

        var result = either
                        .MatchAsync(
                            strRight => Task.FromResult((Either<int, string>)$"{strRight} world!"),
                            intLeft => intLeft * 10
                        ).Result;

        result.IsLeft.ShouldBeTrue();
        result.IfLeft(v => v.ShouldBe(100));
    }

    [Test]
    public void EitherMatchAsync_WhenRightWithLeftTaskFunc_ShouldCallTheRightFunc()
    {
        var either = Task.FromResult((Either<int, string>)"Hello");

        var result = either
                        .MatchAsync(
                            strRight => $"{strRight} world!",
                            intLeft => Task.FromResult((Either<int, string>)(intLeft * 10))
                        ).Result;

        result.IsRight.ShouldBeTrue();
        result.IfRight(v => v.ShouldBe("Hello world!"));
    }

    [Test]
    public void EitherMatchAsync_WhenLeftWithLeftTaskFunc_ShouldCallTheLeftFunc()
    {
        var either = Task.FromResult((Either<int, string>)10);

        var result = either
                        .MatchAsync(
                            strRight => $"{strRight} world!",
                            intLeft => Task.FromResult((Either<int, string>)(intLeft * 10))
                        ).Result;

        result.IsLeft.ShouldBeTrue();
        result.IfLeft(v => v.ShouldBe(100));
    }

    [Test]
    public void EitherMatchAsync_WhenRightWithBothTaskFunc_ShouldCallTheRightFunc()
    {
        var either = Task.FromResult((Either<int, string>)"Hello");

        var result = either
                        .MatchAsync(
                            strRight => Task.FromResult((Either<int, string>)$"{strRight} world!"),
                            intLeft => Task.FromResult((Either<int, string>)(intLeft * 10))
                        ).Result;

        result.IsRight.ShouldBeTrue();
        result.IfRight(v => v.ShouldBe("Hello world!"));
    }

    [Test]
    public void EitherMatchAsync_WhenLeftWithBothTaskFunc_ShouldCallTheLeftFunc()
    {
        var either = Task.FromResult((Either<int, string>)10);

        var result = either
                        .MatchAsync(
                            strRight => Task.FromResult((Either<int, string>)$"{strRight} world!"),
                            intLeft => Task.FromResult((Either<int, string>)(intLeft * 10))
                        ).Result;

        result.IsLeft.ShouldBeTrue();
        result.IfLeft(v => v.ShouldBe(100));
    }

    [Test]
    public void BindLeftAsync_WhenRight_ShouldNotCallTheLeftFunc()
    {
        var result = Task.FromResult((Either<int, string>)"Hello")
                    .BindLeftAsync(intLeft => (Either<int, string>)(intLeft * 10))
                    .Result;

        result.IsRight.ShouldBeTrue();
        result.IfRight(v => v.ShouldBe("Hello"));
    }

    [Test]
    public void BindLeftAsync_WhenLeft_ShouldCallTheLeftFunc()
    {
        var either = Task.FromResult((Either<int, string>)10);

        var result = either.BindLeftAsync(intLeft => (Either<int, string>)(intLeft * 10)).Result;

        result.IsLeft.ShouldBeTrue();
        result.IfLeft(v => v.ShouldBe(100));
    }

    [Test]
    public void MapLeftAsync_WhenRight_ShouldNotCallTheLeftFunc()
    {
        var either = Task.FromResult((Either<int, string>)"Hello");

        var result = either.MapLeftAsync(intLeft => intLeft * 10).Result;

        result.IsRight.ShouldBeTrue();
        result.IfRight(v => v.ShouldBe("Hello"));
    }

    [Test]
    public void MapLeftAsync_WhenLeft_ShouldCallTheLeftFunc()
    {
        var either = Task.FromResult((Either<int, string>)10);

        var result = either.MapLeftAsync(intLeft => intLeft * 10).Result;

        result.IsLeft.ShouldBeTrue();
        result.IfLeft(v => v.ShouldBe(100));
    }

    [Test]
    public void Do_ShouldExecuteAction()
    {
        var temp = true;
        void Action(object _) => temp = false;

        var obj = new object();

        obj.Do(Action);

        temp.ShouldBeFalse();
    }

    [Test]
    public void DoAsync_ShouldExecuteAction()
    {
        var temp = true;
        async Task action(object _)
        {
            await Task.Delay(1000);
            temp = false;
        }

        var obj = new object();

        obj.DoAsync(action).Wait();

        temp.ShouldBeFalse();
    }

    [Test]
    public void MatchUnsafeAsync_WhenItemIsRightShouldApplyOnRight()
    {
        var source = Task.FromResult((Either<int, string>)"value");

        var result = source.MatchUnsafeAsync(_ => null as int?, i => i).Result;

        result.ShouldBeNull();
    }

    [Test]
    public void MatchUnsafeAsync_WhenItemIsLeftShouldApplyOnLeft()
    {
        var source = Task.FromResult((Either<int, string>)10);

        var result = source.MatchUnsafeAsync(Prelude.identity, _ => null).Result;

        result.ShouldBeNull();
    }
}
