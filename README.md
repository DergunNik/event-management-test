# Event Management Test

## О проекте

**Event Management Test** — приложение для управления событиями, категориями и пользователями.  
Система предоставляет REST API для создания, редактирования, удаления и просмотра событий, категорий и пользователей, а также для загрузки изображений к событиям.  
Реализована аутентификация и разграничение ролей (Admin, User).

## Технологический стек

- **C# / .NET 9**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **PostgreSQL** (по умолчанию)
- **FluentValidation** (валидация DTO)
- **Mapster** (маппинг DTO ↔ Entity)
- **Docker, Docker Compose**
- **XUnit** (юнит-тесты)
- **Serilog** (логирование)

## Быстрый запуск через Docker Compose

### Требования

- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

### Переменные окружения

В файле `.env` (создайте сами или экспортируйте переменные) должны быть:

```env
POSTGRES_USER=...
POSTGRES_PASSWORD=...
POSTGRES_DB=...
DB_HOST=db
DB_PORT=5432
CONNECTION_STRING=Host=${DB_HOST};Port=${DB_PORT};Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}
TOKENS_SECRET=...
ASPNETCORE_ENVIRONMENT=Development
```

### Запуск

1. Соберите и запустите всё приложение и БД:
   ```sh
   docker compose up --build
   ```
   Это поднимет контейнеры:
   - **db**: PostgreSQL 16
   - **migrate**: применяет миграции EF Core к БД
   - **webapi**: сам API на .NET 9

2. После старта API будет доступно по адресу:  
   [http://localhost:5000/swagger](http://localhost:5000/swagger)  
   (Swagger UI для ручного тестирования API)

3. Логи приложения пишутся в volume `logs-volume` (смонтировано в `/app/logs` внутри контейнера).

### Альтернативный запуск через Dockerfile

Если нужен только WebAPI (БД уже есть), используйте только сервис `webapi` с нужными переменными среды (смотреть в Dockerfile).

---

## Тесты

Реализованы юнит-тесты (проект `Tests/`):

- Тесты бизнес-логики сервисов (например, создание, обновление, удаление событий и категорий)
- Тесты базовых методов репозиториев (добавление, поиск по ID)
- Используется InMemory-база

Запуск тестов:
```sh
dotnet test
```

---

## Основные возможности WebAPI

- CRUD для событий, категорий и пользователей
- Регистрация и удаление участников событий
- Фильтрация, пагинация, сортировка событий и пользователей
- Загрузка изображений для событий
- JWT-аутентификация, refresh-токены
- Роли пользователей (User/Admin)

---

## Структура репозитория

- `WebAPI/` — контроллеры и API, запуск приложения
- `Application/` — бизнес-логика, сервисы, DTO, маппинги
- `Domain/` — сущности и абстракции
- `Persistence/` — контекст EF Core, миграции, репозитории и UnitOfWork
- `Tests/` — юнит-тесты
- `docker-compose.yml`, `Dockerfile` — контейнеризация и миграции
