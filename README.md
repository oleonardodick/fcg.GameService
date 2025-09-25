# Fiap Cloud Games

## Microsserviço de Jogos

Uma API criada em Dot Net Core 8 responsável pelo controle dos jogos e da biblioteca de jogos utilizado no sistema da Fiap Cloud Games.
Através desta API, será possível pesquisar, cadastrar, excluir e modificar jogos, assim como também criar e adicionar jogos à biblioteca de algum usuário.

## Indice

- [Download](#download)
- [Banco de dados](#banco-de-dados)
- [ElasticSearch](#elasticsearch)
- [Como rodar o projeto?](#como-rodar-o-projeto)
  - [Via Docker Compose](#via-docker-compose)
- [Logs](#logs)
- [Open Telemetry](#open-telemetry)
- [Uso](#uso)
  - [Url Endpoints](#url-endpoints)
  - [Controle de acesso](#controle-de-acesso)
- [Swagger](#swagger)
- [Testes](#testes)
- [Contribuição](#contribuição)

## Download

Primeiramente, deve ser realizado o download (clone) deste repositório do GitHub. Este clone pode ser realizado através do seguinte comando:

```bash
  git clone https://github.com/oleonardodick/fcg.GameService.git
```

## Banco de dados

Este projeto utiliza o MongoDB como banco de dados principal.

## ElasticSearch

Este projeto utiliza o ElasticSearch para algumas funcionalidades. Para configurar este serviço, deve ser adicionado no appSetings o seguinte código:

```json
    "ElasticSettings": {
    "ApiKey": "API KEY DO ELASTIC CLOUD",
    "CloudId": "CLOUD ID DO ELASTIC CLOUD"
  }
```

## Como rodar o projeto?

### Via Docker Compose

Foi disponibilizado neste repositório um arquivo docker-compose.yml para que seja possível buildar e rodar a aplicação e seus correlatos diretamente via docker, sem a necessidade de rodar o projeto via dotnet.

Aconselha-se o uso do docker-compose para rodar o projeto, visto que todos os serviços necessários já estarão configurados, bem como as variáveis de ambiente.

Para isso, deve ser disponibilizado na raiz do projeto um arquivo .env com a seguinte estrutura:

```javascript
PROJECT_NAME=fcg-jogos

# Port Configuration
API_PORT=8080
MONGO_PORT=27017
DASHBOARD_PORT=18888
OTLP_GRPC_PORT=18889
OTLP_HTTP_PORT=18890

# MongoDB Configuration
MONGO_ROOT_USERNAME=userName
MONGO_ROOT_PASSWORD=password
MONGO_DATABASE=databaseName
MONGO_CONNECTION_STRING=mongodb://userName:password@mongodb:27017/databaseName?authSource=admin

# Open Telemetry Configuration
OTEL_ENDPOINT=http://gameService-dashboard:18889
OTEL_PROTOCOL=grpc
OTEL_SERVICE_NAME=GameService
OTEL_RESOURCE_ATTRIBUTES=service.version=1.0.0,deployment.environment=dev

# Database Settings
MONGO_CONNECTION_TIMEOUT=30
MONGO_MAX_POOL_SIZE=100
```

**Importante:** Este arquivo .env é apenas local e não deve ser enviado ao repositório.

Após configurar o .env, pode ser rodado o seguinte comando na raiz do projeto:

```bash
docker compose up --build -d api
```

Com isso, o docker irá buildar a imagem da API e subir todos os correlatos para que seja possível utilizar este microsserviço.

## Logs

Esta aplicação está preparada para utilizar o Serilog para gerar os logs. Para isso, devem ser adicionadas as seguintes configurações no appsettings.json

```json
  "SerilogSettings":{
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { 
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {TraceId} {SpanId} {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  },
```

## Open Telemetry

Esta aplicação possui OpenTelemetry configurado. Utilizando o docker compose, já estará disponível o Aspire Dashboard para, em ambiente de desenvolvimento,
verificar as métricas do serviço.

Para acessar o Aspire Dashboard basta acessar a URL "<http://localhost:18888>". Dentro do arquivo .env será configurada as portas para acesso. As mesmas podem
ser alteradas caso necessário.

```javascript
DASHBOARD_PORT=18888
OTLP_GRPC_PORT=18889
OTLP_HTTP_PORT=18890

OTEL_ENDPOINT=http://gameService-dashboard:18889
```

## Uso

Abaixo algumas explicações sobre a utilização desta API. Maiores detalhes podem ser consultados através do Swagger disponibilizado.

### Url Endpoints

Todos os endpoints da API possuem como padrão a url "<http://url-da-api/api/endpoint>". Através desses endpoints será possível realizar o controle de cada uma das informações disponíveis na aplicação.

### Controle de acesso

Esta API receberá as requisições através de uma API Gateway. Deste modo, o controle de acesso aos endpoints deve ser realizado através da API Gateway.

## Swagger

Esta aplicação foi desenvolvida utilizando o Swagger como documentação. Para acessar a documentação, basta acessar a URL <http://url-da-api/docs>

## Testes

Ao clonar o repositório desta API, um projeto "UnitTests" será baixado também. Neste projeto ficarão todos os testes automatizados da aplicação, que poderão ser configurados para executar em uma pipeline.

## Contribuição

Esta API foi desenvolvida por:

- [Leonardo Dick Bernardes](http://github.com/oleonardodick)
- [Rodrigo Vedovato](https://github.com/guigovedovato)
