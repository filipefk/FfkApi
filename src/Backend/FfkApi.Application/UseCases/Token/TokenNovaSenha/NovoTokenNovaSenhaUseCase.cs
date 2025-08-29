using FfkApi.Application.Extension;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Token.TokenNovaSenha;

public class NovoTokenNovaSenhaUseCase : INovoTokenNovaSenhaUseCase
{
    private readonly ITokenNovaSenhaRepository _tokenNovaSenhaRepository;
    private readonly IGeradorTokenNovaSenha _geradorTokenNovaSenha;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NovoTokenNovaSenhaUseCase(
        ITokenNovaSenhaRepository tokenNovaSenhaRepository,
        IGeradorTokenNovaSenha geradorTokenNovaSenha,
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork)
    {
        _tokenNovaSenhaRepository = tokenNovaSenhaRepository;
        _geradorTokenNovaSenha = geradorTokenNovaSenha;
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseNomeUsuario> Execute(RequestNovoTokenNovaSenha request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var usuario = await _usuarioRepository.PegarUsuarioAptoPorEmail(request.Email!, cancellationToken);

        if (usuario == null)
            throw new NotFoundException(ResourceMessagesException.USUARIO_NAO_ENCONTRADO);

        if (request.Nome != usuario.Nome || request.Cpf != usuario.Cpf)
            throw new ErrorOnValidationException([ResourceMessagesException.DADOS_INVALIDOS]);

        var novoTokenNovaSenha = await CriarESalvarTokenNovaSenha(usuario, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return new ResponseNomeUsuario
        {
            Nome = usuario.Nome
        };
    }

    private async Task<string> CriarESalvarTokenNovaSenha(Domain.Entities.Usuario usuario, CancellationToken cancellationToken)
    {
        var tokenNovaSenha = new Domain.Entities.TokenNovaSenha
        {
            Valor = _geradorTokenNovaSenha.Gerar(),
            IdUsuario = usuario.Id
        };

        await _tokenNovaSenhaRepository.SalvarNovoTokenNovaSenha(tokenNovaSenha, cancellationToken);

        return tokenNovaSenha.Valor;
    }

    private static async Task Validar(RequestNovoTokenNovaSenha request, CancellationToken cancellationToken)
    {
        var validator = new NovoTokenNovaSenhaValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            var mensagensDeErro = result.ToListErros();
            throw new ErrorOnValidationException(mensagensDeErro);
        }
    }
}
