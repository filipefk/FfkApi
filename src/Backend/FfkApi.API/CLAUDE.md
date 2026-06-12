# CLAUDE.md — FfkApi.API

## Responsabilidade

Camada de apresentação da solução. Recebe requisições HTTP, delega para use cases da camada Application e devolve respostas. Controllers devem ser finos — sem lógica de negócio.

## Estrutura

```
FfkApi.API/
├── Controllers/          # 9 controllers de domínio + 2 de debug
├── Attributes/           # Filtros de autorização (substituem [Authorize])
├── Middleware/           # Pipeline de segurança
├── Filters/              # Tratamento global de exceções
├── Security/             # Configuração de autenticação JWT e rate limiting
├── Token/                # Extração do JWT do header (ITokenRecebido)
├── BackgroundServices/   # Jobs Hangfire
├── Documentation/        # Configuração do Swagger
├── HealthCheck/          # Health checks e mapeamento de endpoints
├── Converters/           # StringConverter (JSON)
├── DebugUtil/            # Utilitários só disponíveis em Development
└── logs/                 # Saída do Serilog
```

## Controllers

Todos os controllers seguem o padrão:

| Método | Rota | Operação |
|---|---|---|
| POST | `/{controller}` | Cadastrar |
| PUT | `/{controller}` | Alterar |
| GET | `/{controller}/{id}` | Pegar |
| GET | `/{controller}/pesquisar` | Pesquisar (OData) |
| DELETE | `/{controller}/{id}` | Excluir |
| POST | `/{controller}/lote` | CadastrarEmLote (apenas Feed e Organizacao) |

- Rotas em **lowercase** (`LowercaseUrls = true`)
- Operações em lote (`/lote`) requerem `[SistemaClienteAutenticado]`
- Upload de arquivo usa `[FromForm]` (Anexo, Feed com anexo)
- Pesquisar recebe `RequestODataQueryOptions` para filtros/ordenação

**Controllers disponíveis:** Login, Token, Usuario, Equipe, Organizacao, Feed, Anexo, Indisponibilidade, SistemaCliente
**Só em Development:** CredenciaisController, MensageriaController

## Pipeline de Middleware (ordem de registro)

1. `CabecalhosSegurancaMiddleware` — adiciona HSTS, X-Content-Type-Options, X-Frame-Options, X-XSS-Protection
2. `EventosSegurancaMiddleware` — loga eventos 401/403/429 com corpo da requisição, IP e máscara de senha
3. `TokenAplicacaoMiddleware` — valida header `x-app-token`; rejeita todas as requisições sem token válido, exceto `/jobs` e `/health`
4. Authorization (ASP.NET Core built-in)
5. RateLimiter — limitação global e por IP (configurável em `appsettings`)

## Atributos de Autorização

Substituem o `[Authorize]` padrão do ASP.NET Core:

| Atributo | Uso | Validação |
|---|---|---|
| `[UsuarioAutenticado]` | Endpoints de usuário autenticado | JWT via `IValidadorTokenUsuario`; verifica status ativo |
| `[UsuarioAutenticado("NOME_PERMISSAO")]` | Endpoints com permissão específica | Idem + checa `IAcessoService` |
| `[UsuarioAdministrador]` | Endpoints restritos a admin | JWT + `IAcessoService.UsuarioAdministradorAsync()` |
| `[SistemaClienteAutenticado]` | Endpoints M2M | JWT via `IValidadorTokenSistemaCliente` |

Token expirado → 401. Sem permissão → 403.

## Tratamento de Exceções

`ExceptionFilter` (global) captura todas as exceções:
- Subclasses de `ExceptionBase` → HTTP status mapeado + `ResponseErro` estruturado
- Exceções desconhecidas → 500 + mensagem genérica
- Tudo logado via Serilog

## Background Services (Hangfire)

| Job | Cron | Descrição |
|---|---|---|
| `EnviarEmailAtivacaoUsuarioJob` | `0 * * * * *` (todo minuto) | Envia e-mails de ativação pendentes |
| `EnviarEmailNovaSenhaUsuarioJob` | `30 * * * * *` (todo minuto, no :30) | Envia e-mails de nova senha pendentes |
| `LimpezaAuditoriaSegurancaJob` | `0 1 * * *` (diário 01h) | Remove registros de auditoria antigos |
| `LimpezaArquivosLogJob` | `0 2 * * *` (diário 02h) | Remove arquivos de log antigos |

Dashboard disponível em `/jobs` (sem proteção por enquanto — TODO existente no código).
Storage: PostgreSQL via `Hangfire.PostgreSql`.

## Health Checks

| Endpoint | Tag | Descrição |
|---|---|---|
| `/health` | todas | Requer `x-app-token` válido |
| `/health/ready` | `pronto` | PostgreSQL + Armazenador de Arquivos |
| `/health/live` | — | Sempre saudável (liveness probe) |
| `/health/detail` | todas | Status detalhado: DB, E-mail, Mensageria, Arquivos |

## Swagger

- Disponível apenas em `Development`
- Dois esquemas de segurança: **Bearer** (JWT) e **AppToken** (`x-app-token`)
- Documentação gerada de `FfkApi.API.xml` e `FfkApi.Communication.xml`
- Exemplos auto-registrados via `AddAssemblySwaggerExamples()` em `Documentation/Examples/`

## Configuração (`appsettings`)

```
Configuracoes:
  TokensAplicacao:
    TokensPermitidos[]        # Tokens válidos para x-app-token
    TokenHealthCheck          # Token exclusivo para /health
  JwtUsuario:
    ChaveAssinatura
    TempoValidadeMinutos
  JwtSistemaCliente:
    ChaveAssinatura
    TempoValidadeMinutos
  GlobalRateLimit             # Limite global de requisições
  IpRateLimit                 # Limite por IP
  Cors:
    OrigensPermitidasDev[]
    OrigensPermitidasProd[]
  Front:
    Url                       # URL do frontend (usada nos tokens de e-mail)
```

## Injeção de Dependência

Chamadas no `Program.cs`:
```csharp
builder.Services.AddApplication(configuration);   // FfkApi.Application
builder.Services.AddInfrastructure(configuration); // FfkApi.Infrastructure
```

Use cases são resolvidos por interface via DI automático por namespace (ver Application e Infrastructure).
