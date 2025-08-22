# LotsAPI

API para consultar lotes disponibles de referencia en Business Central, soportando multi-compañía.

## Información General
- **Tipo:** API Business Central
- **Base URL:** `/api/Lots`
- **Método:** GET
- **Formato de respuesta:** JSON
- **Origen de datos:** Business Central

## Parámetro obligatorio
- **companyCode** (query): Código de la compañía sobre la que se realiza la consulta.

## Endpoints

### 1. Consultar lotes disponibles
- **GET** `/api/Lots/available?companyCode={companyCode}&itemCode={itemCode}&locationCode={locationCode}`
- **Descripción:** Obtiene los lotes disponibles para un artículo específico en una bodega determinada.
- **Parámetros:**
  - `companyCode` (query, requerido): Código de la compañía.
  - `itemCode` (query, requerido): Código del artículo/referencia.
  - `locationCode` (query, requerido): Código de la bodega.
- **Respuesta exitosa:** 200 OK
- **Respuesta de error:** 400 Bad Request, 500 Internal Server Error
- **Ejemplo de respuesta:**
```json
[
  {
    "id": 0,
    "code": "ITEM-001",
    "description": null,
    "shortDesc": null,
    "invima": null,
    "lot": "LOT123",
    "quantity": 0.0,
    "unitPrice": null,
    "assemblyNo": "",
    "entryRequestId": 0,
    "entryRequestDetailId": null,
    "quantityConsumed": null,
    "expirationDate": "2024-12-31T00:00:00",
    "name": null,
    "reservedQuantity": 5.0,
    "location_Code_ile": "BODEGA01",
    "classification": null,
    "status": null,
    "lineNo": null,
    "position": null,
    "quantity_ile": 10.0,
    "taxCode": null,
    "lowTurnover": null,
    "isComponent": null,
    "rsFechaVencimiento": "2024-12-31T00:00:00",
    "rsClasifRegistro": "REG001",
    "locationCode": null
  }
]
```

---

### 2. Verificar configuración de compañía
- **GET** `/api/Lots/config?companyCode={companyCode}`
- **Descripción:** Verifica la configuración de Business Central para una compañía específica.
- **Parámetros:**
  - `companyCode` (query, requerido): Código de la compañía.
- **Respuesta exitosa:** 200 OK
- **Respuesta de error:** 400 Bad Request, 500 Internal Server Error
- **Ejemplo de respuesta:**
```json
{
  "businessCentral": {
    "url": "https://api.businesscentral.dynamics.com/v2.0/...",
    "company": "6c265367-24c4-ed11-9a88-002248e00201"
  }
}
```

---

## Códigos de Error

### 400 Bad Request
- `"El código de compañía es requerido"` - Cuando no se proporciona el código de compañía
- `"El código del artículo es requerido"` - Cuando no se proporciona el código del artículo
- `"El código de bodega es requerido"` - Cuando no se proporciona el código de bodega
- `"Compañía con código {companyCode} no encontrada"` - Cuando la compañía no existe

### 500 Internal Server Error
- `"Error interno del servidor: {mensaje}"` - Errores internos del servidor

---

## Ejemplos de Uso

### Consultar lotes disponibles
```http
GET /api/Lots/available?companyCode=COMP001&itemCode=ITEM-001&locationCode=BODEGA01
```

### Verificar configuración
```http
GET /api/Lots/config?companyCode=COMP001
```

---

## Archivos Relacionados
- **Controlador:** `Controllers/LotsController.cs`
- **Servicio BC:** `BO/bcConn.cs`
- **Entidad:** `Entidades/EntryRequestAssembly.cs`
- **DTO:** `DTOs/EntryRequestAssemblyResponseDTO.cs`
- **Servicio de Conexión:** `Services/DynamicBCConnectionService.cs`

---

## Notas Técnicas

### Filtros de Business Central
El endpoint utiliza el filtro OData:
```
?$filter=(itemNo eq '{itemCode}') and (locationCodeile eq '{locationCode}')
```

### Campos de Lote
Los lotes retornados incluyen información sobre:
- **Código del artículo** (`Code`)
- **Número de lote** (`Lot`)
- **Cantidad reservada** (`ReservedQuantity`)
- **Cantidad disponible** (`Quantity_ile`)
- **Código de ubicación** (`Location_Code_ile`)
- **Fecha de vencimiento** (`ExpirationDate`)
- **Clasificación de registro** (`RSClasifRegistro`)
- **Fecha de vencimiento RS** (`RSFechaVencimiento`)

### Manejo de Fechas
- Las fechas de vencimiento se procesan desde el formato de Business Central
- Si no hay fecha de vencimiento, se establece como "2999-01-01"
- Se manejan excepciones para fechas inválidas o nulas

### Multi-compañía
- Cada consulta requiere el parámetro `companyCode`
- La conexión a Business Central se establece dinámicamente según la compañía
- Se valida la existencia de la compañía antes de realizar la consulta
