namespace Domain.Interfaces;

public interface ICorrelationIdProvider
{
    string CorrelationId();
}