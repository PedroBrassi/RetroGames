# RetroGames

RetroGames é um projeto acadêmico de uma loja web de jogos retrô usados. O escopo desta fase é o módulo de **Catálogo e Administração**. O catálogo terá busca, filtros por plataforma e gênero, paginação e detalhes dos jogos. A área administrativa terá autenticação e manutenção de jogos e plataformas.

## Integrantes

| Nome | Matrícula |
| --- | --- |
| Aurélio Junior | `2024.2.08.008` |
| Pedro Brassi Luccas | `2022.2.08.009` |
| Wagner Donizete Gonçalves | `2019.1.08.047` |

## O que já foi implementado

| Área | Estado atual |
| --- | --- |
| Solução .NET | `RetroGames.sln` com seis projetos .NET 10 e as referências entre eles. |
| Banco de dados | Sete tabelas MySQL com chaves, restrições e índice de nome de jogo. Os dados de exemplo incluem 6 plataformas, 8 gêneros e 20 jogos. |
| Consultas SQL | Catálogo ativo, gêneros de um jogo, jogos por gênero, contagem por plataforma e busca paginada. Os scripts também permitem verificar a integridade do banco. |
| Modelos | Sete classes C#, uma por tabela, com as constantes de condição de jogo e perfil. |
| Acesso a dados | Repositórios de plataformas, jogos, gêneros, condições e vínculos jogo–gênero reunidos no `RepositorySet`. Consultas parametrizadas, paginação no MySQL e tratamento de duplicidade. |
| Regras de negócio | Services de plataformas, gêneros, condições e jogos. Incluem regras de cadastro, montagem dos detalhes do catálogo e transações para gravar jogos e seus vínculos. |
| API | ASP.NET Core na porta 5080, configuração pelo `.env`, Swagger em desenvolvimento e consultas de plataformas pelo MySQL. Respostas em envelope e tratamento global de exceções. |
| Front-end | Projeto React 19 com Vite 8, em JavaScript, com a tela inicial do template. |
| Testes .NET | Projeto xUnit v3 com o teste de exemplo do template. |

As consultas HTTP de gêneros, condições e jogos, as rotas de gravação, a autenticação e a integração das telas com a API ainda estão em desenvolvimento. O front-end já pode ser iniciado separadamente.

## Estrutura

| Caminho | Responsabilidade |
| --- | --- |
| [`database/`](database/) | Criação do banco, dados de exemplo, consultas e testes de integridade |
| [`RetroGames.Model/`](RetroGames.Model/) | Modelos das tabelas e constantes |
| [`RetroGames.Repository/`](RetroGames.Repository/) | Acesso ao MySQL com ADO.NET e MySqlConnector |
| [`RetroGames.Services/`](RetroGames.Services/) | Regras de negócio e composição dos dados do catálogo |
| [`RetroGames.WebApi.ApiModels/`](RetroGames.WebApi.ApiModels/) | Envelope de resposta, códigos da API e contratos JSON |
| [`RetroGames.WebApi/`](RetroGames.WebApi/) | Aplicação ASP.NET Core |
| [`RetroGames.WebApi.UnitTests/`](RetroGames.WebApi.UnitTests/) | Projeto de testes do back-end |
| [`retrogames.web/`](retrogames.web/) | Aplicação React |

## Como rodar localmente

Os comandos abaixo são para **Bash no Ubuntu**, inclusive no **WSL** em Windows. Use **Git**, **.NET SDK 10**, **Node.js 22 LTS (22.12 ou superior)** com **npm** e **MySQL Server 8**.

### 1. Clone o repositório

```bash
git clone https://github.com/PedroBrassi/RetroGames.git
cd RetroGames
```

### 2. Prepare o banco de dados

Se o MySQL ainda não estiver instalado no Ubuntu 24.04, instale e inicie o serviço.

```bash
sudo apt update
sudo apt install mysql-server
sudo service mysql start
```

Em uma instalação nova, execute os scripts em ordem a partir da raiz do repositório.

```bash
sudo mysql < database/001_criar_tabelas.sql &&
sudo mysql < database/002_inserir_dados_exemplo.sql &&
sudo mysql retrogames -e 'SHOW TABLES; SELECT COUNT(*) AS jogos FROM tb_jogo;'
```

A conferência deve mostrar sete tabelas e 20 jogos. Se as tabelas já existirem e estiverem vazias, execute apenas o script 002. Se o banco já estiver populado, pule os dois scripts de inicialização.

### 3. Prepare a configuração de conexão

A API lê o `.env` na inicialização. O `.env.example` tem o modelo de conexão. Para preparar a conta padrão do exemplo, entre no console do MySQL.

```bash
sudo env MYSQL_HISTFILE=/dev/null mysql
```

No console, substitua `SUA_SENHA` por uma senha local.

```sql
CREATE USER 'retrogames_dev'@'127.0.0.1' IDENTIFIED BY 'SUA_SENHA';
GRANT SELECT, INSERT, UPDATE, DELETE ON retrogames.* TO 'retrogames_dev'@'127.0.0.1';
EXIT;
```

Copie o exemplo para `.env` e substitua `SUA_SENHA` pela senha da conta. Se já tiver um usuário MySQL para o projeto, ajuste os campos da conexão. O `.env` fica fora do Git.

```bash
cp -n .env.example .env
nano .env
git check-ignore .env
```

Para conferir a conta padrão pelo mesmo endereço da configuração.

```bash
mysql -h 127.0.0.1 -P 3306 -u retrogames_dev -p retrogames -e 'SELECT CURRENT_USER() AS usuario, DATABASE() AS banco;'
```

### 4. Compile e execute a API

Na raiz do repositório.

```bash
dotnet build RetroGames.sln
dotnet test RetroGames.sln --no-build
dotnet run --project RetroGames.WebApi --launch-profile http
```

Com a API rodando, abra o [Swagger](http://localhost:5080/swagger) para executar as consultas. O contrato OpenAPI também fica disponível em [`/openapi/v1.json`](http://localhost:5080/openapi/v1.json).

| Requisição disponível | Resultado esperado |
| --- | --- |
| `GET /api/Plataforma` | HTTP 200, com as seis plataformas de exemplo em `data` |
| `GET /api/Plataforma/1` | HTTP 200, com os dados do NES / Famicom |
| `GET /api/Plataforma/99` | HTTP 404, com `responseCode: 100` e a mensagem de plataforma inexistente |

As respostas de sucesso usam `responseCode: 0`. Falhas inesperadas retornam HTTP 500 com `responseCode: 1` e uma mensagem genérica. Os detalhes da falha ficam no log da API.

### 5. Execute o front-end

Em outro terminal, a partir da raiz do repositório.

```bash
cd retrogames.web
npm ci
npm run dev -- --port 5173 --strictPort
```

Abra [http://localhost:5173](http://localhost:5173) para ver a tela inicial do projeto React. Para conferir a compilação de produção, rode `npm run build` nessa mesma pasta.

## Verificações do banco

Com os dados de exemplo carregados, rode os scripts abaixo a partir da raiz.

```bash
sudo mysql < database/003_consultas_catalogo.sql
sudo mysql --force < database/004_testes_integridade.sql
```

O script de integridade provoca quatro rejeições esperadas do MySQL (**1452, 1062, 1451 e 1048**) e verifica que o banco aceita preço zero. A regra de preço pertence à camada Service. O `--force` permite seguir até o `ROLLBACK`, que desfaz a inserção temporária. A contagem final de jogos de teste deve ser zero.
