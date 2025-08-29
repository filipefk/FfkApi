using AutoMapper;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Services.UsuarioLogado;

namespace FfkApi.Application.UseCases.Usuario.Pegar;

public class PegarUsuarioLogadoUseCase : IPegarUsuarioLogadoUseCase
{
    private readonly IUsuarioLogadoService _usuarioLogadoService;
    private readonly IMapper _mapper;

    public PegarUsuarioLogadoUseCase(
        IUsuarioLogadoService usuarioLogadoService,
        IMapper mapper)
    {
        _usuarioLogadoService = usuarioLogadoService;
        _mapper = mapper;
    }

    public async Task<ResponseDadosUsuario> Execute(CancellationToken cancellationToken)
    {
        var usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);

        var usuarioMapped = _mapper.Map<ResponseDadosUsuario>(usuarioLogado);

        return usuarioMapped;
    }
}
