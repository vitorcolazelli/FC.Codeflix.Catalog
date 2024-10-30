using FC.Codeflix.Catalog.IntegratedTests.Application.UseCases.Category.Common;

namespace FC.Codeflix.Catalog.IntegratedTests.Application.UseCases.Category.DeleteCategory;

[CollectionDefinition(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTestFixtureCollection
    : ICollectionFixture<DeleteCategoryTestFixture>
{}

public class DeleteCategoryTestFixture
    : CategoryUseCasesBaseFixture
{
}