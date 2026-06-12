# CLAUDE.md — FfkApi.Exceptions

## Responsabilidade

Projeto compartilhado de exceções customizadas e mensagens de erro em português. Centraliza todo o contrato de erros da aplicação.

## Estrutura

```
FfkApi.Exceptions/
├── ExceptionsBase/                     # Classes de exceção
│   ├── ExceptionBase.cs                # Classe base abstrata
│   ├── ErrorOnValidationException.cs   # 400 — erros de validação (lista de mensagens)
│   ├── NotFoundException.cs            # 404 — recurso não encontrado
│   ├── UnauthorizedException.cs        # 401 — não autenticado
│   ├── ForbiddenException.cs           # 403 — sem permissão
│   ├── ExpiredSessionException.cs      # sessão/token expirado
│   ├── InvalidLoginUsuarioException.cs # credenciais de usuário inválidas
│   └── InvalidLoginSistemaClienteException.cs # credenciais de sistema cliente inválidas
├── MessagesException.cs                # Helper: busca string do resource pelo nome da chave
├── ResourceMessagesException.resx      # Strings de erro em pt-BR
└── ResourceMessagesException.Designer.cs # Gerado automaticamente — não editar manualmente
```

## Como Usar Mensagens de Erro

```csharp
// Buscar mensagem pelo nome da chave no .resx
var msg = MessagesException.GetString("USUARIO_NAO_ENCONTRADO");

// Lançar exceção de validação com lista de erros
throw new ErrorOnValidationException(new List<string> { msg });

// Lançar not found
throw new NotFoundException(MessagesException.GetString("ORGANIZACAO_NAO_ENCONTRADA"));
```

## Convenções

- **Todas as mensagens ficam no `.resx`** — nunca hardcode strings de erro fora do resource file
- Chaves do resource em `SNAKE_CASE_MAIUSCULO` em português (ex: `USUARIO_NAO_ENCONTRADO`)
- Mensagens com placeholders usam a convenção `{nome-do-placeholder}` (ex: `{permissao}`, `{lista}`) — a substituição é feita manualmente antes de lançar a exceção
- `ExceptionBase` define os contratos `PegarMensagensDeErro()` e `PegarStatusCode()` — toda exceção customizada deve herdar dela e implementar esses dois métodos
- O middleware global de erros em `FfkApi.API` captura as subclasses de `ExceptionBase` e mapeia para o HTTP status code correto

## Quando Adicionar Aqui

- Nova mensagem de erro: adicionar entrada no `ResourceMessagesException.resx`
- Novo tipo de erro com HTTP status diferente: criar nova classe herdando `ExceptionBase` em `ExceptionsBase/`
- Não criar exceções ad-hoc fora deste projeto
