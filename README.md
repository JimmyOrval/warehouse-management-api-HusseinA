# Warehouse Management API
A Warehouse Management API For Managing Warehouse Products Using An In-memory List. The Project Is Built Incrementally During InMind Academy

## Table of Contents

- [Session 2](#session-2)
- [Session 3](#session-3)
- [Session 4](#session-4)
- [Session 5](#session-5)
- [Session 6](#session-6)
- [Session 7](#session-7)
- [Session 8](#session-8)
- [Session 9](#session-9)

# Session 2
## Features

- Basic CRUD operations for products
- Get all products
- Filter products by availability
- Get a specific product by its ID
- Search products by name and supplier
- Create new products
- Update product price and quantity
- Upload product images
- Soft-deletion (archiving) of products\
- Server date display based on header language

## How To Run

1. Clone the repository
2. Open project in JetBrains Rider
3. Run the project using IIS Express button built into the IDE
4. Swagger opens in browser, allowing for endpoint usage

## Available Endpoints

### Products

- `GET /api/products`
- `GET /api/products/{id}`
- `GET /api/products/search`
- `POST /api/products`
- `PUT /api/products/{id}/quantity`
- `PUT /api/products{id}/price`
- `POST /api/products/{id}/image`
- `DELETE /api/products{id}`
- `GET /api/products/server-time`

## Notes

- Date is currently being stored in an in-memory list only, no Database involved
- The list already holds pre-seeded products
- Image uploads are stored in "wwwroot/uploads"

## Additional Features

### Suppliers

- `GET /api/suppliers`
- `GET /api/suppliers/{id}`
- `POST /api/suppliers`
- `DELETE /api/suppliers/{id}`

### Product-Supplier Relationship

- `POST /api/products/{id}/assign-supplier/{supplierId}`

## Homework Feature

- Suppliers can be fetched (all/by ID)
- Suppliers can be created and deleted
- Products can be linked to a supplier



# Session 3

## Architecture Refactor

The project has been refactored to follow a layered architecture inspired by Domain-Driven Design (DDD) and CQRS principles. Business logic has been moved out of controllers into dedicated application use cases, while repositories abstract data access and keep the application independent of the underlying storage implementation.

### Project Layers

- **Domain** - Entities, business rules, repository interfaces, and enums.
- **Application** - Commands, queries, handlers.
- **Infrastructure** - Repository implementations and in-memory data storage.
- **Presentation** - API controllers.

## Refactored Endpoints

All endpoints from the previous session continue to function as expected after the architectural refactor.

### Products

- `GET /api/products`
- `GET /api/products/{id}`
- `GET /api/products/search`
- `POST /api/products`
- `PUT /api/products/{id}/quantity`
- `PUT /api/products/{id}/price`
- `POST /api/products/{id}/image`
- `DELETE /api/products/{id}`
- `GET /api/products/server-time`

### Suppliers

- `GET /api/suppliers`
- `GET /api/suppliers/{id}`
- `POST /api/suppliers`
- `DELETE /api/suppliers/{id}`

### Product-Supplier Relationship

- `POST /api/products/{id}/assign-supplier/{supplierId}`

## Additional Domain Models

The following new domain entities were introduced:

- **WarehouseItem** - Represents product inventory at a specific warehouse location and manages stock updates.
- **StockMovement** - Represents stock movement history and distinguishes between stock-in and stock-out operations.

## Design Notes

A few implementation details differ slightly from the original lab requirements:

- Product quantity has been moved from the `Product` entity to the `WarehouseItem` entity to better separate product information from inventory management.
- Product image handling now references the product through a `ProductId` relationship, and the upload endpoint was updated accordingly.
- Quantity updates are performed on `WarehouseItem` instead of `Product`, while still updating the product's `LastUpdatedAt` timestamp.
- `WarehouseItem` and `StockMovement` have been introduced as part of the domain model for future inventory management features. At this stage, they are only partially integrated into the application.

## Unit Tests

The following unit tests were implemented:

### Domain Tests

- Product price cannot be zero or negative.
- Archived products cannot have their price changed.
- Inactive suppliers cannot be assigned to products.
- Warehouse item quantity cannot become negative.

### Application Tests

- Create Product command calls the repository exactly once.
- Get Product By ID returns a not-found exception when the product does not exist.

### Test Results

All implemented unit tests pass successfully.


# Session 4

## Database Integration

The project was migrated from an in-memory data store to PostgreSQL using Entity Framework Core Code First. The database schema is managed through EF Core Migrations while preserving the existing layered architecture and API behavior.

## Architecture Updates

The Clean Architecture introduced in the previous session was maintained.

- **Domain** - Business entities and repository interfaces.
- **Application** - Commands, Queries, ViewModels, AutoMapper, and business use cases.
- **Infrastructure** - Entity Framework Core DbContext and repository implementations.
- **Presentation** - API controllers.

## Entity Framework Core

A `WarehouseDbContext` was introduced, which was used to create the database using EF Core migrations, including DbSets for:

- Products
- Suppliers
- ProductImages
- WarehouseItems
- StockMovements

## AutoMapper

AutoMapper was added to centralize object mapping and reduce repetitive code.

Mappings include:

- Command → Entity
- Entity → ViewModel

The API now returns ViewModels instead of exposing domain entities directly.

### ViewModels

- ProductViewModel
- SupplierViewModel
- ProductImageViewModel
- WarehouseItemViewModel
- StockMovementViewModel

## Performance Improvements

Repository queries were updated to better leverage Entity Framework Core.

- Replaced `IEnumerable` filtering with `IQueryable` queries where applicable.
- Moved filtering and searching to the database instead of in-memory.
- Used `EF.Functions.ILike` for PostgreSQL case-insensitive searches.
- Reduced repetition by centralizing object mapping.

## Notes

- Existing endpoints continue to behave as before while using PostgreSQL as the persistence layer.
- Unused DTOs/Contracts were removed after introducing ViewModels.


# Session 5

## API Hardening

I focused on making the API more reliable and closer to how a real production API would behave. Most of it was not about adding new features, but improving how the application handles invalid requests and unexpected errors.

## Validation

- Added FluentValidation to validate commands and queries before they reach their handlers.
- Implemented a MediatR FluentValidation pipeline behavior to centralize request validation.
- Validation started happening using commands/queries validators instead of validating request data inside handlers.
- Business validation that depends on application state (such as checking for object availability or duplicate SKUs) remains inside the application layer.

## Exception Handling

Custom exceptions were introduced to better represent common application errors.

- `NotFoundException`
- `BusinessRuleException`
- `RequestValidationException`

A global exception handling middleware now catches unhandled exceptions, logs them internally, and returns a consistent error response without exposing implementation details.

The API error response now contains:

- Error code
- Error message
- Trace ID

## Middleware

Several middleware components were added to handle HTTP-level concerns.

### Correlation ID Middleware

Adds a correlation ID to every request and response so requests can be traced more easily.

### Request Timing Middleware

Measures how long each request takes and logs the execution time.

### Exception Handling Middleware

Handles unexpected exceptions globally and maps known exceptions to the appropriate HTTP status codes.

## MVC Filters

Two MVC filters were introduced.

- `ActionLoggingFilter` logs controller actions before and after execution.
- `ModelValidationFilter` was implemented to demonstrate MVC filters, even though automatic model validation is already provided by the `[ApiController]` attribute.

## Async Improvements

Async/await is now used consistently throughout controllers, handlers, and repositories.

`CancellationToken` is passed from controllers through MediatR into handler and repository methods where applicable.

## Generics & Reflection

A generic `Result<T>` type was introduced to demonstrate generic result handling without changing the overall exception-based architecture.

Reflection here was used to inspect validation attributes applied to request models. A metadata endpoint was added to return example validation information.

## Domain Model

- Replaced the product's archive flag with a ProductStatus enum with 3 states as of this session.
- Updated relevant product business logic and repository queries to use the new status model.

## Notes

- Validation now happens before requests reach the application layer whenever possible.
- A small test was added for reflection.


# Session 6

## Observability and Performance

I focused on making the API production-ready from an operational standpoint: multi-language support, centralized logging, caching, health monitoring, and calling automatic background jobs.

## Localization

- Configured `RequestLocalizationOptions` supporting `en-US` (default), `fr`, and `ar`.
- Culture can be switched directly from Swagger, which adds a `culture` query parameter dropdown to every endpoint.
- Rather than localizing every data annotation individually, localization was applied at the error-response boundary: `ExceptionHandlingMiddleware` now returns a localized message for each error category (Not Found / Business Rule / Validation / Internal) using `IStringLocalizer`, while the specific detail (which product) stays in the Serilog logs. This keeps the response small without leaking internal details to clients.

## Logging

- Serilog was configured as the logging provider, writing structured logs to both console and a daily-generated file (`Logs/log-.txt`, kept for 7 days).
- `CorrelationIdMiddleware` now properly pushes the correlation ID into Serilog's `LogContext`, so every log line for a request can be traced back to the same ID returned in `ErrorResponse.TraceId`.
- Business-relevant events (product created, price changed, archived...) are logged with structured named properties across command handlers.
- `RequestTimingMiddleware` was changed into slow-request logging: only logs requests exceeding 500ms, avoiding duplication with Serilog, which already logs every request's details.

## Caching

- Redis caching was implemented using `IDistributedCache`.
- Cached: `GET /api/products/{id}`, `GET /api/products`, `GET /api/suppliers/{id}`, `GET /api/suppliers`. Expire in 5 minutes.
- Not cached: search (bad hit rate), count (cheap query), server-time, and grouping/paging endpoints.
- Invalidation: writing handlers delete the relevant single-entity and list keys after a successful save.
- Added a cache statistics endpoint (`GET /api/cache/stats`) returning cached keys, hit/miss counts, and last refresh time.

## Health Checks

- `/health` endpoint shows PostgreSQL and Redis status using `AspNetCore.HealthChecks`.
- Health Checks UI runs at `/health-ui`, and uses in-memory storage for check history.
- The Redis check was replaced with a custom `RedisRetryHealthCheck` that retries up to 3 times before showing unhealthy.

## Background Jobs

- Hangfire was configured with PostgreSQL as its storage (`Hangfire.PostgreSql`).
- A daily job (`ExpiryCheckJob`) checks for expired and soon-to-expire (within 30 days) products.
- Added auto-archive functionality for products expired for more than 7 days, updating `ProductStatus` and logging each archived product, including cache invalidation where necessary.
- Hangfire Dashboard runs at `/hangfire`.


# Session 7

## Firebase Authentication and Authorization

Firebase Authentication was integrated into the API. Authentication is fully assigned to Firebase. The API only validates tokens and enforces role-based access (authorization).

- JWT Bearer authentication validates Firebase ID tokens through Google's "Secure Token" and audience, being the project itself.
- `MapInboundClaims` is disabled so claim types are preserved exactly as issued by Firebase, avoiding .NET's default remapping.
- Two authorization policies were added: `AdminOnly` (requires `role = Admin`) and `AuthenticatedUser` (any signed-in user).
- Roles are stored as Firebase custom claims, set using the Admin SDK. Firebase does not handle authorization; it stores the role as a signed claim that the API enforces.
- A custom `AuthorizationMiddleware` returns the existing `ErrorResponse` shape (error code, message, trace ID) for both 401 (unauthenticated/token expired) and 403 (authenticated, insufficient role) outcomes, keeping error responses consistent with the rest of the API.
- Swagger has a token input configured so ID tokens can be tested directly from the UI.
- A login endpoint was introduced to simplify token retrieval.

### Access Levels

- **Admin**: create, read, update, delete, and upload.
- **User**: read-only access.

### Admin Endpoints

- `POST /api/admin/users/{id}/role`: assigns `Admin`/`User` role using Firebase custom claims (admin-only).
- `POST /api/admin/auth/token`: development-only endpoint that signs in with email/password, calling the Firebase API, and returns an ID token, to simplify testing.
- Both endpoints don't work outside the Development environment.

### Notes

- No Firebase passwords or credentials are stored in the warehouse database.
- Role changes only take effect on the next token issued to the user (Firebase does not automatically update already-issued tokens).

## MinIO Object Storage

MinIO was added as the object storage instead of the DB. File bytes are never persisted in Postgres; only object metadata is stored.

- MinIO runs locally via Docker Compose, exposing the API and web UI.
- A bucket (`warehouse-assets`) was created manually through the MinIO UI.
- An `IFileStorageService` interface overrides the MinIO methods (upload, download, delete).
- Uploaded files are stored inside generated GUID-based object keys.
- File size and content type are validated via FluentValidation before any upload is attempted.
- Failed MinIO operations throw a `StorageException`, mapped inside the exception handler to a `502` response in the same `ErrorResponse` format.

### File Types

- **ProductImage:** Refactored from local `wwwroot` storage to MinIO; `FilePath` replaced with `ObjectKey`, `ContentType`, and `Size`. Supports multiple images for each product. Only allows `.png` and `.jpg` formats.
- **SupplierDocument:** also stored in MinIO. Only allows `.pdf` and `.txt` formats.

### New Endpoints

- `POST /api/products/{productId}/image` uploads a product image (admin-only). Returns the new image ID.
- `GET /api/products/image/{imageId}` downloads a product image (authenticated users).
- `DELETE /api/products/image/{imageId}` deletes a product image (admin-only).
- `POST /api/suppliers/{supplierId}/documents` uploads a supplier document (admin-only). Returns the new document ID.
- `GET /api/suppliers/document/{documentId}` downloads a supplier document (authenticated users).
- `DELETE /api/suppliers/document/{documentId}` deletes a supplier document (admin-only).


# Session 8

## Notification Service and RabbitMQ

A separate Notification Service was added as its own solution, living in the same repository: its own layered architecture (Domain, Application, Infrastructure, Presentation), its own PostgreSQL database, and no shared code or database access with the API. Communication between the two happens only through RabbitMQ events, plus one optional direct HTTP call.

## RabbitMQ

- Added RabbitMQ to the existing `docker-compose.yml`, with MinIO.
- RabbitMQ management console runs on port `15672`.
- Exchange: `warehouse.events`.
- Queue consumed by the Notification Service: `notifications.warehouse-events`.
- A dead-letter exchange `warehouse.events.dlx` and queue `notifications.warehouse-events.dlq` catch any message that fails to process, instead of losing it silently.

## Warehouse Events

Four event contracts represent warehouse operations/endpoint:

- `StockLowDetected`: when an item's quantity drops below a configured threshold, set in appsettings. Only works when we cross the threshold, not on every update while already below it.
- `WarehouseFileUploaded`: for both product image and supplier document uploads.
- `StockAdjusted`: every quantity adjustment.
- `ProductCreated`: when a new product is created.

Each event carries an event ID, event time, correlation ID, event type, related entity ID/type, and severity, plus its own data.

Routing keys: `stock.low`, `file.uploaded`, `stock.adjusted`, `product.created`.

## Notification Service

- Consumes all four events through a background service `RabbitMqConsumer`.
- Maps each event to a notification with a type, title, message, severity, and related entity, then persists it in the database.
- Idempotent: a unique constraint on the event ID prevents duplicate notifications.
- The consumer retries connecting if RabbitMQ isn't available yet at startup, instead of crashing.
- Failed messages (bad payloads, mapping errors) are rejected and routed to the dead-letter queue for inspection, instead of being retried forever or dropped.

### Endpoints

- `GET /api/notifications`: lists all notifications.
- `GET /api/notifications/{id}`: gets a single notification.
- `GET /api/notifications/search`: lists notifications with optional filtering by type, severity, and status.
- `PUT /api/notifications/{id}/read`: marks a notification as read.
- `GET /api/notifications/unread-count`: returns the count of unread notifications.

All endpoints require an authenticated Firebase user (same as the warehouse API). No admin privileges required.

## Bonus: Direct HTTP Communication

The warehouse API exposes `GET /api/dashboard/unread-notifications-count`, which calls the Notification Service directly over HTTP. The caller's token is sent to the Notification Service directly. If the Notification Service is unreachable, the endpoint still returns `200` with `{ count: null, isAvailable: false }` instead of an error.

## Bonus: Dead-Letter Queue

Failed messages land in `notifications.warehouse-events.dlq` instead of being lost, and can be inspected manually through the management console.

## Notes

- Bonus 2 was skipped. Severity is already assigned for each event type at the point of publishing, so making it configurable didn't add much value.

## Setup
- To run everything locally: start RabbitMQ via `docker compose up -d` from the repo root, then run the Postgres DB and Redis Cache containers from Docker, and finally run the warehouse API and the Notification Service separately. Each opens its own UI.


markdown
# Session 9

## Testing

Added `UnitTests` and `IntegrationTests` projects to the API's solution.

### Unit Tests

- `Products`: create, search, price updates, and archive, with dependencies mocked via Moq. Quantity tests used for WarehouseItem, as it owns that logic.
- `Suppliers`: create, deactivate, assign-to-product, including rejecting archived products and missing suppliers.
- File upload validation logic and the request-logging middleware.

### Integration Tests

- `CustomWebApplicationFactory` runs the real API through the actual `Program.cs`, but every external dependency was faked instead of connecting to real services:
  - Postgres: EF Core InMemory provider
  - Redis: `AddDistributedMemoryCache` + a fake `IConnectionMultiplexer`
  - RabbitMQ: `FakeEventPublisher`
  - MinIO: in-memory `IFileStorageService`
  - Firebase: `FakeAuthHandler`, a test authentication scheme reading role claims from a `Test-Role` header instead of validating real tokens
- One factory instance is shared for all tests `WarehouseApiCollection` so we don't have to run the services for every test, since `Program.cs` starts all services every time the factory is called.
- Database and distributed cache are both reset before every test, since the in-memory data persists for the entire factory usage, not just for every request.

### Test Utilities

- `Builders`: `ProductBuilder`, `SupplierBuilder`, `WarehouseItemBuilder`: create mock entities, using domain methods like `Archive()`, `Deactivate()`, `StockIn()` instead of calling them in each test.
- `Helpers/`: `JsonContentHelper` for request/response serialization including the enum string converter, and `MultipartFormHelper` builds multipart file-upload bodies for image endpoints.
- Used only in the `IntegrationTests` project, since the unit test project doesn't need them.

### Coverage

- Products: CRUD, quantity adjustment, price updates, and archive on delete.
- Warehouse items: used for quantity adjustment specifically.
- Suppliers: create, get, deactivate, and assign-to-product.
- Product image upload: valid jpg/png, oversized files, wrong file type, and uploading to a missing product.
- Swagger: switched environment temporarily to Testing to use it.
- Full flow test using every step used by real HTTP calls: create supplier, create product, assign a second supplier, upload image, create warehouse item, adjust quantity, update price, archive, check final values.

## GitHub Action

Added `WarehouseTestsAction.yml`: restores, builds, and runs the full test suite with code coverage, then publishes a summary report.

## Bonus Challenges

1. Code coverage: Application coverage over 80%.
2. GitHub Actions: create the testing action.
3. Negative request tests: duplicate SKU, invalid file type and size, zero and over-stock quantity adjustments, and missing/inactive suppliers and products.
