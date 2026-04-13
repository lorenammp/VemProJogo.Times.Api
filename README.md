# VemProJogo.TimesService

API do microsservico responsavel pelos dados de times e jogadores do projeto **Vem Pro Jogo**.

Nesta etapa, o servico atende principalmente:

- **RF-03**: criação e gerenciamento de times
- **RF-04**: gerenciamento de jogadores associados a um time especifico
- **RF-13**: registro de estatisticas individuais de jogadores por partida

## Escopo da API

O que esta entrega cobre:

- CRUD de times
- CRUD de jogadores dentro de cada time
- registro, consulta, edicao e remocao de estatisticas individuais por partida
- persistencia em MongoDB com documentos agregados por time

## Arquitetura da solucao

O microsservico segue uma separacao em camadas:

- `VemProJogo.Times.Api`: contratos HTTP, controllers e middlewares
- `VemProJogo.Times.Application`: casos de uso, DTOs, regras de negocio e interfaces
- `VemProJogo.Times.Domain`: entidades e constantes de dominio
- `VemProJogo.Times.Infrastructure`: persistencia MongoDB, indices e composicao de dependencias

Melhorias recentes de robustez:

- middleware global de excecoes retornando `ProblemDetails`
- validacao de payload de `times` com DataAnnotations + regras de negocio
- tratamento explicito de conflitos de chave unica no MongoDB
- endpoint de saude `GET /health`

## Modelagem NoSQL

O microsservico usa a colecao `times` e armazena os jogadores de forma embutida em cada documento de time.

Estrutura logica principal:

- `campeonatoId`
- `nome`
- `sigla`
- `responsavelNome`
- `responsavelContato`
- `escudoUrl`
- `ativo`
- `players[]`

Cada item de `players[]` contem:

- `id`
- `name`
- `nickname`
- `number`
- `position`
- `active`
- `matchStats[]`

Cada item de `matchStats[]` contem:

- `id`
- `matchId`
- `goals`
- `assists`
- `yellowCards`
- `redCards`
- `minutesPlayed`
- `notes`

## Justificativa da modelagem

- jogadores ficam embutidos no time porque a principal relacao da RF-04 e a associacao direta com um unico time
- estatisticas ficam embutidas no jogador porque a RF-13 exige consulta e manutencao concentradas no historico individual
- essa estrutura reduz joins aplicacionais e simplifica a recuperacao completa do elenco

## Endpoints principais

| Metodo | Rota | Finalidade |
| --- | --- | --- |
| `GET` | `/api/times` | Lista times |
| `GET` | `/api/times/{id}` | Consulta um time |
| `POST` | `/api/times` | Cria um time |
| `PUT` | `/api/times/{id}` | Atualiza um time |
| `PATCH` | `/api/times/{id}` | Atualiza parcialmente um time |
| `DELETE` | `/api/times/{id}` | Remove um time |
| `GET` | `/api/times/{timeId}/jogadores` | Lista jogadores do time |
| `GET` | `/api/times/{timeId}/jogadores/{playerId}` | Consulta um jogador |
| `POST` | `/api/times/{timeId}/jogadores` | Cadastra um jogador no time |
| `PUT` | `/api/times/{timeId}/jogadores/{playerId}` | Atualiza um jogador |
| `DELETE` | `/api/times/{timeId}/jogadores/{playerId}` | Remove um jogador |
| `GET` | `/api/times/{timeId}/jogadores/{playerId}/estatisticas` | Lista estatisticas do jogador |
| `POST` | `/api/times/{timeId}/jogadores/{playerId}/estatisticas` | Registra estatisticas por partida |
| `PUT` | `/api/times/{timeId}/jogadores/{playerId}/estatisticas/{statId}` | Atualiza estatisticas registradas |
| `DELETE` | `/api/times/{timeId}/jogadores/{playerId}/estatisticas/{statId}` | Remove estatisticas registradas |

## Padrao de erros HTTP

Quando ocorre falha de negocio ou validacao, a API retorna `application/problem+json`.

- `400`: erro de validacao de entrada
- `404`: recurso nao encontrado
- `409`: conflito de negocio (ex.: duplicidade de time no mesmo campeonato)
- `500`: erro interno inesperado

## Exemplo de payload para cadastro de times

```json
{
  "championshipId": "67e3f1b2a4c98d1234567890",
  "name": "Tubarões FC",
  "acronym": "TFC",
  "responsibleName": "Thiago Almeida",
  "responsibleContact": "(31) 99876-1234",
  "crestUrl": "https://exemplo.com/escudos/tubaroes-fc.png"
}
```

## Exemplo de payload para cadastro de jogador

```json
{
  "name": "Lucas Lima",
  "nickname": "Lukinha",
  "number": 10,
  "position": "meia"
}
```

## Exemplo de payload para estatisticas

```json
{
  "matchId": "507f1f77bcf86cd799439012",
  "goals": 2,
  "assists": 1,
  "yellowCards": 0,
  "redCards": 0,
  "minutesPlayed": 90,
  "notes": "Participacao decisiva na vitoria"
}
```

## Persistência e hospedagem

Durante esta etapa, o microsservico passou a contar com:

- persistencia de dados em MongoDB
- configuracao de banco em nuvem utilizando MongoDB Atlas
- deploy da API em ambiente externo utilizando a plataforma Render

A API publicada pode ser acessada por meio do endereco:

```
https://vemprojogo-times-api.onrender.com/
```

Endpoint principal de times:

```
https://vemprojogo-times-api.onrender.com/api/times
```

## Execução local

Para executar o microsservico localmente, e necessario:

- ter o .NET SDK instalado
- configurar o MongoDB localmente ou utilizar uma connection string valida do MongoDB Atlas
- restaurar as dependencias da solucao
- executar o projeto VemProJogo.Times.Api

Exemplo de execucao:

```
dotnet restore
dotnet run --project VemProJogo.Times.Api
```

## Relacao com o RF-10

A tabela de classificacao do campeonato foi mantida no `RankingsService`, que agora aceita o tipo de ranking `times` para representar a classificacao materializada por equipes.
