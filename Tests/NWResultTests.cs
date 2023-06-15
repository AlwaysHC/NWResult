using System;
using NWResult;
using Xunit;

namespace Tests;

public class NWResultTests {
    private static Result<string, int> FuncOK()
        => "OK";

    private static Result<string, int> FuncErr()
        => -1;

    private Result<string, int> FuncEx()
        => throw new Exception("Exception");

    [Fact]
    public void TestOnlyIfValueFunc() {
        Assert.Equal("OKOK", FuncOK().OnlyIfValue(v => v + v, "Default"));
        Assert.Equal("Default", FuncErr().OnlyIfValue(v => v + v, "Default"));
        Assert.Throws<Exception>(() => FuncEx().OnlyIfValue(v => v + v, "Default"));
    }

    [Fact]
    public void TestIfValueElseErrorFunc() {
        Assert.Equal("OKOK", FuncOK().IfValue(v => v + v).ElseError(e => e.ToString()));
        Assert.Equal("-1", FuncErr().IfValue(v => v + v).ElseError(e => e.ToString()));
        Assert.Throws<Exception>(() => FuncEx().IfValue(v => v + v).ElseError(e => e.ToString()));
    }

    [Fact]
    public void TestOnlyIfValueAction() {
        string Test = "Default";

        FuncOK().OnlyIfValue(v => Test = v + v, "Default");
        Assert.Equal("OKOK", Test);

        Test = "Default";
        FuncErr().OnlyIfValue(v => Test = v + v, "Default");
        Assert.Equal("Default", Test);

        Test = "";
        Assert.Throws<Exception>(() => FuncEx().OnlyIfValue(v => Test = v + v, "Default"));
    }

    [Fact]
    public void TestIfValueElseErrorAction() {
        string Test = "Default";

        FuncOK().IfValue(v => Test = v + v).ElseError(e => Test = e.ToString());
        Assert.Equal("OKOK", Test);

        Test = "Default";
        FuncErr().IfValue(v => Test = v + v).ElseError(e => Test = e.ToString());
        Assert.Equal("-1", Test);

        Test = "";
        Assert.Throws<Exception>(() => FuncEx().IfValue(v => Test = v + v).ElseError(e => Test = e.ToString()));
    }

    [Fact]
    public void TestOnlyException() {
        Assert.Null(Result.OnlyException(FuncOK));
        Assert.Null(Result.OnlyException(FuncErr));
        Assert.NotNull(Result.OnlyException(FuncEx));

        string Test = "";

        Result.OnlyException(FuncOK, ex => Test = ex.Message);
        Assert.Equal("", Test);

        Test = "";
        Result.OnlyException(FuncErr, ex => Test = ex.Message);
        Assert.Equal("", Test);

        Test = "";
        Result.OnlyException(FuncEx, ex => Test = ex.Message);
        Assert.Equal("Exception", Test);
    }

    [Fact]
    public void TestWithExceptionOnlyIfValueAction() {
        string Test = "";

        Result.WithException(FuncOK, ex => Test = ex.Message).OnlyIfValue(v => Test = v + v);
        Assert.Equal("OKOK", Test);

        Test = "";
        Result.WithException(FuncErr, ex => Test = ex.Message).OnlyIfValue(v => Test = v + v);
        Assert.Equal("", Test);

        Test = "";
        Result.WithException(FuncEx, ex => Test = ex.Message).OnlyIfValue(v => Test = v + v);
        Assert.Equal("Exception", Test);
    }

    [Fact]
    public void TestWithExceptionOnlyIfValueFunc() {
        Assert.Equal("OKOK", Result.WithException(FuncOK, ex => ex.Message).OnlyIfValue(v => v + v, "Default"));
        Assert.Equal("Default", Result.WithException(FuncErr, ex => ex.Message).OnlyIfValue(v => v + v, "Default"));
        Assert.Equal("Exception", Result.WithException(FuncEx, ex => ex.Message).OnlyIfValue(v => v + v, "Default"));
    }

    [Fact]
    public void TestWithExceptionIfValueElseErrorFunc() {
        Assert.Equal("OKOK", Result.WithException(FuncOK, ex => ex.Message).IfValue(v => v + v).ElseError(e => e.ToString()));
        Assert.Equal("-1", Result.WithException(FuncErr, ex => ex.Message).IfValue(v => v + v).ElseError(e => e.ToString()));
        Assert.Equal("Exception", Result.WithException(FuncEx, ex => ex.Message).IfValue(v => v + v).ElseError(e => e.ToString()));
    }

    [Fact]
    public void TestWithExceptionIfValueElseErrorAction() {
        string Test = "";

        Result.WithException(FuncOK, ex => Test = ex.Message).IfValue(v => Test = v + v).ElseError(e => Test = e.ToString());
        Assert.Equal("OKOK", Test);

        Test = "";
        Result.WithException(FuncErr, ex => Test = ex.Message).IfValue(v => Test = v + v).ElseError(e => Test = e.ToString());
        Assert.Equal("-1", Test);

        Test = "";
        Result.WithException(FuncEx, ex => Test = ex.Message).IfValue(v => Test = v + v).ElseError(e => Test = e.ToString());
        Assert.Equal("Exception", Test);
    }

    [Fact]
    public void TestWithExceptionOnlyExceptionAction() {
        string Test = "";

        Result.WithException(FuncOK).OnlyException(ex => Test = ex.Message);
        Assert.Equal("", Test);

        Test = "";
        Result.WithException(FuncErr).OnlyException(ex => Test = ex.Message);
        Assert.Equal("", Test);

        Test = "";
        Result.WithException(FuncEx).OnlyException(ex => Test = ex.Message);
        Assert.Equal("Exception", Test);
    }

    [Fact]
    public void TestWithExceptionOnlyExceptionFunc() {
        Assert.Equal("Default", Result.WithException(FuncOK).OnlyException(ex => ex.Message, "Default"));
        Assert.Equal("Default", Result.WithException(FuncErr).OnlyException(ex => ex.Message, "Default"));
        Assert.Equal("Exception", Result.WithException(FuncEx).OnlyException(ex => ex.Message, "Default"));
    }

    [Fact]
    public void TestWithExceptionExceptionOnlyIfValueAction() {
        string Test = "";

        Result.WithException(FuncOK).Exception(ex => Test = ex.Message).OnlyIfValue(v => Test = v + v);
        Assert.Equal("OKOK", Test);

        Test = "";
        Result.WithException(FuncErr).Exception(ex => Test = ex.Message).OnlyIfValue(v => Test = v + v);
        Assert.Equal("", Test);

        Test = "";
        Result.WithException(FuncEx).Exception(ex => Test = ex.Message).OnlyIfValue(v => Test = v + v);
        Assert.Equal("Exception", Test);
    }

    [Fact]
    public void TestWithExceptionExceptionOnlyIfValueFunc() {
        Assert.Equal("OKOK", Result.WithException(FuncOK).Exception(ex => ex.Message).OnlyIfValue(v => v + v, "Default"));
        Assert.Equal("Default", Result.WithException(FuncErr).Exception(ex => ex.Message).OnlyIfValue(v => v + v, "Default"));
        Assert.Equal("Exception", Result.WithException(FuncEx).Exception(ex => ex.Message).OnlyIfValue(v => v + v, "Default"));
    }

    [Fact]
    public void TestWithExceptionExceptionIfValueElseErrorFunc() {
        Assert.Equal("OKOK", Result.WithException(FuncOK).Exception(ex => ex.Message).IfValue(v => v + v).ElseError(e => e.ToString()));
        Assert.Equal("-1", Result.WithException(FuncErr).Exception(ex => ex.Message).IfValue(v => v + v).ElseError(e => e.ToString()));
        Assert.Equal("Exception", Result.WithException(FuncEx).Exception(ex => ex.Message).IfValue(v => v + v).ElseError(e => e.ToString()));
    }

    [Fact]
    public void TestWithExceptionExceptionIfValueElseErrorAction() {
        string Test = "";

        Result.WithException(FuncOK).Exception(ex => Test = ex.Message).IfValue(v => Test = v + v).ElseError(e => Test = e.ToString());
        Assert.Equal("OKOK", Test);

        Test = "";
        Result.WithException(FuncErr).Exception(ex => Test = ex.Message).IfValue(v => Test = v + v).ElseError(e => Test = e.ToString());
        Assert.Equal("-1", Test);

        Test = "";
        Result.WithException(FuncEx).Exception(ex => Test = ex.Message).IfValue(v => Test = v + v).ElseError(e => Test = e.ToString());
        Assert.Equal("Exception", Test);
    }
}