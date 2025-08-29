using AutoMapper;
using FfkApi.Application.Extension;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Organizacao.Cadastrar;

public class CadastrarOrganizacaoUseCase : ICadastrarOrganizacaoUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrganizacaoRepository _organizacaoRepository;

    public CadastrarOrganizacaoUseCase(
        IOrganizacaoRepository organizacaoRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _organizacaoRepository = organizacaoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseDadosOrganizacao> Execute(RequestCadastrarOrganizacao request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var organizacao = _mapper.Map<Domain.Entities.Organizacao>(request);

        await _organizacaoRepository.Adicionar(organizacao, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return _mapper.Map<ResponseDadosOrganizacao>(organizacao);

    }

    private async Task Validar(RequestCadastrarOrganizacao request, CancellationToken cancellationToken)
    {
        List<string> mensagensDeErro = [];
        mensagensDeErro.AddRange(await ValidarRequisicao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarOrganizacao(request, cancellationToken));

        if (mensagensDeErro.Count > 0)
        {
            throw new ErrorOnValidationException(mensagensDeErro.Distinct().ToList());
        }
    }

    private async Task<List<string>> ValidarRequisicao(RequestCadastrarOrganizacao request, CancellationToken cancellationToken)
    {
        var validator = new CadastrarOrganizacaoValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            return result.ToListErros();
        }

        return [];
    }

    private async Task<List<string>> ValidarOrganizacao(RequestCadastrarOrganizacao request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Nome) && await _organizacaoRepository.ExisteOrganizacaoComNome(request.Nome, cancellationToken))
            return [ResourceMessagesException.NOME_DE_ORGANIZACAO_JA_EXISTE];

        return [];
    }

}
