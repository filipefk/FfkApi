using AutoMapper;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Organizacao.Pegar;

public class PegarOrganizacaoUseCase : IPegarOrganizacaoUseCase
{
    private readonly IMapper _mapper;
    private readonly IOrganizacaoRepository _organizacaoRepository;

    public PegarOrganizacaoUseCase(
        IOrganizacaoRepository organizacaoRepository,
        IMapper mapper)
    {
        _organizacaoRepository = organizacaoRepository;
        _mapper = mapper;
    }

    public async Task<ResponseDadosOrganizacao> Execute(RequestPegarOrganizacao request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);

        var organizacao = await _organizacaoRepository.PegarOrganizacaoPorId(idValido, cancellationToken);

        if (organizacao == null)
            throw new NotFoundException(ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA);

        return _mapper.Map<ResponseDadosOrganizacao>(organizacao);
    }
}
