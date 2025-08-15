# ReloadAssemblyDis API

## Descripción
Este endpoint valida el ensamble de referencias de un pedido al momento de despachar, verificando la disponibilidad de componentes y reservas.

## Endpoint
```
GET /api/EntryRequest/{entryRequestId}/reloadAssemblyDis
```

## Parámetros

### Path Parameters
- `entryRequestId` (int, requerido): ID del pedido a validar

### Query Parameters
- `companyCode` (string, requerido): Código de la compañía

## Respuesta

### ReloadAssemblyDisResponseDTO
```json
{
  "success": true,
  "message": "Validación de ensamble completada exitosamente",
  "entryRequestId": 10300,
  "validationMessages": "El Equipo no tiene registros con reservas: 747-2; Para el Equipo: 747-2, Componente: 213011972, Lote: INST2021 no existe cantidad reservada; ",
  "processedAt": "2025-01-20T10:30:00.000Z"
}
```

### Campos de Respuesta
- `success` (boolean): Indica si la operación fue exitosa
- `message` (string): Mensaje descriptivo del resultado
- `entryRequestId` (int): ID del pedido procesado
- `validationMessages` (string): Texto con las validaciones encontradas (errores o advertencias)
- `processedAt` (datetime): Fecha y hora de la operación

## Códigos de Respuesta

- `200 OK`: Validación completada exitosamente
- `400 Bad Request`: Parámetros inválidos (companyCode vacío o entryRequestId <= 0)
- `404 Not Found`: Pedido no encontrado
- `500 Internal Server Error`: Error interno del servidor

## Ejemplo de Uso

### Request
```http
GET /api/EntryRequest/10300/reloadAssemblyDis?companyCode=COMPANY001
```

### Response Exitosa
```json
{
  "success": true,
  "message": "Validación de ensamble completada exitosamente",
  "entryRequestId": 10300,
  "validationMessages": "",
  "processedAt": "2025-01-20T10:30:00.000Z"
}
```

### Response con Validaciones
```json
{
  "success": true,
  "message": "Validación de ensamble completada exitosamente",
  "entryRequestId": 10300,
  "validationMessages": "El Equipo no tiene registros con reservas: 747-2; Para el Equipo: 747-2, Componente: 213011972, Lote: INST2021 no existe cantidad reservada; Equipo: 747-2 Componente: 213011972; ",
  "processedAt": "2025-01-20T10:30:00.000Z"
}
```

## Funcionalidad

El endpoint realiza las siguientes validaciones:

1. **Obtención de datos del pedido**: Utiliza el endpoint `/api/EntryRequest/{id}/with-details` para obtener la información completa del pedido con sus detalles y ensambles.

2. **Determinación de lista de precios**: 
   - Si el pedido tiene `priceGroup` (propiedad de EntryRequests), lo utiliza directamente
   - Si el cliente tiene `IsSecondPriceList = true`, consulta las listas de precio usando `/api/CustomerPriceList/customer/{customerId}` y filtra por tipo de aseguradora
   - En caso contrario, usa la lista de precios del cliente (`PriceGroup` de Customer)

3. **Validación de ensambles**:
   - Obtiene el ensamble V2 usando `GetEntryReqAssembly("lylassemblyV2", ...)`
   - Obtiene el ensamble normal usando `GetEntryReqAssembly("lylassembly", ...)`
   - Valida que los componentes existan en el ensamble actual
   - Verifica que las cantidades consumidas no excedan las cantidades reservadas

4. **Mensajes de validación**: Genera mensajes descriptivos para:
   - Equipos sin registros de reservas
   - Componentes sin cantidad reservada
   - Cantidades consumidas que exceden las reservadas

## Consideraciones Técnicas

- Utiliza el patrón de conexión dinámica por compañía
- Integra con Business Central para obtener información de ensambles
- Maneja múltiples listas de precios por cliente
- Proporciona logging detallado para debugging
- Incluye manejo de errores robusto
