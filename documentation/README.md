# Documentação técnica

Este diretório descreve o estado atual do PCD Destino e serve como ponto de partida para desenvolvimento, testes, operação e evolução do produto.

## Guias

| Documento | Conteúdo |
| --- | --- |
| [Instalação e execução](SETUP.md) | Pré-requisitos, preparação do ambiente e execução em cada plataforma |
| [Arquitetura](ARCHITECTURE.md) | Estrutura do código, fluxo de dados, estado e decisões atuais |
| [Tecnologias e dependências](DEPENDENCIES.md) | Stack, versões, finalidade de cada pacote e manutenção |
| [Testes e qualidade](TESTING.md) | Comandos, integração contínua, testes manuais e lacunas atuais |
| [Produto e próximos desenvolvimentos](PRODUCT_AND_BACKLOG.md) | Escopo funcional, backend necessário, modelo de dados e roadmap |
| [Acessibilidade, privacidade e segurança](ACCESSIBILITY_AND_PRIVACY.md) | Requisitos para um produto inclusivo e aderente à LGPD |
| [Build, publicação e operação](RELEASE.md) | Builds EAS, lojas, metadados, páginas públicas e releases |

## Leitura recomendada por perfil

- Primeira contribuição: `SETUP.md`, `ARCHITECTURE.md`, `TESTING.md` e `CONTRIBUTING.md`
- Desenvolvimento mobile: `SETUP.md`, `DEPENDENCIES.md` e `ARCHITECTURE.md`
- Backend e produto: `PRODUCT_AND_BACKLOG.md` e `ACCESSIBILITY_AND_PRIVACY.md`
- Publicação: `RELEASE.md` e `store-submission/STORE_CHECKLIST.md`
- Design: `BRAND.md` e `ACCESSIBILITY_AND_PRIVACY.md`

## Fonte de verdade

- Versões instaladas: `package.json` e `package-lock.json`
- Configuração Expo: `app.json`
- Perfis de build: `eas.json`
- Automação: `.github/workflows/`
- Materiais das lojas: `fastlane/` e `assets/store/`
- Requisitos pendentes de publicação: `store-submission/STORE_CHECKLIST.md`

Quando a implementação e a documentação divergirem, a implementação é o comportamento atual, mas a divergência deve ser corrigida na mesma Pull Request.
