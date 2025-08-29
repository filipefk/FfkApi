using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Token.TokenNovaSenha;

public class PegarUsuarioPorTokenNovaSenhaUseCase : IPegarUsuarioPorTokenNovaSenhaUseCase
{
    private readonly ITokenNovaSenhaRepository _tokenNovaSenhaRepository;
    private readonly IGeradorTokenNovaSenha _geradorTokenNovaSenha;

    public PegarUsuarioPorTokenNovaSenhaUseCase(
        ITokenNovaSenhaRepository tokenNovaSenhaRepository,
        IGeradorTokenNovaSenha geradorTokenNovaSenha)
    {
        _tokenNovaSenhaRepository = tokenNovaSenhaRepository;
        _geradorTokenNovaSenha = geradorTokenNovaSenha;
    }

    public async Task<ResponseNomeUsuario> Execute(RequestPegarUsuarioPorTokenNovaSenha request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var token = await _tokenNovaSenhaRepository.PegarTokenNovaSenhaPorToken(request.TokenNovaSenha!, cancellationToken);

        if (token == null)
            throw new NotFoundException(ResourceMessagesException.TOKEN_NOVA_SENHA_NAO_ENCONTRADO);

        if (!_geradorTokenNovaSenha.TokenValido(token))
            throw new ForbiddenException(ResourceMessagesException.TOKEN_NOVA_SENHA_EXPIRADO);

        return new ResponseNomeUsuario
        {
            Nome = token.Usuario.Nome
        };
    }

    private static async Task Validar(RequestPegarUsuarioPorTokenNovaSenha request, CancellationToken cancellationToken)
    {
        var validator = new PegarUsuarioPorTokenNovaSenhaValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            var mensagensDeErro = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
            throw new ErrorOnValidationException(mensagensDeErro);
        }
    }
}
