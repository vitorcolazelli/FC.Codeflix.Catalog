using FC.Codeflix.Catalog.IntegratedTests.Application.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.IntegratedTests.Application.UseCases.Genre.UpdateGenre;

[CollectionDefinition(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTestFixtureCollection
    : ICollectionFixture<UpdateGenreTestFixture>
{
}

public class UpdateGenreTestFixture
    : GenreUseCasesBaseFixture
{
}