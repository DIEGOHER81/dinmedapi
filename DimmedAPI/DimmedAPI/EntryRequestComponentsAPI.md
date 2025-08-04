# EntryRequestComponents API

Esta API proporciona endpoints para gestionar componentes de EntryRequest con soporte multicompañía y sincronización con Business Central.

## Configuración Requerida

Asegúrate de que la compañía esté configurada en la tabla `Companies` con:
- `BCCodigoEmpresa`: Código de la compañía
- `BCURL`: URL de Business Central
- `BCURLWebService`: URL del web service de Business Central

También se requieren las credenciales de Azure AD configuradas en `appsettings.json`:
- `AzureAd:ClientId`
- `AzureAd:ClientSecret`
- `AzureAd:TenantId`

## Endpoints

### 1. Consultar Componentes en Business Central

**GET** `/api/EntryRequestComponents/consultar-bc`

Obtiene componentes de EntryRequest directamente desde Business Central.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía
- `location` (string, opcional): Ubicación
- `stock` (string, opcional): Stock
- `salesCode` (string, opcional): Código de venta

**Ejemplo de respuesta:**
```json
[
  {
    "id": 1,
    "itemNo": "ITEM001",
    "itemName": "Componente 1",
    "warehouse": "WH01",
    "quantity": 10.5,
    "idEntryReq": 0,
    "systemId": "SYS001",
    "quantityConsumed": 0,
    "branch": "BR01",
    "lot": "LOT001",
    "unitPrice": 25.50,
    "status": "ACTIVE",
    "assemblyNo": "ASM001",
    "taxCode": "TAX01",
    "shortDesc": "Comp 1",
    "invima": "INV001",
    "expirationDate": "2024-12-31T00:00:00",
    "traceState": "TRACED",
    "rsFechaVencimiento": "2024-12-31T00:00:00",
    "rsClasifRegistro": "CLASS01"
  }
]
```

### 2. Sincronizar Componentes desde Business Central

**POST** `/api/EntryRequestComponents/sincronizar`

Sincroniza componentes desde Business Central y los almacena en la base de datos local.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía
- `location` (string, opcional): Ubicación
- `stock` (string, opcional): Stock
- `salesCode` (string, opcional): Código de venta

**Ejemplo de respuesta:**
```json
{
  "totalProcesados": 5,
  "componentesNuevos": 3,
  "componentesActualizados": 2,
  "componentes": [
    {
      "id": 1,
      "itemNo": "ITEM001",
      "itemName": "Componente 1",
      "warehouse": "WH01",
      "quantity": 10.5,
      "idEntryReq": 0,
      "systemId": "SYS001",
      "quantityConsumed": 0,
      "branch": "BR01",
      "lot": "LOT001",
      "unitPrice": 25.50,
      "status": "ACTIVE",
      "assemblyNo": "ASM001",
      "taxCode": "TAX01",
      "shortDesc": "Comp 1",
      "invima": "INV001",
      "expirationDate": "2024-12-31T00:00:00",
      "traceState": "TRACED",
      "rsFechaVencimiento": "2024-12-31T00:00:00",
      "rsClasifRegistro": "CLASS01"
    }
  ]
}
```

### 3. Obtener Componentes Locales

**GET** `/api/EntryRequestComponents/locales`

Obtiene todos los componentes almacenados en la base de datos local.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Ejemplo de respuesta:**
```json
[
  {
    "id": 1,
    "itemNo": "ITEM001",
    "itemName": "Componente 1",
    "warehouse": "WH01",
    "quantity": 10.5,
    "idEntryReq": 0,
    "systemId": "SYS001",
    "quantityConsumed": 0,
    "branch": "BR01",
    "lot": "LOT001",
    "unitPrice": 25.50,
    "status": "ACTIVE",
    "assemblyNo": "ASM001",
    "taxCode": "TAX01",
    "shortDesc": "Comp 1",
    "invima": "INV001",
    "expirationDate": "2024-12-31T00:00:00",
    "traceState": "TRACED",
    "rsFechaVencimiento": "2024-12-31T00:00:00",
    "rsClasifRegistro": "CLASS01"
  }
]
```

### 4. Obtener Componente por ItemNo

**GET** `/api/EntryRequestComponents/por-itemno`

Obtiene un componente específico por su número de item.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía
- `itemNo` (string, requerido): Número del item

**Ejemplo de respuesta:**
```json
{
  "id": 1,
  "itemNo": "ITEM001",
  "itemName": "Componente 1",
  "warehouse": "WH01",
  "quantity": 10.5,
  "idEntryReq": 0,
  "systemId": "SYS001",
  "quantityConsumed": 0,
  "branch": "BR01",
  "lot": "LOT001",
  "unitPrice": 25.50,
  "status": "ACTIVE",
  "assemblyNo": "ASM001",
  "taxCode": "TAX01",
  "shortDesc": "Comp 1",
  "invima": "INV001",
  "expirationDate": "2024-12-31T00:00:00",
  "traceState": "TRACED",
  "rsFechaVencimiento": "2024-12-31T00:00:00",
  "rsClasifRegistro": "CLASS01"
}
```

### 5. Obtener Componentes por EntryRequest

**GET** `/api/EntryRequestComponents/por-entryrequest`

Obtiene todos los componentes asociados a un EntryRequest específico.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía
- `idEntryReq` (int, requerido): ID del EntryRequest

**Ejemplo de respuesta:**
```json
[
  {
    "id": 1,
    "itemNo": "ITEM001",
    "itemName": "Componente 1",
    "warehouse": "WH01",
    "quantity": 10.5,
    "idEntryReq": 123,
    "systemId": "SYS001",
    "quantityConsumed": 0,
    "branch": "BR01",
    "lot": "LOT001",
    "unitPrice": 25.50,
    "status": "ACTIVE",
    "assemblyNo": "ASM001",
    "taxCode": "TAX01",
    "shortDesc": "Comp 1",
    "invima": "INV001",
    "expirationDate": "2024-12-31T00:00:00",
    "traceState": "TRACED",
    "rsFechaVencimiento": "2024-12-31T00:00:00",
    "rsClasifRegistro": "CLASS01"
  }
]
```

### 6. Obtener Estadísticas

**GET** `/api/EntryRequestComponents/estadisticas`

Obtiene estadísticas de sincronización de componentes.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Ejemplo de respuesta:**
```json
{
  "totalComponents": 150,
  "componentsWithQuantity": 120,
  "componentsWithExpiration": 80,
  "lastSync": "2024-01-15T10:30:00"
}
```

### 7. Eliminar Componente

**DELETE** `/api/EntryRequestComponents/{id}`

Elimina un componente específico de la base de datos local.

**Parámetros de ruta:**
- `id` (int, requerido): ID del componente a eliminar

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Ejemplo de respuesta:**
```json
{
  "mensaje": "Componente eliminado exitosamente"
}
```

### 8. Obtener Líneas de Ensamble

**GET** `/api/EntryRequestComponents/assembly-lines`

Obtiene líneas de ensamble desde Business Central usando el método `lylassemblylines`.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía
- `documentNo` (string, opcional): Número de documento para filtrar

**Ejemplo de respuesta:**
```json
[
  {
    "id": 0,
    "itemNo": "3104DIL633",
    "itemName": "DILUSOR 633 GAP B1 BLEND CENTER-BLUE Y G",
    "warehouse": "108",
    "quantity": 1,
    "idEntryReq": 10000,
    "systemId": null,
    "quantityConsumed": null,
    "branch": null,
    "lot": null,
    "unitPrice": null,
    "status": "Order",
    "assemblyNo": "1003",
    "taxCode": null,
    "shortDesc": "UND",
    "invima": null,
    "expirationDate": null,
    "traceState": null,
    "rsFechaVencimiento": null,
    "rsClasifRegistro": null
  }
]
```

### 9. Verificar Configuración

**GET** `/api/EntryRequestComponents/verificar-configuracion`

Verifica la configuración de la compañía y Business Central.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Ejemplo de respuesta:**
```json
{
  "company": {
    "id": 1,
    "businessName": "Empresa Ejemplo",
    "bcCodigoEmpresa": "COMP001"
  },
  "businessCentral": {
    "urlWS": "https://api.businesscentral.dynamics.com/v2.0/...",
    "url": "https://api.businesscentral.dynamics.com/v2.0/...",
    "company": "COMP001"
  }
}
```

### 8. Crear Componente Local

**POST** `/api/EntryRequestComponents/crear-local`

Crea un componente localmente con los datos proporcionados, obteniendo información adicional desde las tablas ItemsBC y Branches.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía
- `itemNo` (string, requerido): Número del item
- `quantity` (decimal, requerido): Cantidad del componente
- `idEntryReq` (int, requerido): ID del EntryRequest
- `assemblyNo` (string, requerido): Número de ensamble
- `branch` (int, requerido): ID de la sucursal

**Lógica del endpoint:**
1. Valida que todos los parámetros requeridos estén presentes
2. Busca el item en la tabla `ItemsBC` usando el `itemNo` para obtener:
   - `ItemName`
   - `TaxCode`
   - `ShortDesc`
   - `Invima`
3. Busca la sucursal en la tabla `Branches` usando el `branch` para obtener:
   - `LocationCode` (se asigna a `Warehouse` y `Branch`)
4. Crea el componente con los siguientes valores por defecto:
   - `TraceState`: cadena vacía
   - `RSFechaVencimiento`: null
   - `RSClasifRegistro`: cadena vacía
   - `ExpirationDate`: 2999-12-31
   - `SystemId`: nuevo GUID
   - `QuantityConsumed`: 0
   - `UnitPrice`: 0
   - `status`: "ACTIVE"
   - `Lot`: cadena vacía

**Ejemplo de respuesta exitosa:**
```json
{
  "mensaje": "Componente creado exitosamente",
  "componente": {
    "id": 123,
    "itemNo": "10103B0020066",
    "itemName": "Nombre del Item",
    "warehouse": "LOC",
    "quantity": 10.5,
    "idEntryReq": 456,
    "systemId": "guid-generado",
    "quantityConsumed": 0,
    "branch": "LOC",
    "lot": "",
    "unitPrice": 0,
    "status": "ACTIVE",
    "assemblyNo": "ASM001",
    "taxCode": "TAX01",
    "shortDesc": "Descripción corta",
    "invima": "INV001",
    "expirationDate": "2999-12-31T00:00:00",
    "traceState": "",
    "rsFechaVencimiento": null,
    "rsClasifRegistro": ""
  }
}
```

**Ejemplo de respuesta de error:**
```json
{
  "mensaje": "No se encontró el item con código: 10103B0020066"
}
```

**Códigos de error:**
- `400 Bad Request`: Parámetros faltantes o inválidos
- `400 Bad Request`: Item o sucursal no encontrados
- `500 Internal Server Error`: Error interno del servidor

## Códigos de Error

- **400 Bad Request**: Parámetros requeridos faltantes o inválidos
- **404 Not Found**: Compañía no encontrada o componente no encontrado
- **500 Internal Server Error**: Error interno del servidor

## Ejemplos de Uso

### Consultar componentes en BC (con filtros)
```bash
curl -X GET "https://api.example.com/api/EntryRequestComponents/consultar-bc?companyCode=COMP001&location=WH01&stock=STK01&salesCode=SALES001"
```

### Consultar componentes en BC (sin filtros)
```bash
curl -X GET "https://api.example.com/api/EntryRequestComponents/consultar-bc?companyCode=COMP001"
```

### Sincronizar componentes (con filtros)
```bash
curl -X POST "https://api.example.com/api/EntryRequestComponents/sincronizar?companyCode=COMP001&location=WH01&stock=STK01&salesCode=SALES001"
```

### Sincronizar componentes (sin filtros)
```bash
curl -X POST "https://api.example.com/api/EntryRequestComponents/sincronizar?companyCode=COMP001"
```

### Obtener componentes locales
```bash
curl -X GET "https://api.example.com/api/EntryRequestComponents/locales?companyCode=COMP001"
```

### Obtener líneas de ensamble (todas)
```bash
curl -X GET "https://api.example.com/api/EntryRequestComponents/assembly-lines?companyCode=COMP001"
```

### Obtener líneas de ensamble por documento
```bash
curl -X GET "https://api.example.com/api/EntryRequestComponents/assembly-lines?companyCode=COMP001&documentNo=1003"
```

## Notas Importantes

1. **Soporte Multicompañía**: Todos los endpoints requieren el parámetro `companyCode` para identificar la compañía específica.
2. **Sincronización**: Los componentes se pueden sincronizar desde Business Central y se almacenan en la base de datos local.
3. **Parámetros Opcionales**: Los parámetros `location`, `stock` y `salesCode` son opcionales. Si no se proporcionan, se obtienen todos los componentes disponibles.
4. **Filtros Dinámicos**: Los filtros se aplican dinámicamente según los parámetros proporcionados. Si no se especifica `salesCode`, no se aplica filtro por código de venta.
5. **Líneas de Ensamble**: El endpoint `assembly-lines` consulta el método `lylassemblylines` de Business Central y mapea los campos específicos de líneas de ensamble.
6. **Validación**: Se valida que el parámetro `companyCode` esté presente antes de procesar las solicitudes.
7. **Manejo de Errores**: Los errores se manejan de forma consistente con mensajes descriptivos.
8. **Estadísticas**: Se proporcionan estadísticas de sincronización para monitorear el estado de los datos. 