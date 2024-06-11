using FluentValidation;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Teraa.Extensions.AspNetCore;

// Enable with
// [assembly: Behaviors(typeof(RequestValidationBehavior<,>))]

[PublicAPI]
public sealed class RequestValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : Behavior<TRequest, IActionResult>
{
    public override async ValueTask<IActionResult> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await Next(request, cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var errors = validators
            .Select(x => x.Validate(context))
            .Where(x => !x.IsValid)
            .SelectMany(x => x.Errors)
            .GroupBy(x => x.PropertyName, x => x.ErrorMessage)
            .ToDictionary(x => x.Key, x => x.ToArray());

        if (errors.Count == 0)
            return await Next(request, cancellationToken);

        var problemDetails = new ValidationProblemDetails(errors);
        var result = new BadRequestObjectResult(problemDetails);

        return result;
    }
}
