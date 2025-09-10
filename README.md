# Plataforma de Seguros - Arquitetura Hexagonal e CQRS

Este repositório contém a implementação de uma plataforma de seguros simplificada, desenvolvida com uma arquitetura moderna e robusta baseada em Microserviços, DDD, Arquitetura Hexagonal e CQRS.

O projeto permite criar e gerenciar propostas de seguro. Quando uma proposta é aprovada, um evento é disparado para que o serviço de contratação processe a efetivação do seguro de forma assíncrona.


````
+--------------------------------------------------------------------------------------------------+
|  🏦 Plataforma de Seguros                                                                        |
+--------------------------------------------------------------------------------------------------+
|                                                                                                  |
|   Propostas de Seguro                                                        [ + Nova Proposta ] |
|  +---------------------------------------------------------------------------------------------+ |
|  | NOME DO CLIENTE ▾ | VALOR DO SEGURO |    STATUS    |      MENSAGEM   |          AÇÕES       | |
|  |-------------------+-----------------+--------------+-----------------|----------------------+ |
|  | João da Silva     | R$ 2.500,00     | [ Aprovada ] |                 | [Editar]             | |
|  |-------------------+-----------------+--------------+-----------------|----------------------+ |
|  | Maria Oliveira    | R$ 1.800,00     | [Em Análise] |                 | [Aprovar][Rejeitar]| | |
|  |-------------------+-----------------+--------------+-----------------|----------------------+ |
|  | Pedro Martins     | R$ 3.200,00     | [ Rejeitada ]|                 |                      | |
|  +----------------------------------------------------------------------+----------------------+ |
|                                                                                                  |
+--------------------------------------------------------------------------------------------------+
````


Keywords: `Microserviços` `Arquitetura Hexagonal` `DDD` `CQRS` `MediatR` `C#` `.NET 8` `ASP.NET Core` `APIs REST` `Blazor WebAssembly (Standalone)` `Entity Framework Core` `Dapper` `Mensageria` `RabbitMQ` `MassTransit` `SQL Server` `Migrations` `xUnit` `Teste unitário` `FluentAssertions` `Moq` `Docker` `Docker Compose`


## :books: Arquitetura e Tecnologias

A solução é composta por uma aplicação web e dois microserviços de backend, orquestrados para rodar em contêineres `Docker`.

* **Padrões de Arquitetura**: `Microserviços`, `Arquitetura Hexagonal (Ports & Adapters)`, `DDD`, `CQRS` com `MediatR`.
* **Backend**: `C#` com `.NET 8`, `ASP.NET Core` para APIs REST.
* **Frontend**: `Blazor WebAssembly (Standalone)`.
* **Persistência**:
    * **`EF Core`**: Para o lado de escrita (Commands), garantindo consistência e regras de negócio.
    * **`Dapper`**: Para o lado de leitura (Queries), garantindo máxima performance.
* **Mensageria**: `RabbitMQ` para comunicação assíncrona entre os serviços, `MassTransit` (abstração para mensageria no .NET).
* **Banco de Dados**: `SQL Server` com uso de `Migrations`.
* **Testes**: `xUnit` (framework de testes), `FluentAssertions` (para asserções legíveis), `Moq` (para mocking de dependências)
* **Containerização**: `Docker` e `Docker Compose`.

### Diagrama da Arquitetura Final

O diagrama abaixo ilustra a interação entre todos os componentes do sistema, representando a arquitetura final:

```mermaid
graph TD
    subgraph "Cliente (Swagger / Postman / WebApp)"
        User["Usuário"]
    end

    subgraph PropostaService ["PropostaService (Porta 8080)"]
        style PropostaService fill:#e3f2fd,stroke:#333,stroke-width:2px
        Controller_P["API REST <br> PropostasController"]
        Mediator_P["MediatR"]
        Commands["Handlers de Comando <br> Criar, Aprovar, etc. <br> Usando EF Core"]
        Queries_P["Handlers de Query <br> Listar <br> Usando Dapper"]
        Publisher["Publicador de Eventos <br> MassTransit"]
        
        Controller_P -- Envia Command/Query --> Mediator_P
        Mediator_P --> Commands
        Mediator_P --> Queries_P
        Commands -- Dispara Evento --> Publisher
    end

    subgraph ContratacaoService ["ContratacaoService (Worker e API)"]
        style ContratacaoService fill:#e8f5e9,stroke:#333,stroke-width:2px
        
        %% Adicionando o fluxo de API que faltava %%
        Controller_C["API REST <br> ContratacoesController"]
        Mediator_C["MediatR"]
        Queries_C["Handlers de Query <br> (Listar) <br> Usando Dapper"]
        
        %% Componentes de Worker existentes %%
        Consumer["Consumidor de Eventos <br> MassTransit"]
        Logic["Lógica de Contratação <br> Usando EF Core"]
        
        Controller_C -- Envia Query --> Mediator_C
        Mediator_C --> Queries_C
        Consumer --> Logic
    end
    
    subgraph Infra ["Infraestrutura Compartilhada"]
        style Infra fill:#fbe9e7,stroke:#333,stroke-width:2px
        DB[("SQL Server DB")]
        RabbitMQ["RabbitMQ <br> Fila: contratacao-queue"]
    end
    
    %% Conexões do Cliente %%
    User -- Requisição HTTP para Propostas --> Controller_P
    User -- Requisição HTTP para Contratações --> Controller_C

    %% Conexões do PropostaService %%
    Commands -- Salva/Altera Proposta --> DB
    Queries_P -- Lê Propostas --> DB
    Publisher -- Publica Mensagem --> RabbitMQ
    
    %% Conexões do ContratacaoService %%
    RabbitMQ -- Entrega Mensagem --> Consumer
    Logic -- Salva Contratação --> DB
    Queries_C -- Lê Contratações --> DB
```

<details>
	<summary> Se não for possivel visualizar o diagrama, clique aqui</summary>

![Diagrama da Arquitetura Final](https://github.com/roodriiigooo/InsurancePlatform/blob/main/.assets/_Mermaid_Chart-2025-08-28-183320.png?raw=true)

 
</details>



## Descricão geral do projeto


📌 O que é o sistema?

O sistema é uma aplicação distribuída para gestão de propostas e contratações.
Ele é composto por dois serviços principais (`PropostaService` e `ContratacaoService`) que interagem via mensageria (`RabbitMQ`) e usam um banco de dados SQL Server como persistência compartilhada.

A ideia central:
O usuário (via `Swagger`, `Postman` ou `WebApp`) faz requisições para criar, aprovar ou consultar propostas.
As propostas geram eventos que disparam um fluxo de contratação assíncrono, tratado por outro serviço.

📌 Arquitetura geral

O sistema segue princípios de arquitetura em camadas e comunicação assíncrona.
Temos três blocos principais:

	 1. Cliente (Swagger / Postman / WebApp)
 		- Onde o usuário interage via requisições HTTP.
   		- Pode ser um sistema externo, um frontend ou ferramenta de testes.
	 2. Serviços de Negócio
  		- PropostaService (porta 8080)
			- Camada exposta via API REST.
   			- Responsável por criar, aprovar e consultar propostas.
	  		- Usa MediatR para aplicar o padrão CQRS (separação de comandos e queries).
	 		  - Commands (handlers de escrita) → usam EF Core para salvar/alterar propostas.
	  		  - Queries (handlers de leitura) → usam Dapper para consultas otimizadas.
	   		- Quando ocorre uma alteração relevante (ex: aprovação de proposta), um evento é publicado via MassTransit para o RabbitMQ.
	    - ContratacaoService (Worker)
	 		- Expõe API HTTP (para melhor visualização), porém é um serviço background (worker).
			- Consome eventos do RabbitMQ (ex: "PropostaAprovada").
   			- Processa a lógica de contratação usando EF Core.
	  		- Persiste as informações de contratação no mesmo SQL Server.
	 3. Infraestrutura Compartilhada
  		- Banco de Dados (SQL Server)
			- Usado tanto para propostas quanto para contratações.
   		- RabbitMQ
	 		- Fila "contratacao-queue" que transporta mensagens/eventos entre os serviços.

📌 Fluxo resumido (exemplo: criar e aprovar proposta):

- O usuário envia uma requisição `HTTP` (`POST` /propostas).
- A `API REST` (`Controller`) recebe e envia o comando ao Mediator.
- O Mediator encaminha para o handler de Command (`EF Core`) que grava a proposta no banco.
- Quando a proposta é aprovada, o handler dispara um evento para o Publisher (`MassTransit`).
- O Publisher envia o evento para o `RabbitMQ`.
- O Consumer (`ContratacaoService`) recebe a mensagem.
- O ContratacaoService executa a lógica de contratação e grava no banco (`EF Core`).
- O fluxo se conclui de forma assíncrona, sem bloquear a experiência do usuário.


📌 Padrões e boas práticas aplicadas

`CQRS` (Command Query Responsibility Segregation):
- Separação clara entre operações de escrita (`Commands + EF Core`) e leitura (`Queries + Dapper`).

Mediator Pattern (via `MediatR`):
- Evita que os Controllers chamem diretamente os handlers, centralizando a orquestração.

`Event-driven Architecture` (`EDA`):
- A contratação é disparada por eventos publicados no `RabbitMQ`, promovendo baixo acoplamento entre serviços.

`MassTransit`:
- Abstrai a comunicação com o `RabbitMQ`, simplificando publicação e consumo de mensagens.

Banco relacional centralizado (`SQL Server`):
- Usado como persistência confiável, tanto para propostas quanto contratações.


📌 Benefícios dessa arquitetura

Escalabilidade:
- O PropostaService pode escalar horizontalmente para atender mais requisições HTTP.
- O ContratacaoService pode escalar para consumir mais mensagens da fila.

Resiliência:
- Se o ContratacaoService estiver fora do ar, as mensagens ficam retidas no RabbitMQ até ele voltar.

Separação de responsabilidades:
- PropostaService → gestão de propostas.
- ContratacaoService → fluxo de contratação.

Flexibilidade:
- Fácil adicionar novos consumidores de eventos no futuro (ex: faturamento, notificação, auditoria).


👉 Em resumo:
O sistema é uma aplicação orientada a eventos, que aplica CQRS para manipulação de dados e usa RabbitMQ + MassTransit para integração assíncrona entre serviços. Ele garante desacoplamento, escalabilidade e manutenibilidade, sendo ideal para cenários de alto volume de propostas e contratações.



📌 Fluxo narrado
- O usuário chama `POST` `/api/propostas` → cria proposta pendente no banco.
- O usuário chama `POST` `/api/propostas/{id}/aprovar`.
- O handler muda status para "Aprovada".
- Publica evento PropostaAprovadaEvent no `RabbitMQ`.
- O ContratacaoService consome o evento da fila.
- Cria uma nova Contratação no banco.
- Se o usuário quiser consultar, chama `GET` `/api/propostas` e o `Dapper` retorna os registros.




## :books: Pré-requisitos

> [!IMPORTANT]
> Para executar este projeto, você precisará ter instalado:

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) (ou outra IDE de sua preferência)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* [Postman](https://www.postman.com/downloads/) (opcional para testes de API)


## Estrutura do Projeto

O projeto está organizado da seguinte forma:
```
/docker-compose.yml
/InsurancePlatform.sln
/src
|-- PropostaService
|   |-- PropostaService.Api/              (Driving Adapter: Controllers, Program.cs)
|   |-- PropostaService.Application/      (Application Core: Use Cases, DTOs, Ports)
|   |-- PropostaService.Domain/           (Domain Core: Entities, Enums, Business Rules)
|    `-- PropostaService.Infrastructure/  (Driven Adapters: Repository, EF Core)
|
`-- ContratacaoService
|   |-- ContratacaoService.Api/
|   |-- ContratacaoService.Application/
|   |-- ContratacaoService.Domain/
|    `--ContratacaoService.Infrastructure/
|
`-- Shared
|   |-- InsurancePlatform.Contracts
|    `-- InsurancePlatform.Shared
|
`-- WebApp
|   `-- InsurancePlatform.WebApp
|
`-- tests
|-- | -- PropostaService.UnitTests/
     `-- ContratacaoService.UnitTests/
```



# :writing_hand: Como Executar o Projeto Completo

**Primeiro clone o repositório**

```powershell
git clone https://github.com/roodriiigooo/InsurancePlatform.git
cd InsurancePlatform
```


## :bookmark_tabs: **Opção 1**: Ambiente Completo com Docker Compose
> [!TIP]
> A maneira mais simples e recomendada de executar toda a stack é utilizando Docker Compose.


1.  **Inicie os contêineres**
    Na raiz do projeto (onde o arquivo `docker-compose.yml` está localizado), execute o seguinte comando:
    ```powershell
    docker-compose up --build -d
    ```
    O comando `--build` garante que as imagens Docker para os seus serviços serão construídas. Na primeira vez, isso pode levar alguns minutos.

2.  **Acesse os Serviços**
após a conclusão do build e a inicialização dos contêineres, os serviços estarão disponíveis nos seguintes endereços:

    * 🌐 **Aplicação Web (Frontend)**: [http://localhost:8082](http://localhost:8082)
    * ⚙️ **API do PropostaService**: [http://localhost:8080](http://localhost:8080) / [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html) 
	* ⚙️ **API do ContratacoService**: [http://localhost:8081](http://localhost:8081) / [http://localhost:8081/swagger/index.html](http://localhost:8081/swagger/index.html) 
    * 🐇 **RabbitMQ Management UI**: [http://localhost:15672](http://localhost:15672) (login: `guest` / senha: `guest`)



## :bookmark_tabs: **Opção 2**: Ambiente Híbrido (Debug com Visual Studio)
> [!TIP]
> Esta abordagem é ideal para o desenvolvimento e depuração do código .NET.


1. **Inicie a Infraestrutura**: Abra um terminal na pasta raiz do projeto e inicie o banco de dados e o RabbitMQ em segundo plano com o Docker:

````powershell
docker-compose up -d proposta-db rabbitmq
````

2. **Abra no Visual Studio**: Abra o arquivo `InsurancePlatform.sln` no `Visual Studio`.
3. **Configure a Inicialização Múltipla:**
   - Clique com o botão direito na Solução > "**Definir Projetos de Inicialização...**"
   - Marque "Vários projetos de inicialização".
   - Defina a Ação como "Iniciar" para os três projetos: `PropostaService.Api`, `ContratacaoService.Api` e `InsurancePlatform.WebApp`.
4. **Execute**: Pressione `F5` ou o botão "`Iniciar`". O Visual Studio irá compilar e iniciar os três projetos.

5.  **Acesse os Serviços**
após a conclusão do build e a inicialização dos contêineres, os serviços estarão disponíveis nos seguintes endereços:

    * 🌐 **Aplicação Web (Frontend)**: [http://localhost:7189](http://localhost:7189)
    * ⚙️ **API do PropostaService**: [http://localhost:7999](http://localhost:7999) / [http://localhost:7999/swagger/index.html](http://localhost:7999/swagger/index.html) 
	* ⚙️ **API do ContratacoService**: [http://localhost:7285](http://localhost:7285) / [http://localhost:7285/swagger/index.html](http://localhost:7285/swagger/index.html) 
    * 🐇 **RabbitMQ Management UI**: [http://localhost:15672](http://localhost:15672) (login: `guest` / senha: `guest`)

> [!NOTE]
> A WebApp estará acessível no seu endereço de debug (ex: https://localhost:7189).
> A API do PropostaService estará no seu endereço de debug (ex: https://localhost:7999).
> A API do ContratacaoService estará no seu endereço de debug (ex: https://localhost:7285).


# :page_with_curl: Como Executar os Testes Unitários

Para rodar a suíte de testes unitários, execute o seguinte comando na raiz do projeto:
```bash
dotnet test
```
ou no `Visual Studio`, vá em `Teste` > `Executar todos os Testes`


# :art: Screenshots
<details>
	<summary># 1. WebApp</summary>

 
![WebApp](https://github.com/roodriiigooo/InsurancePlatform/blob/main/.assets/_WebAPP.PNG?raw=true)


</details>

<details>
	<summary># 2. Testes</summary>

 
![Testes](https://github.com/roodriiigooo/InsurancePlatform/blob/main/.assets/_tests.PNG?raw=true)


</details>

 
<details>
	<summary># 3. PropostaService.Api</summary>
 
![PropostaService.Api](https://github.com/roodriiigooo/InsurancePlatform/blob/main/.assets/_PropostaServiceAPI_swagger.PNG?raw=true)


</details>


 
<details>
	<summary># 4. ContratacaoService.Api</summary>


![ContratacaoService.Api](https://github.com/roodriiigooo/InsurancePlatform/blob/main/.assets/_ContratacaoServiceAPI_swagger.PNG?raw=true)


</details>





