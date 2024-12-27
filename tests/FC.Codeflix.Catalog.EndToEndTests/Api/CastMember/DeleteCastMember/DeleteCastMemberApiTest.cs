using System.Net;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.DeleteCastMember;

[Collection(nameof(CastMemberApiBaseFixture))]
public class DeleteCastMemberApiTest : IDisposable
{
    private readonly CastMemberApiBaseFixture _fixture;

    public DeleteCastMemberApiTest(CastMemberApiBaseFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Delete))]
    [Trait("EndToEnd/API", "CatMembers/Delete - EndPoints")]
    public async Task Delete()
    {
        var examples = _fixture.GetExampleCastMembersList(5);
        var example = examples[2];
        await _fixture.Persistence.InsertList(examples);

        var (response, output) =
            await _fixture.ApiClient.Delete<object>(
                $"cast_members/{example.Id.ToString()}"
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        var castMemberExample = await _fixture.Persistence.GetById(example.Id);
        castMemberExample.Should().BeNull();
    }

    [Fact(DisplayName = nameof(NotFound))]
    [Trait("EndToEnd/API", "CatMembers/Delete - EndPoints")]
    public async Task NotFound()
    {
        await _fixture.Persistence.InsertList(
            _fixture.GetExampleCastMembersList(5)
        );
        var randomGuid = Guid.NewGuid();

        var (response, output) =
            await _fixture.ApiClient.Delete<ProblemDetails>(
                $"cast_members/{randomGuid.ToString()}"
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Title.Should().Be("Not Found");
        output!.Detail.Should().Be($"CastMember '{randomGuid}' not found.");
    }

    public void Dispose() => _fixture.CleanPersistence();
}