using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UnitOfWorkInfra = FC.Codeflix.Catalog.Infra.Data.EF;

namespace FC.Codeflix.Catalog.IntegratedTests.Infra.Data.EF.UnitOfWork;

[Collection(nameof(UnitOfWorkTestFixture))]
public class UnitOfWorkTest
{
    private readonly UnitOfWorkTestFixture _fixture;

    public UnitOfWorkTest(UnitOfWorkTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Commit))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Commit()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        await dbContext.AddRangeAsync(exampleCategoriesList);
        var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext);
        
        await unitOfWork.Commit(CancellationToken.None);
        
        var assetDbContext = _fixture.CreateDbContext(true);
        var savedCategories = assetDbContext.Categories.AsNoTracking().ToList();
        savedCategories.Should().HaveCount(exampleCategoriesList.Count);
    }
    
    [Fact(DisplayName = nameof(Rollback))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Rollback()
    {
        var dbContext = _fixture.CreateDbContext();

        var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext);
    
        var task = async () => await unitOfWork.Rollback(CancellationToken.None);
    
        await task.Should().NotThrowAsync();
    }
}