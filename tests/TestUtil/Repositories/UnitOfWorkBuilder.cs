using FfkApi.Domain.Repositories;
using Moq;

namespace TestUtil.Repositories;

public class UnitOfWorkBuilder
{
    public static IUnitOfWork Build()
    {
        var mock = new Mock<IUnitOfWork>();

        return mock.Object;
    }
}
