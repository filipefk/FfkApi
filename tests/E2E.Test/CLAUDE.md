# CLAUDE.md — E2E.Test

## Propósito

Testes de **aceitação (E2E)** que executam fluxos HTTP completos contra a aplicação real iniciada via `WebApplicationFactory`. Diferente de `WebApi.Test`, aqui o servidor é configurado com serviços fake de e-mail e mensageria, mas o banco de dados é o real (configurado em `appsettings.Test.json`).

## Stack

- **NUnit 4.4.0** — framework de teste
- **Microsoft.AspNetCore.Mvc.Testing** — `WebApplicationFactory<Program>`
- **HttpClient** — via `HttpHelper` para chamadas reais à API

## Infraestrutura

| Arquivo | Responsabilidade |
|---|---|
| `E2EClassFixture.cs` | Base class; sobe `WebApplicationFactory<Program>`, cria usuários pré-configurados, inicializa `HttpHelper` e `CadastroHelper`, define `appToken` padrão |
| `NUnitAssemblySettings.cs` | Paralelização: `ParallelScope.Fixtures`, `LevelOfParallelism(1)` |

### Usuários pré-criados em `E2EClassFixture`

| Campo | Valor |
|---|---|
| `_usuarioAdministrador` | Usuário com perfil administrador |
| `_usuarioSemPerfil` | Usuário sem perfil de acesso |
| `_usuarioComPermissao` | Usuário com permissão específica |
| `appToken` | `"testes-2blHt60aerveQI2UaASZssPntfaB8alE6uJRnQdvbkk"` |

### Variável de ambiente

O ambiente E2E define `RODANDO_TESTE_ACEITACAO=true` — usada pela aplicação para ajustar comportamento (ex: pular envio de e-mail real).

## Estrutura de Pastas

```
E2E.Test/
├── Helpers/
│   ├── HttpHelper.cs        # GET, POST, PUT, DELETE com Bearer + x-app-token
│   └── CadastroHelper.cs    # Setup de dados: login, cadastros auxiliares
└── <Entidade>/
    └── <Operacao>/
        └── E2E<Entidade><Operacao>Test.cs
```

## Como Escrever um Teste

```csharp
[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2EAlterarAnexoTest : E2EClassFixture
{
    [Test]
    public async Task Sucesso_Administrador()
    {
        var token  = await _cadastroHelper.PegarTokensLogin(_usuarioAdministrador);
        var anexo  = await _cadastroHelper.CadastrarNovoAnexo(token);
        var request = RequestAlterarAnexoBuilder.Build();
        request.Id  = anexo.Id.ToString();

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

## `HttpHelper` — Métodos Disponíveis

| Método | Assinatura resumida |
|---|---|
| `Get` | `Get(url, token?)` |
| `Post` | `Post(url, body, token?)` |
| `Put` | `Put(url, body, token?)` |
| `Delete` | `Delete(url, token?)` |
| `PostMultipart` | `PostMultipart(url, formData, token?)` — upload de arquivo |

O header `x-app-token` com o valor `appToken` é enviado automaticamente em todas as requisições.

## Casos Obrigatórios

| Cenário | Status HTTP esperado |
|---|---|
| `Sucesso_Administrador` | `200 OK` / `201 Created` / `204 NoContent` |
| `Sucesso_Usuario_Sem_Permissao` | `200 OK` (se a operação permitir) |
| `Erro_Sem_Token` | `401 Unauthorized` |
| `Erro_Sem_Permissao` | `403 Forbidden` |
| `Erro_Nao_Encontrado` | `404 NotFound` |

## Convenções

- Prefixo `E2E` no nome da classe: `E2EAlterarAnexoTest`
- Herdar de `E2EClassFixture` sempre
- Atributos obrigatórios: `[TestFixture]` e `[Parallelizable(ParallelScope.All)]`
- Criar dados via `CadastroHelper` — nunca inserir diretamente no banco
- Assertions de status HTTP com `HttpStatusCode.*`

## Executar

```
dotnet test tests/E2E.Test
```

> Requer banco PostgreSQL acessível conforme `appsettings.Test.json`.
