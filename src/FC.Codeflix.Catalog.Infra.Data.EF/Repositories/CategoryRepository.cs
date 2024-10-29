using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly CodeflixCatalogDbContext _context;
    private DbSet<Category> Categories => _context.Set<Category>();
    
    public CategoryRepository(CodeflixCatalogDbContext context) => 
        _context = context;
    
    public async Task Insert(Category aggregate, CancellationToken cancellationToken) =>
        await Categories.AddAsync(aggregate, cancellationToken);

    public async Task<Category> Get(Guid id, CancellationToken cancellationToken)
    {
        var category = await Categories.FindAsync(new object[] { id }, cancellationToken);

        NotFoundException.ThrowIfNull(category, $"Category '{id}' not found.");
        return category!;
    }

    public Task Delete(Category aggregate, CancellationToken cancellationToken) =>
        Task.FromResult(_context.Remove(aggregate));

    public Task Update(Category aggregate, CancellationToken cancellationToken) =>
        Task.FromResult(_context.Update(aggregate));
    
    public async Task<SearchOutput<Category>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        
        var total = await Categories.CountAsync(cancellationToken);
        
        var items = await Categories
            .AsNoTracking()
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync(cancellationToken);

        return new SearchOutput<Category>(input.Page, input.PerPage, total, items);
    }
    
    public Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Category>> GetListByIds(List<Guid> ids, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}