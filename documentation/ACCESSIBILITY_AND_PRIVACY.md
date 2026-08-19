# Acessibilidade, privacidade e segurança

## Princípio do produto

Acessibilidade não é uma categoria adicional do PCD Destino; é requisito de cada fluxo, conteúdo e decisão técnica. Pessoas com deficiência devem participar da pesquisa, priorização, teste e aprovação do produto.

## Acessibilidade já presente no MVP

- Componentes nativos React Native
- Rótulos em botões e campos principais
- Papéis de acessibilidade em elementos interativos
- Estado selecionado e desabilitado em componentes relevantes
- Áreas seguras para barras e recortes do dispositivo
- Contraste visual planejado na identidade
- Informação acompanhada por texto, ícone e contexto

Esses itens não equivalem a uma auditoria ou conformidade completa.

## Requisitos de implementação

### Conteúdo e interação

- Linguagem simples e instruções objetivas
- Ordem de foco coerente
- Rótulos que descrevam ação e resultado
- Erros anunciados e associados aos campos
- Alternativa textual para imagens e mapas
- Alvos de toque confortáveis e separados
- Nenhuma informação transmitida somente por cor, gesto ou som
- Tempo suficiente e possibilidade de retomar formulários

### Preferências e deficiências

- Não exigir que o usuário revele diagnóstico médico.
- Permitir buscar recursos específicos sem criar perfil sensível.
- Tratar preferências de acessibilidade como dado potencialmente sensível.
- Evitar inferir deficiência a partir de comportamento ou localização.

### Avaliações

- Estruturar critérios em vez de usar apenas uma nota genérica.
- Informar quando e por quem o recurso foi verificado.
- Permitir experiências diferentes sem invalidar relatos.
- Distinguir informação oficial, comunitária e não verificada.

## Testes inclusivos

- VoiceOver, TalkBack e navegação por teclado
- Tamanho de fonte e zoom elevados
- Contraste aumentado e redução de movimento
- Controle por voz e dispositivos de comutação, quando possível
- Conexão lenta, aparelho antigo e tela pequena
- Testes moderados com pessoas com diferentes deficiências

Problemas que impedem uma tarefa crítica devem bloquear a release.

## Privacidade e LGPD

Antes de coletar dados reais, o projeto precisa definir:

- Controlador, operadores e encarregado/canal de privacidade
- Finalidade e base legal de cada tratamento
- Dados obrigatórios e opcionais
- Prazo de retenção e descarte
- Compartilhamentos e transferências internacionais
- Processo de acesso, correção, portabilidade e exclusão
- Resposta a incidentes
- Proteção de crianças e adolescentes, caso estejam no público

Localização precisa, preferências de acessibilidade e conteúdo de avaliações merecem análise específica. O aplicativo deve funcionar com uma cidade escolhida manualmente sempre que a localização exata não for necessária.

## Minimização de dados

- Não coletar documento, diagnóstico ou endereço residencial sem necessidade comprovada.
- Não armazenar localização contínua para uma busca pontual.
- Remover EXIF e coordenadas de fotos antes da publicação.
- Separar identidade privada do perfil público.
- Usar identificadores internos e logs pseudonimizados.
- Solicitar consentimento específico para analytics não essenciais.

## Segurança mínima para produção

- TLS em todas as comunicações
- Tokens em armazenamento seguro do sistema
- Autorização no servidor para toda alteração
- Rate limiting e prevenção de enumeração
- Validação de arquivos e conteúdo
- Segredos apenas no gerenciador do ambiente, nunca no aplicativo
- Dependências verificadas e atualizadas
- Backups criptografados e restauração testada
- Trilha de auditoria protegida
- Plano de resposta e comunicação de incidentes

Vulnerabilidades devem ser reportadas conforme [SECURITY.md](../SECURITY.md), nunca em uma issue pública com detalhes exploráveis.

## Páginas e declarações das lojas

As páginas em `docs/` são textos iniciais e devem ser revisadas quando o backend e os fornecedores forem definidos. Os formulários App Privacy da Apple e Data Safety do Google devem refletir o comportamento efetivo do aplicativo e de todos os SDKs, não apenas a política publicada.
