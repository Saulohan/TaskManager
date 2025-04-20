# 📝 TaskManager

O **TaskManager** é uma aplicação para gerenciamento de projetos. Ele permite que equipes e indivíduos organizem seu dia a dia de forma produtiva, com recursos como criação de projetos, tarefas e geração de relatórios de desempenho.

- [Link Repositorio github](https://github.com/Saulohan/TaskManager)

---

## 📦 Tecnologias Utilizadas

- ![.NET](https://img.shields.io/badge/.NET-8-blue)
- ![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-lightgrey)
- ![XUnit](https://img.shields.io/badge/Testes-xUnit%2FMoq-green)
- ![Docker](https://img.shields.io/badge/Docker-Enabled-blue)
- ![Docker Compose](https://img.shields.io/badge/Docker--Compose-Yes-blue)

---

## 🚀 Como Rodar Localmente

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/)
- [Git](https://git-scm.com/)

### Execução com Docker

```bash
# Clone o repositório
git clone https://github.com/Saulohan/TaskManager.git

# Acesse a pasta
cd taskmanager

# Suba o ambiente com Docker
docker-compose up --build
```

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
![Fluxograma](https://mermaid.ink/img/pako:eNq1VU1vGzcQ_SsDnhLAMSTtaiUtigaBLNsRZEGJ00slHYglLbHVkluS66SR_WOKHooeeir6C_THOlzup9q0QIHKguAl582892aWPJJEMU5i8nBQH5M91RY-XG0k4Ofe4tOL9Vt5-i0RavsSXr36Gt6svzH56SctFCRaUFhp9R23aushb4qY6bpchSmGsGpzWmxeNQksl5YCZQLTS6rhA9X8ARfUWVYPnA4a5KMwOT2IzxSsYsoAfjMPMSXmqsDMjhUTy1P4Cga9soh5_ezjZi7u6V6kT3C9LgkUrOm2HbA8_ayeYCbZ4MV6prWKYSFSYTkw3kq6fbmRHnVdlL_5O8ZlLLC2TI-6KVC3f3HIemjlUMns1kcPjiVtXnkA_JMwKLjSeDvoahh7DRD2ejG4VTj9CpkyRjzyQ9FWDXkKiUqxvCeSUU1xDf32tVReFxPSl5OWO_2tgoWrt8FxhalzAfRguaanX1xBqcBY1GVcJsYNFvUbrCpRkw-65Ccd8lzuHat2asYtTyw28BLuFBMP4oecY8DOKaJpVmppPK0K1uSDFvlwfU8PjxjVNoPXaFCwR-2n37VImjb6xrz1UOiy4x2IG5969w9utt35mZ9PgmeBdCTtzsLcx__7LMzPZmH0f8_CvDUL8-C_2Om9WJx7oXmqHvnZO7HwsY0PBZ1K_OJMfPSP4hk_cJSY4uFSKs2UMNjPlBtUL3OZUHhQonCJ1fOzaAm-q46U946sYK4xre5Xulkj1p91yy-J7R6M_nfpIYP6rOuKXp6JHrZFr77cuGVLxzI8zvwc1cfXikvmwouT1P8VqLBBYbGgXexaSCcX-105im66FO41qBSqaphqHmGL_6q-WkpHlRslMHnCjTkfmXetuwqDMpoVXYUbrh3z16WL7xrG79e4h-Op-YEWTSreUDyfeJrhUYNPOAz5wQo3FEEPmKCmk6V2OazuiTvurigwqAsvWjAokTqy2HRc0bDzZFp3h7MNW0suyE4LRmKrc35BUq5T6h7J0YVtiN3zlG9IjP8yqr_fkI18RkxG5bdKpRVMq3y3rx7yjFHLrwTdadpEuEbqqcqlJfFwOIiKHCQ-kk8k7vd7l5MgHIfD0Sga96NReEF-JPEguuxPJmEUhv1RGAyDaPh8QT4XZTE-CoJxP-xNonDYG_ei5z8BycbdTA)


