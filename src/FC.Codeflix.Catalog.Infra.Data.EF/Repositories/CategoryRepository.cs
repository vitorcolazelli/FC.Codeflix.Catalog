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
    
    public async Task<SearchOutput<Category>> Search(
        SearchInput input, 
        CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        
        var query = Categories.AsNoTracking();
        query = AddOrderToQuery(query, input.OrderBy, input.Order);
        
        if(!string.IsNullOrWhiteSpace(input.Search))
            query = query.Where(x => x.Name.Contains(input.Search));
        
        var total = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync(cancellationToken);
        
        return new SearchOutput<Category>(input.Page, input.PerPage, total, items);
    }

    private static IQueryable<Category> AddOrderToQuery(
        IQueryable<Category> query,
        string orderProperty,
        SearchOrder order)
    { 
        var orderedQuery = (orderProperty.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name)
                .ThenBy(x => x.Id),
            ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name)
                .ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
        };
        
        return orderedQuery;
    }

    public async Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken) => 
        await Categories.AsNoTracking()
            .Where(category => ids.Contains(category.Id))
            .Select(category => category.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Category>> GetListByIds(List<Guid> ids, CancellationToken cancellationToken) => 
        await Categories.AsNoTracking()
            .Where(category => ids.Contains(category.Id))
            .ToListAsync(cancellationToken);
}