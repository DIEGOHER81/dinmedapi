# EntryRequest Cancel Update API

## Descripción
Este endpoint permite actualizar los campos de cancelación de una EntryRequest específica.

## Endpoint
```
PATCH /api/EntryRequest/{id}/cancel?companyCode={companyCode}
```

## Parámetros de URL
- `id` (int, requerido): ID de la EntryRequest a actualizar
- `companyCode` (string, requerido): Código de la compañía

## Cuerpo de la Petición (JSON)
```json
{
  "status": "string (requerido, máximo 50 caracteres)",
  "idCancelReason": "int? (opcional)",
  "cancelReason": "string? (opcional, máximo 200 caracteres)",
  "idCancelDetail": "int? (opcional)",
  "cancelDetail": "string? (opcional, máximo 200 caracteres)"
}
```

## Respuesta Exitosa (200 OK)
```json
{
  "success": true,
  "message": "EntryRequest actualizada exitosamente",
  "entryRequestId": 123,
  "status": "CANCELADO",
  "idCancelReason": 1,
  "cancelReason": "Cliente canceló el pedido",
  "idCancelDetail": 2,
  "cancelDetail": "Cambio de fecha de cirugía",
  "updatedAt": "2024-01-15T10:30:00.000Z"
}
```

## Respuestas de Error

### 400 Bad Request
```json
{
  "success": false,
  "message": "El código de compañía es requerido",
  "entryRequestId": 123,
  "updatedAt": "2024-01-15T10:30:00.000Z"
}
```

### 404 Not Found
```json
{
  "success": false,
  "message": "No se encontró la solicitud de entrada con ID 123",
  "entryRequestId": 123,
  "updatedAt": "2024-01-15T10:30:00.000Z"
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "message": "Error interno del servidor: [descripción del error]",
  "entryRequestId": 123,
  "updatedAt": "2024-01-15T10:30:00.000Z"
}
```

## Ejemplo de Uso

### cURL
```bash
curl -X PATCH "https://api.example.com/api/EntryRequest/123/cancel?companyCode=COMP001" \
  -H "Content-Type: application/json" \
  -d '{
    "status": "CANCELADO",
    "idCancelReason": 1,
    "cancelReason": "Cliente canceló el pedido",
    "idCancelDetail": 2,
    "cancelDetail": "Cambio de fecha de cirugía"
  }'
```

### JavaScript (Fetch)
```javascript
const response = await fetch('/api/EntryRequest/123/cancel?companyCode=COMP001', {
  method: 'PATCH',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    status: 'CANCELADO',
    idCancelReason: 1,
    cancelReason: 'Cliente canceló el pedido',
    idCancelDetail: 2,
    cancelDetail: 'Cambio de fecha de cirugía'
  })
});

const result = await response.json();
console.log(result);
```

## Notas
- El endpoint es multicompañía y requiere el parámetro `companyCode`
- Solo se actualizan los campos específicos de cancelación
- Se invalida automáticamente el caché relacionado con EntryRequests
- Se registra la fecha y hora de la actualización
- El endpoint utiliza el método HTTP PATCH para indicar una actualización parcial
