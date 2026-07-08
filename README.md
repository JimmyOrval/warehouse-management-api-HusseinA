# Warehouse Management API
A Warehouse Management API For Managing Warehouse Products Using An In-memory List. The Project Is Built Incrementally During InMind Academy

## Table of Contents

- [Session 2](#session-2)
- [Session 3](#session-3)

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

## Screenshots

<img width="624" height="377" alt="Picture1" src="https://github.com/user-attachments/assets/8bc96b5a-fa07-4093-b650-d5e0722c1a14" />
<img width="624" height="475" alt="Picture2" src="https://github.com/user-attachments/assets/b0864222-91d3-4674-86f2-75e65ef53b43" />
<img width="624" height="634" alt="Picture3" src="https://github.com/user-attachments/assets/d6304880-0898-4e52-b899-227430d7531b" />
<img width="624" height="430" alt="Picture4" src="https://github.com/user-attachments/assets/f9508c23-c3af-4e8a-a680-a88078277e0a" />
<img width="624" height="532" alt="Picture6" src="https://github.com/user-attachments/assets/88cae919-30b7-4c25-8ed9-2c482170281a" />
