# EntryRequestAssemblyAPI

API para gestionar ensambles de solicitudes de entrada en la base de datos local, soportando multi-compañía.

## Información General
- **Tipo:** API Local (base de datos local)
- **Base URL:** `/api/EntryRequestAssembly`
- **Método:** GET, POST
- **Formato de respuesta:** JSON
- **Base de datos:** SQL Server local

## Parámetro obligatorio
- **companyCode** (query): Código de la compañía sobre la que se realiza la consulta.

## Endpoints

### 1. Obtener todos los ensambles
- **GET** `/api/EntryRequestAssembly?companyCode={companyCode}`
- **Descripción:** Obtiene todos los ensambles de solicitudes de entrada.
- **Parámetros:**
  - `companyCode` (query, requerido): Código de la compañía.
- **Respuesta exitosa:** 200 OK
- **Respuesta de error:** 400 Bad Request
- **Ejemplo de respuesta:**
```json
[
  {
    "id": 1,
    "code": "ASSEMBLY-001",
    "description": "Ensemble de prueba",
    "lot": "LOT123",
    "quantity": 10.0,
    "assemblyNo": "EQ-001",
    "entryRequestId": 22,
    "quantityConsumed": 5.0
  }
]
```

---

### 2. Obtener ensamble por ID
- **GET** `/api/EntryRequestAssembly/{id}?companyCode={companyCode}`
- **Descripción:** Obtiene un ensamble específico por su ID.
- **Parámetros:**
  - `id` (path, requerido): ID del ensamble.
  - `companyCode` (query, requerido): Código de la compañía.
- **Respuesta exitosa:** 200 OK
- **Respuesta de error:** 400 Bad Request, 404 Not Found
- **Ejemplo de respuesta:**
```json
{
  "id": 1,
  "code": "ASSEMBLY-001",
  "description": "Ensemble de prueba",
  "lot": "LOT123",
  "quantity": 10.0,
  "assemblyNo": "EQ-001",
  "entryRequestId": 22,
  "quantityConsumed": 5.0
}
```

---

### 3. Filtrar ensambles
- **GET** `/api/EntryRequestAssembly/filtro?companyCode={companyCode}&id={id}&code={code}&lot={lot}&assemblyNo={assemblyNo}&entryRequestId={entryRequestId}&entryRequestDetailId={entryRequestDetailId}`
- **Descripción:** Filtra ensambles por múltiples criterios (todos opcionales).
- **Parámetros:**
  - `companyCode` (query, requerido): Código de la compañía.
  - `id` (query, opcional): ID del ensemble.
  - `code` (query, opcional): Código del ensemble.
  - `lot` (query, opcional): Lote.
  - `assemblyNo` (query, opcional): Número de ensamble.
  - `entryRequestId` (query, opcional): ID de la solicitud de entrada.
  - `entryRequestDetailId` (query, opcional): ID del detalle de la solicitud.
- **Respuesta exitosa:** 200 OK
- **Respuesta de error:** 400 Bad Request
- **Ejemplo de uso:**
```
GET /api/EntryRequestAssembly/filtro?companyCode=COMP001&lot=LOT123&entryRequestId=22
```

---

### 4. Obtener ensambles con detalles y equipos
- **GET** `/api/EntryRequestAssembly/entry-request/{entryRequestId}/with-details?companyCode={companyCode}&quantityConsumedFilter={filter}`
- **Descripción:** Obtiene ensambles con detalles de solicitud y equipos consumidos usando SQL directo (equivalente a la primera consulta SQL).
- **Parámetros:**
  - `entryRequestId` (path, requerido): ID de la solicitud de entrada.
  - `companyCode` (query, requerido): Código de la compañía.
  - `quantityConsumedFilter` (query, opcional): Filtro de cantidad consumida.
    - `"greater"` (default): Solo registros con QuantityConsumed > 0
    - `"zero"`: Solo registros con QuantityConsumed = 0
    - `"all"`: Todos los registros
- **Respuesta exitosa:** 200 OK
- **Respuesta de error:** 400 Bad Request
- **Ejemplo de respuesta:**
```json
[
  {
    "entryRequest": {
      "id": 22,
      "requestNumber": "REQ-001"
    },
    "entryRequestAssembly": {
      "id": 1,
      "code": "ASSEMBLY-001",
      "quantityConsumed": 5.0
    },
    "entryRequestDetail": {
      "id": 1,
      "idEquipment": 1
    },
    "equipment": {
      "id": 1,
      "code": "EQ-001"
    }
  }
]
```

---

### 5. Crear nuevo ensamble
- **POST** `/api/EntryRequestAssembly?companyCode={companyCode}`
- **Descripción:** Crea un nuevo ensamble de solicitud de entrada.
- **Parámetros:**
  - `companyCode` (query, requerido): Código de la compañía.
- **Body (JSON):**
```json
{
  "code": "ASSEMBLY-001",
  "description": "Ensemble de prueba",
  "shortDesc": "Descripción corta",
  "invima": "INV123",
  "lot": "LOT123",
  "quantity": 10.0,
  "unitPrice": 150.50,
  "assemblyNo": "EQ-001",
  "entryRequestId": 22,
  "entryRequestDetailId": 1,
  "quantityConsumed": 0,
  "expirationDate": "2024-12-31T00:00:00",
  "reservedQuantity": 5.0,
  "location_Code_ile": "LOC001",
  "classification": "CLASE-A",
  "status": "Activo",
  "lineNo": 1,
  "position": 1,
  "quantity_ile": 8.0,
  "taxCode": "IVA",
  "lowTurnover": false,
  "isComponent": false,
  "rsFechaVencimiento": "2024-12-31T00:00:00",
  "rsClasifRegistro": "REG-A"
}
```

**Nota:** Si `entryRequestDetailId` es 0 o null, se guardará como null en la base de datos.
- **Respuesta exitosa:** 201 Created
- **Respuesta de error:** 400 Bad Request
- **Ejemplo de respuesta:**
```json
{
  "id": 1,
  "code": "ASSEMBLY-001",
  "description": "Ensemble de prueba",
  "shortDesc": "Descripción corta",
  "invima": "INV123",
  "lot": "LOT123",
  "quantity": 10.0,
  "unitPrice": 150.50,
  "assemblyNo": "EQ-001",
  "entryRequestId": 22,
  "entryRequestDetailId": 1,
  "quantityConsumed": 0,
  "expirationDate": "2024-12-31T00:00:00",
  "reservedQuantity": 5.0,
  "location_Code_ile": "LOC001",
  "classification": "CLASE-A",
  "status": "Activo",
  "lineNo": 1,
  "position": 1,
  "quantity_ile": 8.0,
  "taxCode": "IVA",
  "lowTurnover": false,
  "isComponent": false,
  "rsFechaVencimiento": "2024-12-31T00:00:00",
  "rsClasifRegistro": "REG-A"
}
```

---

### 6. Obtener componentes de solicitud
- **GET** `/api/EntryRequestAssembly/entry-request/{entryRequestId}/components?companyCode={companyCode}&quantityConsumedFilter={filter}`
- **Descripción:** Obtiene componentes de solicitud de entrada usando SQL directo (equivalente a la segunda consulta SQL).
- **Parámetros:**
  - `entryRequestId` (path, requerido): ID de la solicitud de entrada.
  - `companyCode` (query, requerido): Código de la compañía.
  - `quantityConsumedFilter` (query, opcional): Filtro de cantidad consumida.
    - `"greater"` (default): Solo registros con QuantityConsumed > 0
    - `"zero"`: Solo registros con QuantityConsumed = 0
    - `"all"`: Todos los registros
- **Respuesta exitosa:** 200 OK
- **Respuesta de error:** 400 Bad Request
- **Ejemplo de respuesta:**
```json
[
  {
    "entryRequest": {
      "id": 22,
      "requestNumber": "REQ-001"
    },
    "entryRequestComponent": {
      "id": 1,
      "code": "COMP-001",
      "quantityConsumed": 3.0
    }
  }
]
```

---

## Códigos de Error

### 400 Bad Request
```json
{
  "mensaje": "Error al obtener los ensambles",
  "detalle": "Descripción específica del error"
}
```

**Causas comunes:**
- `companyCode` no proporcionado
- Error de conexión con la base de datos
- Parámetros inválidos

### 404 Not Found
```json
"Ensemble no encontrado"
```

---

## Ejemplos de Uso

### Ejemplo 1: Obtener todos los ensambles
```bash
curl -X GET "https://tu-api.com/api/EntryRequestAssembly?companyCode=COMP001"
```

### Ejemplo 2: Filtrar por lote y solicitud
```bash
curl -X GET "https://tu-api.com/api/EntryRequestAssembly/filtro?companyCode=COMP001&lot=LOT123&entryRequestId=22"
```

### Ejemplo 3: Obtener ensambles con detalles (solo consumidos)
```bash
curl -X GET "https://tu-api.com/api/EntryRequestAssembly/entry-request/22/with-details?companyCode=COMP001&quantityConsumedFilter=greater"
```

### Ejemplo 4: Obtener componentes (todos)
```bash
curl -X GET "https://tu-api.com/api/EntryRequestAssembly/entry-request/22/components?companyCode=COMP001&quantityConsumedFilter=all"
```

---

## Consultas SQL Equivalentes

### Endpoint 4: Ensambles con detalles
```sql
select *
from EntryRequests er 
left join EntryRequestAssembly era on er.id = era.EntryRequestid
left join EntryRequestDetails erd on erd.id = era.EntryRequestDetailId 
left join Equipment eq on erd.IdEquipment = eq.Id and eq.code = era.AssemblyNo
where er.id = 22
  and era.QuantityConsumed >0
```

### Endpoint 5: Componentes
```sql
select *
from EntryRequests er 
left join EntryRequestComponents era on er.id = era.IdEntryReq
where er.id = 22
  and era.QuantityConsumed >0
```

---

## Notas Importantes
- **API Local:** Esta API opera directamente sobre la base de datos local (no Business Central).
- **Multi-compañía:** Todos los endpoints requieren el parámetro `companyCode`.
- **Filtros flexibles:** Los endpoints 4 y 5 permiten filtrar por cantidad consumida de forma flexible.
- **SQL Directo:** Los endpoints 4 y 5 utilizan consultas SQL directas para evitar problemas con Entity Framework.
- **Joins optimizados:** Los endpoints 4 y 5 utilizan LEFT JOINs para obtener datos relacionados.
- **Manejo de errores:** En caso de error, la API retorna un mensaje descriptivo y el código HTTP correspondiente.
- **Parámetros opcionales:** Todos los parámetros de filtro son opcionales y se aplican solo si están presentes.

---

## Archivos Relacionados
- **Controlador:** `Controllers/EntryRequestAssemblyController.cs`
- **Entidad:** `Entidades/EntryRequestAssembly.cs`
- **DTO:** `DTOs/EntryRequestAssemblyResponseDTO.cs`
- **Contexto:** `ApplicationDBContext.cs`
