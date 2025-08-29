using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Token.TokenAtivacao;

public class PegarUsuarioPorTokenAtivacaoUseCase : IPegarUsuarioPorTokenAtivacaoUseCase
{
    private readonly ITokenAtivacaoRepository _tokenAtivacaoRepository;
    private readonly IGeradorTokenAtivacao _geradorTokenAtivacao;

    public PegarUsuarioPorTokenAtivacaoUseCase(
        ITokenAtivacaoRepository tokenAtivacaoRepository,
        IGeradorTokenAtivacao geradorTokenAtivacao)
    {
        _tokenAtivacaoRepository = tokenAtivacaoRepository;
        _geradorTokenAtivacao = geradorTokenAtivacao;
    }

    public async Task<ResponseNomeUsuario> Execute(RequestPegarUsuarioPorTokenAtivacao request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var token = await _tokenAtivacaoRepository.PegarTokenAtivacaoPorToken(request.TokenAtivacao!, cancellationToken);

        if (token == null)
            throw new NotFoundException(ResourceMessagesException.TOKEN_ATIVACAO_NAO_ENCONTRADO);

        if (!_geradorTokenAtivacao.TokenValido(token))
            throw new ForbiddenException(ResourceMessagesException.TOKEN_ATIVACAO_EXPIRADO);

        return new ResponseNomeUsuario
        {
            Nome = token.Usuario.Nome
        };
    }

    private static async Task Validar(RequestPegarUsuarioPorTokenAtivacao request, CancellationToken cancellationToken)
    {
        var validator = new PegarUsuarioPorTokenAtivacaoValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            var mensagensDeErro = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
            throw new ErrorOnValidationException(mensagensDeErro);
        }
    }
}
