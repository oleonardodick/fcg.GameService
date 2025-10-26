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
- [Localstack](#localstack)
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

Este projeto utiliza o ElasticSearch para algumas funcionalidades. Para configurar este serviço, deve ser criado um cluster elastic para
ser apontado no arquivo .ENV desta aplicação.

## Como rodar o projeto?

### Via Docker Compose

Foi disponibilizado neste repositório um arquivo docker-compose.yml para que seja possível buildar e rodar a aplicação, já apontando para
as variáveis de ambiente necessárias.

Aconselha-se o uso do docker-compose para rodar o projeto.

Para isso, deve ser disponibilizado na raiz do projeto um arquivo .env com a seguinte estrutura:

```javascript
  ASPNETCORE_ENVIRONMENT: "Development"

  # MongoDB configuration
  MongoDbSettings__ConnectionString: "<MongoConnectionString>"
  MongoDbSettings__DatabaseName: "<MongoDatabaseName>"

  # ElasticSettings configuration
  ElasticSettings__ApiKey: "<Api Key>"
  ElasticSettings__CloudId: "<Cloud ID>"

  # Azure configuration
  AzureStorage__ConnectionString: "<Azure Connection>"
  AzureStorage__ProducerQueueName: "<Producer queue name>"
  AzureStorage__ConsumerQueueName: "<Consumer queue name>"

  # Open telemetry configuration
  OTEL_RESOURCE_ATTRIBUTES: "<Resource attributes>"
  OTEL_EXPORTER_OTLP_ENDPOINT: "<Endpoint>"
  OTEL_EXPORTER_OTLP_HEADERS: "<Authorization>"
  OTEL_EXPORTER_OTLP_PROTOCOL: "<Protocol>"
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
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore.Hosting.Diagnostics": "Information",
        "Microsoft.AspNetCore.Routing.EndpointMiddleware": "Warning",
        "Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware": "Warning",
        "System": "Warning",
        "System.Net.Http.HttpClient": "Warning",
        "MongoDB.Driver": "Information"
      }
    },
    "Filter": [
      {
        "Name": "ByExcluding",
        "Args": {
          "expression": "RequestPath like '/swagger%'"
        }
      },
      {
        "Name": "ByExcluding",
        "Args": {
          "expression": "RequestPath like '/health%'"
        }
      }
    ]
  },
```

## Open Telemetry

Esta aplicação possui OpenTelemetry configurado. Para enviar as métricas, basta adicionar as variáveis de ambiente citadas
neste documento.

## Localstack

Para facilitar o teste de algumas funcionalidades desta aplicação, aconselha-se a utilizar o localstack.

Para isso, basta realizar a [Instalação](https://docs.localstack.cloud/aws/getting-started/installation/) da ferramenta, e o cadastro. Após a instação e o cadastro, basta adicionar no appSettings a seguinte configuração, para que a aplicação já se conecte com a stack do localstack:

```json
  "AWSSettings": {
    "Region": "us-east-1",
    "AccessKey": "teste",
    "SecretKey": "teste",
    "ServiceURL": "http://localhost:4566",
    "SQS": {
      "GamePurchaseRequested": "game-purchase-requested",
      "GamePurchaseCompleted": "game-purchase-completed"
    }
  }
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
