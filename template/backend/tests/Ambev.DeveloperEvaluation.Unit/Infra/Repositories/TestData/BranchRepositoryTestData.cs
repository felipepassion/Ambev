using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Unit.Infra.Repositories.TestData;

public static class BranchRepositoryTestData
{
    public static Branch CreateBranch(string name = "Test Branch")
    {
        return new Branch
        {
            Name = name
        };
    }
}
