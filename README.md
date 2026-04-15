# Medgrupo Contacts API

API REST em **.NET 8 / C#** para gerenciamento de contatos (CRUD + soft delete), construída como avaliação técnica para o Medgrupo. O projeto privilegia **separação de responsabilidades**, **validação rica**, **erros tratados sem exceções de fluxo** e **ambiente reproduzível via Docker**.

---

## Sumário

- [Stack](#stack)
- [Arquitetura](#arquitetura)
- [Decisões de projeto](#decisões-de-projeto)
- [Como rodar](#como-rodar)
- [Endpoints](#endpoints)
- [Regras de negócio](#regras-de-negócio)
- [Testes](#testes)

---

## Stack

- **.NET 8** (LTS) — `net8.0`
- **ASP.NET Core Web API** + **Swashbuckle/Swagger**
- **Entity Framework Core 8** + **SQL Server 2022**
- **FluentValidation 11**
- **HealthChecks** (SqlServer + UI)
- **xUnit**, **FluentAssertions**, **NSubstitute** para testes

---

## Arquitetura

Solução em **3 camadas** + projeto de testes:

```
src/
├── Medgrupo.WebApi      → Controllers, Program.cs, Configurations/*, Abstractions (MainController)
├── Medgrupo.Business    → Entities, DTOs (records), Services, Validators, Notifications
└── Medgrupo.Data        → DbContext, EntityTypeConfigurations, Repositories, Pagination
tests/
└── Medgrupo.UnitTests   → Testes de entidades, services, validators e notifications
```

**Fluxo de uma requisição:**

```
Controller → Service → (Validator + Notifier) → Repository → EF Core → SQL Server
```

- **WebApi** depende de Business e Data (composition root).
- **Business** expõe contratos e regras; **Data** implementa persistência.
- **Controllers** herdam de um `MainController` que inspeciona o `INotifier` e devolve `200/400/404` de forma consistente — sem `try/catch` para validação.

---

## Decisões de projeto

### Por que o padrão Notification?
Erros de **regra de negócio e validação** não são excepcionais — são parte do fluxo normal. Usar `throw`/`catch` para isso é caro, polui stack traces e mistura erros previstos com falhas reais. O `INotifier` (scoped) acumula erros durante a requisição e o `MainController` traduz para `400 BadRequest` com a lista de mensagens. **Exceptions ficam reservadas para o inesperado** (falha de infra, bug).

### Por que FluentValidation?
- Mantém as regras **fora das entidades e DTOs**, centralizadas e testáveis isoladamente.
- API fluente, composição de regras e mensagens customizadas sem poluir o modelo.
- Plugável no pipeline do ASP.NET, mas aqui é invocado explicitamente no Service para que o resultado alimente o `INotifier` — mesma saída, zero exceptions.

### Por que EF Core 8 (code-first)?
- Produtividade: migrations, LINQ tipado, tracking.
- `IEntityTypeConfiguration<T>` mantém o `DbContext` limpo — cada entidade tem seu arquivo de mapeamento.
- Repositórios finos por cima do `DbSet` isolam a camada de aplicação do provider (trocar SQL Server por outro banco exige mudar só Data).

### Outras escolhas
- **DTOs como `record`** → imutáveis, `value equality`, menos boilerplate.
- **`BaseEntity` com `Id` Guid** → chaves não sequenciais, seguras para expor em URLs, amigáveis para sistemas distribuídos.
- **Soft delete (`IsActive`)** em vez de `DELETE` destrutivo como padrão de leitura/edição; há também `DELETE` hard quando necessário.
- **Paginação** nativa no repositório (`PagedResult<T>`) para evitar listagens ilimitadas.
- **`Program.cs` enxuto**: cada concern vive em `Configurations/*Config.cs` (Swagger, EF, HealthChecks, DI, etc.).
- **Async/await ponta a ponta** em I/O.
- **HealthChecks + UI** para observabilidade mínima out-of-the-box.
- **Namespace flutuante** (file-scoped) e `ImplicitUsings`/`Nullable` ativos.

---

## Como rodar

### Pré-requisitos

- **.NET SDK 8.0** (para rodar na máquina)
- **Docker Desktop** (para rodar via Compose — recomendado)

### Opção 1 — Docker Compose (recomendado)

Sobe API + SQL Server 2022 com rede, volume e healthcheck já configurados:

```bash
docker compose up --build
```

| Serviço      | URL / Porta                         |
|--------------|-------------------------------------|
| API          | http://localhost:5000               |
| Swagger      | http://localhost:5000/swagger       |
| Health       | http://localhost:5000/health        |
| Health UI    | http://localhost:5000/health-ui     |
| SQL Server   | `localhost:1433` (sa / `Your_password123`) |

Derrubar: `docker compose down` (ou `down -v` para apagar o volume do banco).

### Opção 2 — Rodar na máquina

Requer uma instância de SQL Server acessível. Ajuste `ConnectionStrings:DefaultConnection` em `src/Medgrupo.WebApi/appsettings.Development.json` ou via env var:

```bash
export ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=MedgrupoDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True"

dotnet restore
dotnet ef database update --project src/Medgrupo.Data --startup-project src/Medgrupo.WebApi
dotnet run --project src/Medgrupo.WebApi
```

No Windows (PowerShell) use `$env:ConnectionStrings__DefaultConnection = "..."`.

---

## Endpoints

| Método | Rota                                      | Descrição                            |
|--------|-------------------------------------------|--------------------------------------|
| GET    | `/api/contacts?page=1&pageSize=10`        | Lista contatos ativos (paginado)     |
| GET    | `/api/contacts/{id}`                      | Detalhe de contato ativo             |
| POST   | `/api/contacts`                           | Cria contato                         |
| PUT    | `/api/contacts/{id}`                      | Atualiza contato ativo               |
| PATCH  | `/api/contacts/{id}/deactivate`           | Desativa (soft delete)               |
| DELETE | `/api/contacts/{id}`                      | Exclui definitivamente               |

Respostas de erro retornam `400` com a lista de notificações:

```json
{ "errors": ["Contato deve ser maior de idade."] }
```

---

## Regras de negócio

- `Name`, `BirthDate` e `Gender` obrigatórios.
- `Age` calculado em runtime (não persistido).
- Contato deve ser **maior de idade** (≥ 18); idade não pode ser `0`.
- `BirthDate` não pode ser futura.
- Listagem, detalhe e edição consideram **apenas contatos ativos**.

---

## Testes

Testes unitários cobrindo entidades, services, validators e notifications:

```bash
dotnet test
```
