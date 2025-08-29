using AutoMapper;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Usuario.Pegar;

public class PegarUsuarioPorIdUseCase : IPegarUsuarioPorIdUseCase
{
    private readonly IUsuarioLogadoService _usuarioLogadoService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;

    public PegarUsuarioPorIdUseCase(
        IUsuarioLogadoService usuarioLogadoService,
        IUsuarioRepository usuarioRepository,
        IMapper mapper)
    {
        _usuarioLogadoService = usuarioLogadoService;
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
    }

    public async Task<ResponseDadosUsuario> Execute(RequestPegarUsuario request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);

        var usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);
        if (usuarioLogado.Id != idValido && !usuarioLogado.TemPermissao("Cadastro de Usuários"))
            throw new ForbiddenException(ResourceMessagesException.SEM_PERMISSAO.Replace("{permissao}", "Cadastro de Usuários"));

        var usuario = await _usuarioRepository.PegarUsuarioPorId(idValido, cancellationToken);
        if (usuario == null)
            throw new NotFoundException(ResourceMessagesException.USUARIO_NAO_ENCONTRADO);

        return _mapper.Map<ResponseDadosUsuario>(usuario);
    }
}