# CommercialCondition API Documentation

## Descripción
API para gestionar condiciones comerciales en el sistema DimmedAPI.

## Endpoints

### 1. Obtener todas las condiciones comerciales
**GET** `/api/CommercialCondition?companyCode={companyCode}`

Obtiene todas las condiciones comerciales.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
[
  {
    "id": 1,
    "description": "Pago a 30 días",
    "commercialText": "Condiciones de pago estándar a 30 días",
    "isActive": true
  },
  {
    "id": 2,
    "description": "Pago inmediato",
    "commercialText": "Pago al contado con descuento del 5%",
    "isActive": true
  }
]
```

### 2. Obtener condiciones comerciales activas
**GET** `/api/CommercialCondition/active?companyCode={companyCode}`

Obtiene solo las condiciones comerciales que están activas.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

### 3. Obtener condiciones comerciales con conteo de cotizaciones
**GET** `/api/CommercialCondition/with-quotations-count?companyCode={companyCode}`

Obtiene todas las condiciones comerciales con el número de cotizaciones asociadas.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
[
  {
    "id": 1,
    "description": "Pago a 30 días",
    "commercialText": "Condiciones de pago estándar a 30 días",
    "isActive": true,
    "quotationsCount": 45
  },
  {
    "id": 2,
    "description": "Pago inmediato",
    "commercialText": "Pago al contado con descuento del 5%",
    "isActive": true,
    "quotationsCount": 18
  }
]
```

### 4. Obtener condición comercial por ID
**GET** `/api/CommercialCondition/{id}?companyCode={companyCode}`

Obtiene una condición comercial específica por su ID.

**Parámetros:**
- `id` (path, requerido): ID de la condición comercial
- `companyCode` (query, requerido): Código de la compañía

### 5. Buscar condiciones comerciales por descripción
**GET** `/api/CommercialCondition/by-description/{description}?companyCode={companyCode}`

Busca condiciones comerciales que contengan la descripción especificada.

**Parámetros:**
- `description` (path, requerido): Descripción a buscar
- `companyCode` (query, requerido): Código de la compañía

### 6. Buscar condiciones comerciales por texto comercial
**GET** `/api/CommercialCondition/by-commercial-text/{commercialText}?companyCode={companyCode}`

Busca condiciones comerciales que contengan el texto comercial especificado.

**Parámetros:**
- `commercialText` (path, requerido): Texto comercial a buscar
- `companyCode` (query, requerido): Código de la compañía

### 7. Crear condición comercial
**POST** `/api/CommercialCondition?companyCode={companyCode}`

Crea una nueva condición comercial.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Body:**
```json
{
  "description": "Pago a 60 días",
  "commercialText": "Condiciones especiales de pago a 60 días para clientes corporativos",
  "isActive": true
}
```

**Validaciones:**
- La descripción es requerida y no puede exceder 200 caracteres
- El texto comercial es opcional y no puede exceder 1000 caracteres
- No puede existir otra condición comercial con la misma descripción

### 8. Actualizar condición comercial
**PUT** `/api/CommercialCondition/{id}?companyCode={companyCode}`

Actualiza una condición comercial existente.

**Parámetros:**
- `id` (path, requerido): ID de la condición comercial
- `companyCode` (query, requerido): Código de la compañía

**Body:** Mismo formato que en la creación

**Validaciones:**
- La descripción es requerida y no puede exceder 200 caracteres
- El texto comercial es opcional y no puede exceder 1000 caracteres
- No puede existir otra condición comercial con la misma descripción (excluyendo la actual)

### 9. Cambiar estado de la condición comercial
**PATCH** `/api/CommercialCondition/{id}/toggle-status?companyCode={companyCode}`

Cambia el estado activo/inactivo de una condición comercial.

**Parámetros:**
- `id` (path, requerido): ID de la condición comercial
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
{
  "id": 1,
  "isActive": false
}
```

### 10. Eliminar condición comercial
**DELETE** `/api/CommercialCondition/{id}?companyCode={companyCode}`

Elimina una condición comercial. Solo se puede eliminar si no tiene cotizaciones asociadas.

**Parámetros:**
- `id` (path, requerido): ID de la condición comercial
- `companyCode` (query, requerido): Código de la compañía

### 11. Obtener cotizaciones de una condición comercial
**GET** `/api/CommercialCondition/{id}/quotations?companyCode={companyCode}`

Obtiene todas las cotizaciones asociadas a una condición comercial específica.

**Parámetros:**
- `id` (path, requerido): ID de la condición comercial
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
    "monthlyConsumption": 100.5,
    "paymentTerm": 30
  }
]
```

### 12. Obtener estadísticas de condiciones comerciales
**GET** `/api/CommercialCondition/statistics?companyCode={companyCode}`

Obtiene estadísticas detalladas de todas las condiciones comerciales.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
{
  "totalCommercialConditions": 4,
  "activeCommercialConditions": 3,
  "totalQuotations": 63,
  "totalizingQuotations": 25,
  "equipmentRemainsQuotations": 12,
  "averagePaymentTerm": 32.5,
  "details": [
    {
      "id": 1,
      "description": "Pago a 30 días",
      "commercialText": "Condiciones de pago estándar a 30 días",
      "isActive": true,
      "quotationsCount": 45,
      "totalizingQuotationsCount": 18,
      "equipmentRemainsCount": 8,
      "averagePaymentTerm": 30.0
    },
    {
      "id": 2,
      "description": "Pago inmediato",
      "commercialText": "Pago al contado con descuento del 5%",
      "isActive": true,
      "quotationsCount": 18,
      "totalizingQuotationsCount": 7,
      "equipmentRemainsCount": 4,
      "averagePaymentTerm": 0.0
    }
  ]
}
```

### 13. Verificar configuración de compañía
**GET** `/api/CommercialCondition/VerificarConfiguracionCompania?companyCode={companyCode}`

Verifica que la configuración de la compañía esté correcta para las operaciones de condiciones comerciales.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
{
  "companyCode": "COMP001",
  "hasCommercialCondition": true,
  "totalCommercialConditions": 4,
  "activeCommercialConditions": 3,
  "message": "Configuración verificada correctamente"
}
```

## Códigos de Error

- **400 Bad Request**: 
  - Parámetros faltantes o inválidos
  - Descripción duplicada
  - Descripción excede el límite de caracteres
  - Texto comercial excede el límite de caracteres
- **404 Not Found**: Recurso no encontrado
- **500 Internal Server Error**: Error interno del servidor

## Validaciones

### Creación y Actualización
- **Description**: Requerido, máximo 200 caracteres
- **CommercialText**: Opcional, máximo 1000 caracteres
- **IsActive**: Opcional, por defecto true
- **Unicidad**: No puede existir otra condición comercial con la misma descripción

### Eliminación
- Solo se puede eliminar si no tiene cotizaciones asociadas
- Si tiene cotizaciones, se recomienda desactivar en lugar de eliminar

## Estadísticas Disponibles

El endpoint de estadísticas proporciona información detallada sobre:

- **Total de condiciones comerciales**: Número total de condiciones configuradas
- **Condiciones activas**: Número de condiciones que están activas
- **Total de cotizaciones**: Número total de cotizaciones de todas las condiciones
- **Cotizaciones totalizadoras**: Cotizaciones con `TotalizingQuotation = true`
- **Cotizaciones con equipo**: Cotizaciones con `EquipmentRemains = true`
- **Promedio de términos de pago**: Promedio de los términos de pago de todas las cotizaciones
- **Detalles por condición**: Estadísticas individuales de cada condición comercial

## Características Especiales

### Búsqueda por Texto Comercial
A diferencia de otros controladores, este incluye un endpoint específico para buscar por el campo `CommercialText`, que es útil para encontrar condiciones comerciales basadas en su contenido textual.

### Análisis de Términos de Pago
El endpoint de estadísticas incluye el cálculo del promedio de términos de pago, lo que es especialmente útil para análisis financieros y comerciales.

## Notas

- Todos los endpoints requieren el parámetro `companyCode` para identificar la base de datos específica de la compañía
- Las condiciones comerciales no se pueden eliminar si tienen cotizaciones asociadas
- Se incluye cache de salida para mejorar el rendimiento en consultas de solo lectura
- El endpoint de toggle-status es útil para activar/desactivar condiciones comerciales sin eliminarlas
- El endpoint with-quotations-count es útil para mostrar estadísticas de uso
- El endpoint de estadísticas proporciona información detallada para análisis de negocio
- El endpoint de búsqueda por texto comercial es útil para encontrar condiciones específicas 