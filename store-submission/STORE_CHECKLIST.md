# Checklist de publicação

## Dados que dependem das contas do responsável

- [ ] Confirmar titular legal e canal privado para solicitações de privacidade
- [ ] Informar Apple Developer Team ID e App Store Connect App ID
- [ ] Informar conta de serviço do Google Play Console
- [ ] Confirmar classificação etária nas duas lojas
- [ ] Confirmar categoria principal e disponibilidade territorial
- [ ] Revisar as declarações de privacidade conforme o backend final
- [ ] Criar usuário de demonstração se a autenticação for obrigatória
- [ ] Substituir screenshots do MVP pelas capturas do build de produção

## Apple App Store

- [x] Bundle ID configurado: `br.com.pcddestino.app`
- [x] Ícone 1024 × 1024
- [x] Nome, subtítulo, descrição, palavras-chave e notas de versão
- [x] URLs de marketing, suporte e privacidade
- [x] Texto de revisão
- [ ] Responder ao questionário App Privacy no App Store Connect
- [ ] Preencher contato privado da equipe de revisão
- [ ] Gerar build iOS de produção com EAS

## Google Play

- [x] Package configurado: `br.com.pcddestino.app`
- [x] Ícone da loja 512 × 512
- [x] Feature graphic 1024 × 500
- [x] Título, descrições e changelog
- [x] Política de privacidade
- [ ] Responder ao formulário Data safety no Play Console
- [ ] Preencher classificação de conteúdo e público-alvo
- [ ] Gerar Android App Bundle de produção com EAS

## Comandos de build

```bash
npx eas-cli build --platform ios --profile production
npx eas-cli build --platform android --profile production
```
