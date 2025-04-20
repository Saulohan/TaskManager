# 📝 TaskManager

O **TaskManager** é uma aplicação para gerenciamento de projetos. Ele permite que equipes e indivíduos organizem seu dia a dia de forma produtiva, com recursos como criação de projetos, tarefas e geração de relatórios de desempenho.

- [Link Repositorio github](https://github.com/Saulohan/TaskManager)

---

## 📦 Tecnologias Utilizadas

- ![.NET](https://img.shields.io/badge/.NET-8-blue)
- ![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-lightgrey)
- ![XUnit](https://img.shields.io/badge/Testes-xUnit%2FMoq-green)
- ![Docker](https://img.shields.io/badge/Docker-Enabled-blue)

---

## 🚀 Como Rodar Localmente

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/)
- [Git](https://git-scm.com/)

## 🚀 Execução com Docker

Você pode executar a aplicação de duas formas: **construindo localmente** ou **usando a imagem publicada no Docker Hub**.

### 🔨 Opção 1: Construindo a Imagem Localmente

```bash
# Clone o repositório
git clone https://github.com/Saulohan/TaskManager.git

# Construa a imagem Docker localmente
docker build -t saulohan/taskmanager .

# Execute o container
docker run -d -p 8080:8080 --name taskmanager saulohan/taskmanager
```

### 🔨 Opção 2: Usando Imagem do Docker Hub
```bash
# Execute diretamente com a imagem publicada
docker run -d -p 8080:8080 --name taskmanager saulohan/taskmanager
```

### 🌐 Acesse a Aplicação
- Interface Swagger: http://localhost:8080/swagger
- Testes via Postman: http://localhost:8080

---

### 🔍 Refinamento

Durante o refinamento do produto, algumas perguntas importantes que poderiam ser feitas ao Product Owner (PO) para guiar futuras implementações e evoluções:

1. Haverá controle de prazos com alertas automáticos para tarefas com vencimento próximo?
2. Deseja-se funcionalidade de notificação (e-mail) quando tarefas forem atribuídas, comentadas ou modificadas?
3. Deseja a possibilidade de arquivamento de projetos concluídos?
4. Deseja a possibilidade de exportação de dados (ex: PDF, CSV)?

---

## 🛠️ Possíveis Melhorias

Durante o desenvolvimento do projeto, identifiquei algumas oportunidades de melhorias técnicas e arquiteturais para uma futura evolução:

### Arquitetura

- Adoção de **Clean Architecture** para melhor separação de responsabilidades e escalabilidade.
- Considerar a utilização de **RabbitMQ**, permitindo o uso de mensageria por microserviços futuros.
- Criar **transações** para que a API utilize `commit` e `rollback`, garantindo consistência em operações críticas.

### Funcionalidades

- O método de atualização de tarefas poderia permitir a alteração de mais campos, como o **título** e a **data de vencimento**.

### Observabilidade

- Utilizar **Elasticsearch** para salvar log e melhorar nosso rastreio e análise.
- Implementação de **Health Checks** e **monitoramento** com Prometheus + Grafana.

### Escalabilidade e DevOps

- Docker já implementado, mas é possível evoluir para orquestração com **Kubernetes**.
- Externalização de configurações com Azure Key Vault.

### Performance

- Uso de **cache** (ex: Redis) para otimizar performance em endpoints críticos e de tabelas com pouco fluxo de alteração, como a de usuários.
- Paginação e filtros nos endpoints de listagem para evitar sobrecarga.

### Segurança

- Inclusão de autenticação e autorização (ex: JWT).

### Testes

- Adoção de **testes de integração**.
- Introdução de **testes end-to-end**.

### Documentação

- Documentar todos os endpoints com **Swagger**.
- Adicionar exemplos de request/response e códigos de erro.

---
## Fluxograma

- [Link Fluxograma](https://tinyurl.com/5452ksd5)

<br>

 <details>
  <summary>Link Fluxograma (clique para expandir a imagem)</summary>
  <img src="https://tinyurl.com/bdrbdhx3" alt="Fluxograma" />
</details>
