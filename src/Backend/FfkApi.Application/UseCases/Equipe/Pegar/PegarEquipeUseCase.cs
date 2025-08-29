using AutoMapper;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Equipe.Pegar;

public class PegarEquipeUseCase : IPegarEquipeUseCase
{
    private readonly IMapper _mapper;
    private readonly IEquipeRepository _equipeRepository;
    private readonly IUsuarioLogadoService _usuarioLogadoService;

    public PegarEquipeUseCase(
        IEquipeRepository equipeRepository,
        IMapper mapper,
        IUsuarioLogadoService usuarioLogadoService)
    {
        _equipeRepository = equipeRepository;
        _mapper = mapper;
        _usuarioLogadoService = usuarioLogadoService;
    }

    public async Task<ResponseDadosEquipe> Execute(RequestPegarEquipe request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);

        var usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);

        var equipe = usuarioLogado.TemPerfilAdministrador() ?
            await _equipeRepository.PegarEquipePorId(idValido, cancellationToken) :
            await _equipeRepository.PegarEquipePorId(idValido, usuarioLogado.Organizacao.Id, cancellationToken);

        if (equipe == null)
            throw new NotFoundException(ResourceMessagesException.EQUIPE_NAO_ENCONTRADA);

        return _mapper.Map<ResponseDadosEquipe>(equipe);
    }
}
