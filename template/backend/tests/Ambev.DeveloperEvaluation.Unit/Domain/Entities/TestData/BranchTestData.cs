using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class BranchTestData
{
    private static readonly Faker<Branch> BranchFaker = new Faker<Branch>()
        .RuleFor(b => b.Name, f => f.Company.CompanyName())
        .RuleFor(b => b.IsActive, true)
        .RuleFor(b => b.CreatedAt, f => f.Date.Past());

    public static Branch GenerateValidBranch() => BranchFaker.Generate();
}
