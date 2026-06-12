# CLAUDE.md — GeradorDeCodigo

## Visão Geral

Console app .NET 8 que faz scaffold de um CRUD completo na solução FfkApi. Gera arquivos novos e injeta trechos de código em arquivos existentes com base em templates JSON. O resultado é um esqueleto funcional que **não builda imediatamente** — ajustes manuais nos pontos marcados com `// TODO : [GERADOR DE CÓDIGO] =>` são sempre necessários.

## Estrutura do Projeto

```
GeradorDeCodigo/
├── Program.cs                  # Entrypoint: instancia e chama GeradorCrud.GerarCrud()
├── Controle/
│   ├── ControleCodigo.cs       # Representa um template: variáveis, caminho destino, injeções
│   ├── InclusaoCodigo.cs       # Representa um bloco de código a injetar em arquivo existente
│   └── VariavelCodigo.cs       # Representa um placeholder: nome, variável fonte, modificador
├── Geradores/
│   └── GeradorCrud.cs          # Pipeline de geração completo
├── Modelos/
│   └── Crud/                   # 48 templates organizados por camada (ver abaixo)
└── Util/
    ├── ArquivoUtil.cs          # I/O: listar, ler, criar arquivos; injetar blocos; mapear pastas
    ├── Substituidor.cs         # Substituição de {{Variavel}} com aplicação de modificadores
    └── MultiTextWriter.cs      # Redireciona Console para stdout + arquivo de log simultâneos
```

## Como Executar

```
dotnet run --project gerador/GeradorDeCodigo
```

O gerador é **interativo**: sem argumentos de linha de comando. Após iniciar, solicita três valores:

| Variável | Exemplo | Descrição |
|---|---|---|
| `NomeEntidade` | `Produto` | Nome singular da entidade (PascalCase) |
| `NomeEntidadePlural` | `Produtos` | Nome plural (usado em rotas e coleções) |
| `PermissaoCadastro` | `CRIAR_PRODUTO` | String de permissão para autorização na API |

Pressionar Enter sem valor encerra o gerador.

Ao final, gera o arquivo `LogGeradorCrud_{timestamp}.log` com todo o output do console.

## Pipeline de Geração

1. **Redirecionar output** → cria arquivo de log
2. **Carregar templates** → lê todos os arquivos em `Modelos/Crud/`, extrai metadados JSON do bloco `<# ... #>`
3. **Mapear pastas do sistema** → navega 3 níveis acima do executável para localizar `src/`, `tests/` etc.
4. **Coletar inputs** → solicita ao usuário os valores das variáveis não-pasta
5. **Preparar substituições** → aplica modificadores em cada variável (ver abaixo)
6. **Criar arquivos novos** → substitui placeholders no caminho destino e no conteúdo; pula se o arquivo já existir
7. **Injetar código em arquivos existentes** → cada template pode ter `inclusoesCodigo`: localiza a função pelo `assinaturaFuncao`, conta chaves para achar o fechamento e insere o bloco antes dele (ou antes de `adicionarAntesDaLinha` se definido); respeita `ignorarSeJaExistir`
8. **Relatório final** → exibe contagem de arquivos criados e injeções realizadas

## Formato dos Templates

Cada arquivo em `Modelos/Crud/` tem duas partes separadas pelo delimitador `<# ... #>`:

```
<#
{
  "variaveis": [
    { "nome": "{{Entidade}}", "substituirPor": "NomeEntidade", "modificador": null },
    { "nome": "{{ENTIDADE}}", "substituirPor": "NomeEntidade", "modificador": "ToUpper" },
    { "nome": "{{entidade}}", "substituirPor": "NomeEntidade", "modificador": "Variavel" }
  ],
  "arquivoDestino": "{{PastaApi}}\\Controllers\\{{Entidade}}Controller.cs",
  "inclusoesCodigo": [
    {
      "caminhoArquivo": "{{PastaDomain}}\\...",
      "assinaturaFuncao": "public void Configure(...)",
      "blocoAdicionar": "    // código a injetar",
      "ignorarSeJaExistir": true,
      "adicionarAntesDaLinha": null
    }
  ]
}
#>
// conteúdo do arquivo gerado com {{Entidade}}, {{entidade}}, etc.
```

### Modificadores de Variável

| Modificador | Efeito | Exemplo (`NomeEntidade = "Produto"`) |
|---|---|---|
| `null` | Valor original | `Produto` |
| `"ToUpper"` | MAIÚSCULAS | `PRODUTO` |
| `"ToLower"` | minúsculas | `produto` |
| `"Variavel"` | primeira letra minúscula | `produto` |

### Variáveis de Pasta (auto-populadas)

| Variável | Caminho resolvido |
|---|---|
| `{{PastaApi}}` | `src/Backend/FfkApi.API` |
| `{{PastaApplication}}` | `src/Backend/FfkApi.Application` |
| `{{PastaDomain}}` | `src/Backend/FfkApi.Domain` |
| `{{PastaInfrastructure}}` | `src/Backend/FfkApi.Infrastructure` |
| `{{PastaCommunication}}` | `src/Shared/FfkApi.Communication` |
| `{{PastaShared}}` | `src/Shared` |
| `{{PastaBackend}}` | `src/Backend` |
| `{{PastaTestes}}` | `tests/` |

## Templates Disponíveis (48 arquivos)

### API
- `EntidadeController.cs` → `FfkApi.API/Controllers/`

### Application (Use Cases + Interfaces + Validadores)
- `Cadastrar`, `CadastrarEmLote`, `Alterar`, `Excluir`, `Pegar`, `Pesquisar`
- Cada operação: `UseCase.cs` + `IUseCase.cs`; Cadastrar e Alterar também têm `Validator.cs`

### Communication (DTOs)
- `RequestCadastrar`, `RequestCadastrarEmLote`, `RequestAlterar`, `RequestExcluir`, `RequestPegar`
- `ResponseDados`

### Domain
- `IEntidadeRepository.cs` → `FfkApi.Domain/Repositories/`

### Infrastructure
- `EntidadeRepository.cs` → `FfkApi.Infrastructure/DataAccess/Repositories/`
- Injeta `DbSet<Entidade>` no `FfkApiDbContext` e registra o repositório no DI

### Testes
- **TestUtil**: `EntidadeBuilder.cs` (Bogus), `EntidadeRepositoryBuilder.cs` (Moq), builders de Request
- **UseCases.Test**: testes unitários para Cadastrar, Alterar, Excluir, Pegar
- **Validators.Test**: testes unitários para Cadastrar, Alterar
- **WebApi.Test**: testes de integração para todas as operações + CadastrarEmLote
- **E2E.Test**: testes de aceitação para todas as operações + CadastrarEmLote

## TODOs no Código Gerado

Após rodar o gerador, busque `// TODO : [GERADOR DE CÓDIGO] =>` em todos os arquivos criados. Exemplos dos pontos marcados:

| Localização | O que fazer |
|---|---|
| `Request*.cs` | Preencher as propriedades do DTO |
| `ResponseDados*.cs` | Preencher as propriedades da resposta |
| `*Validator.cs` | Completar as regras de validação da entidade |
| `*Repository.cs` | Adicionar `Include` necessários nas queries |
| `*Builder.cs` (TestUtil) | Preencher os campos do builder com valores fake (Bogus) |
| `*UseCaseTest.cs` | Completar os testes do use case |
| `*ValidatorTest.cs` | Completar os casos de teste do validador |
| `*Test.cs` (WebApi/E2E) | Completar os asserts dos campos da entidade |

## Fluxo Completo Após Geração

1. Resolver todos os `// TODO : [GERADOR DE CÓDIGO] =>`
2. Adicionar a entidade ao `FfkApiDbContext` (o gerador injeta o `DbSet`, mas pode precisar de ajuste)
3. Aplicar migration:
   ```
   dotnet ef migrations add Add{{Entidade}} --project src/Backend/FfkApi.Infrastructure --startup-project src/Backend/FfkApi.API
   ```
4. Verificar build: `dotnet build`
5. Rodar testes: `dotnet test`
