using FC.Codeflix.Catalog.IntegratedTests.Application.UseCases.CastMember.Common;

namespace FC.Codeflix.Catalog.IntegratedTests.Application.UseCases.CastMember.CreateCastMember;

[CollectionDefinition(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTestFixtureCollection
    : ICollectionFixture<CreateCastMemberTestFixture>
{
}

public class CreateCastMemberTestFixture
    : CastMemberUseCasesBaseFixture
{
}