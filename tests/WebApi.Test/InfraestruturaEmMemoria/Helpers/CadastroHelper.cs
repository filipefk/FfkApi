using FfkApi.Domain.Entities;
using FfkApi.Domain.Enums;
using NUnit.Framework;
using System.Net;
using TestUtil.Extension;
using TestUtil.HttpUtil;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Integracao.Test.InfraestruturaEmMemoria.Helpers;

public class CadastroHelper
{
    private readonly HttpHelper _httpHelper;

    public CadastroHelper(
        HttpHelper httpHelper)
    {
        _httpHelper = httpHelper;
    }

    public async Task<FfkApi.Domain.Entities.SistemaCliente> CadastrarNovoSistemaCliente(Guid idUsuario)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(idUsuario);

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
            Status = EnumUtil.ConverterTextoParaEnum<StatusSistemaCliente>(status!)
        };
    }

    public async Task<FfkApi.Domain.Entities.Equipe> CadastrarNovaEquipe(
        FfkApi.Domain.Entities.Usuario usuarioLogin,
        List<FfkApi.Domain.Entities.Usuario> usuariosEquipe,
        FfkApi.Domain.Entities.Organizacao? organizacao = null)
    {
        organizacao ??= usuarioLogin.Organizacao;
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioLogin.Id);

        var request = RequestCadastrarEquipeBuilder.Build();
        request.Organizacao = organizacao.Nome;
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
        Assert.That(organizacaoResposta, Is.EqualTo(organizacao!.Nome));

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
            Organizacao = organizacao
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

    public async Task<FfkApi.Domain.Entities.Feed> CadastrarNovoFeed(
        FfkApi.Domain.Entities.Usuario usuarioLogin,
        FfkApi.Domain.Entities.Organizacao? organizacao = null)
    {
        organizacao ??= usuarioLogin.Organizacao;

        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioLogin.Id);

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = organizacao.Nome;
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
        Assert.That(!string.IsNullOrWhiteSpace(organizacaoResposta));
        Assert.That(organizacaoResposta, Is.EqualTo(request.Organizacao));

        return new FfkApi.Domain.Entities.Feed
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
    }
}
