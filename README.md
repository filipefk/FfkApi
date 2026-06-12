# FfkApi — API Base em .NET 8

FfkApi é uma **API REST base** construída em .NET 8 com C#, pronta para ser usada como ponto de partida em projetos reais. Ela fornece toda a infraestrutura comum que praticamente todo sistema precisa — autenticação, multi-tenancy, permissões, jobs, mensageria, testes — para que você foque nas regras de negócio do seu domínio.

> Renomeie, adapte o domínio e saia codificando. Toda a fundação já está pronta.

---

## Por que usar esta base?

Iniciar um projeto do zero em .NET exige decisões e implementações que consomem tempo antes de qualquer linha de negócio ser escrita: como estruturar as camadas, como autenticar, como validar, como testar, como fazer scaffold de CRUDs. O FfkApi já tomou essas decisões e implementou tudo de forma coerente e extensível.

- **Arquitetura DDD em camadas** — separação clara entre Domain, Application, Infrastructure e API
- **Multi-tenant** — isolamento de dados por organização desde a fundação
- **Autenticação completa** — JWT para usuários e para sistemas cliente (M2M), refresh token, ativação de conta e reset de senha
- **Autorização granular** — perfis de acesso + permissões individuais, com bypass automático para administradores
- **Scaffold de CRUD** — gerador de código que cria tudo de uma vez: entidade, repositório, use cases, DTOs, controller, validadores e testes
- **Testes em quatro camadas** — unitários de use cases, unitários de validadores, integração em memória e E2E contra banco real
- **Infraestrutura de produção** — rate limiting, health checks, logging (Serilog), jobs agendados (Hangfire), mensageria (RabbitMQ), e-mail (SendGrid), resiliência (Polly)

---

## Stack Tecnológica

| Tecnologia | Versão | Uso |
|---|---|---|
| .NET / ASP.NET Core | 8.0 | Runtime e Web API |
| Entity Framework Core | 8.0.19 | ORM |
| PostgreSQL (Npgsql) | 8.0.11 | Banco de dados |
| FluentValidation | 12.0.0 | Validação de entrada |
| AutoMapper | 14.0.0 | Mapeamento DTO ↔ entidade |
| Swashbuckle (Swagger) | 8.1.4 | Documentação da API |
| OData | 8.3.1 | Filtros e paginação em queries |
| Hangfire | 1.8.21 | Jobs em background |
| RabbitMQ.Client | 6.8.1 | Mensageria assíncrona |
| Polly | 8.6.3 | Retry, timeout e backoff |
| BCrypt.Net-Next | 4.0.3 | Hash de senhas |
| System.IdentityModel.Tokens.Jwt | 8.14.0 | Autenticação JWT |
| Serilog | 8.0.3 | Logging em arquivo |
| NUnit | 4.4.0 | Framework de testes |
| Bogus | 35.6.3 | Dados fake para testes |
| Moq | 4.20.72 | Mocks para testes unitários |

---

## Estrutura da Solução

```
FfkApi/
├── src/
│   ├── Backend/
│   │   ├── FfkApi.API            # Controllers, middleware, segurança, jobs, Swagger
│   │   ├── FfkApi.Application    # Use cases, validadores, AutoMapper
│   │   ├── FfkApi.Domain         # Entidades, enums, interfaces (sem dependências externas)
│   │   ├── FfkApi.Infrastructure # EF Core, repositórios, JWT, BCrypt, SendGrid, RabbitMQ
│   │   └── FfkApi.Initialization # Seeds de dados iniciais
│   └── Shared/
│       ├── FfkApi.Communication  # DTOs (Requests e Responses)
│       └── FfkApi.Exceptions     # Exceções customizadas e mensagens de erro
├── tests/
│   ├── UseCases.Test             # Testes unitários de use cases (NUnit + Moq)
│   ├── Validators.Test           # Testes unitários de validadores (NUnit)
│   ├── WebApi.Test               # Testes de integração em memória (WebApplicationFactory)
│   ├── E2E.Test                  # Testes E2E contra banco real
│   └── TestUtil                  # Builders, mocks e helpers compartilhados entre testes
└── gerador/
    └── GeradorDeCodigo           # Console app de scaffold de CRUD
```

---

## Recursos e Funcionalidades

### Autenticação e Segurança

- **JWT de Usuário** — sessões interativas com refresh token (padrão: 7 dias de validade)
- **JWT de SistemaCliente** — integração M2M com chave de assinatura separada
- **Ativação de conta** — token enviado por e-mail (padrão: 12 horas)
- **Reset de senha** — token enviado por e-mail (padrão: 1 hora)
- **Rate limiting** — global e por IP, configurável via `appsettings`
- **Auditoria de segurança** — registro de eventos sensíveis (401, 403, 429) com IP e payload
- **Middleware de segurança** — HSTS, X-Content-Type-Options, X-Frame-Options, X-XSS-Protection
- **App token** — todas as requisições exigem um header `x-app-token` válido

### Autorização

Quatro atributos substituem o `[Authorize]` padrão:

| Atributo | Uso |
|---|---|
| `[UsuarioAutenticado]` | Endpoint acessível a qualquer usuário autenticado |
| `[UsuarioAutenticado("PERMISSAO")]` | Endpoint com permissão específica |
| `[UsuarioAdministrador]` | Endpoint restrito a administradores |
| `[SistemaClienteAutenticado]` | Endpoint M2M |

### API e Controllers

Todos os controllers seguem o padrão REST consistente:

| Método | Rota | Operação |
|---|---|---|
| POST | `/{recurso}` | Cadastrar |
| PUT | `/{recurso}` | Alterar |
| GET | `/{recurso}/{id}` | Pegar por ID |
| GET | `/{recurso}/pesquisar` | Pesquisar com filtros OData |
| DELETE | `/{recurso}/{id}` | Excluir |
| POST | `/{recurso}/lote` | Cadastrar em lote (M2M) |

Controllers disponíveis na base: **Login**, **Token**, **Usuario**, **Equipe**, **Organizacao**, **Feed**, **Anexo**, **Indisponibilidade**, **SistemaCliente**.

### Jobs em Background (Hangfire)

| Job | Agendamento | Descrição |
|---|---|---|
| Envio de e-mail de ativação | Todo minuto | Processa fila de e-mails de ativação pendentes |
| Envio de e-mail de nova senha | Todo minuto (:30s) | Processa fila de e-mails de reset pendentes |
| Limpeza de auditoria | Diário 01h | Remove registros antigos (padrão: 15 dias) |
| Limpeza de logs | Diário 02h | Remove arquivos de log antigos (padrão: 10 dias) |

Dashboard do Hangfire disponível em `/jobs`.

### Health Checks

| Endpoint | Descrição |
|---|---|
| `/health` | Status geral |
| `/health/ready` | PostgreSQL + armazenamento de arquivos |
| `/health/live` | Liveness probe (sempre saudável) |
| `/health/detail` | Detalhado: banco, e-mail, mensageria, arquivos |

### Domínio Pré-configurado

O projeto já vem com um domínio funcional de referência que pode ser mantido, adaptado ou substituído:

- **Organizacao** — raiz multi-tenant
- **Usuario** — com perfis, permissões, indisponibilidades e tokens
- **Equipe** — com membros e fila de trabalho
- **Feed** — com anexos de arquivo e visibilidade por usuário/equipe
- **Checklist** — formulários configuráveis com múltipla escolha e dependências entre itens
- **SistemaCliente** — integração M2M
- **Pessoa** — contato externo vinculado à organização

### Testes

Quatro projetos de teste com infraestrutura pronta:

- **UseCases.Test** — testes unitários com mocks (Moq), builders de entidades (Bogus) e sem I/O real
- **Validators.Test** — testes unitários de regras de validação FluentValidation
- **WebApi.Test** — testes de integração com `WebApplicationFactory` e banco em memória; nenhuma dependência externa
- **E2E.Test** — testes de aceitação contra banco PostgreSQL real

```bash
dotnet test tests/UseCases.Test
dotnet test tests/Validators.Test
dotnet test tests/WebApi.Test
dotnet test tests/E2E.Test
```

### Gerador de Código

Console app interativo que gera um CRUD completo em toda a solução a partir de três inputs:

```bash
dotnet run --project gerador/GeradorDeCodigo
```

Arquivos gerados por entidade: entidade de domínio, interface de repositório, configuração EF Core, repositório, DTOs, use cases (Cadastrar, Alterar, Excluir, Pegar, Pesquisar, CadastrarEmLote), validadores, controller, migration e esqueleto completo de testes (unitários, integração e E2E). Após gerar, localize os comentários `// TODO : [GERADOR DE CÓDIGO] =>` para os ajustes específicos do seu domínio.

---

## Padrões Arquiteturais

- **Use Case Pattern** — uma classe por operação de negócio em `Application/UseCases/<Entidade>/<Operacao>/`
- **Repository Pattern + Unit of Work** — toda persistência via `IRepository` + `IUnitOfWork`
- **Fluent Validation** — validação de entrada exclusivamente via `AbstractValidator<TRequest>`
- **AutoMapper** — perfil único com mapeamentos `Request → Entidade` e `Entidade → Response`
- **Reflection-based DI** — repositórios e use cases registrados automaticamente por namespace; adicionar um novo arquivo no namespace correto é suficiente
- **EF Fluent API** — configurações de entidade via `EntityBaseConfiguration<T>` carregadas por reflection
- **Polly** — retry exponencial + timeout em todas as operações de banco

---

## Configuração

Principais seções do `appsettings.json`:

| Seção | Descrição |
|---|---|
| `ConnectionStrings:Default` | String de conexão PostgreSQL |
| `Configuracoes:JwtUsuario:*` | Chave e validade do JWT de usuário |
| `Configuracoes:JwtSistemaCliente:*` | Chave e validade do JWT M2M |
| `Configuracoes:TokensAplicacao:*` | Tokens válidos para o header `x-app-token` |
| `Configuracoes:GlobalRateLimit` | Limite global de requisições |
| `Configuracoes:IpRateLimit` | Limite por IP |
| `Configuracoes:Cors:*` | Origens permitidas (padrão: `localhost:5173`) |
| `SendGrid:*` | Configuração de envio de e-mail |
| `RabbitMq:*` | Configuração do broker de mensagens |
| `Configuracoes:Resiliencia:Polly:*` | Tentativas, backoff e timeout do banco |
| `Limpeza:*` | Retenção de auditoria e logs |

---

## Docker

```bash
docker build -t ffkapi .
docker run -p 8080:8080 -v ./logs:/app/logs ffkapi
```

Build multi-stage: SDK 8.0 para compilação, ASP.NET 8.0 runtime para execução.

---

## Migrations

```bash
# Adicionar migration
dotnet ef migrations add <Nome> --project src/Backend/FfkApi.Infrastructure --startup-project src/Backend/FfkApi.API

# Aplicar manualmente
dotnet ef database update --project src/Backend/FfkApi.Infrastructure --startup-project src/Backend/FfkApi.API
```

As migrations pendentes são aplicadas automaticamente no startup da aplicação.

---

## Arquivos CLAUDE.md — Contexto para LLMs

Esta solução possui arquivos `CLAUDE.md` na raiz e em cada projeto, com documentação detalhada da arquitetura, padrões, convenções e responsabilidades de cada camada. Esses arquivos foram escritos para o [Claude Code](https://claude.ai/code) (Anthropic), mas podem ser renomeados para funcionar com qualquer LLM que suporte contexto de projeto:

| LLM / Ferramenta | Arquivo de contexto |
|---|---|
| Claude Code (Anthropic) | `CLAUDE.md` (padrão, já configurado) |
| GitHub Copilot | `.github/copilot-instructions.md` |
| Cursor | `.cursorrules` |
| Aider | `.aider.conf.yml` ou `CONVENTION.md` |
| Codeium / Windsurf | `.windsurfrules` |
| Qualquer outra | Consulte a documentação da ferramenta |

Basta copiar ou renomear o conteúdo dos `CLAUDE.md` para o formato exigido pela LLM de sua preferência. O conteúdo é agnóstico de ferramenta — descreve a arquitetura e os padrões do projeto, não comandos específicos do Claude.

---

## Convenções de Nomenclatura

O projeto adota **português do Brasil** para tudo: entidades, use cases, variáveis, nomes de arquivo e métodos. As convenções principais são:

- `Request*` / `Response*` — prefixos obrigatórios para DTOs
- `I*UseCase` — interfaces de use case
- `I*Repository` — interfaces de repositório
- Sufixos de operação: `Cadastrar`, `Alterar`, `Excluir`, `Pegar`, `Pesquisar`
- Operações em lote: sufixo `EmLote` (ex: `CadastrarFeedEmLote`)
