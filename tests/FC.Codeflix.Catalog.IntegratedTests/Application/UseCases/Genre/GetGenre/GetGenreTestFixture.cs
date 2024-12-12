using FC.Codeflix.Catalog.IntegratedTests.Application.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.IntegratedTests.Application.UseCases.Genre.GetGenre;

[CollectionDefinition(nameof(GetGenreTestFixture))]
public class GetGenreTestFixtureCollection : ICollectionFixture<GetGenreTestFixture>
{
}

public class GetGenreTestFixture : GenreUseCasesBaseFixture
{
}