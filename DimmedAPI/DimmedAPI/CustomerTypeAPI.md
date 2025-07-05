# CustomerType API Documentation

## Descripción
API para gestionar tipos de cliente en el sistema DimmedAPI.

## Endpoints

### 1. Obtener todos los tipos de cliente
**GET** `/api/CustomerType?companyCode={companyCode}`

Obtiene todos los tipos de cliente.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
[
  {
    "id": 1,
    "description": "Cliente Regular",
    "isActive": true
  },
  {
    "id": 2,
    "description": "Cliente VIP",
    "isActive": true
  }
]
```

### 2. Obtener tipos de cliente activos
**GET** `/api/CustomerType/active?companyCode={companyCode}`

Obtiene solo los tipos de cliente que están activos.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

### 3. Obtener tipos de cliente con conteo de cotizaciones
**GET** `/api/CustomerType/with-quotations-count?companyCode={companyCode}`

Obtiene todos los tipos de cliente con el número de cotizaciones asociadas.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
[
  {
    "id": 1,
    "description": "Cliente Regular",
    "isActive": true,
    "quotationsCount": 15
  },
  {
    "id": 2,
    "description": "Cliente VIP",
    "isActive": true,
    "quotationsCount": 8
  }
]
```

### 4. Obtener tipo de cliente por ID
**GET** `/api/CustomerType/{id}?companyCode={companyCode}`

Obtiene un tipo de cliente específico por su ID.

**Parámetros:**
- `id` (path, requerido): ID del tipo de cliente
- `companyCode` (query, requerido): Código de la compañía

### 5. Buscar tipos de cliente por descripción
**GET** `/api/CustomerType/by-description/{description}?companyCode={companyCode}`

Busca tipos de cliente que contengan la descripción especificada.

**Parámetros:**
- `description` (path, requerido): Descripción a buscar
- `companyCode` (query, requerido): Código de la compañía

### 6. Crear tipo de cliente
**POST** `/api/CustomerType?companyCode={companyCode}`

Crea un nuevo tipo de cliente.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Body:**
```json
{
  "description": "Cliente Corporativo",
  "isActive": true
}
```

**Validaciones:**
- La descripción es requerida y no puede exceder 200 caracteres
- No puede existir otro tipo de cliente con la misma descripción

### 7. Actualizar tipo de cliente
**PUT** `/api/CustomerType/{id}?companyCode={companyCode}`

Actualiza un tipo de cliente existente.

**Parámetros:**
- `id` (path, requerido): ID del tipo de cliente
- `companyCode` (query, requerido): Código de la compañía

**Body:** Mismo formato que en la creación

**Validaciones:**
- La descripción es requerida y no puede exceder 200 caracteres
- No puede existir otro tipo de cliente con la misma descripción (excluyendo el actual)

### 8. Cambiar estado del tipo de cliente
**PATCH** `/api/CustomerType/{id}/toggle-status?companyCode={companyCode}`

Cambia el estado activo/inactivo de un tipo de cliente.

**Parámetros:**
- `id` (path, requerido): ID del tipo de cliente
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
{
  "id": 1,
  "isActive": false
}
```

### 9. Eliminar tipo de cliente
**DELETE** `/api/CustomerType/{id}?companyCode={companyCode}`

Elimina un tipo de cliente. Solo se puede eliminar si no tiene cotizaciones asociadas.

**Parámetros:**
- `id` (path, requerido): ID del tipo de cliente
- `companyCode` (query, requerido): Código de la compañía

### 10. Obtener cotizaciones de un tipo de cliente
**GET** `/api/CustomerType/{id}/quotations?companyCode={companyCode}`

Obtiene todas las cotizaciones asociadas a un tipo de cliente específico.

**Parámetros:**
- `id` (path, requerido): ID del tipo de cliente
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
[
  {
    "id": 1,
    "idCustomer": 123,
    "creationDateTime": "2024-01-15T10:30:00",
    "dueDate": "2024-02-15T10:30:00",
    "fk_idEmployee": 1,
    "totalizingQuotation": true,
    "equipmentRemains": false
  }
]
```

### 11. Verificar configuración de compañía
**GET** `/api/CustomerType/VerificarConfiguracionCompania?companyCode={companyCode}`

Verifica que la configuración de la compañía esté correcta para las operaciones de tipos de cliente.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
{
  "companyCode": "COMP001",
  "hasCustomerType": true,
  "totalCustomerTypes": 5,
  "activeCustomerTypes": 4,
  "message": "Configuración verificada correctamente"
}
```

## Códigos de Error

- **400 Bad Request**: 
  - Parámetros faltantes o inválidos
  - Descripción duplicada
  - Descripción excede el límite de caracteres
- **404 Not Found**: Recurso no encontrado
- **500 Internal Server Error**: Error interno del servidor

## Validaciones

### Creación y Actualización
- **Description**: Requerido, máximo 200 caracteres
- **IsActive**: Opcional, por defecto true
- **Unicidad**: No puede existir otro tipo de cliente con la misma descripción

### Eliminación
- Solo se puede eliminar si no tiene cotizaciones asociadas
- Si tiene cotizaciones, se recomienda desactivar en lugar de eliminar

## Notas

- Todos los endpoints requieren el parámetro `companyCode` para identificar la base de datos específica de la compañía
- Los tipos de cliente no se pueden eliminar si tienen cotizaciones asociadas
- Se incluye cache de salida para mejorar el rendimiento en consultas de solo lectura
- El endpoint de toggle-status es útil para activar/desactivar tipos de cliente sin eliminarlos
- El endpoint with-quotations-count es útil para mostrar estadísticas de uso 