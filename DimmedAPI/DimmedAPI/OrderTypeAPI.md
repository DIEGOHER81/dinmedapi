# OrderType API

## Descripción
API para gestionar los tipos de orden (OrderType).

## Endpoints

### GET /api/OrderType
Obtiene todos los tipos de orden.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Respuesta exitosa (200):**
```json
[
  {
    "id": 1,
    "description": "Orden de compra",
    "isActive": true
  },
  {
    "id": 2,
    "description": "Orden de venta",
    "isActive": false
  }
]
```

### GET /api/OrderType/active
Obtiene solo los tipos de orden activos.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Respuesta exitosa (200):**
```json
[
  {
    "id": 1,
    "description": "Orden de compra",
    "isActive": true
  }
]
```

### GET /api/OrderType/{id}
Obtiene un tipo de orden específico por su ID.

**Parámetros de ruta:**
- `id` (long, requerido): ID del tipo de orden

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Respuesta exitosa (200):**
```json
{
  "id": 1,
  "description": "Orden de compra",
  "isActive": true
}
```

**Respuesta de error (404):**
```json
"No se encontró el tipo de orden con ID 1"
```

### POST /api/OrderType
Crea un nuevo tipo de orden.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Cuerpo de la petición:**
```json
{
  "description": "Nuevo tipo de orden",
  "isActive": true
}
```

**Respuesta exitosa (201):**
```json
{
  "id": 3,
  "description": "Nuevo tipo de orden",
  "isActive": true
}
```

### PUT /api/OrderType/{id}
Actualiza un tipo de orden existente.

**Parámetros de ruta:**
- `id` (long, requerido): ID del tipo de orden

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Cuerpo de la petición:**
```json
{
  "description": "Tipo de orden actualizado",
  "isActive": false
}
```

**Respuesta exitosa (204):** Sin contenido

### DELETE /api/OrderType/{id}
Elimina un tipo de orden.

**Parámetros de ruta:**
- `id` (long, requerido): ID del tipo de orden

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Respuesta exitosa (204):** Sin contenido

### PATCH /api/OrderType/{id}/estado
Actualiza solo el estado activo/inactivo de un tipo de orden.

**Parámetros de ruta:**
- `id` (long, requerido): ID del tipo de orden

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Cuerpo de la petición:**
```json
true
```

**Respuesta exitosa (204):** Sin contenido

## Códigos de Error

- **400 Bad Request**: Cuando falta el código de compañía
- **404 Not Found**: Cuando no se encuentra el tipo de orden con el ID especificado
- **500 Internal Server Error**: Error interno del servidor

## Validaciones

- La descripción es obligatoria y no puede exceder 100 caracteres
- El código de compañía es obligatorio para todas las operaciones
- El ID debe ser un número entero positivo 