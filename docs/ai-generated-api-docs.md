# Warehouse Management API — Documentation

## Overview

REST API for managing warehouse inventory: products, suppliers, and stock levels across
warehouse locations. Built on ASP.NET Core 8 using a CQRS architecture (MediatR), EF Core
against PostgreSQL, Redis for caching, RabbitMQ for domain events, MinIO for file storage,
Hangfire for background jobs, and Firebase for authentication.

## Authentication

All endpoints require a Firebase-issued JWT bearer token unless marked `[AllowAnonymous]`.
Two authorization policies are used throughout:

- **AuthenticatedUser** — any valid, authenticated request.
- **AdminOnly** — requires a `role` claim of `Admin` on the token.

```
Authorization: Bearer <firebase-id-token>
```

---

## Products — `/api/products`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/` | AuthenticatedUser | List products. `onlyAvailable` query param (default `true`) filters to active products with stock > 0. |
| GET | `/{id}` | AuthenticatedUser | Get a single product by id. |
| GET | `/search` | AuthenticatedUser | Search by `name` and/or `supplier` (prefix match, case-insensitive). |
| GET | `/expiring-soon` | AuthenticatedUser | Active products expiring within the next 30 days. |
| GET | `/supplier` | AuthenticatedUser | Products by supplier name (`supplierName`, `isAscending` query params). |
| GET | `/year` | AuthenticatedUser | Products grouped by expiry year. |
| GET | `/year/country` | AuthenticatedUser | Products grouped by expiry year + supplier country. |
| GET | `/count` | AuthenticatedUser | Total product count. |
| GET | `/page` | AuthenticatedUser | Paged product list (`pageNumber`, `pageSize`). |
| GET | `/{productId}/items` | AuthenticatedUser | Warehouse items (stock locations) for a product. |
| GET | `/{productId}/quantity` | AuthenticatedUser | Total stock quantity across all locations for a product. |
| GET | `/server-time` | Anonymous | Returns the server's current date, formatted per `Accept-Language`. |
| POST | `/` | AdminOnly | Create a product. |
| PUT | `/{id}/price` | AdminOnly | Update a product's price. Body is a raw decimal, not an object. |
| POST | `/{productId}/image` | AdminOnly | Upload a product image (multipart form, field name `image`). |
| GET | `/image/{imageId}` | AuthenticatedUser | Download a product image. |
| DELETE | `/image/{imageId}` | AdminOnly | Delete a product image. |
| DELETE | `/{id}` | AdminOnly | Archive a product (soft delete — status becomes `Archived`, record is retained). |

### Create product

**Request** — `POST /api/products`
```json
{
  "name": "Industrial Shelving Unit",
  "sku": "SHELF-001",
  "description": "Heavy-duty steel shelving",
  "price": 249.99,
  "supplierId": "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
  "expiryDate": "2027-01-01T00:00:00Z"
}
```

**Response** — `201 Created`, empty body, `Location` header points to the new product's `GET /{id}` URL.

### Get product

**Response** — `200 OK`
```json
{
  "id": "e78bbc4b-1e1a-437c-bdb6-fb96d44845ea",
  "name": "Industrial Shelving Unit",
  "sku": "SHELF-001",
  "description": "Heavy-duty steel shelving",
  "price": 249.99,
  "expiryDate": "2027-01-01T00:00:00Z",
  "status": "Active",
  "supplierId": "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
  "supplierName": "Acme Supplies",
  "createdAt": "2026-01-15T10:30:00Z"
}
```

`status` is one of `Active` or `Archived`.

### Errors

Products endpoints follow the app-wide error shape:
```json
{
  "errorCode": "NOT_FOUND",
  "errorMessage": "Product 'xyz' not found",
  "traceId": "0HN..."
}
```
Common status codes: `404` (not found), `409` (business rule violation), `400` (validation failure), `500` (internal server error).

---

## Suppliers — `/api/suppliers`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/` | AuthenticatedUser | List all suppliers. |
| GET | `/{id}` | AuthenticatedUser | Get a supplier by id. |
| POST | `/` | AdminOnly | Create a supplier. |
| DELETE | `/{id}` | AdminOnly | Deactivate a supplier. Returns `200` with the updated supplier, not `204`. |
| POST | `/{id}/assign-supplier/{supplierId}` | AdminOnly | Assign a supplier to a product. `{id}` is the **product** id, `{supplierId}` is the supplier being assigned. |
| POST | `/{supplierId}/document` | AdminOnly | Upload a supplier document (multipart form). |
| GET | `/document/{documentId}` | AuthenticatedUser | Download a supplier document. |
| DELETE | `/document/{documentId}` | AdminOnly | Delete a supplier document. |

### Create supplier

**Request** — `POST /api/suppliers`
```json
{
  "name": "Acme Supplies",
  "country": "Lebanon",
  "contactEmail": "contact@acmesupplies.com",
  "phone": "+9611234567"
}
```

**Response** — `201 Created`, empty body, `Location` header points to `GET /{id}`.

### Assign supplier to product

**Request** — `POST /api/suppliers/{productId}/assign-supplier/{supplierId}` (no body)

**Response** — `200 OK`, returns the updated `ProductViewModel` with the new `supplierId`.
`409` if the target supplier is inactive or already assigned; `404` if either id doesn't exist.

---

## Warehouse Items — `/api/warehouse-items`

Stock quantity lives here, not on `Product` — a product's total stock is the sum of its
warehouse items' quantities across locations.

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/` | AuthenticatedUser | List all warehouse items. |
| GET | `/{itemId}` | AuthenticatedUser | Get a warehouse item by id. |
| POST | `/` | AdminOnly | Create a warehouse item for a product at a location. |
| PUT | `/{itemId}/quantity` | AdminOnly | Adjust stock. `quantity` is a **query parameter**, not a body — positive stocks in, negative stocks out. |
| DELETE | `/{itemId}` | AdminOnly | Delete a warehouse item. |
| GET | `/{itemId}/movements` | AuthenticatedUser | Stock movement history for an item. |

### Adjust quantity

**Request** — `PUT /api/warehouse-items/{itemId}/quantity?quantity=25` (stock in) or
`?quantity=-10` (stock out). `quantity=0` is rejected as invalid.

**Response** — `200 OK` with the updated `WarehouseItemViewModel`. `409` if a stock-out
would take the item below zero.

---

## Dashboard — `/api/dashboard`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/` | AuthenticatedUser | Aggregated dashboard summary. |
| GET | `/unread-notification-count` | AuthenticatedUser | Count of unread notifications for the current user. |

---

## Cache — `/api/cache`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/stats` | AdminOnly | Current cache statistics — hit/miss counts and tracked keys. |

---

## Metadata — `/api/metadata`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/product` | AdminOnly | Returns `Product`'s validation rules (from its data annotations), for client-side form generation. |

---

## Admin — `/api/admin`

Development-environment only; returns `404` everywhere else.

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/auth/token` | Anonymous (dev only) | Exchanges an email/password for a Firebase ID token — for local testing without a real client. |
| POST | `/users/{id}/role` | AdminOnly (dev only) | Sets a Firebase custom claim (`role`) on a user. |

---

## Architecture Notes

- **CQRS via MediatR** — every write is a `Command`, every read is a `Query`, each with its
  own handler. Controllers are thin: they build the request object and call
  `mediator.Send(...)`, nothing else.
- **Repository pattern** — `IProductRepository`, `ISupplierRepository`,
  `IWarehouseItemRepository` wrap EF Core. Handlers never touch `DbContext` directly.
- **Validation** — FluentValidation validators run as a MediatR pipeline behavior before a
  command handler executes; failures surface as `400` with field-level messages.
- **Caching** — `IDistributedCache` (Redis in production) caches individual product/supplier
  lookups and list queries for 5 minutes. Every command that mutates cached data explicitly
  invalidates the relevant keys. `ICacheStatsTracker` records hits/misses/sets for
  observability via `/api/cache/stats`.
- **Domain events** — significant actions (product created, stock adjusted, low-stock
  detected, files uploaded) publish events to RabbitMQ via `IEventPublisher`, for downstream
  consumers rather than being handled inline.
- **File storage** — product images and supplier documents go to MinIO via
  `IFileStorageService`, referenced by object key, not stored in the database.
- **Background jobs** — Hangfire runs a daily recurring job (`ExpiryCheckJob`) that auto-flags
  expiring/expired products.
- **Error handling** — a global `ExceptionHandlingMiddleware` maps domain exceptions to HTTP
  status codes consistently (`NotFoundException` → 404, `BusinessRuleException` → 409,
  `ValidationException` → 400), so handlers just throw domain-meaningful exceptions rather than
  returning status codes directly.
