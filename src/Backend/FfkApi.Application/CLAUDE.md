# CLAUDE.md — FfkApi.Application

## Responsabilidade

Camada de aplicação (orquestração). Contém os use cases, validadores, mapeamentos e a lógica de coordenação entre domínio e infraestrutura. Não possui acesso direto ao banco — tudo passa por interfaces do Domain.

## Estrutura

```
FfkApi.Application/
├── IUseCases/            # Interfaces genéricas base para os use cases
├── UseCases/             # Implementações organizadas por entidade/operação
│   ├── Anexo/
│   ├── EnvioEmail/
│   ├── Equipe/
│   ├── Feed/
│   ├── Indisponibilidade/
│   ├── Limpeza/
│   ├── Login/
│   ├── Organizacao/
│   ├── SistemaCliente/
│   ├── Token/
│   └── Usuario/
├── Services/
│   ├── AutoMapper/       # Perfil único de mapeamento (AutoMapping.cs)
│   └── Anexo/            # ArmazenadorDeAnexoService
├── Validators/           # Validadores reutilizáveis (Id, Data, Senha)
└── Extension/            # DependencyInjectionExtension, FluentValidationExtension
```

## Interfaces Genéricas de Use Case

Definidas em `IUseCases/`:

| Interface | Assinatura | Uso |
|---|---|---|
| `ICadastrarUseCase<TReq, TRes>` | `Execute(TReq, token) → Task<TRes>` | Criação de entidade |
| `IAlterarUseCase<TReq>` | `Execute(TReq, token) → Task` | Alteração |
| `IExcluirUseCase<TReq>` | `Execute(TReq, token) → Task` | Exclusão |
| `IPegarUseCase<TReq, TRes>` | `Execute(TReq, token) → Task<TRes>` | Busca por ID |
| `IPesquisarUseCase<TRes>` | `Execute(HttpRequest, token) → Task<ResponsePaginado<TRes>>` | Busca OData |
| `ICadastrarEmLoteUseCase<TReq, TReqs, TRes>` | `Execute(TReqs, token) → Task<TRes>` | Criação em lote |

Cada entidade tem suas interfaces concretas herdando dessas (ex: `ICadastrarEquipeUseCase : ICadastrarUseCase<RequestCadastrarEquipe, ResponseDadosEquipe>`).

## Padrão de Implementação dos Use Cases

```
UseCases/{Entidade}/{Operacao}/
  I{Operacao}{Entidade}UseCase.cs   ← interface concreta
  {Operacao}{Entidade}UseCase.cs    ← implementação
  {Operacao}{Entidade}Validator.cs  ← (apenas Cadastrar e Alterar)
```

**Estrutura interna de um use case:**

```csharp
public class CadastrarEquipeUseCase : ICadastrarEquipeUseCase
{
    // dependências injetadas via construtor
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEquipeRepository _equipeRepository;
    private readonly IUsuarioLogadoService _usuarioLogadoService;

    public async Task<ResponseDadosEquipe> Execute(RequestCadastrarEquipe request, CancellationToken token)
    {
        await Preparar(request, token);   // pré-processamento (ex: normalizar dados)
        await Validar(request, token);    // validação — lança exceção se inválido

        var equipe = _mapper.Map<Equipe>(request);
        await _equipeRepository.Adicionar(equipe, token);
        await _unitOfWork.CommitAsync(token);

        return _mapper.Map<ResponseDadosEquipe>(equipe);
    }
}
```

**Padrões de validação:**
- Erros coletados em `List<string>` antes de lançar `ErrorOnValidationException(erros)`
- Cada método de validação retorna `Task<List<string>>`
- FluentValidation para validação de formato/regras do DTO; lógica de negócio validada manualmente
- Conversão: `validationResult.ToListErros()` (extensão em `FluentValidationExtension`)

**Cache de entidade dentro do use case:**
```csharp
private Equipe? _equipe = null;
private bool jaProcurouEquipe = false;
// evita múltiplas queries ao banco durante o mesmo use case
```

## FluentValidation

Validadores ficam na mesma pasta do use case (`{Operacao}{Entidade}Validator.cs`):

```csharp
public class CadastrarEquipeValidator : AbstractValidator<RequestCadastrarEquipe>
{
    public CadastrarEquipeValidator()
    {
        RuleFor(r => r.Nome).NotEmpty().WithMessage(ResourceMessagesException.NOME_VAZIO);
        When(r => r.Membros != null, () =>
        {
            RuleForEach(r => r.Membros).ChildRules(m =>
                m.RuleFor(x => x.Email).NotEmpty());
        });
    }
}
```

**Extensões customizadas** (`FluentValidationExtension.cs`):
- `.Cpf<T>()` — valida CPF
- `.Telefone<T>()` — valida telefone
- `.ToListErros()` — converte `ValidationResult` para `List<string>`

## Validadores Reutilizáveis (`Validators/`)

| Classe | Métodos principais |
|---|---|
| `IdValidator` | `ValidarId(id)` → lança se inválido; `IdEstaValido(id)` → bool |
| `DataValidator` | `DataValida(data)` valida `dd/MM/yyyy`; `DataFinalMaiorOuIgualDataInicial()` |
| `SenhaValidator<T>` | `PropertyValidator` para senha: mín. 8 chars, maiúscula, minúscula, dígito, especial |

## AutoMapper

Perfil único em `Services/AutoMapper/AutoMapping.cs` com dois grupos:

- **`RequestToDomain()`** — `Request* → Entidade`; propriedades complexas são ignoradas (`opt.Ignore()`) e preenchidas manualmente no use case
- **`DomainToResponse()`** — `Entidade → Response*`; achata objetos aninhados, converte enums para string, datas para string formatada

## Injeção de Dependência

`DependencyInjectionExtension.AddApplication()` faz:

1. **Auto-registro por reflection** — escaneia o assembly buscando classes públicas concretas em `FfkApi.Application.UseCases`, registra cada uma como `Scoped` pela primeira interface (exceto `IValidator<>`)
2. **AutoMapper** — registra `IMapper` como `Scoped` com o perfil `AutoMapping`
3. **Configuração de anexo** — lê `Configuracoes:ArquivoAnexo:TamanhoMaximoBytes` e inicializa `ConfiguracaoArquivoAnexo`
4. **ArmazenadorDeAnexoService** — registra como `Scoped`

> Adicionar um novo use case dentro do namespace `FfkApi.Application.UseCases` é suficiente para ele ser registrado automaticamente no DI — não é necessário editar `DependencyInjectionExtension`.

## Dependências do Projeto

- `FfkApi.Domain` — entidades e interfaces
- `FfkApi.Communication` — DTOs
- `FfkApi.Exceptions` — exceções e mensagens
- AutoMapper 14.0.0
- FluentValidation 12.0.0
- Microsoft.AspNetCore.OData 8.3.1
- Serilog.AspNetCore 8.0.3
