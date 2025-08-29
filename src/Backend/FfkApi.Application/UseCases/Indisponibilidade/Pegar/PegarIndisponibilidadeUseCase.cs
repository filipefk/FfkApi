using AutoMapper;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Indisponibilidade.Pegar;

public class PegarIndisponibilidadeUseCase : IPegarIndisponibilidadeUseCase
{
    private readonly IMapper _mapper;
    private readonly IIndisponibilidadeRepository _indisponibilidadeRepository;
    private readonly IUsuarioLogadoService _usuarioLogadoService;

    public PegarIndisponibilidadeUseCase(
        IIndisponibilidadeRepository indisponibilidadeRepository,
        IMapper mapper,
        IUsuarioLogadoService usuarioLogadoService)
    {
        _indisponibilidadeRepository = indisponibilidadeRepository;
        _mapper = mapper;
        _usuarioLogadoService = usuarioLogadoService;
    }

    public async Task<ResponseDadosIndisponibilidade> Execute(RequestPegarIndisponibilidade request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);

        var usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);

        var indisponibilidade = usuarioLogado.TemPerfilAdministrador() ?
            await _indisponibilidadeRepository.PegarIndisponibilidadePorId(idValido, cancellationToken) :
            await _indisponibilidadeRepository.PegarIndisponibilidadePorId(idValido, usuarioLogado.Organizacao.Id, cancellationToken);

        if (indisponibilidade == null)
            throw new NotFoundException(ResourceMessagesException.INDISPONIBILIDADE_NAO_ENCONTRADA);

        return _mapper.Map<ResponseDadosIndisponibilidade>(indisponibilidade);
    }
}
