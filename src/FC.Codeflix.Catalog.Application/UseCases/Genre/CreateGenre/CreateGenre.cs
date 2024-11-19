using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;

public class CreateGenre : ICreateGenre
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGenre(
        IGenreRepository genreRepository, 
        IUnitOfWork unitOfWork,
        ICategoryRepository categoryRepository) 
    {
        _genreRepository = genreRepository;
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
    }

    public async Task<GenreModelOutput> Handle(
        CreateGenreInput request, 
        CancellationToken cancellationToken) 
    {
        var genre = new DomainEntity.Genre(
            request.Name,
            request.IsActive
        );
        
        if ((request.CategoriesId?.Count ?? 0) > 0)
        {
            await ValidateCategoriesIds(request, cancellationToken);
            request.CategoriesId?.ForEach(genre.AddCategory);
        }
        
        await _genreRepository.Insert(genre, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        
        return GenreModelOutput.FromGenre(genre);
    }

    private async Task ValidateCategoriesIds(
        CreateGenreInput request,
        CancellationToken cancellationToken)
    {
        var idsInPersistence = await _categoryRepository
            .GetIdsListByIds(
                request.CategoriesId!,
                cancellationToken
            );
        
        if (idsInPersistence.Count < request.CategoriesId!.Count)
        {
            var notFoundIds = request.CategoriesId.FindAll(x => !idsInPersistence.Contains(x));
            
            var notFoundIdsAsString = string.Join(", ", notFoundIds);
            
            throw new RelatedAggregateException(
                $"Related category id (or ids) not found: {notFoundIdsAsString}"
            );
        }
    }
}