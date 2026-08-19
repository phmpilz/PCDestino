# PCD Destino

![Logotipo do PCD Destino](docs/assets/logo.png)

Aplicativo colaborativo multiplataforma para encontrar e recomendar locais, serviços públicos e privados com boas condições de acessibilidade.

## O que já funciona

- Descoberta de locais e serviços próximos
- Busca e filtros por categoria
- Indicador e recursos de acessibilidade por local
- Tela detalhada, favoritos e avaliações da comunidade
- Fluxo de contribuição em três etapas
- Pontos, níveis, conquistas e ranking municipal
- Perfil com histórico e preferências
- Interface adaptada para Android, iOS e web
- Identidade visual e assets oficiais para App Store e Google Play
- Metadados em português do Brasil e páginas de privacidade, termos e suporte

Os dados atuais são demonstrativos e ficam em memória. A arquitetura visual está pronta para ser conectada a autenticação, banco de dados, mapas e moderação.

## Executar

Requer Node.js 20.19.4 ou superior.

```bash
npm install
npm start
```

Depois, use o QR Code no Expo Go ou escolha Android, iOS ou web no painel do Expo.

## Publicação

O projeto usa EAS Build e possui perfis em `eas.json`. Os textos e imagens das lojas estão em `fastlane/`, enquanto o checklist de informações que dependem das contas Apple e Google está em `store-submission/STORE_CHECKLIST.md`.

Todas as mudanças devem passar por pull request. Consulte `CONTRIBUTING.md`.

## Próxima etapa recomendada

Conectar um backend com autenticação e geolocalização. As entidades principais são usuários, cidades, locais, contatos/serviços, avaliações, recursos de acessibilidade, denúncias, pontos e conquistas. Avaliações e novos cadastros devem passar por regras de confiança e moderação antes de influenciarem a nota pública.
