# CLAUDE.md — FfkApi

## Visão Geral

API REST monolítica em .NET 8 com C#, organizada em camadas seguindo DDD (Domain-Driven Design). Multi-tenant por organização, com autenticação JWT, filas via RabbitMQ, jobs com Hangfire e banco PostgreSQL.

## Estrutura da Solução

```
FfkApi/
├── src/
│   ├── Backend/
│   │   ├── FfkApi.API           # Entrypoint: controllers, middleware, segurança
│   │   ├── FfkApi.Application   # Use cases, validadores (FluentValidation), AutoMapper
│   │   ├── FfkApi.Domain        # Entidades, enums, interfaces de repositório e serviços
│   │   ├── FfkApi.Infrastructure# EF Core, repositórios, JWT, BCrypt, SendGrid, RabbitMQ
│   │   └── FfkApi.Initialization# Seeds e inicialização do banco
│   └── Shared/
│       ├── FfkApi.Communication # DTOs: Requests e Responses
│       └── FfkApi.Exceptions    # Exceções customizadas e mensagens de erro (pt-BR)
├── tests/
│   ├── UseCases.Test            # Testes unitários de use cases (NUnit)
│   ├── Validators.Test          # Testes unitários de validadores (NUnit)
│   ├── WebApi.Test              # Testes de integração
│   └── E2E.Test                 # Testes de aceitação/E2E
└── gerador/
    └── GeradorDeCodigo          # Console app para scaffold de CRUD
```

Cada projeto possui um CLAUDE.md com detalhes específicos. Leia-o **somente se a tarefa envolver diretamente aquele projeto**:
- [FfkApi.API](src/Backend/FfkApi.API/CLAUDE.md)
- [FfkApi.Application](src/Backend/FfkApi.Application/CLAUDE.md)
- [FfkApi.Domain](src/Backend/FfkApi.Domain/CLAUDE.md)
- [FfkApi.Infrastructure](src/Backend/FfkApi.Infrastructure/CLAUDE.md)
- [FfkApi.Initialization](src/Backend/FfkApi.Initialization/CLAUDE.md)
- [FfkApi.Communication](src/Shared/FfkApi.Communication/CLAUDE.md)
- [FfkApi.Exceptions](src/Shared/FfkApi.Exceptions/CLAUDE.md)
- [UseCases.Test](tests/UseCases.Test/CLAUDE.md)
- [Validators.Test](tests/Validators.Test/CLAUDE.md)
- [WebApi.Test](tests/WebApi.Test/CLAUDE.md)
- [E2E.Test](tests/E2E.Test/CLAUDE.md)
- [GeradorDeCodigo](gerador/GeradorDeCodigo/CLAUDE.md)

## Camadas e Responsabilidades

| Projeto | Camada DDD | Regra |
|---|---|---|
| `FfkApi.Domain` | Domain | Sem dependências externas; só lógica de negócio pura |
| `FfkApi.Application` | Application | Orquestra use cases; depende apenas de Domain e Shared |
| `FfkApi.Infrastructure` | Infrastructure | EF Core, tokens JWT, criptografia, e-mail, fila |
| `FfkApi.API` | Presentation | Controllers finos; delega tudo para use cases |
| `FfkApi.Communication` | Shared | DTOs sem lógica; usados por API e Application |
| `FfkApi.Exceptions` | Shared | Exceções e mensagens em pt-BR via resource files |

## Padrões Arquiteturais

- **Use Case Pattern** — cada operação de negócio tem sua própria classe em `Application/UseCases/<Entidade>/<Operação>/`
- **Repository Pattern + Unit of Work** — toda persistência via `IRepository` + `IUnitOfWork`
- **Fluent Validation** — validação de entrada exclusivamente via `AbstractValidator<TRequest>` em `Application/Validators/`
- **AutoMapper** — mapeamentos de DTO↔entidade configurados em `Application/Services/AutoMapper/`
- **Reflection-based DI** — repositórios e serviços registrados automaticamente por namespace em `DependencyInjectionExtension`
- **EF Fluent API** — configurações de entidade via `EntityBaseConfiguration<T>` carregadas por reflection
- **Polly** — políticas de retry/timeout/backoff para operações de banco de dados

## Domínio Principal

**Agregados:**
- `Organizacao` → raiz multi-tenant
- `Usuario` → `PerfilAcesso`, `Permissao`, `Indisponibilidade`, `RefreshToken`, tokens de ativação/nova senha
- `Equipe` → `MembroEquipe`, `Fila`
- `Feed` → `Anexo` (lista), visibilidade por usuários/equipes
- `Checklist` → `ChecklistItem`, `ChecklistRespostaPossivel`, `ChecklistPreenchido`

**Enums principais:** `StatusUsuario`, `StatusEquipe`, `StatusFeed`, `StatusSistemaCliente`, `TipoChecklistItem`, `TipoPessoa`, `Periodicidade`

## Stack Tecnológica

| Tecnologia | Versão | Uso |
|---|---|---|
| .NET | 8.0 | Runtime |
| ASP.NET Core | 8.0 | Web API |
| Entity Framework Core | 8.0.19 | ORM |
| Npgsql (PostgreSQL) | 8.0.11 | Banco de dados |
| FluentValidation | 12.0.0 | Validação de entrada |
| AutoMapper | 14.0.0 | Mapeamento DTO↔entidade |
| Swashbuckle (Swagger) | 8.1.4 | Documentação da API |
| Microsoft.AspNetCore.OData | 8.3.1 | Filtros de consulta |
| Hangfire | 1.8.21 | Jobs em background |
| RabbitMQ.Client | 6.8.1 | Mensageria assíncrona |
| Polly | 8.6.3 | Resiliência (retry, timeout) |
| BCrypt.Net-Next | 4.0.3 | Hash de senhas |
| System.IdentityModel.Tokens.Jwt | 8.14.0 | Autenticação JWT |
| Serilog | 8.0.3 | Logging em arquivo |
| NUnit | 4.4.0 | Testes |

## Convenções de Nomenclatura

- **Português para tudo** — entidades, use cases, variáveis, nomes de arquivo e métodos são em pt-BR
- **Prefixo por tipo** — `Request*`, `Response*` para DTOs; `I*UseCase` para interfaces de use case; `I*Repository` para repositórios
- **Sufixo por operação** — `Cadastrar`, `Alterar`, `Excluir`, `Pegar`, `Pesquisar` identificam a ação do use case
- **Lote** — operações em massa usam o sufixo `EmLote` (ex: `CadastrarEmLote`)

## Como Adicionar um Novo CRUD

Use o **GeradorDeCodigo** (`gerador/GeradorDeCodigo`) para scaffold. Ele cria um esqueleto completo com boa parte do código, mas **não garante build imediato** — ajustes manuais são sempre necessários. Arquivos gerados:
- Entidade em Domain + interface de repositório
- Configuração EF Core em Infrastructure + implementação do repositório
- DTOs (Request/Response) em Communication
- Use cases e validadores em Application
- Controller em API
- Esqueletos de testes (unitários, integração, E2E)

Após gerar, procure por `// TODO : [GERADOR DE CÓDIGO] =>` em todos os arquivos gerados — esses comentários marcam os pontos que exigem ajuste manual para o caso específico. Só depois aplique a migration:

```
dotnet ef migrations add <Nome> --project FfkApi.Infrastructure --startup-project FfkApi.API
```

## Autenticação e Segurança

- Dois tipos de token JWT: **Usuário** (sessão interativa) e **SistemaCliente** (integração M2M)
- **RefreshToken** — validade configurável (padrão: 7 dias)
- **TokenAtivacao** — ativação de conta (padrão: 12 horas)
- **TokenNovaSenha** — reset de senha (padrão: 1 hora)
- Rate limiting global e por IP configurados em `appsettings`
- `AuditoriaSeguranca` registra eventos sensíveis de acesso

## Testes

```
# Unitários
dotnet test tests/UseCases.Test
dotnet test tests/Validators.Test

# Integração
dotnet test tests/WebApi.Test

# E2E
dotnet test tests/E2E.Test
```

Configurações de teste via `appsettings.Test.json`.

## Variáveis de Configuração Relevantes (`appsettings`)

| Seção | Descrição |
|---|---|
| `ConnectionStrings:Default` | String de conexão PostgreSQL |
| `Jwt:*` | Chaves e validades dos tokens |
| `RateLimit:*` | Limites de requisição global e por IP |
| `Cors:Origins` | Origens permitidas (padrão: `localhost:5173`) |
| `ConfiguracaoArquivoAnexo:TamanhoMaximoMb` | Tamanho máximo de anexos (padrão: 10 MB) |
| `SendGrid:*` | Configuração de envio de e-mail |
| `RabbitMq:*` | Configuração do broker de mensagens |
| `Polly:*` | Retry, timeout e backoff para banco |
| `Limpeza:*` | Retenção de auditoria (15 dias) e logs (10 dias) |

## Docker

```bash
docker build -t ffkapi .
docker run -p 8080:8080 -v ./logs:/app/logs ffkapi
```

Build multi-stage: SDK 8.0 para compilação, AspNet 8.0 runtime para execução.
