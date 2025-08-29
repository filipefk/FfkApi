using AutoMapper;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Configurations;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Domain.Services.Email;

namespace FfkApi.Application.UseCases.EnvioEmail.EnvioEmailNovaSenha;

public class EnvioEmailNovaSenhaUseCase : IEnvioEmailNovaSenhaUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenNovaSenhaRepository _tokenNovaSenhaRepository;
    private readonly IEnviarEmailService _enviarEmailService;
    private readonly IGeradorTokenNovaSenha _geradorTokenNovaSenha;

    public EnvioEmailNovaSenhaUseCase(
        IUsuarioRepository usuarioRepository,
        ITokenNovaSenhaRepository tokenNovaSenhaRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEnviarEmailService enviarEmailService,
        IGeradorTokenNovaSenha geradorTokenNovaSenha)
    {
        _usuarioRepository = usuarioRepository;
        _tokenNovaSenhaRepository = tokenNovaSenhaRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _enviarEmailService = enviarEmailService;
        _geradorTokenNovaSenha = geradorTokenNovaSenha;
    }

    public async Task<IList<ResponseDadosUsuario>?> Execute(CancellationToken cancellationToken)
    {
        var usuarios = await _usuarioRepository.PegarUsuariosParaEnvioEmailNovaSenha(cancellationToken);

        if (usuarios == null || usuarios.Count == 0)
            return null!;

        var response = new List<ResponseDadosUsuario>();

        foreach (var usuario in usuarios)
        {
            var tokenNovaSenha = await _tokenNovaSenhaRepository.PegarTokenNovaSenhaPorUsuario(usuario.Id, cancellationToken);

            var validade = tokenNovaSenha!.DataCriacaoUtc
                .AddHours(_geradorTokenNovaSenha.TempoValidadeHoras)
                .ToLocalTime();

            string? textoEmail = null;
            if (usuario.Organizacao.ModeloEmailNovaSenha == null)
            {
                textoEmail = $"Olá {usuario.Nome}!\n\n" +
                    $"Você solicitou redefinição da senha no sistema FfkApi - {usuario.Organizacao.Nome}. " +
                    "Clique no link abaixo ou copie e cole no seu browser para redefinir a sua senha:\n" +
                    $"Obs.: Esta redefinição de senha só é válida até {validade:dd/MM/yyyy HH:mm}\n\n" +
                    $"{ConfiguracaoFront.UrlFront}/nova-senha/{tokenNovaSenha!.Valor}";
            }

            var resposta = await _enviarEmailService.EnviarEmailAsync(
                remetenteEmail: usuario.Organizacao.RemetenteEmail,
                remetenteNome: usuario.Organizacao.RemetenteNome,
                destinatarioEmail: usuario.Email,
                destinatarioNome: usuario.Nome,
                assunto: $"Renovação de senha do usuário do sistema FfkApi - {usuario.Organizacao.Nome}",
                modeloEmail: usuario.Organizacao.ModeloEmailNovaSenha,
                textoEmail: textoEmail,
                variaveis: new Dictionary<string, string>
                    {
                        { "nome_usuario", usuario.Nome },
                        { "nome_sistema", $"FfkApi - {usuario.Organizacao.Nome}" },
                        { "validade", $"{validade:dd/MM/yyyy HH:mm}" },
                        { "url_nova_senha", $"{ConfiguracaoFront.UrlFront}/nova-senha/{tokenNovaSenha!.Valor}" }
                    },
                cancellationToken: cancellationToken);

            tokenNovaSenha.UltimaTentativaEnvioEmail = DateTime.UtcNow;

            if (resposta.Enviado)
            {
                response.Add(_mapper.Map<ResponseDadosUsuario>(usuario));
                tokenNovaSenha.EmailEnviado = true;
                tokenNovaSenha.ErroEnvioEmail = null;
            }
            else
            {
                tokenNovaSenha.EmailEnviado = false;
                tokenNovaSenha.ErroEnvioEmail = resposta.Mensagem;
            }
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        return response;
    }
}
