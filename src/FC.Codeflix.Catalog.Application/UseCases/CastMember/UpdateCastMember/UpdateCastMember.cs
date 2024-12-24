using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;

public class UpdateCastMember : IUpdateCastMember
{
    private readonly ICastMemberRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCastMember(ICastMemberRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CastMemberModelOutput> Handle(
        UpdateCastMemberInput input, 
        CancellationToken cancellationToken)
    {
        var castMember = await _repository.Get(input.Id, cancellationToken);
        
        castMember.Update(input.Name, input.Type);
        
        await _repository.Update(castMember, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        
        return CastMemberModelOutput.FromCastMember(castMember);
    }
}