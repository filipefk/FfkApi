# CLAUDE.md — TestUtil

## Propósito

Biblioteca de utilitários **compartilhada** entre todos os projetos de teste. Fornece builders de entidades, mocks de repositórios, builders de requests, serviços fake, extensões Bogus e helpers HTTP. Não contém testes — apenas infraestrutura de apoio.

## Dependências

- **Bogus 35.6.3** — geração de dados fake realistas
- **Moq 4.20.72** — mocks de interfaces
- **AutoMapper 14.0.0** — via `MapperBuilder`
- Referencia: `FfkApi.Application`, `FfkApi.Infrastructure`, `FfkApi.Communication`

## Estrutura e Conteúdo

### `AutoMapper/`
- `MapperBuilder.cs` — constrói `IMapper` configurado com o perfil `AutoMapping` da aplicação

### `Entities/` — Entity Builders
Geram entidades de domínio com dados fake via Bogus. Padrão:

```csharp
var usuario = UsuarioBuilder.Build();
var lista   = UsuarioBuilder.BuildList(5);
```

Builders disponíveis: `AnexoBuilder`, `EquipeBuilder`, `FeedBuilder`, `IndisponibilidadeBuilder`, `MembroEquipeBuilder`, `OrganizacaoBuilder`, `PerfilAcessoBuilder`, `PermissaoBuilder`, `SistemaClienteBuilder`, `TokenAtivacaoBuilder`, `TokenNovaSenhaBuilder`, `RefreshTokenBuilder`, `UsuarioBuilder`.

### `Repositories/` — Repository Mocks
Encapsulam `Mock<IRepository>` com métodos `Setup*` prontos:

```csharp
var repoBuilder = new AnexoRepositoryBuilder();
repoBuilder.SetupPegarAnexoPorIdReturnsAnexo(anexo, CancellationToken.None);
IAnexoRepository repo = repoBuilder.Build();
```

Builders disponíveis para todos os repositórios do domínio + `UnitOfWorkBuilder`.

### `Requests/` — Request Builders
Geram `Request*` válidos com Bogus. Uso:

```csharp
var request = RequestAlterarAnexoBuilder.Build();
```

Há um builder para cada operação de cada entidade (40+ builders).

### `Services/` — Serviços Fake/Mock
| Classe | Substitui |
|---|---|
| `ArmazenadorDeArquivoEmMemoriaService` | `IArmazenadorDeArquivoService` (ConcurrentDictionary) |
| `EnviarEmailServiceBuilder` | `IEnviarEmailService` (no-op Mock) |
| `ArmazenadorDeAnexoServiceBuilder` | `IArmazenadorDeAnexoService` |
| `UsuarioLogadoServiceBuilder` | `IUsuarioLogadoService` |

### `Tokens/`
- `GeradorTokenUsuarioBuilder` — constrói `IGeradorTokenUsuario` com chave de teste fixa

### `HttpUtil/`
- `HttpHelper` — cliente HTTP reutilizável (GET/POST/PUT/DELETE + multipart)
- `HttpResponseUtil` — extrai dados tipados de `HttpResponseMessage`

### `Extension/`
- `BogusExtensions` — extensões do `Faker<T>`:
  - `.Proverbio()`, `.Animal()`, `.Adjetivo()`, `.Objeto()`, `.VerboGerundio()` — dados de arquivos JSON
  - `.Senha()` — senha válida conforme regras da aplicação
  - `.CpfSoNumeros()` — CPF brasileiro sem máscara
  - `.CelularBrasileiro()` — telefone celular brasileiro

### `Criptografia/`
- `CpfUtil` — geração e validação de CPF
- `EncriptadorSenhaBuilder` — mock de `IEncriptadorSenha`

### `InlineData/`
- `ListaStringNulaVaziaInlineData` — fonte de dados `[TestCaseSource]` para listas nulas/vazias
- `StatusAoAlterarStatusUsuarioInlineData` — casos de status de usuário

### `Json/`
Arquivos de dados usados por `BogusExtensions`: `Adjetivo.json`, `Animal.json`, `Indisponibilidade.json`, `Objeto.json`, `Proverbio.json`, `VerboGerundio.json`.

## Como Adicionar um Novo Builder

Ao criar CRUD para uma nova entidade, adicione em `TestUtil`:

1. `Entities/<Entidade>Builder.cs` — gera a entidade com Bogus
2. `Repositories/<Entidade>RepositoryBuilder.cs` — mock de `I<Entidade>Repository`
3. `Requests/Request<Operacao><Entidade>Builder.cs` — um builder por operação

Siga o padrão dos builders existentes (ex: `AnexoBuilder`, `AnexoRepositoryBuilder`).

## Convenções

- Builders estáticos usam `Build()` e `BuildList(int)` como métodos de fábrica
- Builders de repositório são instâncias com métodos `Setup*` e `Build()` final
- Nunca colocar lógica de teste aqui — apenas infraestrutura reutilizável
- Dados sensíveis fixos (token de app, chave JWT de teste) ficam apenas aqui
