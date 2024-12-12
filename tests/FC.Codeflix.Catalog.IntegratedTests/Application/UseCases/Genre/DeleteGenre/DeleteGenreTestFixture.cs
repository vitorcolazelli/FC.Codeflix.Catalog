using FC.Codeflix.Catalog.IntegratedTests.Application.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.IntegratedTests.Application.UseCases.Genre.DeleteGenre;

[CollectionDefinition(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTestFixtureCollection
    : ICollectionFixture<DeleteGenreTestFixture>
{
}

public class DeleteGenreTestFixture
    : GenreUseCasesBaseFixture
{
}