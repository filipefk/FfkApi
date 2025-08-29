using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Equipe.Excluir;

public class ExcluirEquipeUseCase : IExcluirEquipeUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEquipeRepository _equipeRepository;
    private readonly IUsuarioLogadoService _usuarioLogadoService;

    public ExcluirEquipeUseCase(
        IEquipeRepository equipeRepository,
        IUnitOfWork unitOfWork,
        IUsuarioLogadoService usuarioLogadoService)
    {
        _equipeRepository = equipeRepository;
        _unitOfWork = unitOfWork;
        _usuarioLogadoService = usuarioLogadoService;
    }

    public async Task Execute(RequestExcluirEquipe request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);

        var usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);

        var equipe = usuarioLogado.TemPerfilAdministrador() ?
            await _equipeRepository.PegarEquipePorId(idValido, cancellationToken) :
            await _equipeRepository.PegarEquipePorId(idValido, usuarioLogado.Organizacao.Id, cancellationToken);

        if (equipe == null)
            throw new NotFoundException(ResourceMessagesException.EQUIPE_NAO_ENCONTRADA);

        await _equipeRepository.Excluir(idValido, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
