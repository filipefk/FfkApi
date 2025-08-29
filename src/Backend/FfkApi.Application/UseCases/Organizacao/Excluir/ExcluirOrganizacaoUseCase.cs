using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Organizacao.Excluir;

public class ExcluirOrganizacaoUseCase : IExcluirOrganizacaoUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrganizacaoRepository _organizacaoRepository;

    public ExcluirOrganizacaoUseCase(
        IOrganizacaoRepository organizacaoRepository,
        IUnitOfWork unitOfWork)
    {
        _organizacaoRepository = organizacaoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(RequestExcluirOrganizacao request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);

        var organizacao = await _organizacaoRepository.PegarOrganizacaoPorId(idValido, cancellationToken);

        if (organizacao == null)
            throw new NotFoundException(ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA);

        try
        {
            await _organizacaoRepository.Excluir(idValido, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch (InvalidOperationException)
        {
            throw new ErrorOnValidationException([ResourceMessagesException.ORGANIZACAO_JA_VINCULADA]);
        }
    }
}
