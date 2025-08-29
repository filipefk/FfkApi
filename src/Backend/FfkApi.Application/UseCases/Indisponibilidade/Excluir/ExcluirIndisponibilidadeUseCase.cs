using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Indisponibilidade.Excluir;

public class ExcluirIndisponibilidadeUseCase : IExcluirIndisponibilidadeUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIndisponibilidadeRepository _indisponibilidadeRepository;
    private readonly IUsuarioLogadoService _usuarioLogadoService;

    public ExcluirIndisponibilidadeUseCase(
        IIndisponibilidadeRepository indisponibilidadeRepository,
        IUnitOfWork unitOfWork,
        IUsuarioLogadoService usuarioLogadoService)
    {
        _indisponibilidadeRepository = indisponibilidadeRepository;
        _unitOfWork = unitOfWork;
        _usuarioLogadoService = usuarioLogadoService;
    }

    public async Task Execute(RequestExcluirIndisponibilidade request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);
        await _indisponibilidadeRepository.Excluir(Guid.Parse(request.Id!), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task Validar(RequestExcluirIndisponibilidade request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);
        var usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);

        var indisponibilidade = usuarioLogado.TemPerfilAdministrador() ?
                await _indisponibilidadeRepository.PegarIndisponibilidadePorId(Guid.Parse(request.Id!), cancellationToken) :
                await _indisponibilidadeRepository.PegarIndisponibilidadePorId(Guid.Parse(request.Id!), usuarioLogado.Organizacao.Id, cancellationToken);

        if (indisponibilidade == null)
            throw new NotFoundException(ResourceMessagesException.INDISPONIBILIDADE_NAO_ENCONTRADA);

        var alterandoSeusDados = usuarioLogado.Id == indisponibilidade.Usuario.Id;
        var temPermissaoCadastroIndisponibilidade = usuarioLogado.TemPermissao("Cadastro de Indisponibilidades");

        if (!alterandoSeusDados && !temPermissaoCadastroIndisponibilidade)
            throw new ForbiddenException(ResourceMessagesException.SEM_PERMISSAO.Replace("{permissao}", "Cadastro de Indisponibilidades"));
    }
}
