# CLAUDE.md — FfkApi.Initialization

## Responsabilidade

Seed de dados iniciais no banco. Cria a organização padrão, permissões, perfis de acesso e usuários de referência na primeira execução — e é idempotente (não duplica dados se já existirem).

## Estrutura

```
FfkApi.Initialization/
├── DataInitialization/
│   └── DadosIniciais.cs          # Orquestrador do seed
└── Extension/
    └── DataInitializationExtension.cs  # Extension method ApplyDataInitialization()
```

## Quando é Executado

Chamado em `Program.cs` **após** as migrations, somente quando **não** está em modo de teste em memória:

```csharp
if (!builder.Configuration.RodandoTesteEmMemoria())
{
    app.Services.ApplyMigrations();
    app.Services.ApplyDataInitialization();  // ← aqui
}
```

## Ordem de Seed

### 1. Organizacao
Cria a organização `"FfkApi"` se não existir nenhuma. Inclui configurações de e-mail (remetente e template IDs do SendGrid para ativação e nova senha).

### 2. Permissoes
Cria 7 permissões se não existir nenhuma:

| Nome |
|---|
| Cadastro de Usuários |
| Cadastro de Organizações |
| Cadastro de Indisponibilidades |
| Cadastro de Feeds |
| Cadastro de Equipes |
| Cadastro de perfil de permissão de usuários |
| Cadastro de Formulários/Check Lists |

### 3. PerfisAcesso
Cria 3 perfis se não existir nenhum:

| Perfil | Permissões atribuídas |
|---|---|
| Administrador | Nenhuma no seed (acesso total via lógica de negócio) |
| Cadastrador de usuários e equipes | Cadastro de Usuários + Cadastro de Equipes |
| Gerente | Cadastro de Equipes |

### 4. Usuarios
Cria 3 usuários se não existir nenhum:

| Nome | E-mail | Perfil | Permissões | Senha |
|---|---|---|---|---|
| Admin | admin@ffkapi.com | Administrador | — | `Senha.InicialFfkApiAdmin1` |
| SemPerfilNemPermissao | SemPerfilNemPermissao@provedor.com | — | — | `Senha.valida1` |
| PermissaoCadastroUsuarios | PermissaoCadastroUsuarios@provedor.com | — | Cadastro de Usuários | `Senha.valida1` |

> O usuário Admin é exportado na propriedade estática `DadosIniciais.UsuarioAdministrador` (com senha em texto plano) para uso nos projetos de teste.

## Idempotência

Cada bloco de seed verifica com `.Any()` antes de inserir. Executar múltiplas vezes não gera dados duplicados.

## Dependências do Projeto

- `FfkApi.Domain` — entidades
- `FfkApi.Infrastructure` — `FfkApiDbContext`
- `TestUtil` — `EncriptadorSenhaBuilder` (para hash das senhas iniciais)
