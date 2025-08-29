using AutoMapper;
using FfkApi.Application.Extension;
using FfkApi.Application.UseCases.Organizacao.Cadastrar;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;

namespace FfkApi.Application.UseCases.Organizacao.CadastrarEmLote;

public class CadastrarOrganizacaoEmLoteUseCase : ICadastrarOrganizacaoEmLoteUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrganizacaoRepository _organizacaoRepository;

    public CadastrarOrganizacaoEmLoteUseCase(
        IOrganizacaoRepository organizacaoRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _organizacaoRepository = organizacaoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseCadastrarEmLote<RequestCadastrarOrganizacao, ResponseDadosOrganizacao>> Execute(RequestCadastrarOrganizacaoEmLote requests, CancellationToken cancellationToken)
    {
        var response = new ResponseCadastrarEmLote<RequestCadastrarOrganizacao, ResponseDadosOrganizacao>();

        if (requests == null || requests.Organizacoes == null || requests.Organizacoes.Count == 0)
        {
            var erro = new ResponseErroCadastroLote<RequestCadastrarOrganizacao>
            {
                Request = null!,
                MensagensDeErro = [ResourceMessagesException.LISTA_DE_ORGANIZACAO_VAZIA]
            };
            response.Erros.Add(erro);
            return response;
        }

        var nomesOrganizacao = new HashSet<string>();

        foreach (var request in requests.Organizacoes)
        {
            var mensagensDeErro = await Validar(request, cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.Nome) && !nomesOrganizacao.Add(request.Nome))
            {
                mensagensDeErro.Remove(ResourceMessagesException.NOME_DE_ORGANIZACAO_JA_EXISTE);
                mensagensDeErro.Add(ResourceMessagesException.NOME_DE_ORGANIZACAO_REPETIDO);
            }

            if (mensagensDeErro.Count > 0)
            {
                var erro = new ResponseErroCadastroLote<RequestCadastrarOrganizacao>
                {
                    Request = request,
                    MensagensDeErro = mensagensDeErro
                };
                response.Erros.Add(erro);
                continue;
            }

            var organizacao = _mapper.Map<Domain.Entities.Organizacao>(request);
            await _organizacaoRepository.Adicionar(organizacao, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            response.Cadastrados.Add(_mapper.Map<ResponseDadosOrganizacao>(organizacao));
        }
        return response;
    }

    private async Task<List<string>> Validar(RequestCadastrarOrganizacao request, CancellationToken cancellationToken)
    {
        List<string> mensagensDeErro = [];
        mensagensDeErro.AddRange(await ValidarRequisicao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarOrganizacao(request, cancellationToken));

        return mensagensDeErro;
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
