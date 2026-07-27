using Moq;

namespace Tests.Helpers;

public class RepositoryMockHelper
{
    // this can take both product and supplier
    public static Mock<TRepository> MockRepository<TRepository>() where TRepository : class
    {
        return new Mock<TRepository>(MockBehavior.Loose);
    }
}