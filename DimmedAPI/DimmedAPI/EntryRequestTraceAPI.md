# EntryRequestTrace API

Este endpoint permite obtener el trace de un EntryRequest usando el procedimiento almacenado `[dbo].[GET_TRACE_RQ_2]`.

## Endpoints Disponibles

### 1. GET /api/EntryRequestTrace

Obtiene el trace de un EntryRequest usando parámetros de consulta.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía
- `rqId` (int, opcional): ID del EntryRequest
- `branchId` (int, opcional): ID de la sucursal
- `dateIni` (DateTime, opcional): Fecha inicial
- `dateEnd` (DateTime, opcional): Fecha final

**Ejemplos de uso:**

**Obtener todos los traces:**
```
GET /api/EntryRequestTrace?companyCode=COMP001
```

**Obtener traces con filtros:**
```
GET /api/EntryRequestTrace?companyCode=COMP001&rqId=10113&branchId=1&dateIni=2024-01-01&dateEnd=2024-12-31
```

**Respuesta:**
```json
[
  {
    "id": 1,
    "loadDate": "2024-01-15T10:30:00",
    "traceState": "En Proceso",
    "customerName": "Cliente Ejemplo",
    "branch": "Sucursal Principal",
    "surgeryInit": "2024-01-16T08:00:00",
    "status": "Activo",
    "userName": "Usuario Sistema",
    "entryRequestTraceState": "Pendiente",
    "eName": "Equipo Ejemplo",
    "eCode": "EQ001",
    "equipos": "Equipo 1, Equipo 2",
    "componentes": "Componente A, Componente B"
  }
]
```

### 2. POST /api/EntryRequestTrace/filter

Obtiene el trace de un EntryRequest usando un DTO de filtro.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía

**Body (JSON):**
```json
{
  "rqId": 10113,
  "branchId": 1,
  "dateIni": "2024-01-01T00:00:00",
  "dateEnd": "2024-12-31T23:59:59"
}
```

**Ejemplo con todos los parámetros opcionales:**
```json
{
  "rqId": null,
  "branchId": null,
  "dateIni": null,
  "dateEnd": null
}
```

**Ejemplos de uso:**

**Obtener todos los traces:**
```
POST /api/EntryRequestTrace/filter?companyCode=COMP001
Content-Type: application/json

{
  "rqId": null,
  "branchId": null,
  "dateIni": null,
  "dateEnd": null
}
```

**Obtener traces con filtros:**
```
POST /api/EntryRequestTrace/filter?companyCode=COMP001
Content-Type: application/json

{
  "rqId": 10113,
  "branchId": null,
  "dateIni": null,
  "dateEnd": null
}
```

### 3. DELETE /api/EntryRequestTrace/cache

Limpia el cache del trace de EntryRequest.

**Ejemplo de uso:**
```
DELETE /api/EntryRequestTrace/cache
```

## Estructura de Datos

### EntryRequestTraceDTO
```json
{
  "id": "int?",
  "loadDate": "DateTime?",
  "traceState": "string?",
  "customerName": "string?",
  "branch": "string?",
  "surgeryInit": "DateTime?",
  "status": "string?",
  "userName": "string?",
  "entryRequestTraceState": "string?",
  "eName": "string?",
  "eCode": "string?",
  "equipos": "string?",
  "componentes": "string?"
}
```

### EntryRequestTraceFilterDTO
```json
{
  "rqId": "int? (opcional)",
  "branchId": "int? (opcional)",
  "dateIni": "DateTime? (opcional)",
  "dateEnd": "DateTime? (opcional)"
}
```

## Notas Importantes

1. **Multicompañía**: Todos los endpoints requieren el parámetro `companyCode` para identificar la compañía.
2. **Parámetros opcionales**: Todos los parámetros son opcionales. Si no se proporciona ningún parámetro, se obtienen todos los traces.
3. **Valores nulos**: Los campos pueden devolver valores nulos según el procedimiento almacenado.
4. **Cache**: Los endpoints GET tienen cache habilitado con una duración de 15 segundos.
5. **Validaciones**: Se validan los parámetros requeridos antes de ejecutar el procedimiento almacenado.

## Códigos de Error

- **400 Bad Request**: Cuando faltan parámetros requeridos o son inválidos
- **500 Internal Server Error**: Error interno del servidor

## Ejemplos de Uso con curl

### Obtener todos los traces:
```bash
curl -X GET "https://api.example.com/api/EntryRequestTrace?companyCode=COMP001"
```

### Obtener trace básico:
```bash
curl -X GET "https://api.example.com/api/EntryRequestTrace?companyCode=COMP001&rqId=10113"
```

### Obtener trace con filtros:
```bash
curl -X GET "https://api.example.com/api/EntryRequestTrace?companyCode=COMP001&rqId=10113&branchId=1&dateIni=2024-01-01&dateEnd=2024-12-31"
```

### Obtener todos los traces usando POST:
```bash
curl -X POST "https://api.example.com/api/EntryRequestTrace/filter?companyCode=COMP001" \
  -H "Content-Type: application/json" \
  -d '{
    "rqId": null,
    "branchId": null,
    "dateIni": null,
    "dateEnd": null
  }'
```

### Obtener trace usando POST con filtro:
```bash
curl -X POST "https://api.example.com/api/EntryRequestTrace/filter?companyCode=COMP001" \
  -H "Content-Type: application/json" \
  -d '{
    "rqId": 10113,
    "branchId": 1,
    "dateIni": "2024-01-01T00:00:00",
    "dateEnd": "2024-12-31T23:59:59"
  }'
```

### Limpiar cache:
```bash
curl -X DELETE "https://api.example.com/api/EntryRequestTrace/cache"
``` 