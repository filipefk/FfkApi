# CLAUDE.md — Validators.Test

## Propósito

Testes **unitários** dos validadores de request (`AbstractValidator<TRequest>`) definidos em `FfkApi.Application/Validators/`. Verificam apenas regras de validação de entrada — sem lógica de negócio, sem mocks de repositório.

## Stack

- **NUnit 4.4.0** — framework de teste
- **FluentValidation** — sob teste
- **Bogus** — dados fake nos builders (via `TestUtil`)

## Estrutura de Pastas

```
Validators.Test/
├── IdValidator/          # Validador genérico de IDs
└── <Entidade>/           # Anexo, Equipe, Feed, Usuario, etc.
    └── <Operacao>/       # Alterar, Cadastrar, etc.
        └── <Entidade><Operacao>ValidatorTest.cs
```

## Como Escrever um Teste

```csharp
[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarAnexoValidatorTest
{
    [Test]
    public async Task Sucesso()
    {
        var request   = RequestAlterarAnexoBuilder.Build();
        var validator = new AlterarAnexoValidator();

        var result = await validator.ValidateAsync(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public async Task Erro_Nome_Vazio(string nome)
    {
        var request   = RequestAlterarAnexoBuilder.Build();
        request.Nome  = nome;
        var validator = new AlterarAnexoValidator();

        var result = await validator.ValidateAsync(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].ErrorMessage,
            Is.EqualTo(ResourceMessagesException.NOME_VAZIO));
    }
}
```

### Casos obrigatórios por validator

| Cenário | O que verificar |
|---|---|
| `Sucesso` | `result.IsValid == true` |
| `Erro_<Campo>_Vazio` | `IsValid == false`, mensagem correta, 1 erro por campo |
| `Erro_<Campo>_Invalido` | Idem (formato inválido, fora do range, etc.) |

### Valores nulos/vazios

Use `[TestCase(null)]`, `[TestCase("")]`, `[TestCase("        ")]` para strings; para listas use `ListaStringNulaVaziaInlineData` de `TestUtil/InlineData/`.

## Mensagens de Erro

Sempre referencie constantes de `ResourceMessagesException` (`FfkApi.Exceptions`) — nunca use strings literais nas assertions.

## Convenções

- Nome do teste: `Sucesso`, `Erro_<Campo>_<Motivo>` (snake_case com maiúsculas por segmento)
- Atributos obrigatórios: `[TestFixture]` e `[Parallelizable(ParallelScope.All)]`
- Sem mocks, sem banco, sem I/O
- Um caso de teste por regra de campo (não agrupar múltiplas regras)

## Executar

```
dotnet test tests/Validators.Test
```
