# UpdateQuantitys API

## Descripción
Este endpoint reporta el consumo de un pedido a Business Central y genera un pedido de venta en BC. Actualiza las cantidades consumidas de ensambles y componentes, valida las cantidades, y envía la información actualizada a Business Central.

## Endpoint
```
POST /api/EntryRequest/update-quantities?companyCode={companyCode}
```

## Parámetros

### Query Parameters
- `companyCode` (string, requerido): Código de la compañía

### Body Parameters
```json
{
  "id": 10300,
  "entryRequestAssembly": [
    {
      "id": 4733021,
      "quantityConsumed": 1
    },
    {
      "id": 4733022,
      "quantityConsumed": 2
    }
  ],
  "entryRequestComponents": [
    {
      "id": 12345,
      "quantityConsumed": 5
    }
  ]
}
```

## Respuesta

### UpdateQuantitysResponseDTO
```json
{
  "isSuccess": true,
  "message": "Cantidades actualizadas exitosamente",
  "result": {
    "entryRequestId": 10300,
    "status": "CREPORTED",
    "result": "PEDIDO REGISTRADO",
    "updatedAt": "2025-01-20T10:30:00.000Z"
  }
}
```

### Campos de Respuesta
- `isSuccess` (boolean): Indica si la operación fue exitosa
- `message` (string): Mensaje descriptivo del resultado
- `result` (object): Datos adicionales del resultado
  - `entryRequestId` (int): ID del pedido procesado
  - `status` (string): Nuevo estado del pedido
  - `result` (string): Resultado de la operación en BC
  - `updatedAt` (datetime): Fecha y hora de la actualización

## Códigos de Respuesta

- `200 OK`: Cantidades actualizadas exitosamente
- `400 Bad Request`: Parámetros inválidos o errores de validación
- `404 Not Found`: Pedido no encontrado
- `500 Internal Server Error`: Error interno del servidor

## Flujo de Procesamiento

1. **Validación de parámetros**: Verifica que el `companyCode` esté presente y que los datos del pedido sean válidos.

2. **Validación de cantidades**: Utiliza el endpoint `ReloadAssemblyDis` para validar que las cantidades del pedido coincidan.

3. **Actualización de ensambles**: Actualiza las cantidades consumidas de los ensambles proporcionados.

4. **Actualización de componentes**: Actualiza las cantidades consumidas de los componentes proporcionados.

5. **Sincronización con BC**: Utiliza el endpoint `ReloadAssemblyToBC` para sincronizar las cantidades desde Business Central.

6. **Envío a Business Central**: 
   - Elimina el pedido de venta existente en BC
   - Envía el nuevo pedido con las cantidades actualizadas
   - Actualiza el estado del pedido a "CREPORTED"

7. **Registro de historial**: Crea registros en `EntryRequestHistory` y `EventLog` para auditoría.

## Ejemplo de Uso

### Request
```http
POST /api/EntryRequest/update-quantities?companyCode=COMPANY001
Content-Type: application/json

{
  "id": 10300,
  "entryRequestAssembly": [
    {
      "id": 4733021,
      "quantityConsumed": 1
    }
  ],
  "entryRequestComponents": [
    {
      "id": 12345,
      "quantityConsumed": 2
    }
  ]
}
```

### Response Exitosa
```json
{
  "isSuccess": true,
  "message": "Cantidades actualizadas exitosamente",
  "result": {
    "entryRequestId": 10300,
    "status": "CREPORTED",
    "result": "PEDIDO REGISTRADO",
    "updatedAt": "2025-01-20T10:30:00.000Z"
  }
}
```

### Response con Error de Validación
```json
{
  "isSuccess": false,
  "message": "Las Cantidades del pedido no coinciden. El Equipo no tiene registros con reservas: 747-2;"
}
```

### Response con Error de BC
```json
{
  "isSuccess": false,
  "message": "ERROR: El pedido no tiene consumos"
}
```

## Consideraciones

- **Validación de cantidades**: El sistema valida que las cantidades consumidas no excedan las cantidades disponibles.
- **Sincronización**: Se sincroniza automáticamente con Business Central antes de enviar el pedido.
- **Auditoría**: Se registran todas las operaciones en el historial del pedido y en el log de eventos.
- **Estado del pedido**: El estado se actualiza a "CREPORTED" cuando el pedido se envía exitosamente a BC.
- **Manejo de errores**: Se manejan errores de validación, conexión con BC y errores internos del sistema.

## Dependencias

- **ReloadAssemblyDis**: Para validar cantidades del pedido
- **ReloadAssemblyToBC**: Para sincronizar cantidades desde BC
- **Business Central API**: Para enviar y eliminar pedidos de venta
- **EntryRequestHistory**: Para registro de auditoría
- **EventLog**: Para registro de eventos del sistema

