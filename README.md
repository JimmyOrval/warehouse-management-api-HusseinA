# Warehouse Management API
A Warehouse Management API For Managing Warehouse Products Using An In-memory List. The Project Is Built Incrementally During InMind Academy

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