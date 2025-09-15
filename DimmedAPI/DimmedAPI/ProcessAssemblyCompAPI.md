# ProcessAssemblyComp API

## Descripción
Endpoint para procesar pedidos de ensamble de componentes adicionales y enviarlos a Business Central.

## Endpoint
```
POST /api/EntryRequest/process-assembly-comp
```

## Parámetros

### Query Parameters
- `IdEntryReq` (int): ID del registro de EntryRequest a procesar (requerido)
- `companyCode` (string): Código de la compañía (requerido)

## Ejemplo de Request

```http
POST /api/EntryRequest/process-assembly-comp?IdEntryReq=123&companyCode=COMP001
```

## Respuesta

### Respuesta Exitosa (200 OK)
```json
{
    "isSuccess": true,
    "result": null,
    "message": "PEDIDO REGISTRADO"
}
```

### Respuesta de Error (400 Bad Request)
```json
{
    "isSuccess": false,
    "result": null,
    "message": "El código de compañía es requerido"
}
```

```json
{
    "isSuccess": false,
    "result": null,
    "message": "MODEL_NOT_VALID"
}
```

### Respuesta de Error (404 Not Found)
```json
{
    "isSuccess": false,
    "result": null,
    "message": "EntryRequest no encontrado"
}
```

### Respuesta de Error (500 Internal Server Error)
```json
{
    "isSuccess": false,
    "result": null,
    "message": "Error detallado del servidor"
}
```

## Funcionalidad

1. **Validación**: Verifica que el `companyCode` y `IdEntryReq` sean válidos
2. **Obtención de datos**: Obtiene el EntryRequest con sus componentes desde la base de datos específica de la compañía
3. **Procesamiento**: Crea un pedido de ensamble con los componentes del EntryRequest
4. **Envío a BC**: Envía el pedido a Business Central usando el endpoint `lylgenassembly`
5. **Actualización**: Si el envío es exitoso, marca el EntryRequest con `AssemblyComponents = true`
6. **Creación de registros**: Crea registros de `EntryRequestDetails` y `EntryRequestAssembly` para el seguimiento

## Estructura del Pedido a BC

El endpoint crea un objeto `AssemblyApiBC_Header` con la siguiente estructura:

```json
{
    "documentNo": "P-{IdEntryReq}",
    "branch": "{Nombre de la sucursal}",
    "itemNo": "9999",
    "lines": [
        {
            "documentNo": "P-{IdEntryReq}",
            "itemNo": "{Código del componente}",
            "quantity": {Cantidad},
            "price": {Precio unitario},
            "locationCode": "{Código de ubicación}",
            "lotNo": "{Número de lote}",
            "quantityLine": {Cantidad total},
            "idWeb": {ID del componente}
        }
    ]
}
```

## Notas Técnicas

- **Multicompañía**: El endpoint está adaptado para funcionar con múltiples compañías usando `IDynamicConnectionService` y `IDynamicBCConnectionService`
- **Validación de componentes**: Solo procesa EntryRequests que tengan componentes asociados
- **Manejo de errores**: Incluye logging detallado y manejo de excepciones
- **IDs hardcodeados**: Los valores `IdEquipment = 1713` y `AssemblyNo = "9910"` están marcados como TODO para ser configurados según la lógica de negocio

## Dependencias

- `EntryRequestBO`: Clase de lógica de negocio para EntryRequest
- `AssemblyApiBC_Header` y `AssemblyApiBC_Line`: DTOs para la comunicación con BC
- `IDynamicConnectionService`: Servicio para conexiones dinámicas a BD
- `IDynamicBCConnectionService`: Servicio para conexiones dinámicas a BC
