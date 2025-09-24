using AutoMapper;
using FfkApi.Application.Extension;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Organizacao.Alterar;

public class AlterarOrganizacaoUseCase : IAlterarOrganizacaoUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrganizacaoRepository _organizacaoRepository;

    public AlterarOrganizacaoUseCase(
        IOrganizacaoRepository organizacaoRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _organizacaoRepository = organizacaoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Execute(RequestAlterarOrganizacao request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var organizacao = await _organizacaoRepository.PegarOrganizacaoPorId(Guid.Parse(request.Id!), cancellationToken);
        _mapper.Map(request, organizacao);

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task Validar(RequestAlterarOrganizacao request, CancellationToken cancellationToken)
    {
        List<string> mensagensDeErro = [];
        mensagensDeErro.AddRange(await ValidarRequisicao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarOrganizacao(request, cancellationToken));

        if (mensagensDeErro.Count > 0)
        {
            throw new ErrorOnValidationException(mensagensDeErro.Distinct().ToList());
        }
    }

    private async Task<List<string>> ValidarRequisicao(RequestAlterarOrganizacao request, CancellationToken cancellationToken)
    {
        var validator = new AlterarOrganizacaoValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            return result.ToListErros();
        }

        return [];
    }

    private async Task<List<string>> ValidarOrganizacao(RequestAlterarOrganizacao request, CancellationToken cancellationToken)
    {
        if (IdValidator.IdEstaValido(request.Id!))
        {
            var organizacao = await _organizacaoRepository.PegarOrganizacaoPorId(Guid.Parse(request.Id!), cancellationToken);
            if (organizacao == null)
                return [ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA];

            if (!string.IsNullOrWhiteSpace(request.Nome) && request.Nome != organizacao.Nome && await _organizacaoRepository.ExisteOrganizacaoComNome(request.Nome, cancellationToken))
                return [ResourceMessagesException.NOME_DE_ORGANIZACAO_JA_EXISTE];

            if (!string.IsNullOrWhiteSpace(request.Nome) && !string.IsNullOrWhiteSpace(request.Descricao) && request.Nome == organizacao.Nome && request.Descricao == organizacao.Descricao)
                return [ResourceMessagesException.NENHUMA_ALTERACAO];
        }

        return [];
    }
}
