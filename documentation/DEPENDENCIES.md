# Tecnologias e dependências

## Ambiente

| Tecnologia | Versão do projeto | Finalidade |
| --- | --- | --- |
| Node.js | `20.19.4` em `.nvmrc` | Execução das ferramentas e do Metro |
| npm | Fornecido com Node.js | Instalação reproduzível por `package-lock.json` |
| TypeScript | `~5.9.2` | Tipagem e validação estática |
| Expo SDK | `~54.0.0` | Toolchain e runtime multiplataforma |
| React | `19.1.0` | Componentes e estado da interface |
| React Native | `0.81.5` | Interface nativa Android e iOS |
| .NET SDK | `10.0.400` em `global.json` | Compilação e testes do backend |
| ASP.NET Core | `10` | API HTTP, autenticação e controles operacionais |
| Entity Framework Core | `10.0.4` | Persistência e migrações |
| PostgreSQL/PostGIS | `17 / 3.5` | Dados relacionais e consultas geográficas |
| AWS CDK | `2.262.0` (biblioteca) | Infraestrutura AWS como código em C# |

As versões exatas instaladas estão no `package-lock.json`. Não edite o lockfile manualmente.

## Dependências de produção

| Pacote | Responsabilidade |
| --- | --- |
| `expo` | CLI, bundler, configuração e APIs base do Expo |
| `react` | Renderização declarativa e hooks de estado |
| `react-native` | Componentes e APIs nativas multiplataforma |
| `react-dom` | Renderização da aplicação no navegador |
| `react-native-web` | Implementação web dos componentes React Native |
| `@expo/vector-icons` | Biblioteca de ícones usada na interface |
| `expo-font` | Carregamento de fontes usado pelos ícones Expo |
| `expo-status-bar` | Controle visual da barra de status |
| `react-native-safe-area-context` | Respeito a notch, barras e áreas seguras |

## Dependências de desenvolvimento

| Pacote | Responsabilidade |
| --- | --- |
| `typescript` | Compilador e verificação estática |
| `@types/react` | Tipos TypeScript para React |

## Backend

| Pacote | Responsabilidade |
| --- | --- |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | Provedor PostgreSQL para EF Core |
| `Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite` | Tipos e consultas PostGIS |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | Validação de access tokens Cognito |
| `Microsoft.AspNetCore.OpenApi` | Contrato OpenAPI gerado pela aplicação |
| `OpenTelemetry.*` | Métricas, traces e exportação OTLP opcional |
| `Testcontainers.PostgreSql` | Banco PostGIS isolado nos testes integrados |
| `Amazon.CDK.Lib` | ECS, Aurora, Cognito, VPC, WAF e observabilidade |

## Dependências que ainda não existem no mobile

O projeto não deve instalar bibliotecas antecipadamente. As categorias abaixo dependem das decisões de arquitetura e produto:

- Navegação e deep links
- Cliente HTTP e cache de dados remotos
- Cliente Cognito/OIDC e armazenamento seguro de tokens
- Mapas, geocodificação e localização em segundo plano
- Upload, compressão e exibição de fotos
- Notificações push
- Persistência offline
- Analytics e observabilidade com consentimento
- Testes de componentes e testes ponta a ponta

As escolhas devem considerar acessibilidade, privacidade, custo, manutenção, suporte a Android/iOS/web e compatibilidade com o Expo.

## Instalar uma nova dependência

Para pacotes que interagem com Expo ou React Native, prefira:

```bash
npx expo install nome-do-pacote
```

O Expo seleciona uma versão compatível com o SDK atual. Para um pacote puramente JavaScript:

```bash
npm install nome-do-pacote
```

Depois, execute:

```bash
npm run validate
```

Se o pacote incluir código nativo, será necessário reconstruir o development build.

## Atualizações

1. Crie uma branch exclusiva.
2. Consulte o guia de atualização do Expo para o novo SDK.
3. Atualize um conjunto pequeno de dependências por vez.
4. Execute `npx expo install --fix` quando indicado pelo Expo.
5. Execute `npm run validate`.
6. Teste Android, iOS e web manualmente.
7. Registre impactos e migrações na Pull Request.

Não use atualização forçada de todas as dependências sem revisar compatibilidade, especialmente para React Native, Expo e módulos nativos.
