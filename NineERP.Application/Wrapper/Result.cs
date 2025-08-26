﻿namespace NineERP.Application.Wrapper
{
    public class Result : IResult
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Result()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

#pragma warning disable IDE0028 // Simplify collection initialization
        public List<string> Messages { get; set; } = new();
#pragma warning restore IDE0028 // Simplify collection initialization

        public bool Succeeded { get; set; }

        public string LanguageType { get; set; }

        public static IResult Fail()
        {
            return new Result { Succeeded = false };
        }

        public static IResult Fail(string message)
        {
#pragma warning disable IDE0028 // Simplify collection initialization
            return new Result { Succeeded = false, Messages = new List<string> { message } };
#pragma warning restore IDE0028 // Simplify collection initialization
        }

        public static IResult Fail(List<string> messages)
        {
            return new Result { Succeeded = false, Messages = messages };
        }

        public static Task<IResult> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        public static Task<IResult> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }

        public static Task<IResult> FailAsync(List<string> messages)
        {
            return Task.FromResult(Fail(messages));
        }

        public static IResult Success()
        {
            return new Result { Succeeded = true };
        }

        public static IResult Success(string message)
        {
#pragma warning disable IDE0028 // Simplify collection initialization
            return new Result { Succeeded = true, Messages = new List<string> { message } };
#pragma warning restore IDE0028 // Simplify collection initialization
        }

        public static Task<IResult> SuccessAsync()
        {
            return Task.FromResult(Success());
        }

        public static Task<IResult> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }
    }

    public class Result<T> : Result, IResult<T>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Result()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public T Data { get; set; }

        public new static Result<T> Fail()
        {
            return new Result<T> { Succeeded = false };
        }

        public new static Result<T> Fail(string message)
        {
#pragma warning disable IDE0028 // Simplify collection initialization
            return new Result<T> { Succeeded = false, Messages = new List<string> { message } };
#pragma warning restore IDE0028 // Simplify collection initialization
        }

        public new static Result<T> Fail(List<string> messages)
        {
            return new Result<T> { Succeeded = false, Messages = messages };
        }

        public new static Task<Result<T>> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        public new static Task<Result<T>> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }

        public new static Task<Result<T>> FailAsync(List<string> messages)
        {
            return Task.FromResult(Fail(messages));
        }

        public new static Result<T> Success()
        {
            return new Result<T> { Succeeded = true };
        }

        public new static Result<T> Success(string message)
        {
#pragma warning disable IDE0028
            return new Result<T> { Succeeded = true, Messages = new List<string> { message } };
#pragma warning restore IDE0028
        }

        public static Result<T> Success(T data)
        {
            return new Result<T> { Succeeded = true, Data = data };
        }

        public static Result<T> Success(T data, string message)
        {
#pragma warning disable IDE0028 // Simplify collection initialization
            return new Result<T> { Succeeded = true, Data = data, Messages = new List<string> { message } };
#pragma warning restore IDE0028 // Simplify collection initialization
        }

        public static Result<T> Success(T data, List<string> messages)
        {
            return new Result<T> { Succeeded = true, Data = data, Messages = messages };
        }

        public new static Task<Result<T>> SuccessAsync()
        {
            return Task.FromResult(Success());
        }

        public new static Task<Result<T>> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }

        public static Task<Result<T>> SuccessAsync(T data)
        {
            return Task.FromResult(Success(data));
        }

        public static Task<Result<T>> SuccessAsync(T data, string message)
        {
            return Task.FromResult(Success(data, message));
        }
    }
}