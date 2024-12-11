using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly CodeflixCatalogDbContext _context;
    private DbSet<Genre> Genres => _context.Set<Genre>();
    private DbSet<GenresCategories> GenresCategories => _context.Set<GenresCategories>();

    public GenreRepository(CodeflixCatalogDbContext context)
        => _context = context;

    public async Task Insert(
        Genre genre, 
        CancellationToken cancellationToken) 
    {
        await Genres.AddAsync(genre, cancellationToken);
        
        if (genre.Categories.Count > 0)
        {
            var relations = genre.Categories
                .Select(categoryId => new GenresCategories(
                    categoryId,
                    genre.Id
                ));
            
            await GenresCategories.AddRangeAsync(relations, cancellationToken);
        }
    }

    public async Task<Genre> Get(
        Guid id, 
        CancellationToken cancellationToken)
    {
        var genre = await Genres
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        NotFoundException.ThrowIfNull(genre, $"Genre '{id}' not found.");
        
        var categoryIds = await GenresCategories
            .Where(x => x.GenreId == genre!.Id)
            .Select(x => x.CategoryId)
            .ToListAsync(cancellationToken);
        
        categoryIds.ForEach(genre!.AddCategory);
        
        return genre;
    }

    public Task Delete(
        Genre aggregate, 
        CancellationToken cancellationToken)
    {
        GenresCategories.RemoveRange(
            GenresCategories.Where(x => x.GenreId == aggregate.Id)
        );
        
        Genres.Remove(aggregate);
        
        return Task.CompletedTask;
    }

    public async Task Update(Genre genre, CancellationToken cancellationToken)
    {
        Genres.Update(genre);
        
        GenresCategories.RemoveRange(GenresCategories
            .Where(x => x.GenreId == genre.Id));
        
        if (genre.Categories.Count > 0)
        {
            var relations = genre.Categories
                .Select(categoryId => new GenresCategories(
                    categoryId,
                    genre.Id
                ));
            
            await GenresCategories.AddRangeAsync(relations, cancellationToken);
        }
    }

    public async Task<SearchOutput<Genre>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        var query = Genres.AsNoTracking();

        query = AddOrderToQuery(query, input.OrderBy, input.Order);

        if(!string.IsNullOrWhiteSpace(input.Search))
            query = query.Where(genre => genre.Name.Contains(input.Search));

        var genres = await query
                .Skip(toSkip).Take(input.PerPage).ToListAsync();

        var total = await query.CountAsync(cancellationToken: cancellationToken);

        var genresIds = genres.Select(genre => genre.Id).ToList();
        
        var relations = await GenresCategories
            .Where(relation => genresIds.Contains(relation.GenreId))
            .ToListAsync(cancellationToken: cancellationToken);
        
        var relationsByGenreIdGroup = 
            relations.GroupBy(x => x.GenreId).ToList();
        
        relationsByGenreIdGroup.ForEach(relationGroup => {
            var genre = genres.Find(genre => genre.Id == relationGroup.Key);
            
            if (genre is null) 
                return;
            
            relationGroup.ToList()
                .ForEach(relation => genre.AddCategory(relation.CategoryId));
        });
        
        return new SearchOutput<Genre>(
            input.Page,
            input.PerPage,
            total,
            genres
        );
    }

    private static IQueryable<Genre> AddOrderToQuery(
        IQueryable<Genre> query,
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

    public async Task<IReadOnlyList<Guid>> GetIdsListByIds(
        List<Guid> ids, 
        CancellationToken cancellationToken) 
        => await Genres.AsNoTracking()
        .Where(genre => ids.Contains(genre.Id))
        .Select(genre => genre.Id)
        .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Genre>> GetListByIds(
        List<Guid> ids, 
        CancellationToken cancellationToken) 
        => await Genres.AsNoTracking()
        .Where(genre => ids.Contains(genre.Id))
        .ToListAsync(cancellationToken);
}