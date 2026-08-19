# Build, publicação e operação

## Visão geral

O projeto usa Expo Application Services (EAS) para gerar binários. Os perfis estão em `eas.json`:

| Perfil | Uso |
| --- | --- |
| `development` | Cliente de desenvolvimento para equipe |
| `preview` | Distribuição interna para homologação |
| `production` | Binário assinado para as lojas |

O EAS Build pode ser usado com uma conta Expo. Consulte [Create your first build](https://docs.expo.dev/build/setup/).

## Preparação inicial do responsável

1. Criar ou acessar uma conta Expo.
2. Ter acesso ao Apple Developer e App Store Connect.
3. Ter acesso ao Google Play Console.
4. Conferir o identificador `br.com.pcddestino.app` nas duas plataformas.
5. Preencher os dados pendentes em `store-submission/STORE_CHECKLIST.md`.
6. Configurar credenciais sem adicioná-las ao Git.

Autenticação no EAS:

```bash
npx eas-cli@latest login
npx eas-cli@latest whoami
```

`eas.json` já existe. Ao vincular o repositório a um projeto Expo real, a configuração resultante deve ser revisada antes do commit.

## Validação antes do build

```bash
npm ci
npm run validate
```

Também execute o roteiro manual de `documentation/TESTING.md` em dispositivos reais.

## Builds internos

```bash
npx eas-cli@latest build --platform android --profile preview
npx eas-cli@latest build --platform ios --profile preview
```

Distribua apenas para pessoas autorizadas e não use dados pessoais reais em homologação sem controles equivalentes aos de produção.

## Builds de produção

```bash
npx eas-cli@latest build --platform android --profile production
npx eas-cli@latest build --platform ios --profile production
```

O resultado esperado é um Android App Bundle (`.aab`) e um arquivo iOS (`.ipa`) assinados.

## Submissão

Depois de validar os builds:

```bash
npx eas-cli@latest submit --platform android --profile production
npx eas-cli@latest submit --platform ios --profile production
```

O EAS Submit envia os binários, mas os metadados, screenshots e notas precisam ser administrados separadamente nas lojas. Veja [Submit to app stores](https://docs.expo.dev/deploy/submit-to-app-stores/).

## Materiais versionados

### Apple

- Textos: `fastlane/metadata/pt-BR/`
- Informações de revisão: `fastlane/metadata/review_information/`
- Screenshots: `fastlane/screenshots/pt-BR/`
- Ícone: `assets/store/apple/`

### Google

- Textos e changelog: `fastlane/metadata/android/pt-BR/`
- Ícone, feature graphic e screenshots: `fastlane/metadata/android/pt-BR/images/`
- Arquivos mestres: `assets/store/google/` e `assets/store/screenshots/`

As capturas atuais representam o MVP. Elas devem ser substituídas por capturas do build final se a interface ou os dados forem alterados.

## Versionamento

Antes de uma release:

- Atualize `expo.version` em `app.json` para a versão pública.
- Registre notas de versão nas duas lojas.
- Confirme os números de build. O perfil de produção usa `autoIncrement` remoto.
- Crie uma tag Git após a aprovação do commit publicado.
- Não reutilize um número de build já enviado.

Adote versionamento semântico para o aplicativo quando o ciclo de releases estiver definido.

## GitHub Pages

O workflow `.github/workflows/pages.yml` publica `docs/` após mudanças na `main`. O site contém página inicial, política de privacidade, termos e suporte.

Endereço configurado:

`https://phmpilz.github.io/PCDestino/`

Confirme os links após cada alteração nas páginas públicas.

## Checklist de release

1. Escopo aprovado e documentação atualizada.
2. `npm run validate` aprovado localmente e no GitHub.
3. Testes manuais e de acessibilidade concluídos.
4. Migrações e rollback testados, quando houver backend.
5. Política de privacidade e declarações das lojas revisadas.
6. Build `preview` homologado.
7. Build `production` gerado e identificado.
8. Metadados e screenshots conferidos.
9. Rollout gradual configurado.
10. Monitoramento e responsável de plantão definidos.

## Rollback e incidentes

Antes da produção, documente por ambiente:

- Como interromper rollout nas lojas
- Como desativar uma funcionalidade remota
- Como reverter API e banco de dados
- Como comunicar indisponibilidade e incidente de segurança
- Quem decide, executa e comunica cada ação

Não publique uma mudança sem uma forma proporcional de detectar falhas e reduzir seu impacto.
