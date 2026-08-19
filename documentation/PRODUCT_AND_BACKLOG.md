# Produto e próximos desenvolvimentos

## Objetivo

Ser uma referência confiável por cidade para pessoas com deficiência encontrarem serviços, contatos públicos e privados, turismo, lazer e esporte, com informações de acessibilidade verificáveis e atualizadas pela comunidade.

## Estado do MVP

O MVP mobile valida a experiência visual e ainda usa dados demonstrativos. O repositório já possui uma API .NET 10 funcional para cidades, locais, avaliações, favoritos, ranking e moderação, mas o aplicativo ainda precisa ser conectado a ela.

## Capacidades necessárias para produção

### Contas e identidade

- Cadastro e login acessíveis
- Recuperação de conta
- Perfis públicos e privados
- Consentimento e preferências de comunicação
- Papéis de usuário, moderador e administrador
- Exclusão e exportação de dados

### Diretório de cidades, serviços e locais

- Cidades, bairros e coordenadas geográficas
- Contatos de órgãos públicos e serviços privados
- Horários, telefones, sites e canais de atendimento
- Categorias de lazer, turismo, esporte, saúde e serviços
- Recursos de acessibilidade estruturados por tipo de deficiência
- Histórico de alterações e data da última verificação

### Busca e mapas

- Busca textual tolerante a erros
- Filtros por distância, categoria e recurso de acessibilidade
- Mapa e lista com a mesma fonte de dados
- Geocodificação e cálculo de distância reais
- Opção de informar cidade sem compartilhar localização precisa
- Cache e funcionamento degradado em conexão lenta

### Avaliações e contribuições

- Formulários acessíveis e validação no servidor
- Fotos com texto alternativo e remoção de metadados sensíveis
- Avaliação separada por dimensões de acessibilidade
- Edição, contestação e histórico
- Denúncia e direito de resposta
- Confirmação comunitária e verificação oficial

### Gamificação

- Livro-razão imutável de eventos de pontos
- Regras versionadas e transparentes
- Limites diários e detecção de duplicidade
- Pontos concedidos após validação, não apenas após envio
- Reversão auditável em caso de fraude
- Ranking por cidade e período com opção de não participar
- Conquistas que não estimulem avaliações apressadas ou inseguras

### Moderação e administração

- Fila de revisão para novos cadastros e alterações
- Painel administrativo acessível
- Detecção de spam, fraude e conteúdo ofensivo
- Bloqueio, recurso e trilha de auditoria
- SLA para correções de informações públicas críticas
- Gestão de fontes oficiais e responsáveis por estabelecimentos

### Operação

- Logs sem dados sensíveis
- Monitoramento de erros e disponibilidade
- Backup, restauração e plano de incidentes
- Feature flags e rollout gradual
- Suporte ao usuário e métricas de qualidade dos dados

## Modelo de dados inicial

As entidades abaixo são uma proposta a ser validada antes da implementação:

| Entidade | Responsabilidade |
| --- | --- |
| `User` | Conta, preferências, cidade e consentimentos |
| `City` e `Neighborhood` | Organização territorial |
| `Place` | Estabelecimento, atração ou ponto físico |
| `Service` | Serviço público ou privado, inclusive sem endereço físico |
| `ContactChannel` | Telefone, site, e-mail ou canal de atendimento |
| `AccessibilityFeature` | Recurso padronizado de acessibilidade |
| `PlaceFeature` | Recurso existente, estado, evidência e data de verificação |
| `Review` | Avaliação textual e notas por dimensão |
| `Media` | Foto, texto alternativo, autoria e moderação |
| `Submission` | Proposta de criação ou alteração |
| `Report` | Denúncia e tratamento de conteúdo |
| `Verification` | Confirmação comunitária, oficial ou da moderação |
| `PointEvent` | Crédito ou débito auditável da gamificação |
| `Achievement` | Critério e conquista do usuário |
| `AuditLog` | Ações administrativas relevantes |

## API e infraestrutura implementadas

O backend já fornece:

- API autenticada e versionada
- Autorização por recurso, não apenas por tela
- Banco relacional com suporte geoespacial
- Busca textual e geográfica
- Rate limiting e proteção WAF contra automação abusiva
- Migrações versionadas e execução isolada em deploy
- Infraestrutura AWS reproduzível com Cognito, Fargate e Aurora/PostGIS

Ainda precisam ser implementados armazenamento de mídia com URLs temporárias, processamento assíncrono, idempotência formal, homologação independente e integrações mobile. As decisões atuais e o procedimento de publicação estão em [Backend e API](BACKEND.md) e [Implantação do backend na AWS](AWS_BACKEND.md).

## Roadmap recomendado

### Fase 1 — Fundação

- Modularizar o aplicativo
- Definir navegação e contratos da API
- Projetar banco de dados e autenticação
- Criar ambiente de homologação
- Adicionar testes de componentes

### Fase 2 — Diretório funcional

- Cidades, locais, serviços e contatos reais
- Busca, filtros, mapas e localização opcional
- Cadastro e edição com moderação
- Painel administrativo básico

### Fase 3 — Comunidade confiável

- Avaliações estruturadas, evidências e denúncias
- Gamificação auditável e antifraude
- Notificações e favoritos persistentes
- Verificação por responsáveis e fontes oficiais

### Fase 4 — Piloto

- Auditoria de acessibilidade com PCDs
- Revisão de privacidade e segurança
- Testes de carga, dispositivos e recuperação
- Piloto limitado por cidade

### Fase 5 — Produção

- Builds assinados e publicação nas lojas
- Suporte, observabilidade e resposta a incidentes
- Expansão gradual baseada em qualidade dos dados

## Critérios mínimos para lançamento

- Testes com usuários PCD remunerados e representativos
- Fluxos críticos utilizáveis por leitor de tela e teclado
- Política de moderação e canal de contestação
- App Privacy e Data Safety coerentes com a implementação real
- Exclusão de conta e atendimento a direitos da LGPD
- Backup e restauração testados
- Monitoramento e responsável por incidentes
- Conteúdo inicial confiável na cidade piloto
