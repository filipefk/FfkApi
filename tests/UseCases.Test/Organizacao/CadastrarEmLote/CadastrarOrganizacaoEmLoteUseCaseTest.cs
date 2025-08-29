
using FfkApi.Application.UseCases.Organizacao.CadastrarEmLote;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Enums;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;

namespace UnidadeUseCases.Test.Organizacao.CadastrarEmLote;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CadastrarOrganizacaoEmLoteUseCaseTest
{
    [Test]
    public async Task Sucesso_Total()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarOrganizacaoEmLoteBuilder.Build();

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Cadastrados, Is.Not.Null);
        Assert.That(response.Cadastrados.Count, Is.EqualTo(request.Organizacoes.Count));

        var cadastradosPorNome = response.Cadastrados
            .ToDictionary(
                c => c.Nome,
                c => c
            );

        foreach (var orgRequest in request.Organizacoes)
        {
            Assert.That(cadastradosPorNome.ContainsKey(orgRequest.Nome!), $"Organização '{orgRequest.Nome}' não encontrada nos cadastrados.");

            var cadastrado = cadastradosPorNome[orgRequest.Nome!];

            Assert.That(cadastrado.Descricao, Is.EqualTo(orgRequest.Descricao), $"Descrição divergente para '{orgRequest.Nome}'.");
        }

        Assert.That(response.Erros, Is.Not.Null);
        Assert.That(response.Erros, Is.Empty);

        Assert.That(response.TotalCadastrados, Is.Not.Null);
        Assert.That(response.TotalCadastrados, Is.EqualTo(request.Organizacoes.Count));

        Assert.That(response.TotalErros, Is.Not.Null);
        Assert.That(response.TotalErros, Is.EqualTo(0));

        Assert.That(response.Resultado, Is.EqualTo(StatusCadastroLote.SucessoTotal.ToString()));
    }

    [Test]
    public async Task Sucesso_Parcial()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarOrganizacaoEmLoteBuilder.Build();
        var organizacao = request.Organizacoes.FirstOrDefault();
        organizacao!.Nome = null;

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Cadastrados, Is.Not.Null);
        Assert.That(response.Cadastrados.Count, Is.EqualTo(request.Organizacoes.Count - 1));

        var cadastradosPorNome = response.Cadastrados
            .ToDictionary(
                c => c.Nome,
                c => c
            );

        var organizacoesSemNomeNull = request.Organizacoes
            .Where(o => o.Nome != null)
            .ToList();

        foreach (var orgRequest in organizacoesSemNomeNull)
        {
            Assert.That(cadastradosPorNome.ContainsKey(orgRequest.Nome!), $"Organização '{orgRequest.Nome}' não encontrada nos cadastrados.");

            var cadastrado = cadastradosPorNome[orgRequest.Nome!];
            var descricaoCadastrado = cadastrado.Descricao;

            Assert.That(descricaoCadastrado, Is.EqualTo(orgRequest.Descricao), $"Descrição divergente para '{orgRequest.Nome}'.");
        }

        Assert.That(response.Erros, Is.Not.Null);
        Assert.That(response.Erros.Count, Is.EqualTo(1));

        Assert.That(response.Erros.FirstOrDefault()!.MensagensDeErro[0], Is.EqualTo(ResourceMessagesException.NOME_VAZIO));

        Assert.That(response.TotalCadastrados, Is.Not.Null);
        Assert.That(response.TotalCadastrados, Is.EqualTo(request.Organizacoes.Count - 1));

        Assert.That(response.TotalErros, Is.Not.Null);
        Assert.That(response.TotalErros, Is.EqualTo(1));

        Assert.That(response.Resultado, Is.EqualTo(StatusCadastroLote.SucessoParcial.ToString()));
    }

    [Test]
    public async Task Falha_Total()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarOrganizacaoEmLoteBuilder.Build(2);
        var requestNomeNull = request.Organizacoes.FirstOrDefault();
        requestNomeNull!.Nome = null;

        var requestDescricaoNull = request.Organizacoes.Skip(1).FirstOrDefault();
        requestDescricaoNull!.Descricao = null;

        var organizacaoNomeJaExiste = OrganizacaoBuilder.Build();
        request.Organizacoes.Add(RequestCadastrarOrganizacaoBuilder.Build(organizacaoNomeJaExiste));

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacao: organizacaoNomeJaExiste);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Cadastrados, Is.Not.Null);
        Assert.That(response.Cadastrados, Is.Empty);

        Assert.That(response.Erros, Is.Not.Null);
        Assert.That(response.Erros.Count, Is.EqualTo(request.Organizacoes.Count));

        var responseErroQueRequestTemNomeNull = response.Erros.FirstOrDefault(e => string.IsNullOrEmpty(e.Request.Nome));

        var responseRequestQueTemNomeNull = responseErroQueRequestTemNomeNull!.Request;
        Assert.That(responseRequestQueTemNomeNull.Descricao, Is.EqualTo(requestNomeNull!.Descricao));

        var mensagensDeErroNomeNull = responseErroQueRequestTemNomeNull.MensagensDeErro;
        Assert.That(mensagensDeErroNomeNull.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErroNomeNull.FirstOrDefault()!, Is.EqualTo(ResourceMessagesException.NOME_VAZIO));

        var responseErroQueRequestTemDescricaoNull = response.Erros.FirstOrDefault(e => string.IsNullOrEmpty(e.Request.Descricao));

        var responseRequestQueTemDescricaoNull = responseErroQueRequestTemDescricaoNull!.Request;
        Assert.That(responseRequestQueTemDescricaoNull.Nome, Is.EqualTo(requestDescricaoNull!.Nome));

        var mensagensDeErroDescricaoNull = responseErroQueRequestTemDescricaoNull.MensagensDeErro;
        Assert.That(mensagensDeErroDescricaoNull.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErroDescricaoNull.FirstOrDefault()!, Is.EqualTo(ResourceMessagesException.DESCRICAO_VAZIA));

        var requestNomeJaExiste = request.Organizacoes.FirstOrDefault(o => o.Nome == organizacaoNomeJaExiste.Nome);
        var responseErroQueRequestTemNomeJaExiste = response.Erros.FirstOrDefault(e => e.Request.Nome == organizacaoNomeJaExiste.Nome);

        var responseRequestQueTemNomeJaExiste = responseErroQueRequestTemNomeJaExiste!.Request;
        Assert.That(responseRequestQueTemNomeJaExiste.Nome, Is.EqualTo(requestNomeJaExiste!.Nome));

        var mensagensDeErroNomeJaExiste = responseErroQueRequestTemNomeJaExiste.MensagensDeErro;
        Assert.That(mensagensDeErroNomeJaExiste.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErroNomeJaExiste.FirstOrDefault()!, Is.EqualTo(ResourceMessagesException.NOME_DE_ORGANIZACAO_JA_EXISTE));

        Assert.That(response.TotalCadastrados, Is.Not.Null);
        Assert.That(response.TotalCadastrados, Is.EqualTo(0));

        Assert.That(response.TotalErros, Is.Not.Null);
        Assert.That(response.TotalErros, Is.EqualTo(request.Organizacoes.Count));

        Assert.That(response.Resultado, Is.EqualTo(StatusCadastroLote.Falha.ToString()));
    }

    [Test]
    public async Task Erro_Nome_Organizacao_Repetido_Na_Lista()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarOrganizacaoEmLoteBuilder.Build(2);
        request.Organizacoes[1].Nome = request.Organizacoes[0].Nome;

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Cadastrados, Is.Not.Null);
        Assert.That(response.Cadastrados.Count, Is.EqualTo(1));

        Assert.That(response.Erros, Is.Not.Null);
        Assert.That(response.Erros.Count, Is.EqualTo(1));

        Assert.That(response.Erros.FirstOrDefault()!.MensagensDeErro[0], Is.EqualTo(ResourceMessagesException.NOME_DE_ORGANIZACAO_REPETIDO));

        Assert.That(response.TotalCadastrados, Is.Not.Null);
        Assert.That(response.TotalCadastrados, Is.EqualTo(1));

        Assert.That(response.TotalErros, Is.Not.Null);
        Assert.That(response.TotalErros, Is.EqualTo(1));

        Assert.That(response.Resultado, Is.EqualTo(StatusCadastroLote.SucessoParcial.ToString()));
    }

    [Test]
    public async Task Erro_Lista_Organizacoes_Vazia()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = new RequestCadastrarOrganizacaoEmLote
        {
            Organizacoes = []
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Cadastrados, Is.Not.Null);
        Assert.That(response.Cadastrados, Is.Empty);

        Assert.That(response.Erros, Is.Not.Null);
        Assert.That(response.Erros.Count, Is.EqualTo(1));
        Assert.That(response.Erros.FirstOrDefault()!.MensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(response.Erros.FirstOrDefault()!.MensagensDeErro, Contains.Item(ResourceMessagesException.LISTA_DE_ORGANIZACAO_VAZIA));
        Assert.That(response.Erros.FirstOrDefault()!.Request, Is.Null);
    }


    private static CadastrarOrganizacaoEmLoteUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Organizacao? organizacao = null)
    {
        var organizacaoRepository = new OrganizacaoRepositoryBuilder();

        if (organizacao != null)
        {
            organizacaoRepository.SetupExisteOrganizacaoComNomeReturnsTrue(organizacao.Nome, cancellationToken);
        }

        return new CadastrarOrganizacaoEmLoteUseCase(
            organizacaoRepository.Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build());

    }
}
