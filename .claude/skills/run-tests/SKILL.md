---
name: run-tests
description: Executa os testes do escopo indicado (usecases, validators, webapi, e2e ou todos)
disable-model-invocation: true
---

Execute os testes conforme o escopo solicitado:
- `usecases` → `dotnet test tests/UseCases.Test`
- `validators` → `dotnet test tests/Validators.Test`
- `webapi` → `dotnet test tests/WebApi.Test`
- `e2e` → `dotnet test tests/E2E.Test`
- `todos` → execute todos os quatro acima em sequência

Reporte erros e sugestões de correção ao final.