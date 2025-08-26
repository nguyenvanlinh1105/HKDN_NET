using FluentValidation;
using MediatR;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            // Trả về kiểu phù hợp với TResponse nếu nó là IResult
            if (typeof(TResponse).GetInterfaces().Any(i => i == typeof(IResult)))
            {
                var errorMessages = failures.Select(f => f.ErrorMessage).ToList();

                var resultType = typeof(Result<>).MakeGenericType(typeof(string));
                var failMethod = resultType.GetMethod(nameof(Result<string>.FailAsync), new[] { typeof(List<string>) });

                if (failMethod != null)
                {
                    var task = (Task<TResponse>)failMethod.Invoke(null, new object[] { errorMessages })!;
                    return await task;
                }
            }

            throw new ValidationException(failures);
        }

        return await next();
    }
}
