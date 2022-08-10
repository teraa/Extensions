using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Teraa.Extensions.AspNetCore;

[PublicAPI]
public static class Results
{
    public static BadRequestObjectResult BadRequestDetails(string title)
        => new(new ProblemDetails
        {
            Title = title,
        });
}
