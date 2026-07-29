using FluentValidation;
using MediatR;

namespace Application.Validation;

public class FluentValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);
        
        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(
            validators.Select(v =>
                v.ValidateAsync(context, cancellationToken)));

        var fails = results
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (fails.Count != 0)
            throw new ValidationException(fails);
        
        return await next(cancellationToken);
    }
}