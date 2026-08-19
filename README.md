# PCD Destino

![Logotipo do PCD Destino](docs/assets/logo.png)

Central colaborativa multiplataforma para pessoas com deficiência encontrarem e recomendarem locais, serviços públicos, serviços privados, turismo, lazer e esporte com boas condições de acessibilidade.

## Estado do projeto

O repositório contém um MVP navegável para Android, iOS e web. A interface, a identidade visual, o fluxo de contribuição e a gamificação estão demonstrados com dados locais em memória.

Ainda não existem backend, autenticação, banco de dados, mapas reais, upload de imagens ou moderação. Esses itens estão detalhados em [Produto e próximos desenvolvimentos](documentation/PRODUCT_AND_BACKLOG.md).

## Funcionalidades demonstradas

- Página inicial com localização, categorias e recomendações
- Busca e filtros por categoria
- Indicadores e recursos de acessibilidade por local
- Detalhes, favoritos e avaliações da comunidade
- Contribuição de locais e serviços em três etapas
- Pontos, níveis, conquistas e ranking municipal
- Perfil com histórico e preferências
- Interface adaptada para Android, iOS e web
- Materiais e metadados para App Store e Google Play
- Páginas públicas de privacidade, termos e suporte

## Tecnologias principais

- Expo SDK 54
- React 19
- React Native 0.81
- TypeScript 5.9
- React Native Web
- Expo Application Services (EAS) para builds e submissão
- GitHub Actions para validação e GitHub Pages

Consulte [Tecnologias e dependências](documentation/DEPENDENCIES.md) para versões, responsabilidades e critérios de atualização.

## Início rápido

Requisitos mínimos:

- Node.js `20.19.4` ou superior compatível
- npm
- Git
- Expo Go em um celular, ou um navegador moderno

```bash
git clone https://github.com/phmpilz/PCDestino.git
cd PCDestino
npm ci
npm start
```

Com o servidor aberto, escaneie o QR Code usando o Expo Go ou pressione `w` para abrir a versão web. Para Android e iOS, consulte [Instalação e execução](documentation/SETUP.md).

## Comandos principais

| Comando | Finalidade |
| --- | --- |
| `npm start` | Inicia o servidor de desenvolvimento Expo |
| `npm run android` | Abre no emulador ou dispositivo Android |
| `npm run ios` | Abre no simulador iOS; exige macOS e Xcode |
| `npm run web` | Abre no navegador |
| `npm test` | Executa a suíte automatizada disponível atualmente |
| `npm run typecheck` | Valida os tipos TypeScript |
| `npm run check:deps` | Confere a compatibilidade das dependências Expo |
| `npm run build:web` | Gera o build web de produção em `dist/` |
| `npm run validate` | Executa todas as verificações usadas na integração contínua |

## Documentação

- [Índice da documentação](documentation/README.md)
- [Instalação e execução](documentation/SETUP.md)
- [Arquitetura](documentation/ARCHITECTURE.md)
- [Tecnologias e dependências](documentation/DEPENDENCIES.md)
- [Testes e qualidade](documentation/TESTING.md)
- [Produto e próximos desenvolvimentos](documentation/PRODUCT_AND_BACKLOG.md)
- [Acessibilidade, privacidade e segurança](documentation/ACCESSIBILITY_AND_PRIVACY.md)
- [Build, publicação e operação](documentation/RELEASE.md)
- [Guia de contribuição](CONTRIBUTING.md)
- [Identidade visual](BRAND.md)
- [Política de segurança](SECURITY.md)

## Contribuição

A branch `main` é protegida e todas as mudanças devem passar por Pull Request. Antes de abrir uma PR, execute:

```bash
npm run validate
```

Veja o processo completo em [CONTRIBUTING.md](CONTRIBUTING.md).

## Licença

O projeto ainda não possui uma licença de código definida. Até que uma licença seja adicionada, o conteúdo do repositório permanece protegido pelos direitos autorais de seus titulares. O símbolo universal de acessibilidade possui atribuição própria em [assets/brand/ATTRIBUTION.md](assets/brand/ATTRIBUTION.md).
