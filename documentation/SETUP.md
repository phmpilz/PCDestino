# Instalação e execução

## 1. Pré-requisitos comuns

Instale:

- Git
- Node.js `20.19.4`, versão registrada em `.nvmrc`
- npm, distribuído com o Node.js
- Um editor de código com suporte a TypeScript
- .NET SDK `10.0.400` ou patch compatível
- Docker Desktop, OrbStack ou outro mecanismo compatível, para backend e testes integrados

O Expo recomenda uma versão LTS do Node.js e oferece suporte a macOS, Windows e Linux. A documentação oficial está em [Create a project](https://docs.expo.dev/get-started/create-a-project/).

Confira o ambiente:

```bash
node --version
npm --version
git --version
dotnet --version
docker version
```

Com `nvm`, a versão correta pode ser selecionada assim:

```bash
nvm install
nvm use
```

## 2. Obter e instalar o projeto

```bash
git clone https://github.com/phmpilz/PCDestino.git
cd PCDestino
npm ci
```

Use `npm ci` para reproduzir exatamente o `package-lock.json`. `npm install` deve ser usado apenas quando houver intenção de adicionar ou atualizar dependências.

O mobile demonstrativo não exige backend nem conta Expo. Copie `.env.example` somente quando iniciar a integração com a API; ele contém apenas identificadores públicos e nunca deve receber credenciais AWS.

## 3. Iniciar a API e o banco

Com Docker em execução, a partir da raiz:

```bash
docker compose -f backend/compose.yaml up --build
```

A API estará em `http://localhost:5205` e o contrato OpenAPI em `http://localhost:5205/openapi/v1.json`. O banco local é inicializado com dados demonstrativos.

A imagem oficial do PostGIS é publicada para `linux/amd64`. Em computadores ARM, o Docker executa essa imagem por emulação; mantenha habilitado o suporte a imagens x86/AMD64.

Para encerrar preservando os dados:

```bash
docker compose -f backend/compose.yaml down
```

Consulte [Backend e API](BACKEND.md) para executar diretamente pelo .NET e usar autenticação local.

## 4. Iniciar o aplicativo

```bash
npm start
```

O terminal exibirá um QR Code e atalhos para as plataformas. O servidor pode ser encerrado com `Ctrl+C`.

Se o celular não conseguir acessar o computador pela rede local:

```bash
npx expo start --tunnel
```

Se houver comportamento causado por cache:

```bash
npx expo start --clear
```

## 5. Executar no navegador

```bash
npm run web
```

Também é possível executar `npm start` e pressionar `w`. A versão web usa React Native Web e o Metro Bundler.

## 6. Executar em celular com Expo Go

1. Instale o Expo Go no Android ou iOS.
2. Conecte celular e computador à mesma rede.
3. Execute `npm start`.
4. Escaneie o QR Code.

O projeto usa Expo SDK 54, compatível com o fluxo atual do Expo Go documentado em [Create your first app](https://docs.expo.dev/tutorial/create-your-first-app/).

Expo Go é suficiente para o MVP atual. Quando o projeto adicionar bibliotecas nativas não incluídas nele, será necessário criar um development build.

## 7. Executar no Android

Opções:

- Celular com Expo Go
- Dispositivo conectado com depuração USB
- Emulador do Android Studio

Para o emulador:

1. Instale o [Android Studio](https://developer.android.com/studio/install).
2. Instale o Android SDK e o Android Emulator pelo assistente.
3. Crie e inicie um Android Virtual Device.
4. Execute:

```bash
npm run android
```

O guia oficial do emulador está em [Run apps on the Android Emulator](https://developer.android.com/studio/run/emulator).

## 8. Executar no iOS

O simulador iOS local exige macOS e Xcode.

1. Instale o Xcode pela Mac App Store.
2. Abra o Xcode uma vez e conclua a instalação dos componentes.
3. Instale um runtime de simulador iOS.
4. Execute:

```bash
npm run ios
```

Consulte [Installing Xcode and Simulators](https://developer.apple.com/documentation/safari-developer-tools/installing-xcode-and-simulators).

Em Windows ou Linux, use um iPhone com Expo Go ou um build criado pelo EAS.

## 9. Development build

Development builds serão necessários quando o aplicativo passar a usar módulos nativos fora do Expo Go, como determinados SDKs de mapas, autenticação ou notificações.

Preparação futura:

```bash
npx expo install expo-dev-client
npx eas-cli@latest login
npx eas-cli@latest build --platform android --profile development
npx eas-cli@latest build --platform ios --profile development
```

Essa dependência ainda não está instalada. Não execute a etapa antes de existir uma necessidade nativa. Veja [Introduction to development builds](https://docs.expo.dev/develop/development-builds/introduction/).

## 10. Problemas comuns

### Versão do Node incompatível

Use a versão de `.nvmrc` e reinstale as dependências:

```bash
nvm use
npm ci
```

### Dependência incompatível com o Expo

```bash
npm run check:deps
npx expo install nome-do-pacote
```

### QR Code não conecta

- Confirme que os dispositivos estão na mesma rede.
- Desative temporariamente VPN ou isolamento de clientes da rede.
- Use `npx expo start --tunnel`.

### Emulador não é encontrado

- Inicie o simulador ou emulador antes do comando.
- Confirme que Android SDK ou Xcode foram instalados corretamente.
- Como alternativa, abra o Expo Go em um dispositivo físico.
