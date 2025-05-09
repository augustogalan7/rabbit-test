# Sistema de Notificaciones de Inventario

Este proyecto implementa un sistema de gestión de inventario con notificaciones usando RabbitMQ como middleware de mensajería.

## Arquitectura

El sistema consta de dos microservicios:

1. **Inventory.API**: API REST para gestionar productos de inventario
   - Endpoints CRUD para productos
   - Publica eventos a RabbitMQ cuando hay cambios

2. **Notification.Service**: Servicio que consume eventos de RabbitMQ
   - Procesa eventos de creación, actualización y eliminación de productos
   - Almacena las notificaciones en una base de datos SQLite

## Requisitos

- Docker y Docker Compose
- .NET 8.0 SDK (para desarrollo local)

## Ejecución

1. Clonar el repositorio:
```bash
git clone <url-del-repositorio>
cd <nombre-del-directorio>
```

2. Ejecutar con Docker Compose:
```bash
docker-compose up -d
```

Los servicios estarán disponibles en:
- Inventory API: http://localhost:5000
- RabbitMQ Management: http://localhost:15672 (usuario/contraseña: guest/guest)

## Endpoints de la API

### Productos

- `GET /api/products` - Listar todos los productos
- `GET /api/products/{id}` - Obtener un producto por ID
- `POST /api/products` - Crear un nuevo producto
- `PUT /api/products/{id}` - Actualizar un producto existente
- `DELETE /api/products/{id}` - Eliminar un producto

### Ejemplo de creación de producto

```json
POST /api/products
{
    "name": "Producto de ejemplo",
    "description": "Descripción del producto",
    "price": 99.99,
    "stock": 100,
    "category": "Categoría de ejemplo"
}
```

## Desarrollo Local

1. Restaurar dependencias:
```bash
dotnet restore
```

2. Ejecutar Inventory.API:
```bash
cd Inventory.API
dotnet watch --project ./Notification.Api/
dotnet run
```

3. Ejecutar Notification.Service:
```bash
cd Notification.Service
dotnet watch --project ./Notification.Service/
dotnet run
```

## Estructura del Proyecto

```
.
├── Inventory.API/
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   └── Configuration/
├── Notification.Service/
│   ├── Consumers/
│   ├── Models/
│   └── Data/
└── docker-compose.yml
``` 