namespace Presentation.Errors;

public record ErrorResponse
(
    string ErrorCode,
    string ErrorMessage,
    string TraceId
);