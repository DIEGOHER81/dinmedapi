# QuotationDetail API Documentation

## Descripción
API para gestionar detalles de cotizaciones en el sistema DimmedAPI.

## Endpoints

### 1. Obtener todos los detalles de cotización
**GET** `/api/QuotationDetail?companyCode={companyCode}`

Obtiene todos los detalles de cotización con información de la cotización maestra.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
[
  {
    "id": 1,
    "fk_IdQuotationMasterId": 1,
    "productType": "E",
    "codProduct": "PROD001",
    "unit": "UN",
    "quantity": 10,
    "price": 100.00,
    "porcTax": 19.0,
    "taxValue": 19.00,
    "contractTime": 12,
    "warrantyPeriod": "1 año",
    "quotationMaster": {
      "id": 1,
      "idCustomer": 123,
      "creationDateTime": "2024-01-15T10:30:00",
      "dueDate": "2024-02-15T10:30:00",
      "fk_idEmployee": 1,
      "totalizingQuotation": true,
      "equipmentRemains": false
    }
  }
]
```

### 2. Obtener detalle de cotización por ID
**GET** `/api/QuotationDetail/{id}?companyCode={companyCode}`

Obtiene un detalle de cotización específico por su ID.

**Parámetros:**
- `id` (path, requerido): ID del detalle de cotización
- `companyCode` (query, requerido): Código de la compañía

### 3. Obtener detalles de cotización por cotización maestra
**GET** `/api/QuotationDetail/by-quotation/{quotationId}?companyCode={companyCode}`

Obtiene todos los detalles de una cotización específica.

**Parámetros:**
- `quotationId` (path, requerido): ID de la cotización maestra
- `companyCode` (query, requerido): Código de la compañía

### 4. Obtener detalles de cotización por producto
**GET** `/api/QuotationDetail/by-product/{codProduct}?companyCode={companyCode}`

Obtiene todos los detalles de cotización que contengan un producto específico.

**Parámetros:**
- `codProduct` (path, requerido): Código del producto
- `companyCode` (query, requerido): Código de la compañía

### 5. Obtener detalles de cotización por tipo de producto
**GET** `/api/QuotationDetail/by-product-type/{productType}?companyCode={companyCode}`

Obtiene todos los detalles de cotización de un tipo de producto específico.

**Parámetros:**
- `productType` (path, requerido): Tipo de producto (char)
- `companyCode` (query, requerido): Código de la compañía

### 6. Obtener estadísticas de detalles de cotización
**GET** `/api/QuotationDetail/statistics?companyCode={companyCode}`

Obtiene estadísticas detalladas de todos los detalles de cotización.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
{
  "totalQuotationDetails": 150,
  "totalQuotations": 45,
  "totalProducts": 25,
  "productTypes": ["E", "S", "P"],
  "averagePrice": 125.50,
  "averageQuantity": 5.2,
  "averageTaxPercentage": 19.0,
  "totalValue": 18750.00,
  "totalTaxValue": 3562.50
}
```

### 7. Crear detalle de cotización
**POST** `/api/QuotationDetail?companyCode={companyCode}`

Crea un nuevo detalle de cotización.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Body:**
```json
{
  "fk_IdQuotationMasterId": 1,
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

**Validaciones:**
- `fk_IdQuotationMasterId`: Requerido, debe existir la cotización maestra
- `productType`: Requerido, tipo de producto
- `codProduct`: Requerido, máximo 50 caracteres
- `unit`: Requerido, máximo 20 caracteres
- `quantity`: Opcional, mayor a 0
- `price`: Opcional, mayor o igual a 0
- `porcTax`: Opcional, entre 0 y 100
- `taxValue`: Opcional, mayor o igual a 0
- `contractTime`: Opcional, mayor a 0
- `warrantyPeriod`: Opcional, máximo 100 caracteres

### 8. Actualizar detalle de cotización
**PUT** `/api/QuotationDetail/{id}?companyCode={companyCode}`

Actualiza un detalle de cotización existente.

**Parámetros:**
- `id` (path, requerido): ID del detalle de cotización
- `companyCode` (query, requerido): Código de la compañía

**Body:** Mismo formato que en la creación

**Validaciones:** Mismas que en la creación

### 9. Eliminar detalle de cotización
**DELETE** `/api/QuotationDetail/{id}?companyCode={companyCode}`

Elimina un detalle de cotización.

**Parámetros:**
- `id` (path, requerido): ID del detalle de cotización
- `companyCode` (query, requerido): Código de la compañía

### 10. Calcular impuestos para detalle de cotización
**GET** `/api/QuotationDetail/calculate-tax/{id}?companyCode={companyCode}`

Calcula los impuestos y totales para un detalle de cotización específico.

**Parámetros:**
- `id` (path, requerido): ID del detalle de cotización
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
{
  "quotationDetailId": 1,
  "price": 100.00,
  "quantity": 10,
  "subtotal": 1000.00,
  "taxPercentage": 19.0,
  "taxAmount": 190.00,
  "total": 1190.00,
  "currentTaxValue": 19.00
}
```

### 11. Verificar configuración de compañía
**GET** `/api/QuotationDetail/VerificarConfiguracionCompania?companyCode={companyCode}`

Verifica que la configuración de la compañía esté correcta para las operaciones de detalles de cotización.

**Parámetros:**
- `companyCode` (query, requerido): Código de la compañía

**Respuesta:**
```json
{
  "companyCode": "COMP001",
  "hasQuotationDetail": true,
  "hasQuotationMaster": true,
  "totalQuotationDetails": 150,
  "totalQuotations": 45,
  "message": "Configuración verificada correctamente"
}
```

## Códigos de Error

- **400 Bad Request**: 
  - Parámetros faltantes o inválidos
  - Cotización maestra no existe
  - Validaciones de rango o longitud fallidas
- **404 Not Found**: Recurso no encontrado
- **500 Internal Server Error**: Error interno del servidor

## Validaciones

### Creación y Actualización
- **Fk_IdQuotationMasterId**: Requerido, debe existir la cotización maestra
- **ProductType**: Requerido, tipo de producto
- **CodProduct**: Requerido, máximo 50 caracteres
- **Unit**: Requerido, máximo 20 caracteres
- **Quantity**: Opcional, mayor a 0
- **Price**: Opcional, mayor o igual a 0
- **PorcTax**: Opcional, entre 0 y 100
- **TaxValue**: Opcional, mayor o igual a 0
- **ContractTime**: Opcional, mayor a 0
- **WarrantyPeriod**: Opcional, máximo 100 caracteres

### Eliminación
- Se puede eliminar sin restricciones (no afecta integridad referencial)

## Estadísticas Disponibles

El endpoint de estadísticas proporciona información detallada sobre:

- **Total de detalles de cotización**: Número total de detalles
- **Total de cotizaciones**: Número de cotizaciones maestras únicas
- **Total de productos**: Número de productos únicos
- **Tipos de producto**: Lista de tipos de producto utilizados
- **Precio promedio**: Promedio de precios
- **Cantidad promedio**: Promedio de cantidades
- **Porcentaje de impuesto promedio**: Promedio de porcentajes de impuesto
- **Valor total**: Suma de todos los valores (precio * cantidad)
- **Valor total de impuestos**: Suma de todos los valores de impuesto

## Características Especiales

### Cálculo de Impuestos
El endpoint `calculate-tax` permite calcular automáticamente:
- Subtotal (precio * cantidad)
- Monto de impuesto (subtotal * porcentaje de impuesto / 100)
- Total (subtotal + monto de impuesto)

### Búsquedas Especializadas
- **Por cotización**: Obtener todos los detalles de una cotización específica
- **Por producto**: Obtener todos los detalles que contengan un producto específico
- **Por tipo de producto**: Obtener todos los detalles de un tipo de producto específico

### Relaciones
Todos los endpoints incluyen la relación con `QuotationMaster` para proporcionar contexto completo.

## Notas

- Todos los endpoints requieren el parámetro `companyCode` para identificar la base de datos específica de la compañía
- Los detalles de cotización se pueden eliminar sin restricciones
- Se incluye cache de salida para mejorar el rendimiento en consultas de solo lectura
- El endpoint de cálculo de impuestos es útil para validar cálculos antes de guardar
- El endpoint de estadísticas proporciona información detallada para análisis de negocio
- Las búsquedas especializadas facilitan el análisis por diferentes criterios 