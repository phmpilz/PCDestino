# Como contribuir

Obrigado por contribuir com o PCD Destino. Mudanças de produto devem considerar a experiência de pessoas com diferentes deficiências, dispositivos e condições de conectividade.

## Antes de começar

1. Leia [Instalação e execução](documentation/SETUP.md).
2. Consulte [Arquitetura](documentation/ARCHITECTURE.md).
3. Verifique se já existe uma issue ou Pull Request relacionada.
4. Não inclua dados pessoais, médicos ou endereços particulares em exemplos e testes.

## Fluxo obrigatório

A `main` é protegida e não aceita commits diretos.

```bash
git switch main
git pull --ff-only
git switch -c tipo/descricao-curta
```

Use branches curtas e focadas. Exemplos: `feat/busca-por-cidade`, `fix/foco-do-modal` e `docs/configuracao-android`.

Antes do commit:

```bash
npm run validate
```

Faça commits pequenos e descritivos, publique a branch e abra uma Pull Request usando o modelo do repositório.

## Pull Request

Descreva:

- Problema ou necessidade
- Solução escolhida
- Impacto para usuários e acessibilidade
- Como a mudança foi testada
- Capturas ou gravações quando houver alteração visual
- Riscos, migração e rollback quando aplicável

O job obrigatório `quality` e a revisão devem ser concluídos antes do merge. Resolva todas as conversas abertas.

## Padrões de código

- Use TypeScript e evite `any` sem justificativa.
- Prefira componentes pequenos e responsabilidades claras.
- Centralize cores e tokens em `src/theme.ts`.
- Preserve rótulos, papéis, estados e ordem de foco acessíveis.
- Não coloque chamadas HTTP diretamente nos componentes de apresentação.
- Não adicione uma dependência sem explicar por que a plataforma não atende a necessidade.
- Atualize documentação e testes junto com o comportamento.

## Mudanças de dependências

Para pacotes Expo ou React Native:

```bash
npx expo install nome-do-pacote
```

Inclua `package.json` e `package-lock.json` na mesma PR. Execute `npm run check:deps` e teste todas as plataformas afetadas.

## Conteúdo e dados

- Use somente dados fictícios ou autorizados.
- Não exponha deficiência, localização precisa ou contato privado.
- Inclua texto alternativo em imagens informativas.
- Diferencie informação verificada, comunitária e demonstrativa.
- Cadastros reais deverão passar por moderação antes de se tornarem públicos.

## Segurança

Vulnerabilidades não devem ser relatadas em issues públicas. Siga [SECURITY.md](SECURITY.md).
