# CLAUDE.md — FfkApi.Infrastructure

## Responsabilidade

Camada de infraestrutura. Implementa todas as interfaces definidas em `FfkApi.Domain`: repositórios EF Core, tokens JWT, criptografia de senha, envio de e-mail (SendGrid), mensageria (RabbitMQ), armazenamento de arquivos e serviços de suporte.

## Estrutura

```
FfkApi.Infrastructure/
├── DataAccess/
│   ├── FfkApiDbContext.cs          # DbContext com 16 DbSets
│   ├── Repositories/               # 13 repositórios + UnitOfWork
│   └── DataConfigurations/         # 23 configurações EF Core (Fluent API)
├── Security/
│   ├── Credenciais/                # GeradorSenhaValida, GeradorToken
│   ├── Criptografia/               # EncriptadorBCrypt
│   └── Tokens/                     # Geradores e validadores JWT
├── Services/
│   ├── Acesso/                     # AcessoService
│   ├── Arquivos/                   # ArmazenadorDeArquivoLocalService
│   ├── Auditoria/                  # AuditoriaSegurancaService
│   ├── Email/                      # EnviarEmailSendGridService / EnviarEmailFakeService
│   ├── Fila/                       # FilaService
│   ├── Mensageria/                 # RabbitMqPublicarService
│   └── UsuarioLogado/              # UsuarioLogadoService
├── Migrations/                     # Migrations EF Core
└── Extension/
    ├── DependencyInjectionExtension.cs
    ├── ConfigurationExtension.cs
    └── MigrationExtension.cs
```

## DbContext (`FfkApiDbContext`)

16 `DbSet<>`: `Usuarios`, `Equipes`, `Organizacoes`, `Feeds`, `Anexos`, `Indisponibilidades`, `SistemasClientes`, `PerfisAcesso`, `Permissoes`, `RefreshTokens`, `TokensAtivacao`, `TokensNovaSenha`, `AuditoriaSeguranca`, `Checklists`, `Filas`, `FilaItens`

`OnModelCreating` auto-aplica por reflection todas as classes que herdam de `EntityBaseConfiguration<>` — não é necessário registrar manualmente ao adicionar uma nova configuração.

## Configurações de Entidade (EF Fluent API)

**`EntityBaseConfiguration<T>`** (base):
- `Id` → UUID com default `gen_random_uuid()`
- `DataCriacaoUtc` → `timestamp with time zone`

Todas as 23 configurações de entidade herdam dessa base. Padrões comuns:
- Índices únicos com escopo por organização (ex: nome da equipe único por organização)
- Relacionamentos many-to-many com join tables explícitas (`UsuariosPerfisAcesso`, `UsuariosPermissoes`)
- `OnDelete(DeleteBehavior.Cascade)` para filhos dependentes; `Restrict` para referências de organização

## Repositórios

Todos os repositórios injetam `FfkApiDbContext` e a `AsyncRetryPolicy` do Polly. Cada método async envolve a operação com a política:

```csharp
return await _pollyPolicy.ExecuteAsync(() =>
    _dbContext.Usuarios.FirstOrDefaultAsync(u => u.Email == email, token));
```

**Polly (configurável via `appsettings`):**
- Tentativas: 3 (padrão)
- Backoff: exponencial (2s base)
- Timeout: 10s por tentativa

**`UnitOfWork`** — `CommitAsync` também usa retry policy.

## Segurança

### JWT

**`GeradorTokenUsuario` / `ValidadorTokenUsuario`:**
- Algoritmo: HS256 com `SymmetricSecurityKey`
- Claim: único `ClaimTypes.Sid` com o ID do usuário (sem roles no token — permissões são carregadas do banco)
- Validade configurável em minutos

**`GeradorTokenSistemaCliente` / `ValidadorTokenSistemaCliente`:**
- Mesmo padrão, chave de assinatura separada

**`GeradorRefreshToken`:**
- 32 bytes criptográficos aleatórios → Base64 URL-safe (sem `+`, `/`, `=`)

**`GeradorTokenAtivacao` / `GeradorTokenNovaSenha`:**
- Mesmo mecanismo do RefreshToken

### Criptografia de Senha

`EncriptadorBCrypt` — BCrypt custo 13 (enhanced), `EnhancedHashPassword` / `EnhancedVerify`.

## Serviços

### E-mail (`IEnviarEmailService`)

| Ambiente | Implementação | Comportamento |
|---|---|---|
| `DEBUG` | `EnviarEmailFakeService` | No-op (não envia nada) |
| `RELEASE` | `EnviarEmailSendGridService` | HTTP POST para SendGrid API |

`EnviarEmailSendGridService` usa `HttpClient` estático, suporta templates com variáveis de personalização, e aplica Polly (retry + timeout) nas chamadas HTTP. `EstaDisponivel()` consulta endpoint de quota da SendGrid.

### Mensageria (`IPublicarMensagemService`)

`RabbitMqPublicarService` (Singleton):
- Conexão lazy: conecta na primeira chamada
- Fila durável, mensagens persistentes, prefetch=1
- Serialização JSON
- Degradação graciosa: se indisponível, loga aviso e retorna sem lançar exceção

### Armazenamento de Arquivos (`IArmazenadorDeArquivoService`)

`ArmazenadorDeArquivoLocalService` — salva em pasta `anexos/` com nome `{Guid}{extensao}`. `EstaDisponivel()` sempre retorna `true`.

### Acesso (`IAcessoService`)

`AcessoService`:
- `AplicarPerfisAoUsuarioAsync` — atribui perfis e consolida permissões no usuário
- `UsuarioTemPermissaoAsync` — verifica permissão (admin tem bypass automático)
- `UsuarioAdministradorAsync` — checa perfil administrador
- `AtualizarUsuariosDoPerfilAsync` — propaga alterações de permissões do perfil para usuários

### Usuário Logado (`IUsuarioLogadoService`)

`UsuarioLogadoService` — extrai ID do `ClaimTypes.Sid` do JWT e carrega o `Usuario` do banco com eager load de perfis e permissões.
- `PegarUsuarioLogadoAtivo` — exige status Ativo ou Ausente; lança exceção se não encontrado
- `PegarUsuarioDoTokenEnviado` — retorna `null` se sem token (para endpoints opcionalmente autenticados)

### Auditoria (`IAuditoriaSegurancaService`)

`AuditoriaSegurancaService` — persiste `AuditoriaSeguranca` com repositório + `CommitAsync`.

### Fila (`IFilaService`)

`FilaService`:
- `PegarProximoItemDaFilaAsync` — item de menor posição na fila
- `CriarFilaParaEquipesSemFilaAsync` — cria `Fila` para equipes que ainda não possuem

## Injeção de Dependência

`DependencyInjectionExtension.AddInfrastructure()`:

1. **Auto-registro de repositórios** por reflection (namespace `FfkApi.Infrastructure.DataAccess.Repositories`) como `Scoped`
2. **DbContext** com Npgsql — string de conexão de `ConnectionStrings:Default`
3. **Serviços de domínio** registrados como `Scoped` (Acesso, Arquivos, Auditoria, Email, Fila, UsuarioLogado)
4. **RabbitMQ** como `Singleton`
5. **Polly** — `AsyncRetryPolicy` como `Singleton`, configurada via `Configuracoes:Resiliencia:Polly`
6. **JWT** — geradores e validadores registrados com suas configurações (`JwtUsuario`, `JwtSistemaCliente`, `RefreshToken`, `TokenAtivacao`, `TokenNovaSenha`)
7. **Configurações estáticas** — inicializa `ConfiguracaoFront.UrlFront`

> Adicionar um repositório no namespace correto é suficiente para registro automático — não é necessário editar `DependencyInjectionExtension`.

## Migrations

```bash
# Adicionar migration
dotnet ef migrations add <Nome> --project src/Backend/FfkApi.Infrastructure --startup-project src/Backend/FfkApi.API

# Aplicar manualmente
dotnet ef database update --project src/Backend/FfkApi.Infrastructure --startup-project src/Backend/FfkApi.API
```

Em runtime, `MigrationExtension.ApplyMigrations()` aplica automaticamente as migrations pendentes no startup (exceto em modo de teste em memória).
