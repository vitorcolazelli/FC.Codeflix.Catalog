using Bogus;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.IntegratedTests.Base;

public class BaseFixture
{
    public BaseFixture() 
        => Faker = new Faker("pt_BR");

    protected Faker Faker { get; set; }

    public CodeflixCatalogDbContext CreateDbContext(bool preserveData = false)
    {
        var context = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                .UseInMemoryDatabase("integrated-tests-db")
                .Options);
        
        if (preserveData == false)
            context.Database.EnsureDeleted();
        
        return context;
    }
}