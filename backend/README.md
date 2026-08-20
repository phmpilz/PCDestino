# Backend do PCD Destino

API HTTP em .NET 10 para catálogo acessível, contribuições, avaliações, favoritos, gamificação e moderação.

## Início rápido

Com Docker em execução:

```bash
docker compose -f backend/compose.yaml up --build
```

A API ficará disponível em `http://localhost:5205`, com OpenAPI em `http://localhost:5205/openapi/v1.json` e verificações de saúde em `/health/live` e `/health/ready`.

Para encerrar sem apagar o banco local:

```bash
docker compose -f backend/compose.yaml down
```

O guia completo está em [Backend e API](../documentation/BACKEND.md) e a implantação em [AWS](../documentation/AWS_BACKEND.md).

## Estrutura

- `src/PCDestino.Domain`: entidades e regras de negócio sem dependência de infraestrutura.
- `src/PCDestino.Application`: contratos, comandos e consultas da aplicação.
- `src/PCDestino.Infrastructure`: Entity Framework Core, PostgreSQL/PostGIS e repositórios.
- `src/PCDestino.Api`: autenticação, endpoints, segurança, saúde e observabilidade.
- `tests/`: testes unitários e integrados com PostgreSQL/PostGIS real em contêiner.
- `infra/PCDestino.Aws`: infraestrutura como código com AWS CDK.

## Comandos principais

```bash
dotnet restore backend/PCDestino.Backend.sln
dotnet build backend/PCDestino.Backend.sln --configuration Release
dotnet test backend/PCDestino.Backend.sln --configuration Release
dotnet run --project backend/src/PCDestino.Api
```

Os testes integrados precisam de um mecanismo Docker ativo.
