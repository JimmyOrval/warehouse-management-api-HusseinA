using System.ComponentModel.DataAnnotations;

namespace Domain.Exceptions;

public class RequestValidationException : Exception
{
    public IEnumerable<ValidationResult> Errors { get; }

    public RequestValidationException(IEnumerable<ValidationResult> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}