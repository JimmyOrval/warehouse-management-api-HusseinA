using System.ComponentModel.DataAnnotations;
using Domain.Exceptions;
using MediatR;

namespace Application.Validation;

public class DataAnnotationValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (typeof(TRequest).GetProperties().Length == 0)
        {
            return await next(cancellationToken);
        }
        
        var context = new ValidationContext(request, serviceProvider: null, items: null);
        var validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            request, 
            context, 
            validationResults, 
            validateAllProperties: true
        );

        if (!isValid)
        {
            throw new RequestValidationException(validationResults);
        }

        return await next(cancellationToken);
    }
}