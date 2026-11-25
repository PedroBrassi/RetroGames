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
| Acesso a dados | Base com MySqlConnector, comandos parametrizados e tratamento do erro de duplicidade. Repositórios de plataformas, jogos e vínculos jogo–gênero implementados. |
| API | Projeto ASP.NET Core com controllers. A rota de exemplo do template está disponível para testar a inicialização. |
| Front-end | Projeto React 19 com Vite 8, em JavaScript, com a tela inicial do template. |
| Testes .NET | Projeto xUnit v3 com o teste de exemplo do template. |

A próxima etapa é implementar os repositórios de gêneros e condições e reuni-los no `RepositorySet`. Depois vêm as regras de negócio, os endpoints do catálogo, a autenticação e as telas integradas. Hoje, a API e o front-end podem ser iniciados separadamente para verificar o ambiente.

## Estrutura

| Caminho | Responsabilidade |
| --- | --- |
| [`database/`](database/) | Criação do banco, dados de exemplo, consultas e testes de integridade |
| [`RetroGames.Model/`](RetroGames.Model/) | Modelos das tabelas e constantes |
| [`RetroGames.Repository/`](RetroGames.Repository/) | Acesso ao MySQL com ADO.NET e MySqlConnector |
| [`RetroGames.Services/`](RetroGames.Services/) | Projeto reservado à camada de regras de negócio |
| [`RetroGames.WebApi.ApiModels/`](RetroGames.WebApi.ApiModels/) | Projeto reservado aos contratos da API |
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

O `.env.example` descreve a conexão local que será usada quando a API for integrada ao banco. Para preparar a conta padrão do exemplo, entre no console do MySQL.

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

Com a API rodando, abra [http://localhost:5259/weatherforecast](http://localhost:5259/weatherforecast). Esse é o endpoint de exemplo da estrutura inicial. A leitura do `.env` pela API e as rotas do RetroGames ficam para a integração.

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
