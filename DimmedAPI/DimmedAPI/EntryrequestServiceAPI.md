# EntryrequestService API

## Descripción
API para gestionar los servicios de solicitudes de entrada (EntryrequestService).

## Endpoints

### GET /api/EntryrequestService
Obtiene todos los servicios de solicitudes de entrada.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Respuesta exitosa (200):**
```json
[
  {
    "id": 1,
    "description": "Servicio de consulta médica",
    "isActive": true
  },
  {
    "id": 2,
    "description": "Servicio de laboratorio",
    "isActive": false
  }
]
```

### GET /api/EntryrequestService/active
Obtiene solo los servicios activos de solicitudes de entrada.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Respuesta exitosa (200):**
```json
[
  {
    "id": 1,
    "description": "Servicio de consulta médica",
    "isActive": true
  }
]
```

### GET /api/EntryrequestService/{id}
Obtiene un servicio específico por su ID.

**Parámetros de ruta:**
- `id` (long, requerido): ID del servicio

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Respuesta exitosa (200):**
```json
{
  "id": 1,
  "description": "Servicio de consulta médica",
  "isActive": true
}
```

**Respuesta de error (404):**
```json
"No se encontró el servicio con ID 1"
```

### POST /api/EntryrequestService
Crea un nuevo servicio de solicitud de entrada.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Cuerpo de la petición:**
```json
{
  "description": "Nuevo servicio médico",
  "isActive": true
}
```

**Respuesta exitosa (201):**
```json
{
  "id": 3,
  "description": "Nuevo servicio médico",
  "isActive": true
}
```

### PUT /api/EntryrequestService/{id}
Actualiza un servicio existente.

**Parámetros de ruta:**
- `id` (long, requerido): ID del servicio

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Cuerpo de la petición:**
```json
{
  "description": "Servicio médico actualizado",
  "isActive": false
}
```

**Respuesta exitosa (204):** Sin contenido

### DELETE /api/EntryrequestService/{id}
Elimina un servicio.

**Parámetros de ruta:**
- `id` (long, requerido): ID del servicio

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Respuesta exitosa (204):** Sin contenido

### PATCH /api/EntryrequestService/{id}/estado
Actualiza solo el estado activo/inactivo de un servicio.

**Parámetros de ruta:**
- `id` (long, requerido): ID del servicio

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Cuerpo de la petición:**
```json
true
```

**Respuesta exitosa (204):** Sin contenido

## Códigos de Error

- **400 Bad Request**: Cuando falta el código de compañía
- **404 Not Found**: Cuando no se encuentra el servicio con el ID especificado
- **500 Internal Server Error**: Error interno del servidor

## Validaciones

- La descripción es obligatoria y no puede exceder 100 caracteres
- El código de compañía es obligatorio para todas las operaciones
- El ID debe ser un número entero positivo 