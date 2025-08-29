using AutoMapper;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Configurations;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Domain.Services.Email;

namespace FfkApi.Application.UseCases.EnvioEmail.EnvioEmailAtivacao;

public class EnvioEmailAtivacaoUseCase : IEnvioEmailAtivacaoUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenAtivacaoRepository _tokenAtivacaoRepository;
    private readonly IGeradorTokenAtivacao _geradorTokenAtivacao;
    private readonly IEnviarEmailService _enviarEmailService;

    public EnvioEmailAtivacaoUseCase(
        IUsuarioRepository usuarioRepository,
        ITokenAtivacaoRepository tokenAtivacaoRepository,
        IGeradorTokenAtivacao geradorTokenAtivacao,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEnviarEmailService enviarEmailService)
    {
        _usuarioRepository = usuarioRepository;
        _tokenAtivacaoRepository = tokenAtivacaoRepository;
        _geradorTokenAtivacao = geradorTokenAtivacao;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _enviarEmailService = enviarEmailService;
    }

    public async Task<IList<ResponseDadosUsuario>?> Execute(CancellationToken cancellationToken)
    {
        var usuarios = await _usuarioRepository.PegarUsuariosParaAtivacao(cancellationToken);

        if (usuarios != null && usuarios.Count > 0)
        {
            foreach (var usuario in usuarios)
            {
                await CriarESalvarTokenAtivacao(usuario, cancellationToken);
            }
        }

        usuarios = await _usuarioRepository.PegarUsuariosParaEnvioEmailAtivacao(cancellationToken);

        if (usuarios == null || usuarios.Count == 0)
            return null!;

        var response = new List<ResponseDadosUsuario>();

        foreach (var usuario in usuarios)
        {
            var tokenAtivacao = await _tokenAtivacaoRepository.PegarTokenAtivacaoPorUsuario(usuario.Id, cancellationToken);

            var validade = tokenAtivacao!.BaseExpiracaoUtc
                .AddHours(_geradorTokenAtivacao.TempoValidadeHoras)
                .ToLocalTime();

            string? textoEmail = null;
            if (usuario.Organizacao.ModeloEmailAtivacao == null)
            {
                textoEmail = $"Olá {usuario.Nome}!\n\n" +
                    $"Você foi cadastrado como usuário do sistema FfkApi - {usuario.Organizacao.Nome}. " +
                    "Clique no link abaixo ou copie e cole no seu browser para ativar sua conta:\n" +
                    $"Obs.: Esta redefinição de senha só é válida até {validade:dd/MM/yyyy HH:mm}\n\n" +
                    $"{ConfiguracaoFront.UrlFront}/ativacao/{tokenAtivacao!.Valor}";
            }

            var resposta = await _enviarEmailService.EnviarEmailAsync(
                remetenteEmail: usuario.Organizacao.RemetenteEmail,
                remetenteNome: usuario.Organizacao.RemetenteNome,
                destinatarioEmail: usuario.Email,
                destinatarioNome: usuario.Nome,
                assunto: $"Ativação do usuário do sistema FfkApi - {usuario.Organizacao.Nome}",
                modeloEmail: usuario.Organizacao.ModeloEmailAtivacao,
                textoEmail: textoEmail,
                variaveis: new Dictionary<string, string>
                    {
                        { "nome_usuario", usuario.Nome },
                        { "nome_sistema", $"FfkApi - {usuario.Organizacao.Nome}" },
                        { "validade", $"{validade:dd/MM/yyyy HH:mm}" },
                        { "url_ativacao", $"{ConfiguracaoFront.UrlFront}/ativacao/{tokenAtivacao!.Valor}" }
                    },
                cancellationToken: cancellationToken);

            tokenAtivacao.UltimaTentativaEnvioEmail = DateTime.UtcNow;

            if (resposta.Enviado)
            {
                response.Add(_mapper.Map<ResponseDadosUsuario>(usuario));
                tokenAtivacao.EmailEnviado = true;
                tokenAtivacao.ErroEnvioEmail = null;
            }
            else
            {
                tokenAtivacao.EmailEnviado = false;
                tokenAtivacao.ErroEnvioEmail = resposta.Mensagem;
            }
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        return response;
    }

    private async Task CriarESalvarTokenAtivacao(Domain.Entities.Usuario usuario, CancellationToken cancellationToken)
    {
        var tokenAtivacao = new Domain.Entities.TokenAtivacao
        {
            Valor = _geradorTokenAtivacao.Gerar(),
            IdUsuario = usuario.Id
        };
        await _tokenAtivacaoRepository.SalvarNovoTokenAtivacao(tokenAtivacao, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
