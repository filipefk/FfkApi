using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Security.Criptografia;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Login.LoginSistema;

public class LoginSistemaClienteUseCase : ILoginSistemaClienteUseCase
{
    private readonly IGeradorTokenSistemaCliente _geradorTokenSistemaCliente;
    private readonly IEncriptadorSenha _encriptadorSenha;
    private readonly ISistemaClienteRepository _sistemaClienteRepository;

    public LoginSistemaClienteUseCase(
        ISistemaClienteRepository sistemaClienteRepository,
        IEncriptadorSenha encriptadorSenha,
        IGeradorTokenSistemaCliente geradorTokenSistemaCliente)
    {
        _sistemaClienteRepository = sistemaClienteRepository;
        _encriptadorSenha = encriptadorSenha;
        _geradorTokenSistemaCliente = geradorTokenSistemaCliente;
    }

    public async Task<ResponseLoginSistemaCliente> Execute(RequestLoginSistemaCliente request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var sistemaCliente = await _sistemaClienteRepository.PegarSistemaClienteAtivoPorAppId(Guid.Parse(request.AppId!), cancellationToken);

        if (sistemaCliente is null)
            throw new InvalidLoginSistemaClienteException();

        var validatePassword = _encriptadorSenha.SenhaValida(request.Senha!, sistemaCliente.Senha!);

        if (!validatePassword)
            throw new InvalidLoginSistemaClienteException();

        return new ResponseLoginSistemaCliente()
        {
            Nome = sistemaCliente.Nome,
            AccessToken = _geradorTokenSistemaCliente.Gerar(sistemaCliente.Id)
        };
    }

    private static async Task Validar(RequestLoginSistemaCliente request, CancellationToken cancellationToken)
    {
        var validator = new LoginSistemaClienteValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            var mensagensDeErro = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
            throw new ErrorOnValidationException(mensagensDeErro);
        }
    }
}
