# EntryRequestComponents Update API

## Descripción
Este endpoint permite actualizar los campos Warehouse, ExpirationDate y Lot de un componente específico de EntryRequestComponents.

## Endpoint

### PATCH: api/EntryRequestComponents/{id}/warehouse-lot-expiration

Actualiza los campos Warehouse, ExpirationDate y Lot de un componente específico.

#### Parámetros de URL
- `id` (int, requerido): ID del componente a actualizar

#### Parámetros de Query
- `companyCode` (string, requerido): Código de la compañía

#### Cuerpo de la Petición (JSON)
```json
{
  "warehouse": "string (opcional)",
  "expirationDate": "datetime (opcional, puede ser null)",
  "lot": "string (opcional)"
}
```

#### Ejemplo de Petición
```http
PATCH /api/EntryRequestComponents/123/warehouse-lot-expiration?companyCode=COMP001
Content-Type: application/json

{
  "warehouse": "BODEGA01",
  "expirationDate": "2024-12-31T00:00:00",
  "lot": "LOT123456"
}
```

#### Respuesta Exitosa (200 OK)
```json
{
  "success": true,
  "message": "Campos Warehouse, ExpirationDate y Lot actualizados exitosamente",
  "updatedAt": "2024-01-15T10:30:00Z",
  "data": {
    "id": 123,
    "itemNo": "ITEM001",
    "warehouse": "BODEGA01",
    "expirationDate": "2024-12-31T00:00:00",
    "lot": "LOT123456"
  }
}
```

#### Respuesta de Error (400 Bad Request)
```json
{
  "success": false,
  "message": "El código de compañía es requerido",
  "updatedAt": "2024-01-15T10:30:00Z"
}
```

#### Respuesta de Error (404 Not Found)
```json
{
  "success": false,
  "message": "No se encontró el componente con ID: 123",
  "updatedAt": "2024-01-15T10:30:00Z"
}
```

#### Respuesta de Error (500 Internal Server Error)
```json
{
  "success": false,
  "message": "Error interno del servidor: [detalle del error]",
  "updatedAt": "2024-01-15T10:30:00Z"
}
```

## Características Especiales

### Manejo de Valores Nulos
- Los campos `warehouse`, `expirationDate` y `lot` pueden ser nulos
- Para establecer explícitamente `expirationDate` como null, enviar `null` en el JSON
- Los campos no incluidos en la petición no se modifican

### Ejemplo con Valores Nulos
```http
PATCH /api/EntryRequestComponents/123/warehouse-lot-expiration?companyCode=COMP001
Content-Type: application/json

{
  "warehouse": null,
  "expirationDate": null,
  "lot": "NUEVO_LOTE"
}
```

### Actualización Parcial
Solo se actualizan los campos que se incluyen en la petición:

```http
PATCH /api/EntryRequestComponents/123/warehouse-lot-expiration?companyCode=COMP001
Content-Type: application/json

{
  "warehouse": "NUEVA_BODEGA"
}
```

En este caso, solo se actualiza el campo `warehouse`, manteniendo los valores actuales de `expirationDate` y `lot`.

## Validaciones

1. **CompanyCode requerido**: El código de compañía es obligatorio
2. **Componente existente**: El componente con el ID especificado debe existir
3. **Datos válidos**: El DTO de actualización debe ser válido
4. **Manejo de nulos**: Los campos pueden ser nulos sin generar errores

## Logs

El endpoint genera logs detallados para facilitar el debugging:
- Inicio y fin de la operación
- Valores anteriores y nuevos de cada campo actualizado
- Errores detallados con stack trace

## Cache

Después de una actualización exitosa, se invalida el cache relacionado con EntryRequestComponents para asegurar que los datos actualizados estén disponibles inmediatamente.
