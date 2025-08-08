# Dispatch API

## Marcar como despachado un pedido

Marca un pedido como despachado, validando que todos los componentes tengan lote asignado y que no haya conflictos con otros pedidos.

### Request

```http
POST /api/EntryRequest/dispatch?IdEntryReq={id}&companyCode={companyCode}
```

### Parameters

| Name | Type | In | Required | Description |
|------|------|------|----------|-------------|
| `IdEntryReq` | integer | query | Yes | ID del registro de pedido |
| `companyCode` | string | query | Yes | Código de la compañía |

### Response

#### Success Response

```json
{
  "isSuccess": true,
  "result": null,
  "message": "SAVE"
}
```

#### Error Responses

**Error de validación:**
```json
{
  "isSuccess": false,
  "result": null,
  "message": "No puede despachar, existen componentes que no tienen asignado lote"
}
```

**Error de ensamble:**
```json
{
  "isSuccess": false,
  "result": null,
  "message": "No puede despachar, debe generar pedido de ensamble para los componentes"
}
```

**Error de conflictos:**
```json
{
  "isSuccess": false,
  "result": null,
  "message": "El equipo EQ001 ya se encuentra en el pedido 123 en estado DESPACHADO; "
}
```

### Validaciones

El endpoint realiza las siguientes validaciones:

1. **Validación de entrada**: Verifica que el ID del pedido sea válido y que se proporcione el código de compañía.

2. **Validación de lotes**: Si el pedido tiene componentes, verifica que todos tengan un lote asignado.

3. **Validación de ensamble**: Si hay componentes, verifica que se haya generado un pedido de ensamble.

4. **Validación de conflictos**: Verifica que no haya conflictos con otros pedidos que estén usando los mismos equipos.

### Estados actualizados

Al marcar como despachado, se actualizan los siguientes estados:

- **EntryRequests.Status**: Cambia a "DESPACHADO"
- **EntryRequestDetails.status**: Cambia a "DESPACHADO" para detalles en estado "DISPATCH_PAR" o "Pendiente"
- **EntryRequestComponents.status**: Cambia a "DESPACHADO" para componentes en estado "DISPATCH_PAR" o "Pendiente"

### Historial

Se crea automáticamente un registro en `EntryRequestHistory` con:
- Descripción: "Pedido Despachado."
- Información: "Pedido: {Id}"
- Fecha: Fecha y hora actual
- Ubicación: "WEB"

### Status Codes

| Status Code | Description |
|-------------|-------------|
| 200 | OK - Pedido marcado como despachado exitosamente |
| 400 | Bad Request - Error de validación |
| 404 | Not Found - Pedido no encontrado |
| 500 | Internal Server Error - Error del servidor |

### Ejemplo de uso

```bash
curl -X POST "https://api.example.com/api/EntryRequest/dispatch?IdEntryReq=123&companyCode=COMP001" \
  -H "Content-Type: application/json"
```

### Notas

- El endpoint requiere que todos los componentes del pedido tengan lotes asignados
- Si hay componentes, debe existir un pedido de ensamble generado
- Se valida que no haya conflictos con otros pedidos que usen los mismos equipos
- Se actualiza automáticamente el historial del pedido
