using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;

namespace FC.Codeflix.Catalog.IntegratedTests.Application.UseCases.Genre.DeleteGenre;

[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest
{
    private readonly DeleteGenreTestFixture _fixture;

    public DeleteGenreTest(DeleteGenreTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenre()
    {
        var genresExampleList = _fixture.GetExampleListGenres(10);
        var targetGenre = genresExampleList[5];
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var unitOfWork = new UnitOfWork(actDbContext);
        var useCase = new UseCase.DeleteGenre(
            new GenreRepository(actDbContext),
            unitOfWork
        );
        var input = new UseCase.DeleteGenreInput(targetGenre.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenreWithRelations()
    {
        var genresExampleList = _fixture.GetExampleListGenres(10);
        var targetGenre = genresExampleList[5];
        var exampleCategories = _fixture.GetExampleCategoriesList(5);
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.Categories.AddRangeAsync(exampleCategories);
        await dbArrangeContext.GenresCategories.AddRangeAsync(
            exampleCategories.Select(category => 
                new GenresCategories(
                    category.Id, 
                    targetGenre.Id
                )
            )
        );
        await dbArrangeContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var unitOfWork = new UnitOfWork(actDbContext);
        var useCase = new UseCase.DeleteGenre(
            new GenreRepository(actDbContext),
            unitOfWork
        );
        var input = new UseCase.DeleteGenreInput(targetGenre.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().BeNull();
        var relations = await assertDbContext.GenresCategories.AsNoTracking()
            .Where(relation => relation.GenreId == targetGenre.Id)
            .ToListAsync();
        relations.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(DeleteGenreThrowsWhenNotFound))]
    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenreThrowsWhenNotFound()
    {
        var genresExampleList = _fixture.GetExampleListGenres(10);
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var unitOfWork = new UnitOfWork(actDbContext);
        var useCase = new UseCase.DeleteGenre(
            new GenreRepository(actDbContext),
            unitOfWork
        );
        var randomGuid = Guid.NewGuid();
        var input = new UseCase.DeleteGenreInput(randomGuid);

        Func<Task> action = 
            async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{randomGuid}' not found.");
    }
}