# Calendar Dispatch API

## Get Calendar Dispatch

Obtiene una lista de pedidos pendientes por despachar para visualización en calendario.

### Request

```http
GET /api/EntryRequestDetails/calendar-dispatch?companyCode={companyCode}
```

### Parameters

| Name | Type | In | Required | Description |
|------|------|------|----------|-------------|
| `companyCode` | string | query | Yes | Código de la compañía |

### Response

```json
[
  {
    "resourceId": 1,
    "code": "EQ001",
    "name": "Equipment Name",
    "dateIni": "2024-03-15T10:00:00",
    "dateEnd": "2024-03-20T18:00:00",
    "client": "Client Full Name",
    "status": "PENDIENTE",
    "equipmentStatus": "DISPONIBLE"
  }
]
```

### Response Fields

| Field | Type | Description |
|-------|------|-------------|
| `resourceId` | integer | ID del equipo |
| `code` | string | Código del equipo |
| `name` | string | Nombre del equipo |
| `dateIni` | datetime | Fecha de inicio |
| `dateEnd` | datetime | Fecha de fin |
| `client` | string | Nombre completo del cliente |
| `status` | string | Estado del detalle de la solicitud |
| `equipmentStatus` | string | Estado del equipo |

### Status Codes

| Status Code | Description |
|-------------|-------------|
| 200 | OK - Retorna la lista de pedidos pendientes |
| 400 | Bad Request - El código de compañía es requerido |
| 500 | Internal Server Error - Error del servidor |
