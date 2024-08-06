using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Teraa.Extensions.AspNetCore.MinimalApis;

[PublicAPI]
public class RequestValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, IResult>
    where TRequest : IRequest<TResponse>, IRequest<IResult>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public RequestValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public Task<IResult> Handle(TRequest request, RequestHandlerDelegate<IResult> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return next();

        var context = new ValidationContext<TRequest>(request);

        var errors = _validators
            .Select(x => x.Validate(context))
            .Where(x => !x.IsValid)
            .SelectMany(x => x.Errors)
            .GroupBy(x => x.PropertyName, x => x.ErrorMessage)
            .ToDictionary(x => x.Key, x => x.ToArray());

        if (!errors.Any())
            return next();

        var problemDetails = new ValidationProblemDetails(errors);

        return Task.FromResult(Microsoft.AspNetCore.Http.Results.BadRequest(problemDetails));
    }
}
