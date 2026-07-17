# Warehouse Management API
A Warehouse Management API For Managing Warehouse Products Using An In-memory List. The Project Is Built Incrementally During InMind Academy

## Table of Contents

- [Session 2](#session-2)
- [Session 3](#session-3)
- [Session 4](#session-4)
- [Session 5](#session-5)

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

Validation started happing using custom data annotations on commands/queries instead of validating request data inside handlers.

Custom `GuidStringAttribute` and `QuantityAttribute` were also introduced to validate parameters that are reused by multiple requests.

Business validation that depends on application state (such as checking for object availability or duplicate SKUs) remains inside the application layer.

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

Async/await is now used consistently throughout controllers, handlers and repositories.

`CancellationToken` is passed from controllers through MediatR into handler and repository methods where applicable.

## Generics & Reflection

A generic `Result<T>` type was introduced to demonstrate generic result handling without changing the overall exception-based architecture.

Reflection here was used here to inspect validation attributes applied to request models. A metadata endpoint was added to return example validation information.

## Notes

- Validation now happens before requests reach the application layer whenever possible.
- A small test was added for reflection.