using System;

namespace NWResult;

public static class Result {
    public static Result<TValue, TError> WithException<TValue, TError>(Func<Result<TValue, TError>> func, Action<Exception> exception) {
        try {
            return func();
        }
        catch (Exception Ex) {
            exception(Ex);
            return new Result<TValue, TError>();
        }
    }

    public static Result<TValue, TError> WithException<TResult, TValue, TError>(Func<Result<TValue, TError>> func, Func<Exception, TResult> exception) {
        try {
            return func();
        }
        catch (Exception Ex) {
            return new Result<TValue, TError>(exception(Ex));
        }
    }

    /// <summary>
    /// Use this overload when you DON'T want to chain IfValue
    /// </summary>
    public static Exception? OnlyException<TValue, TError>(Func<Result<TValue, TError>> func, Action<Exception>? exception = null) {
        try {
            func();
            return null;
        }
        catch (Exception Ex) {
            exception?.Invoke(Ex);
            return Ex;
        }
    }

    public static Result<TValue, TError>.ResultException WithException<TValue, TError>(Func<Result<TValue, TError>> func) {
        try {
            return new Result<TValue, TError>.ResultException(func(), null);
        }
        catch (Exception Ex) {
            return new Result<TValue, TError>.ResultException(new Result<TValue, TError>(), Ex);
        }
    }
}

public sealed record Result<TValue, TError> {
    private readonly TValue _Value;
    private readonly TError _Error;
    private object? _Result;

    public bool IsValue {
        get;
    }

    public bool IsError {
        get;
    }

    public Exception? Exception {
        get; private set;
    }

    public Result(TValue value) {
        _Value = value;
        _Error = default!;
        IsValue = true;
        IsError = false;
    }

    public Result(TError error) {
        _Value = default!;
        _Error = error;
        IsValue = false;
        IsError = true;
    }

    internal Result() {
        _Value = default!;
        _Error = default!;
    }

    internal Result(object? result) {
        _Value = default!;
        _Error = default!;
        _Result = result;
        IsValue = false;
        IsError = false;
    }

    public static implicit operator Result<TValue, TError>(TValue value)
        => new(value);

    public static implicit operator Result<TValue, TError>(TError error)
        => new(error);

    public TResult Match<TResult>(Func<TValue, TResult> value, Func<TError, TResult> error)
        => IsValue
           ? value(_Value)
           : IsError
             ? error(_Error)
             : throw Exception ?? new NullReferenceException("No Value, no Error, no Exception");

    public TResult Value<TResult>(Func<TValue, TResult> value, TResult @default = default!)
        => IsValue
           ? value(_Value)
           : Exception != null
             ? throw Exception
             : @default;

    public TResult Error<TResult>(Func<TError, TResult> error, TResult @default = default!)
        => IsError
           ? error(_Error)
           : Exception != null
             ? throw Exception
             : @default;

    /// <summary>
    /// Use this overload when you DON'T want to chain ElseError
    /// </summary>
    public TResult OnlyIfValue<TResult>(Func<TValue, TResult> value, TResult @default = default!)
        => IsValue
           ? value(_Value)
           : IsError
             ? @default
             : Exception != null
               ? throw Exception
               : (TResult)_Result!;

    /// <summary>
    /// Use this overload when you WANT to chain ElseError and you want the lambda RETURN a value
    /// </summary>
    public ResultElseFunc<TResult> IfValue<TResult>(Func<TValue, TResult> value) {
        if (Exception != null) {
            throw Exception;
        }

        if (IsValue) {
            _Result = value(_Value);
        }

        return new ResultElseFunc<TResult>(this);
    }

    /// <summary>
    /// Use this overload when you WANT to chain ElseError and you want to JUST EXECUTE code inside the lambda
    /// </summary>
    public ResultElseAction IfValue(Action<TValue> value) {
        if (Exception != null) {
            throw Exception;
        }

        if (IsValue) {
            value(_Value);
        }

        return new ResultElseAction(this);
    }

    public readonly struct ResultException {
        private readonly Result<TValue, TError> _Result;
        private readonly Exception? _Exception;

        internal ResultException(Result<TValue, TError> result, Exception? exception) {
            _Result = result;
            _Exception = exception;
        }

        /// <summary>
        /// Use this overload when you WANT to chain ElseError and you want to JUST EXECUTE code inside the lambda
        /// </summary>
        public ResultIfAction Exception(Action<Exception> exception) {
            if (_Exception != null) {
                exception(_Exception);
            }
            return new ResultIfAction(_Result);
        }

        /// <summary>
        /// Use this overload when you DON'T want to chain IfValue
        /// </summary>
        public TResult OnlyException<TResult>(Func<Exception, TResult> exception, TResult @default = default!) {
            if (_Exception != null) {
                return exception(_Exception);
            }

            return @default;
        }

        /// <summary>
        /// Use this overload when you WANT to chain IfValue and you want the lambda RETURN a value
        /// </summary>
        public ResultIfFunc<TResult> Exception<TResult>(Func<Exception, TResult> exception) {
            if (_Exception != null) {
                return new ResultIfFunc<TResult>(new Result<TValue, TError>(exception(_Exception)));
            }
            return new ResultIfFunc<TResult>(_Result);
        }
    }

    public class ResultIfAction {
        private readonly Result<TValue, TError> _Result;

        internal ResultIfAction(Result<TValue, TError> result) {
            _Result = result;
        }

        public ResultElseAction IfValue(Action<TValue> value) {
            if (_Result.IsValue) {
                value(_Result._Value);
            }
            return new ResultElseAction(_Result);
        }
    }

    public class ResultIfFunc<TResult> {
        private readonly Result<TValue, TError> _Result;

        internal ResultIfFunc(Result<TValue, TError> result) {
            _Result = result;
        }

        /// <summary>
        /// Use this overload when you WANT to chain ElseError and you want the lambda RETURN a value
        /// </summary>
        public ResultElseFunc<TResult> IfValue(Func<TValue, TResult> value) {
            if (_Result.IsValue) {
                return new ResultElseFunc<TResult>(new Result<TValue, TError>(value(_Result._Value)));
            }
            return new ResultElseFunc<TResult>(_Result);
        }

        /// <summary>
        /// Use this overload when you DON'T want to chain ElseError
        /// </summary>
        public TResult OnlyIfValue(Func<TValue, TResult> value, TResult @default = default!) {
            if (_Result.IsValue) {
                return value(_Result._Value);
            }
            return (TResult?)_Result._Result ?? @default;
        }
    }

    public readonly struct ResultElseFunc<TResult> {
        private readonly Result<TValue, TError> _Result;

        internal ResultElseFunc(Result<TValue, TError> result) {
            _Result = result;
        }

        public TResult ElseError(Func<TError, TResult> error)
            => _Result.Error(error) ?? (TResult)_Result._Result!;
    }

    public readonly struct ResultElseAction {
        private readonly Result<TValue, TError> _Result;

        internal ResultElseAction(Result<TValue, TError> result) {
            _Result = result;
        }

        public void ElseError(Action<TError> error) {
            if (_Result.IsError) {
                error(_Result._Error);
            }
            else if (_Result.Exception != null) {
                throw _Result.Exception;
            }
        }
    }
}
