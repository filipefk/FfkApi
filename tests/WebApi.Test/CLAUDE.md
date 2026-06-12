# CLAUDE.md — WebApi.Test

## Propósito

Testes de **integração** que sobem a aplicação completa em memória (`WebApplicationFactory`) com banco `InMemory` e serviços fake substituídos. Validam o comportamento HTTP de ponta a ponta dentro do processo, sem dependências externas.

## Stack

- **NUnit 4.4.0** — framework de teste
- **Microsoft.AspNetCore.Mvc.Testing** — `WebApplicationFactory<Program>`
- **Microsoft.EntityFrameworkCore.InMemory** — banco de dados em memória
- **Moq** / serviços fake — substituição de e-mail e armazenamento de arquivos

## Infraestrutura (`InfraestruturaEmMemoria/`)

| Arquivo | Responsabilidade |
|---|---|
| `CustomWebApplicationFactory.cs` | Substitui banco real por InMemory; troca `IArmazenadorDeArquivoService` e `IEnviarEmailService` por fakes; popula `DadosIniciais` |
| `TestServerSingleton.cs` | Instância singleton do servidor — compartilhada entre fixtures para evitar múltiplos boots |
| `FfkApiClassFixture.cs` | Base class de todos os testes; expõe `HttpHelper`, `CadastroHelper` e `EntidadesCriadas()` |

### Entidades pré-criadas (`EntidadesCriadas()`)

O dicionário retornado por `_factory.EntidadesCriadas()` contém as entidades criadas em `DadosIniciais`:

| Chave | Tipo |
|---|---|
| `"OrganizacaoFfkApi"` | `Organizacao` |
| `"PerfilAcessoAdministrador"` | `PerfilAcesso` |
| `"UsuarioAdministrador"` | `Usuario` |
| `"UsuarioSemPerfil"` | `Usuario` |
| `"UsuarioComPermissao"` | `Usuario` |
| _(demais conforme `DadosIniciais`)_ | variado |

## Estrutura de Pastas

```
WebApi.Test/
├── InfraestruturaEmMemoria/
│   ├── CustomWebApplicationFactory.cs
│   ├── FfkApiClassFixture.cs
│   └── TestServerSingleton.cs
└── <Entidade>/
    └── <Operacao>/
        └── <Entidade><Operacao>Test.cs
```

## Como Escrever um Teste

```csharp
[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarAnexoTest : FfkApiClassFixture
{
    private readonly Usuario _usuarioAdministrador;

    public AlterarAnexoTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioAdministrador = (Usuario)entidades["UsuarioAdministrador"];
    }

    [Test]
    public async Task Sucesso_Administrador()
    {
        var token   = await _cadastroHelper.PegarTokensLogin(_usuarioAdministrador);
        var request = RequestAlterarAnexoBuilder.Build();

        var response = await _httpHelper.Put($"/api/anexo/{request.Id}", request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var response = await _httpHelper.Put("/api/anexo/qualquer-id", new { });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}
```

### Casos obrigatórios

| Cenário | Status HTTP esperado |
|---|---|
| `Sucesso_Administrador` | `200 OK` / `201 Created` / `204 NoContent` |
| `Erro_Sem_Token` | `401 Unauthorized` |
| `Erro_Sem_Permissao` | `403 Forbidden` |
| `Erro_Nao_Encontrado` | `404 NotFound` |
| `Erro_Validacao` | `400 BadRequest` |

## Serviços Substituídos

- `IArmazenadorDeArquivoService` → `ArmazenadorDeArquivoEmMemoriaService` (ConcurrentDictionary)
- `IEnviarEmailService` → `EnviarEmailFakeService` (no-op)

## Convenções

- Herdar de `FfkApiClassFixture` (nunca usar `WebApplicationFactory` diretamente)
- Atributos obrigatórios: `[TestFixture]` e `[Parallelizable(ParallelScope.All)]`
- Não modificar `TestServerSingleton` — é compartilhado entre todos os testes
- Assertions de status HTTP com `HttpStatusCode.*`; de body com `HttpResponseUtil`

## Executar

```
dotnet test tests/WebApi.Test
```
