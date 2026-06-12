# CLAUDE.md — UseCases.Test

## Propósito

Testes **unitários** da camada de aplicação (`FfkApi.Application`). Cada use case é testado isoladamente com mocks de repositórios e serviços — sem banco de dados, sem I/O real.

## Stack

- **NUnit 4.4.0** — framework de teste
- **Moq** — mocks de repositórios e serviços (via `TestUtil`)
- **Bogus** — dados fake nos builders (via `TestUtil`)
- **AutoMapper** — via `MapperBuilder` (via `TestUtil`)

## Estrutura de Pastas

```
UseCases.Test/
└── <Entidade>/           # Anexo, Equipe, Feed, Usuario, etc.
    └── <Operacao>/       # Alterar, Cadastrar, Excluir, Pegar, Pesquisar
        └── <Entidade><Operacao>UseCaseTest.cs
```

Espelha a estrutura de `FfkApi.Application/UseCases/`.

## Como Escrever um Teste

Cada teste instancia o use case injetando mocks construídos pelos builders de `TestUtil`:

```csharp
[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarAnexoUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var request   = RequestAlterarAnexoBuilder.Build();
        var anexo     = AnexoBuilder.Build();
        var repoBuilder = new AnexoRepositoryBuilder();
        repoBuilder.SetupPegarAnexoPorIdReturnsAnexo(anexo, CancellationToken.None);

        var useCase = new AlterarAnexoUseCase(
            repoBuilder.Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build());

        var response = await useCase.Executar(request);

        Assert.That(response, Is.Not.Null);
    }

    [Test]
    public void Erro_Nenhuma_Alteracao()
    {
        // ...
        Assert.ThrowsAsync<ErrorOnValidationException>(() => useCase.Executar(request));
    }
}
```

### Casos obrigatórios por use case

| Cenário | O que verificar |
|---|---|
| `Sucesso` | Retorno correto / chamada de `Commit()` |
| `Erro_*_Nao_Encontrado` | `ErrorOnValidationException` lançada |
| `Erro_Nenhuma_Alteracao` | Idem (para use cases de alteração) |
| Campos inválidos | Idem (para casos não cobertos pelos Validators) |

## Builders Disponíveis (TestUtil)

- **Entity Builders** (`TestUtil/Entities/`) — `AnexoBuilder`, `UsuarioBuilder`, etc.
- **Repository Builders** (`TestUtil/Repositories/`) — `AnexoRepositoryBuilder`, etc.
- **Request Builders** (`TestUtil/Requests/`) — `RequestAlterarAnexoBuilder`, etc.
- **Infra** — `UnitOfWorkBuilder`, `MapperBuilder`, `UsuarioLogadoServiceBuilder`

## Convenções

- Nome do teste: `Sucesso`, `Erro_<Motivo>` (snake_case com maiúsculas por segmento)
- Atributos obrigatórios: `[TestFixture]` e `[Parallelizable(ParallelScope.All)]`
- Assertions com NUnit fluent API: `Assert.That(..., Is.EqualTo(...))`
- Nunca acessar banco real, sistema de arquivos ou rede

## Executar

```
dotnet test tests/UseCases.Test
```
