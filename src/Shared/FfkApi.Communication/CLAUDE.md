# CLAUDE.md — FfkApi.Communication

## Responsabilidade

Projeto compartilhado de DTOs (Data Transfer Objects). Não contém lógica de negócio — apenas estruturas de dados usadas para entrada e saída da API.

## Estrutura

```
FfkApi.Communication/
├── Requests/   # DTOs de entrada (parâmetros de use cases e endpoints)
└── Responses/  # DTOs de saída (retorno dos use cases e endpoints)
```

## Convenções

- **Prefixo obrigatório** — todos os arquivos e classes seguem `Request*` ou `Response*`
- **Sufixo por operação** — `Cadastrar`, `Alterar`, `Excluir`, `Pegar`, `Pesquisar` no nome do Request indicam a ação correspondente
- **Lote** — operações em massa usam sufixo `EmLote` (ex: `RequestCadastrarFeedEmLote`)
- Sem construtores com lógica, sem métodos, sem dependências externas
- Nomes em português, seguindo as convenções gerais do projeto

## Dependências

Nenhuma dependência de outros projetos da solução. Referenciado por `FfkApi.API` e `FfkApi.Application`.

## Quando Adicionar Aqui

- Ao criar um novo use case: adicionar o `Request*` e o `Response*` correspondentes
- Ao usar o GeradorDeCodigo: os DTOs gerados já ficam nesta pasta — revisar e ajustar conforme necessário
- Respostas genéricas reutilizáveis (ex: `ResponsePaginado<T>`, `ResponseCadastrarEmLote`) ficam em `Responses/` sem vínculo com entidade específica
