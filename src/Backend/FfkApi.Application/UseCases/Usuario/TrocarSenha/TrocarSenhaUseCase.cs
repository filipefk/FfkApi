using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Security.Criptografia;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using FluentValidation.Results;

namespace FfkApi.Application.UseCases.Usuario.TrocarSenha;

public class TrocarSenhaUseCase : ITrocarSenhaUseCase
{
    private readonly IUsuarioLogadoService _usuarioLogadoService;
    private readonly IEncriptadorSenha _encriptadorSenha;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TrocarSenhaUseCase(
        IUsuarioLogadoService usuarioLogadoService,
        IEncriptadorSenha passwordEncryptor,
        IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork)
    {
        _usuarioLogadoService = usuarioLogadoService;
        _encriptadorSenha = passwordEncryptor;
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(RequestTrocarSenha request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var usuario = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);

        await _usuarioRepository.AlterarSenha(usuario.Id, _encriptadorSenha.Encriptar(request.NovaSenha!), cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task Validar(RequestTrocarSenha request, CancellationToken cancellationToken)
    {
        var validator = new TrocarSenhaValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.SenhaAntiga) && !string.IsNullOrWhiteSpace(request.NovaSenha))
        {
            var usuario = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);

            var validatePassword = _encriptadorSenha.SenhaValida(request.SenhaAntiga, usuario.Senha!);

            if (!validatePassword)
                result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.SENHA_ANTIGA_DIFERENTE_DA_SENHA_INFORMADA));

            if (validatePassword && request.SenhaAntiga == request.NovaSenha)
                result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.NENHUMA_ALTERACAO));
        }

        if (!result.IsValid)
        {
            var mensagensDeErro = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
            throw new ErrorOnValidationException(mensagensDeErro);
        }
    }

}