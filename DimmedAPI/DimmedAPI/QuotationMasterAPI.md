# QuotationMaster API Documentation

## Descripción
API para gestionar cotizaciones maestras y sus detalles en el sistema DimmedAPI.

## Endpoints

### 1. Obtener todas las cotizaciones
**GET** `/api/QuotationMaster?companyCode={companyCode}`

Obtiene todas las cotizaciones con sus entidades relacionadas.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
[
  {
    "id": 1,
    "fk_idBranch": 1,
    "customerOrigin": "N",
    "fk_idCustomerType": 1,
    "idCustomer": 123,
    "creationDateTime": "2024-01-15T10:30:00",
    "dueDate": "2024-02-15T10:30:00",
    "fk_idEmployee": 1,
    "fk_QuotationTypeId": 1,
    "paymentTerm": 30,
    "fk_CommercialConditionId": 1,
    "totalizingQuotation": true,
    "equipmentRemains": false,
    "monthlyConsumption": 100.5,
    "branch": { ... },
    "customerType": { ... },
    "employee": { ... },
    "quotationType": { ... },
    "commercialCondition": { ... }
  }
]
```

### 2. Obtener cotización por ID
**GET** `/api/QuotationMaster/{id}?companyCode={companyCode}`

Obtiene una cotización específica por su ID.

**Parámetros:**
- `id` (path, requerido): ID de la cotización
- `companyCode` (query, requerido): Código de la compañía

### 3. Obtener cotización con detalles
**GET** `/api/QuotationMaster/with-details/{id}?companyCode={companyCode}`

Obtiene una cotización específica con todos sus detalles incluidos.

**Parámetros:**
- `id` (path, requerido): ID de la cotización
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
{
  "id": 1,
  "fk_idBranch": 1,
  "customerOrigin": "N",
  "fk_idCustomerType": 1,
  "idCustomer": 123,
  "creationDateTime": "2024-01-15T10:30:00",
  "dueDate": "2024-02-15T10:30:00",
  "fk_idEmployee": 1,
  "fk_QuotationTypeId": 1,
  "paymentTerm": 30,
  "fk_CommercialConditionId": 1,
  "totalizingQuotation": true,
  "equipmentRemains": false,
  "monthlyConsumption": 100.5,
  "branch": {
    "id": 1,
    "name": "Sucursal Principal",
    "systemId": "001",
    "locationCode": "001"
  },
  "customerType": {
    "id": 1,
    "description": "Cliente Regular",
    "isActive": true
  },
  "employee": {
    "id": 1,
    "code": "EMP001",
    "name": "Juan Pérez",
    "charge": "Vendedor",
    "phone": "3001234567",
    "email": "juan.perez@empresa.com"
  },
  "quotationType": {
    "id": 1,
    "description": "Cotización Estándar",
    "isActive": true
  },
  "commercialCondition": {
    "id": 1,
    "description": "Condición Regular",
    "commercialText": "Pago a 30 días",
    "isActive": true
  },
  "details": [
    {
      "id": 1,
      "productType": "E",
      "codProduct": "PROD001",
      "unit": "UN",
      "quantity": 10,
      "price": 100.00,
      "porcTax": 19.0,
      "taxValue": 19.00,
      "contractTime": 12,
      "warrantyPeriod": "1 año"
    }
  ]
}
```

### 4. Obtener cotizaciones por cliente
**GET** `/api/QuotationMaster/by-customer/{customerId}?companyCode={companyCode}`

Obtiene todas las cotizaciones de un cliente específico.

**Parámetros:**
- `customerId` (path, requerido): ID del cliente
- `companyCode` (query, requerido): Código de la compañía

### 5. Obtener cotizaciones por sucursal
**GET** `/api/QuotationMaster/by-branch/{branchId}?companyCode={companyCode}`

Obtiene todas las cotizaciones de una sucursal específica.

**Parámetros:**
- `branchId` (path, requerido): ID de la sucursal
- `companyCode` (query, requerido): Código de la compañía

### 6. Obtener cotizaciones por empleado
**GET** `/api/QuotationMaster/by-employee/{employeeId}?companyCode={companyCode}`

Obtiene todas las cotizaciones creadas por un empleado específico.

**Parámetros:**
- `employeeId` (path, requerido): ID del empleado
- `companyCode` (query, requerido): Código de la compañía

### 7. Crear cotización
**POST** `/api/QuotationMaster?companyCode={companyCode}`

Crea una nueva cotización.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Body:**
```json
{
  "fk_idBranch": 1,
  "customerOrigin": "N",
  "fk_idCustomerType": 1,
  "idCustomer": 123,
  "dueDate": "2024-02-15T10:30:00",
  "fk_idEmployee": 1,
  "fk_QuotationTypeId": 1,
  "paymentTerm": 30,
  "fk_CommercialConditionId": 1,
  "totalizingQuotation": true,
  "equipmentRemains": false,
  "monthlyConsumption": 100.5
}
```

### 8. Actualizar cotización
**PUT** `/api/QuotationMaster/{id}?companyCode={companyCode}`

Actualiza una cotización existente.

**Parámetros:**
- `id` (path, requerido): ID de la cotización
- `companyCode` (query, requerido): Código de la compañía

**Body:** Mismo formato que en la creación

### 9. Eliminar cotización
**DELETE** `/api/QuotationMaster/{id}?companyCode={companyCode}`

Elimina una cotización. Solo se puede eliminar si no tiene detalles asociados.

**Parámetros:**
- `id` (path, requerido): ID de la cotización
- `companyCode` (query, requerido): Código de la compañía

### 10. Obtener detalles de cotización
**GET** `/api/QuotationMaster/{id}/details?companyCode={companyCode}`

Obtiene todos los detalles de una cotización específica.

**Parámetros:**
- `id` (path, requerido): ID de la cotización
- `companyCode` (query, requerido): Código de la compañía

### 11. Agregar detalle a cotización
**POST** `/api/QuotationMaster/{id}/details?companyCode={companyCode}`

Agrega un nuevo detalle a una cotización existente.

**Parámetros:**
- `id` (path, requerido): ID de la cotización
- `companyCode` (query, requerido): Código de la compañía

**Body:**
```json
{
  "productType": "E",
  "codProduct": "PROD001",
  "unit": "UN",
  "quantity": 10,
  "price": 100.00,
  "porcTax": 19.0,
  "taxValue": 19.00,
  "contractTime": 12,
  "warrantyPeriod": "1 año"
}
```

### 12. Eliminar detalle de cotización
**DELETE** `/api/QuotationMaster/{quotationId}/details/{detailId}?companyCode={companyCode}`

Elimina un detalle específico de una cotización.

**Parámetros:**
- `quotationId` (path, requerido): ID de la cotización
- `detailId` (path, requerido): ID del detalle
- `companyCode` (query, requerido): Código de la compañía

### 13. Verificar configuración de compañía
**GET** `/api/QuotationMaster/VerificarConfiguracionCompania?companyCode={companyCode}`

Verifica que la configuración de la compañía esté correcta para las operaciones de cotizaciones.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

## Códigos de Error

- **400 Bad Request**: Parámetros faltantes o inválidos
- **404 Not Found**: Recurso no encontrado
- **500 Internal Server Error**: Error interno del servidor

## Notas

- Todos los endpoints requieren el parámetro `companyCode` para identificar la base de datos específica de la compañía
- Las cotizaciones no se pueden eliminar si tienen detalles asociados
- La fecha de creación se establece automáticamente al crear una cotización
- Se incluye cache de salida para mejorar el rendimiento en consultas de solo lectura 