# ReloadAssemblyToBC API

API para actualizar cantidades de referencias de un pedido desde Business Central.

## Información General
- **Tipo:** API Local (no requiere autenticación externa)
- **Base URL:** `/api/EntryRequest`
- **Método:** GET
- **Formato de respuesta:** JSON

## Parámetro obligatorio
- **companyCode** (query): Código de la compañía sobre la que se realiza la operación.

## Endpoints

### 1. Actualizar cantidades de referencias de un pedido
- **GET** `/api/EntryRequest/{entryRequestId}/reloadAssemblyToBC?companyCode={companyCode}`
- **Descripción:** Actualiza las cantidades de referencias de un pedido específico desde Business Central.
- **Parámetros:**
  - `entryRequestId` (path, requerido): ID del pedido a actualizar.
  - `companyCode` (query, requerido): Código de la compañía.
- **Respuesta exitosa:** 200 OK
- **Respuesta de error:** 400 Bad Request, 404 Not Found, 500 Internal Server Error

## Respuestas

### Respuesta Exitosa (200 OK)
```json
true
```

### Respuestas de Error

**400 Bad Request**
```json
"El código de compañía es requerido"
```
```json
"El ID del pedido debe ser mayor a 0"
```

**404 Not Found**
```json
"No se encontró la solicitud de entrada con ID {entryRequestId}"
```

**500 Internal Server Error**
```json
"Error interno del servidor: [mensaje de error]"
```

## Funcionalidad

El endpoint realiza las siguientes operaciones:

1. **Validación de parámetros:** Verifica que el `entryRequestId` sea válido y que el `companyCode` esté presente.

2. **Obtención de datos:** 
   - Obtiene la EntryRequest con todos sus detalles, ensambles y relaciones
   - Establece conexión con Business Central

3. **Determinación de lista de precios:**
   - Si el pedido tiene `priceGroup`, lo usa directamente
   - Si el cliente tiene `IsSecondPriceList = true`, consulta las listas de precio del cliente y selecciona la que coincida con el tipo de aseguradora
   - En caso contrario, usa la lista de precios del cliente

4. **Procesamiento de ensambles V2:**
   - Para cada detalle del pedido, obtiene el ensamble V2 desde Business Central
   - Actualiza los ensambles existentes con las nuevas cantidades, precios y reservas
   - Identifica nuevos componentes que podrían necesitar inserción

5. **Procesamiento de ensambles estándar:**
   - Obtiene el ensamble estándar desde Business Central
   - Actualiza los ensambles existentes con información completa (cantidades, precios, fechas de vencimiento, etc.)
   - Maneja errores para componentes no encontrados

6. **Persistencia:** Guarda todos los cambios en la base de datos.

## Consideraciones Técnicas

- **Multi-compañía:** El endpoint utiliza el sistema de conexiones dinámicas para operar sobre la base de datos y configuración de Business Central de la compañía especificada.
- **Conexión BC:** Utiliza el servicio `IDynamicBCConnectionService` para conectarse dinámicamente a Business Central.
- **Manejo de errores:** Incluye logging detallado para facilitar el debugging.
- **Transacciones:** Los cambios se guardan en una sola transacción al final del proceso.

## Ejemplo de Uso

```bash
curl -X GET "https://api.example.com/api/EntryRequest/10300/reloadAssemblyToBC?companyCode=COMP001" \
  -H "accept: application/json"
```

## Logs

El endpoint genera logs detallados que incluyen:
- Inicio y fin del proceso
- Validación de parámetros
- Obtención de datos
- Procesamiento de ensambles
- Errores y excepciones

## Archivos Relacionados
- **Controlador:** `Controllers/EntryRequestController.cs`
- **Servicios:** `Services/DynamicConnectionService.cs`, `Services/DynamicBCConnectionService.cs`
- **BO:** `BO/CustomerPriceListBO.cs`
- **Entidades:** `Entidades/EntryRequests.cs`, `Entidades/EntryRequestAssembly.cs`
