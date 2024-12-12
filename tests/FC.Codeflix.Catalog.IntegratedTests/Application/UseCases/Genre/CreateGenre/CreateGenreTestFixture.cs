using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.IntegratedTests.Application.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.IntegratedTests.Application.UseCases.Genre.CreateGenre;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection
    : ICollectionFixture<CreateGenreTestFixture>
{
}

public class CreateGenreTestFixture
    : GenreUseCasesBaseFixture
{
    public CreateGenreInput GetExampleInput()
        => new CreateGenreInput(
            GetValidGenreName(),
            GetRandomBoolean()
        );
}