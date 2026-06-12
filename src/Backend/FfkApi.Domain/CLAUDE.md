# CLAUDE.md — FfkApi.Domain

## Responsabilidade

Camada de domínio. Contém entidades, enums, interfaces de repositório, interfaces de serviços de domínio e objetos de configuração. **Sem dependências externas** — nenhuma referência a EF Core, HTTP ou qualquer framework de infraestrutura.

## Estrutura

```
FfkApi.Domain/
├── Entities/             # Entidades de domínio
├── Repositories/         # Interfaces de repositório (IUnitOfWork + 13 repositórios)
├── Services/             # Interfaces de serviços de domínio
├── Enums/                # Enumerações do domínio
├── Configurations/       # Objetos de configuração estáticos
└── Extensions/           # EnumerableExtension, EnumUtil
```

## EntityBase

Todas as entidades herdam de `EntityBase`:

```csharp
public class EntityBase
{
    public Guid Id { get; init; }             // gerado pelo banco (gen_random_uuid())
    public DateTime DataCriacaoUtc { get; init; }  // setado automaticamente
}
```

## Entidades

### Organizacao
Raiz multi-tenant. Todo dado sensível é isolado por organização.

| Propriedade | Tipo |
|---|---|
| Nome, Descricao | string |
| RemetenteEmail, RemetenteNome | string |
| ModeloEmailAtivacao, ModeloEmailNovaSenha | string (template ID SendGrid) |

### Usuario
| Propriedade | Tipo |
|---|---|
| Nome, Email, Senha, Cpf, Telefone | string |
| Status | `StatusUsuario` |
| IdOrganizacao | Guid (FK) |
| PerfisAcesso | `ICollection<PerfilAcesso>` |
| Permissoes | `ICollection<Permissao>` |
| Fila | `Fila?` |

Métodos: `TemPerfilAdministrador()`, `TemPerfil(nome)`, `TemPermissao(nome)`, `StatusPermitidosAoAlterarStatusDeOutroUsuario()`, `StatusPermitidosAoAlterarSeuProprioStatus()`

### Equipe
| Propriedade | Tipo |
|---|---|
| Nome, Descricao | string |
| Status | `StatusEquipe` |
| IdOrganizacao | Guid (FK) |
| Membros | `ICollection<MembroEquipe>` |
| Fila | `Fila?` |

### MembroEquipe
FK para `Equipe` e `Usuario`. `Lider` (bool).

### PerfilAcesso / Permissao
Controle de acesso baseado em perfis e permissões. `PerfilAcesso` contém `ICollection<Permissao>`.

### Feed
| Propriedade | Tipo |
|---|---|
| Nome, Descricao, PalavrasChave | string |
| Status | `StatusFeed` |
| ExpiraEm | `DateOnly?` |
| IdOrganizacao | Guid (FK) |
| Anexos | `IList<Anexo>` |
| VisibilidadeUsuarios | `IList<Usuario>` |
| VisibilidadeEquipes | `IList<Equipe>` |

### Anexo
Arquivo físico associado a um Feed. Propriedades: `Nome`, `Descricao`, `NomeArquivo`, `NomeArquivoArmazenamento`, `Extensao`, `TamanhoBytes`, `MimeType`, `Texto`.

### Indisponibilidade
Período de ausência de um usuário. `DataInicial` / `DataFinal` (`DateOnly`). FK para `Usuario`.

### Checklist / ChecklistItem
Formulários configuráveis com suporte a itens simples e múltipla escolha, dependências entre itens e gatilhos por resposta.

`ChecklistItem` factory methods: `ItemSimples(descricao, ordem, respostaInconformidade)`
`ChecklistRespostaPossivel` factory method: `Resposta(descricao, ordem, geraInconformidade)`

### ChecklistPreenchido / ChecklistPreenchidoItem
Instância preenchida de um `Checklist`. Armazena snapshot do nome/descrição no momento do preenchimento.

### Fila / FilaItem
Fila de trabalho associada a um `Usuario` ou `Equipe`. `FilaItem` tem `Posicao` (long).

### Tokens
| Entidade | Uso | Campos extras |
|---|---|---|
| `RefreshToken` | Renovação de sessão | `Valor` |
| `TokenAtivacao` | Ativação de conta | `Valor`, `BaseExpiracaoUtc`, `EmailEnviado`, `UltimaTentativaEnvioEmail`, `ErroEnvioEmail` |
| `TokenNovaSenha` | Reset de senha | `Valor`, `EmailEnviado`, `UltimaTentativaEnvioEmail`, `ErroEnvioEmail` |

### SistemaCliente
Integração M2M. `AppId` (Guid), `Nome`, `Descricao`, `Senha`, `Status` (`StatusSistemaCliente`).

### AuditoriaSeguranca
Log de eventos de segurança. `Evento`, `Usuario`, `EnderecoIp`, `Caminho`, `Metodo`, `Detalhes`.

### Pessoa
Contato externo. `TipoPessoa`, `Nome`, `Telefone`, `Email`, `CpfCnpj`. FK para `Organizacao`.

## Interfaces de Repositório

Todas as operações recebem `CancellationToken` como último parâmetro.

**`IUnitOfWork`**
- `CommitAsync(token)`

**Padrão comum nos repositórios:**
- `Adicionar(entidade, token)`
- `Excluir(id, token)`
- `Pegar{Entidade}PorId(id, token)` — com overload `(id, idOrganizacao, token)` para multi-tenant
- `Existe{Entidade}ComId(id, token)`
- `QuantidadeTotal(token)` — com overload por organização
- `AsQueryable()` — expõe `IQueryable` para OData (com overload por organização)

Repositórios disponíveis: `IUsuarioRepository`, `IEquipeRepository`, `IOrganizacaoRepository`, `IFeedRepository`, `IAnexoRepository`, `IIndisponibilidadeRepository`, `ISistemaClienteRepository`, `IPerfilAcessoRepository`, `IPermissaoRepository`, `IRefreshTokenRepository`, `ITokenAtivacaoRepository`, `ITokenNovaSenhaRepository`, `IAuditoriaSegurancaRepository`

## Interfaces de Serviços de Domínio

| Interface | Responsabilidade |
|---|---|
| `IAcessoService` | Aplicar perfis, verificar permissões, checar admin |
| `IAuditoriaSegurancaService` | Registrar eventos de segurança |
| `IArmazenadorDeArquivoService` | Salvar, obter, remover arquivos; `EstaDisponivel()` |
| `IEnviarEmailService` | Enviar e-mail com template; `EstaDisponivel()` |
| `IFilaService` | Próximo item da fila; criar filas para equipes sem fila |
| `IPublicarMensagemService` | Publicar mensagem em fila (`PublicarAsync<T>`); `EstaDisponivel()` |
| `IUsuarioLogadoService` | Obter usuário do JWT atual (`PegarUsuarioLogadoAtivo`, `PegarUsuarioDoTokenEnviado`) |
| `IEncriptadorSenha` | Hash e verificação de senha (BCrypt) |
| `IGeradorTokenUsuario` / `IValidadorTokenUsuario` | Gerar/validar JWT de usuário |
| `IGeradorTokenSistemaCliente` / `IValidadorTokenSistemaCliente` | Gerar/validar JWT M2M |
| `IGeradorRefreshToken` | Gerar refresh token |
| `IGeradorTokenAtivacao` / `IGeradorTokenNovaSenha` | Gerar tokens de e-mail |

## Enums

| Enum | Valores |
|---|---|
| `StatusUsuario` | Indefinido, Inativo, Ativo, Ausente, Suspenso, Excluido |
| `StatusEquipe` | Indefinido, Inativa, Ativa |
| `StatusFeed` | Indefinido, Rascunho, Publicado, Arquivado |
| `StatusSistemaCliente` | Indefinido, Inativo, Ativo |
| `StatusCadastroLote` | Indefinido, SucessoTotal, SucessoParcial, Falha |
| `TipoPessoa` | Indefinida, Fisica, Juridica |
| `TipoChecklistItem` | Indefinido, Simples, MultiplaEscolha |
| `Periodicidade` | Indefinida, Diaria, Semanal, Quinzenal, Mensal, Bimestral, Trimestral, Semestral, Anual |

**`EnumUtil`** — helpers estáticos: `TextoEnumValido<T>()`, `ConverterTextoParaEnum<T>()`, `PegarListaNomesEnum<T>()`, `PegarListaEnum<T>()`, `PegarNomesEnumSeparadosPorVirgula<T>()`

## Configurações Estáticas

Inicializadas no startup via `Inicializar()` — valores lidos de `appsettings` pela camada de infraestrutura:

- **`ConfiguracaoArquivoAnexo`** — `TamanhoMaximoBytes`, `TamanhoMaximoBytesTexto`
- **`ConfiguracaoFront`** — `UrlFront` (URL do frontend para links em e-mails)

## Extensions

**`EnumerableExtension`** — `EquivalenteA<T>()`, `ListaVazia<T>()`, `ListaNullOrWhiteSpace()`, `ListaSepadadaPorVirgula()`, `ToListNome<T>()`, `ListaStringTemSoUmItem()`
