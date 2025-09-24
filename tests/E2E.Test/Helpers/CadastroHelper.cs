using FfkApi.Domain.Entities;
using FfkApi.Domain.Enums;
using System.Net;
using TestUtil.Entities;
using TestUtil.Extension;
using TestUtil.HttpUtil;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Aceitacao.Test.Helpers;

public class CadastroHelper
{
    private readonly HttpHelper _httpHelper;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAdministrador;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioSemPerfilNemPermissao;

    public CadastroHelper(
        HttpHelper httpHelper,
        FfkApi.Domain.Entities.Usuario usuarioAdministrador,
        FfkApi.Domain.Entities.Usuario usuarioSemPerfilNemPermissao)
    {
        _httpHelper = httpHelper;
        _usuarioAdministrador = usuarioAdministrador;
        _usuarioSemPerfilNemPermissao = usuarioSemPerfilNemPermissao;
    }

    public async Task<(string accessToken, string refreshToken)> PegarTokensLogin(FfkApi.Domain.Entities.Usuario usuario)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestLoginUsuarioBuilder.Build(usuario);

        var response = await _httpHelper.DoPost("login", cancellationToken, request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString()));

        var accessToken = dadosDaResposta.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString();
        var refreshToken = dadosDaResposta.RootElement.GetProperty("tokens").GetProperty("refreshToken").GetString();

        return (accessToken!, refreshToken!);
    }

    public async Task PegarDadosUsuario(FfkApi.Domain.Entities.Usuario usuario)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        (var token, _) = PegarTokensLogin(usuario).GetAwaiter().GetResult();

        var response = await _httpHelper.DoGet("usuario", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));

        var cpf = dadosDaResposta.RootElement.GetProperty("cpf").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(cpf));

        var telefone = dadosDaResposta.RootElement.GetProperty("telefone").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(telefone));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));

        var organizacao = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));

        var perfisAcesso = dadosDaResposta.RootElement.GetProperty("perfisAcesso").EnumerateArray();
        Assert.That(perfisAcesso, Is.Not.Null);

        var permissoes = dadosDaResposta.RootElement.GetProperty("permissoes").EnumerateArray();
        Assert.That(permissoes, Is.Not.Null);

        usuario.Id = Guid.Parse(id!);
        usuario.Cpf = cpf!;
        usuario.Telefone = telefone!;
        usuario.Status = EnumUtil.ConverterTextoParaEnum<StatusUsuario>(status!);
        usuario.Organizacao = new FfkApi.Domain.Entities.Organizacao { Nome = organizacao! };
        usuario.PerfisAcesso = perfisAcesso.Select(p => new FfkApi.Domain.Entities.PerfilAcesso { Nome = p.GetString()! }).ToList();
        usuario.Permissoes = permissoes.Select(p => new FfkApi.Domain.Entities.Permissao { Nome = p.GetString()! }).ToList();
    }

    public async Task<FfkApi.Domain.Entities.Usuario> CadastrarNovoUsuario(string? nomeOrganizacao = null, List<string>? perfisAcesso = null, List<string>? permissoes = null, bool ativar = false)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var request = RequestCadastrarUsuarioBuilder.Build(usuario);
        request.Organizacao = nomeOrganizacao;
        request.PerfisAcesso = perfisAcesso;
        request.Permissoes = permissoes;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await _httpHelper.DoPost("usuario", cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));

        var cpf = dadosDaResposta.RootElement.GetProperty("cpf").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(cpf));

        var telefone = dadosDaResposta.RootElement.GetProperty("telefone").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(telefone));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));

        var organizacao = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));

        var respPerfisAcesso = dadosDaResposta.RootElement.GetProperty("perfisAcesso").EnumerateArray();
        Assert.That(respPerfisAcesso, Is.Not.Null);

        var respPermissoes = dadosDaResposta.RootElement.GetProperty("permissoes").EnumerateArray();
        Assert.That(respPermissoes, Is.Not.Null);

        usuario.Id = Guid.Parse(id!);
        usuario.Cpf = cpf!;
        usuario.Telefone = telefone!;
        usuario.Status = EnumUtil.ConverterTextoParaEnum<StatusUsuario>(status!);
        usuario.Organizacao = new FfkApi.Domain.Entities.Organizacao { Nome = organizacao! };
        usuario.PerfisAcesso = respPerfisAcesso.Select(p => new FfkApi.Domain.Entities.PerfilAcesso { Nome = p.GetString()! }).ToList();
        usuario.Permissoes = respPermissoes.Select(p => new FfkApi.Domain.Entities.Permissao { Nome = p.GetString()! }).ToList();

        if (ativar)
        {
            var requestAlterar = RequestAlterarUsuarioBuilder.Build(usuario);
            requestAlterar.Status = "Ativo";
            response = await _httpHelper.DoPut("usuario", cancellationToken, requestAlterar, token);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            usuario.Status = StatusUsuario.Ativo;
        }

        return usuario;
    }

    public async Task<FfkApi.Domain.Entities.SistemaCliente> CadastrarNovoSistemaCliente()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarSistemaClienteBuilder.Build();

        var response = await _httpHelper.DoPost(url: "sistemaCliente", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));

        var appId = dadosDaResposta.RootElement.GetProperty("appId").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(appId));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(request.Nome));

        var descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(descricao));
        Assert.That(descricao, Is.EqualTo(request.Descricao));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo(request.Status));

        return new FfkApi.Domain.Entities.SistemaCliente
        {
            Id = Guid.Parse(id!),
            AppId = Guid.Parse(appId!),
            Nome = nome!,
            Descricao = descricao!,
            Senha = request.Senha!,
            Status = EnumUtil.ConverterTextoParaEnum<StatusSistemaCliente>(status!)
        };
    }

    public async Task<FfkApi.Domain.Entities.Organizacao> CadastrarNovaOrganizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarOrganizacaoBuilder.Build();

        var response = await _httpHelper.DoPost(url: "organizacao", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(request.Nome));

        var descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(descricao));
        Assert.That(descricao, Is.EqualTo(request.Descricao));

        return new FfkApi.Domain.Entities.Organizacao
        {
            Id = Guid.Parse(id!),
            Nome = nome!,
            Descricao = descricao!
        };
    }

    public async Task<FfkApi.Domain.Entities.Anexo> CadastrarNovoAnexo()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var anexo = AnexoBuilder.Build();

        var request = RequestCadastrarAnexoBuilder.Build(anexo);

        var response = await _httpHelper.DoPostCadastrarAnexo(url: "anexo", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(request.Nome));

        var descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(descricao));
        Assert.That(descricao, Is.EqualTo(request.Descricao));

        var nomeArquivo = dadosDaResposta.RootElement.GetProperty("nomeArquivo").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nomeArquivo));
        Assert.That(nomeArquivo, Is.EqualTo(request.Arquivo!.FileName));

        var tamanhoBytes = dadosDaResposta.RootElement.GetProperty("tamanhoBytes").GetInt64();
        Assert.That(tamanhoBytes, Is.Not.Null);
        Assert.That(tamanhoBytes, Is.EqualTo(request.Arquivo.Length));

        anexo.Id = Guid.Parse(id!);
        anexo.TamanhoBytes = tamanhoBytes;
        return anexo;
    }

    public async Task<FfkApi.Domain.Entities.Feed> CadastrarNovoFeed(
        int quantAnexos = 0,
        long tamanhoMaximoArquivo = 1024,
        FfkApi.Domain.Entities.Organizacao? organizacao = null)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        organizacao ??= _usuarioAdministrador.Organizacao;

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = organizacao?.Nome;
        request.VisibilidadeEquipes = [];
        request.VisibilidadeUsuarios = [];

        var response = await _httpHelper.DoPost(url: "feed", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(request.Nome));

        var descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(descricao));
        Assert.That(descricao, Is.EqualTo(request.Descricao));

        var palavrasChave = dadosDaResposta.RootElement.GetProperty("palavrasChave").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(palavrasChave));
        Assert.That(palavrasChave, Is.EqualTo(request.PalavrasChave));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo(request.Status));

        var visibilidadeUsuarios = dadosDaResposta.RootElement.GetProperty("visibilidadeUsuarios").EnumerateArray();
        Assert.That(visibilidadeUsuarios, Is.Not.Null);
        Assert.That(visibilidadeUsuarios.ToListString(), Is.EquivalentTo(request.VisibilidadeUsuarios!));

        var visibilidadeEquipes = dadosDaResposta.RootElement.GetProperty("visibilidadeEquipes").EnumerateArray();
        Assert.That(visibilidadeEquipes, Is.Not.Null);
        Assert.That(visibilidadeEquipes.ToListString(), Is.EquivalentTo(request.VisibilidadeEquipes!));

        var expiraEm = dadosDaResposta.RootElement.GetProperty("expiraEm").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(expiraEm));
        Assert.That(expiraEm, Is.EqualTo(request.ExpiraEm));

        var organizacaoResposta = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(organizacaoResposta, Is.EqualTo(organizacao!.Nome));

        var novoFeed = new FfkApi.Domain.Entities.Feed
        {
            Id = Guid.Parse(id!),
            Nome = nome!,
            Descricao = descricao!,
            PalavrasChave = palavrasChave!,
            Status = EnumUtil.ConverterTextoParaEnum<StatusFeed>(status!),
            VisibilidadeUsuarios = [],
            VisibilidadeEquipes = [],
            ExpiraEm = DateOnly.ParseExact(expiraEm!, "dd/MM/yyyy"),
            IdOrganizacao = organizacao.Id,
            Organizacao = organizacao
        };

        for (int i = 0; i < quantAnexos; i++)
        {
            var anexo = AnexoBuilder.Build();
            anexo.TamanhoBytes = (long)(tamanhoMaximoArquivo * 0.01);

            var requestAnexoFeed = RequestAdicionarAnexoFeedBuilder.Build(anexo, novoFeed.Id.ToString());

            response = await _httpHelper.DoPostCadastrarAnexo(url: "feed/anexo", request: requestAnexoFeed, token: token, cancellationToken: cancellationToken);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

            id = dadosDaResposta.RootElement.GetProperty("id").GetString();
            Assert.That(!string.IsNullOrWhiteSpace(id));

            nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
            Assert.That(!string.IsNullOrWhiteSpace(nome));
            Assert.That(nome, Is.EqualTo(requestAnexoFeed.Nome));

            descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
            Assert.That(!string.IsNullOrWhiteSpace(descricao));
            Assert.That(descricao, Is.EqualTo(requestAnexoFeed.Descricao));

            var nomeArquivo = dadosDaResposta.RootElement.GetProperty("nomeArquivo").GetString();
            Assert.That(!string.IsNullOrWhiteSpace(nomeArquivo));
            Assert.That(nomeArquivo, Is.EqualTo(requestAnexoFeed.Arquivo!.FileName));

            var tamanhoBytes = dadosDaResposta.RootElement.GetProperty("tamanhoBytes").GetInt64();
            Assert.That(tamanhoBytes, Is.Not.Null);
            Assert.That(tamanhoBytes, Is.EqualTo(requestAnexoFeed.Arquivo.Length));

            anexo.Id = Guid.Parse(id!);

            novoFeed.Anexos.Add(anexo);
        }

        return novoFeed;
    }

    public async Task<FfkApi.Domain.Entities.Equipe> CadastrarNovaEquipe(
        List<FfkApi.Domain.Entities.Usuario> usuariosEquipe,
        FfkApi.Domain.Entities.Organizacao? organizacao = null)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarEquipeBuilder.Build();
        request.Organizacao = organizacao?.Nome;
        request.Membros = [];
        foreach (var usuario in usuariosEquipe)
        {
            var lider = request.Membros.Count == 0;
            request.Membros.Add(RequestMembroEquipeBuilder.Build(usuario, lider));
        }

        var response = await _httpHelper.DoPost(url: "equipe", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(nome, Is.EqualTo(request.Nome));

        var descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
        Assert.That(descricao, Is.EqualTo(request.Descricao));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(status, Is.EqualTo(request.Status));

        var organizacaoResposta = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(organizacaoResposta, Is.EqualTo(organizacao == null ? _usuarioAdministrador.Organizacao.Nome : organizacao.Nome));

        var requestMembrosByEmail = request.Membros!
            .Where(m => m.Email != null)
            .ToDictionary(m => m.Email!);
        var membros = dadosDaResposta.RootElement.GetProperty("membros").EnumerateArray();
        Assert.That(membros.Count, Is.EqualTo(request.Membros!.Count));
        var responseMembrosByEmail = membros
            .ToDictionary(
                c => c.GetProperty("email").GetString()!,
                c => c
            );
        Assert.That(responseMembrosByEmail.Keys, Is.EquivalentTo(requestMembrosByEmail.Keys));

        foreach (var email in responseMembrosByEmail.Keys)
        {
            var responseMembro = responseMembrosByEmail[email];
            var requestMembro = requestMembrosByEmail[email];

            Assert.That(!string.IsNullOrWhiteSpace(responseMembro.GetProperty("id").GetString()));
            Assert.That(!string.IsNullOrWhiteSpace(responseMembro.GetProperty("idUsuario").GetString()));
            Assert.That(!string.IsNullOrWhiteSpace(responseMembro.GetProperty("nome").GetString()));
            Assert.That(responseMembro.GetProperty("lider").GetBoolean(), Is.EqualTo(requestMembro.Lider!.Value), $"Lider diferente para o email {email}");
        }

        var novaEquipe = new FfkApi.Domain.Entities.Equipe
        {
            Id = Guid.Parse(id!),
            Nome = nome!,
            Descricao = descricao!,
            Status = EnumUtil.ConverterTextoParaEnum<StatusEquipe>(status!),
            Organizacao = organizacao ?? _usuarioAdministrador.Organizacao!
        };

        foreach (var email in responseMembrosByEmail.Keys)
        {
            var responseMembro = responseMembrosByEmail[email];

            var usuario = usuariosEquipe.FirstOrDefault(m => m.Email == email);

            var membroEquipe = new MembroEquipe
            {
                Id = Guid.Parse(responseMembro.GetProperty("id").GetString()!),
                IdUsuario = usuario!.Id,
                Usuario = usuario,
                IdEquipe = novaEquipe.Id,
                Equipe = novaEquipe,
                Lider = responseMembro.GetProperty("lider").GetBoolean()
            };

            novaEquipe.Membros.Add(membroEquipe);
        }

        return novaEquipe;
    }

    public async Task<FfkApi.Domain.Entities.Indisponibilidade> CadastrarNovaIndisponibilidade(
        FfkApi.Domain.Entities.Usuario usuario,
        DateOnly dataInicial,
        DateOnly dataFinal)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();
        request.Usuario = usuario.Email;
        request.DataInicial = dataInicial.ToString("dd/MM/yyyy");
        request.DataFinal = dataFinal.ToString("dd/MM/yyyy");

        var response = await _httpHelper.DoPost(url: "indisponibilidade", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));

        var descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
        Assert.That(descricao, Is.EqualTo(request.Descricao));

        var dataInicialResposta = dadosDaResposta.RootElement.GetProperty("dataInicial").GetString();
        Assert.That(dataInicialResposta, Is.EqualTo(request.DataInicial));

        var dataFinalResposta = dadosDaResposta.RootElement.GetProperty("dataFinal").GetString();
        Assert.That(dataFinalResposta, Is.EqualTo(request.DataFinal));

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("usuario").GetProperty("id").GetString()));
        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("usuario").GetProperty("nome").GetString()));

        var email = dadosDaResposta.RootElement.GetProperty("usuario").GetProperty("email").GetString();
        Assert.That(email, Is.EqualTo(request.Usuario));

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("usuario").GetProperty("organizacao").GetString()));

        return new FfkApi.Domain.Entities.Indisponibilidade
        {
            Id = Guid.Parse(id!),
            Descricao = descricao!,
            DataInicial = dataInicial,
            DataFinal = dataFinal,
            IdUsuario = usuario.Id,
            Usuario = usuario
        };
    }
}
