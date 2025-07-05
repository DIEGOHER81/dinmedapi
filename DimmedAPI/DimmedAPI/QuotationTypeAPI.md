# QuotationType API Documentation

## Descripción
API para gestionar tipos de cotización en el sistema DimmedAPI.

## Endpoints

### 1. Obtener todos los tipos de cotización
**GET** `/api/QuotationType?companyCode={companyCode}`

Obtiene todos los tipos de cotización.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
[
  {
    "id": 1,
    "description": "Cotización Estándar",
    "isActive": true
  },
  {
    "id": 2,
    "description": "Cotización VIP",
    "isActive": true
  }
]
```

### 2. Obtener tipos de cotización activos
**GET** `/api/QuotationType/active?companyCode={companyCode}`

Obtiene solo los tipos de cotización que están activos.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

### 3. Obtener tipos de cotización con conteo de cotizaciones
**GET** `/api/QuotationType/with-quotations-count?companyCode={companyCode}`

Obtiene todos los tipos de cotización con el número de cotizaciones asociadas.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
[
  {
    "id": 1,
    "description": "Cotización Estándar",
    "isActive": true,
    "quotationsCount": 25
  },
  {
    "id": 2,
    "description": "Cotización VIP",
    "isActive": true,
    "quotationsCount": 12
  }
]
```

### 4. Obtener tipo de cotización por ID
**GET** `/api/QuotationType/{id}?companyCode={companyCode}`

Obtiene un tipo de cotización específico por su ID.

**Parámetros:**
- `id` (path, requerido): ID del tipo de cotización
- `companyCode` (query, requerido): Código de la compañía

### 5. Buscar tipos de cotización por descripción
**GET** `/api/QuotationType/by-description/{description}?companyCode={companyCode}`

Busca tipos de cotización que contengan la descripción especificada.

**Parámetros:**
- `description` (path, requerido): Descripción a buscar
- `companyCode` (query, requerido): Código de la compañía

### 6. Crear tipo de cotización
**POST** `/api/QuotationType?companyCode={companyCode}`

Crea un nuevo tipo de cotización.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Body:**
```json
{
  "description": "Cotización Corporativa",
  "isActive": true
}
```

**Validaciones:**
- La descripción es requerida y no puede exceder 200 caracteres
- No puede existir otro tipo de cotización con la misma descripción

### 7. Actualizar tipo de cotización
**PUT** `/api/QuotationType/{id}?companyCode={companyCode}`

Actualiza un tipo de cotización existente.

**Parámetros:**
- `id` (path, requerido): ID del tipo de cotización
- `companyCode` (query, requerido): Código de la compañía

**Body:** Mismo formato que en la creación

**Validaciones:**
- La descripción es requerida y no puede exceder 200 caracteres
- No puede existir otro tipo de cotización con la misma descripción (excluyendo el actual)

### 8. Cambiar estado del tipo de cotización
**PATCH** `/api/QuotationType/{id}/toggle-status?companyCode={companyCode}`

Cambia el estado activo/inactivo de un tipo de cotización.

**Parámetros:**
- `id` (path, requerido): ID del tipo de cotización
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
{
  "id": 1,
  "isActive": false
}
```

### 9. Eliminar tipo de cotización
**DELETE** `/api/QuotationType/{id}?companyCode={companyCode}`

Elimina un tipo de cotización. Solo se puede eliminar si no tiene cotizaciones asociadas.

**Parámetros:**
- `id` (path, requerido): ID del tipo de cotización
- `companyCode` (query, requerido): Código de la compañía

### 10. Obtener cotizaciones de un tipo de cotización
**GET** `/api/QuotationType/{id}/quotations?companyCode={companyCode}`

Obtiene todas las cotizaciones asociadas a un tipo de cotización específico.

**Parámetros:**
- `id` (path, requerido): ID del tipo de cotización
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
    "equipmentRemains": false,
    "monthlyConsumption": 100.5
  }
]
```

### 11. Obtener estadísticas de tipos de cotización
**GET** `/api/QuotationType/statistics?companyCode={companyCode}`

Obtiene estadísticas detalladas de todos los tipos de cotización.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
{
  "totalQuotationTypes": 3,
  "activeQuotationTypes": 2,
  "totalQuotations": 37,
  "totalizingQuotations": 15,
  "equipmentRemainsQuotations": 8,
  "details": [
    {
      "id": 1,
      "description": "Cotización Estándar",
      "isActive": true,
      "quotationsCount": 25,
      "totalizingQuotationsCount": 10,
      "equipmentRemainsCount": 5
    },
    {
      "id": 2,
      "description": "Cotización VIP",
      "isActive": true,
      "quotationsCount": 12,
      "totalizingQuotationsCount": 5,
      "equipmentRemainsCount": 3
    }
  ]
}
```

### 12. Verificar configuración de compañía
**GET** `/api/QuotationType/VerificarConfiguracionCompania?companyCode={companyCode}`

Verifica que la configuración de la compañía esté correcta para las operaciones de tipos de cotización.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
{
  "companyCode": "COMP001",
  "hasQuotationType": true,
  "totalQuotationTypes": 3,
  "activeQuotationTypes": 2,
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
- **Unicidad**: No puede existir otro tipo de cotización con la misma descripción

### Eliminación
- Solo se puede eliminar si no tiene cotizaciones asociadas
- Si tiene cotizaciones, se recomienda desactivar en lugar de eliminar

## Estadísticas Disponibles

El endpoint de estadísticas proporciona información detallada sobre:

- **Total de tipos de cotización**: Número total de tipos configurados
- **Tipos activos**: Número de tipos que están activos
- **Total de cotizaciones**: Número total de cotizaciones de todos los tipos
- **Cotizaciones totalizadoras**: Cotizaciones con `TotalizingQuotation = true`
- **Cotizaciones con equipo**: Cotizaciones con `EquipmentRemains = true`
- **Detalles por tipo**: Estadísticas individuales de cada tipo de cotización

## Notas

- Todos los endpoints requieren el parámetro `companyCode` para identificar la base de datos específica de la compañía
- Los tipos de cotización no se pueden eliminar si tienen cotizaciones asociadas
- Se incluye cache de salida para mejorar el rendimiento en consultas de solo lectura
- El endpoint de toggle-status es útil para activar/desactivar tipos de cotización sin eliminarlos
- El endpoint with-quotations-count es útil para mostrar estadísticas de uso
- El endpoint de estadísticas proporciona información detallada para análisis de negocio 