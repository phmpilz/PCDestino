# Testes e qualidade

## Situação atual

O MVP possui validação automatizada de tipos, compatibilidade de dependências e geração web. Ainda não há testes unitários, testes de componentes nem testes ponta a ponta.

Por compatibilidade com ferramentas de CI, `npm test` executa a suíte automatizada disponível hoje: a verificação TypeScript. Isso não substitui os testes de comportamento que devem ser adicionados antes da produção.

## Comandos

### Suíte atual

```bash
npm test
```

### Tipos TypeScript

```bash
npm run typecheck
```

### Compatibilidade com Expo

```bash
npm run check:deps
```

### Build web de produção

```bash
npm run build:web
```

O resultado é criado em `dist/`, que não deve ser versionado.

### Validação completa

```bash
npm run validate
```

Esse é o comando obrigatório antes de uma Pull Request. Ele executa os mesmos controles essenciais do workflow `.github/workflows/quality.yml`.

## Integração contínua

Toda Pull Request para `main` executa o job obrigatório `quality` no GitHub Actions:

1. Checkout do código
2. Instalação com `npm ci`
3. Compatibilidade das dependências Expo
4. Verificação TypeScript
5. Exportação web

A `main` não pode receber mudanças enquanto esse job falhar.

## Roteiro manual mínimo

Teste em Android, iOS e web sempre que possível.

### Navegação

- Abrir todas as abas inferiores.
- Voltar de detalhes e fechar modais.
- Confirmar que nenhum conteúdo fica escondido pelas áreas seguras.

### Busca e categorias

- Pesquisar por nome e categoria.
- Limpar a busca.
- Alternar categorias e retornar ao estado inicial.

### Locais

- Abrir cada cartão.
- Favoritar e desfavoritar.
- Conferir nota, distância, recursos e selo de verificação.

### Contribuição

- Abrir pelo botão central e pelo perfil.
- Avançar e voltar pelas três etapas.
- Validar estados desabilitados e conclusão.

### Ranking e perfil

- Conferir ordem, pontos e conquistas.
- Verificar contagem de favoritos.

### Layout

- Testar telas pequenas e grandes.
- Aumentar o tamanho da fonte do sistema.
- Alternar orientação se o produto passar a permiti-la.
- Verificar teclado virtual, rolagem e foco.

## Roteiro de acessibilidade

- Navegar com VoiceOver no iOS e TalkBack no Android.
- Conferir rótulos, papéis, estados selecionados e ordem de foco.
- Usar somente teclado na web.
- Testar aumento de texto e zoom.
- Verificar contraste e não depender apenas de cor.
- Confirmar áreas de toque adequadas.
- Validar linguagem simples e mensagens de erro acionáveis.

## Testes a implementar

Antes do piloto:

- Testes unitários para pontuação, filtros e regras de gamificação
- Testes de componentes para busca, cartões, favoritos e formulários
- Testes de contrato entre aplicativo e API
- Testes de acessibilidade automatizados onde houver suporte

Antes da produção:

- Testes ponta a ponta dos fluxos críticos
- Testes de API, autorização e moderação
- Testes de migração do banco de dados
- Testes de carga para busca geográfica e envio de mídia
- Testes de segurança e abuso da gamificação
- Matriz de dispositivos e versões mínimas suportadas

Ao escolher ferramentas, registre a decisão em uma ADR e atualize `package.json`, este documento e o workflow de qualidade na mesma PR.
